using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DtmDemo.WebApi.Data;
using DtmDemo.WebApi.Models;
using Dtmcli;
using DtmCommon;

namespace DtmDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SagaDemoController : ControllerBase
{
    private readonly DtmDemoWebApiContext _context;
    private readonly IConfiguration _configuration;
    private readonly IDtmClient _dtmClient;
    private readonly IDtmTransFactory _transFactory;

    private readonly IBranchBarrierFactory _barrierFactory;
    private readonly ILogger<BankAccountsController> _logger;

    public SagaDemoController(DtmDemoWebApiContext context, IConfiguration configuration, IDtmClient dtmClient,
        IDtmTransFactory transFactory, ILogger<BankAccountsController> logger, IBranchBarrierFactory barrierFactory)
    {
        this._context = context;
        this._configuration = configuration;
        this._dtmClient = dtmClient;
        this._transFactory = transFactory;
        this._logger = logger;
        this._barrierFactory = barrierFactory;
    }

    [HttpPost("Transfer")]
    public async Task<IActionResult> Transfer(int fromUserId, int toUserId, decimal amount,
        CancellationToken cancellationToken)
    {
        var msg = $"用户{fromUserId}转账{amount}元到用户{toUserId}";
        try
        {
            _logger.LogInformation($"转账事务-启动：{msg}");
            //1. 生成全局事务ID
            var gid = await _dtmClient.GenGid(cancellationToken);
            var bizUrl = _configuration.GetValue<string>("TransferBaseURL");
            //2. 创建Saga
            var saga = _transFactory.NewSaga(gid);
            //3. 添加子事务
            saga.Add(bizUrl + "/TransferOut", bizUrl + "/TransferOut_Compensate",
                    new TransferRequest(fromUserId, amount))
                .Add(bizUrl + "/TransferIn", bizUrl + "/TransferIn_Compensate",
                    new TransferRequest(toUserId, amount))
                .EnableWaitResult(); // 4. 按需启用是否等待事务执行结果

            //5. 提交Saga事务
            await saga.Submit(cancellationToken);
        }
        catch (DtmException ex) // 6. 如果开启了`EnableWaitResult()`，则可通过捕获异常的方式，捕获事务失败的结果。
        {
            _logger.LogError($"转账事务-失败：{msg}");
            return new BadRequestObjectResult($"转账失败:{ex.Message}");
        }

        _logger.LogError($"转账事务-完成：{msg}");
        return Ok($"转账事务-完成：{msg}");
    }

    [HttpPost("TransferIn")]
    public async Task<IActionResult> TransferIn([FromBody] TransferRequest request)
    {
        var msg = $"用户{request.UserId}转入{request.Amount}元";
        _logger.LogInformation($"转入子事务-启动：{msg}");
        var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);
        try
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                await branchBarrier.Call(conn, async (tx) =>
                {
                    _logger.LogInformation($"转入子事务-执行：{msg}");
                    await _context.Database.UseTransactionAsync(tx);
                    var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                    if (bankAccount == null)
                        throw new InvalidDataException("账户不存在！");
                    bankAccount.Balance += request.Amount;
                    await _context.SaveChangesAsync();
                });
            }
        }
        catch (InvalidDataException ex)
        {
            _logger.LogInformation($"转入子事务-失败：{ex.Message}");
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }
        _logger.LogInformation($"转入子事务-成功：{msg}");
        return Ok();
    }

    [HttpPost("TransferIn_Compensate")]
    public async Task<IActionResult> TransferIn_Compensate([FromBody] TransferRequest request)
    {
        var msg = "用户{request.UserId}回滚转入{request.Amount}元";
        _logger.LogInformation($"转入补偿子事务-启动：{msg}");
        var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);

        using (var conn = _context.Database.GetDbConnection())
        {
            await branchBarrier.Call(conn, async (tx) =>
            {
                _logger.LogInformation($"转入补偿子事务-执行：{msg}");
                await _context.Database.UseTransactionAsync(tx);
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null) return;
                bankAccount.Balance -= request.Amount;
                await _context.SaveChangesAsync();
            });
        }
        _logger.LogInformation($"转入补偿子事务-成功！");
        return Ok();
    }

    [HttpPost("TransferOut")]
    public async Task<IActionResult> TransferOut([FromBody] TransferRequest request)
    {
        var msg = $"用户{request.UserId}转出{request.Amount}元";
        _logger.LogInformation($"转出子事务-启动：{msg}");
        // 1. 创建子事务屏障
        var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);
        try
        {
            using (var conn = _context.Database.GetDbConnection())
            {
                // 2. 在子事务屏障内执行事务操作
                await branchBarrier.Call(conn, async (tx) =>
                {
                    _logger.LogInformation($"转出子事务-执行：{msg}");
                    await _context.Database.UseTransactionAsync(tx);
                    var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                    if (bankAccount == null || bankAccount.Balance < request.Amount)
                        throw new InvalidDataException("账户不存在或余额不足！");
                    bankAccount.Balance -= request.Amount;
                    await _context.SaveChangesAsync();
                });
            }
        }
        catch (InvalidDataException ex)
        {
            _logger.LogInformation($"转出子事务-失败：{ex.Message}");
            // 3. 按照接口协议，返回409，以表示子事务失败
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }
        _logger.LogInformation($"转出子事务-成功：{msg}");
        return Ok();
    }

    [HttpPost("TransferOut_Compensate")]
    public async Task<IActionResult> TransferOut_Compensate([FromBody] TransferRequest request)
    {
        var msg = $"用户{request.UserId}回滚转出{request.Amount}元";
        _logger.LogInformation($"转出补偿子事务-启动：{msg}");
        // 1. 创建子事务屏障
        var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);
        using (var conn = _context.Database.GetDbConnection())
        {
            // 在子事务屏障内执行事务操作
            await branchBarrier.Call(conn, async (tx) =>
            {
                _logger.LogInformation($"转出补偿子事务-执行：{msg}");
                await _context.Database.UseTransactionAsync(tx);
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                if (bankAccount == null)
                    return; //对于补偿操作，可直接返回，中断后续操作
                bankAccount.Balance += request.Amount;
                await _context.SaveChangesAsync();
            });
        }
        _logger.LogInformation($"转出补偿子事务-成功！");
        // 2. 因补偿操作必须成功，所以必须返回200。
        return Ok();
    }
}