using Dtmcli;
using DtmDemo.WebApi.Data;
using DtmDemo.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DtmDemo.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MsgDemoController : ControllerBase
{
    private readonly IDtmTransFactory _dtmTransFactory;
    private readonly DtmDemoWebApiContext _context;
    private readonly IConfiguration _configuration;
    private readonly IDtmClient _dtmClient;
    private readonly IBranchBarrierFactory _barrierFactory;
    private readonly ILogger<MsgDemoController> _logger;

    public MsgDemoController(IDtmTransFactory dtmTransFactory, DtmDemoWebApiContext dbContext, IConfiguration configuration,
        IBranchBarrierFactory branchBarrierFactory, ILogger<MsgDemoController> logger, IDtmClient dtmClient)
    {
        this._dtmTransFactory = dtmTransFactory;
        this._context = dbContext;
        this._configuration = configuration;
        this._barrierFactory = branchBarrierFactory;
        this._logger = logger;
        this._dtmClient = dtmClient;
    }
    [HttpPost("Transfer")]
    public async Task<IActionResult> Transfer(int fromUserId, int toUserId, decimal amount,
    CancellationToken cancellationToken)
    {
        var info = $"用户{fromUserId}转账{amount}元到用户{toUserId}";
        try
        {
            // 1.业务规则前置判断
            var toAccount = await _context.BankAccount.FindAsync(toUserId);
            if (toAccount == null)
                return new BadRequestObjectResult("目标账户不存在");

            _logger.LogInformation($"转账事务-启动：{info}");

            var svc = _configuration.GetValue<string>("MsgBaseURL");
            //2. 生成全局事务ID
            var gid = await _dtmClient.GenGid(cancellationToken);
            //3. 创建二阶段消息
            var msg = _dtmTransFactory.NewMsg(gid);
            //4. 添加子事务分支
            //msg.Add(svc + "/TransferOut", new TransferRequest(fromUserId, amount));
            msg.Add(svc + "/TransferIn", new TransferRequest(toUserId, amount));
            //5. 按需启用是否等待事务执行结果
            msg.EnableWaitResult();

            //6. 执行转出本地事务 
            using var connection = _context.Database.GetDbConnection();
            await msg.DoAndSubmitDB(
                svc + "/msg-queryprepared", connection, async tx =>
                {
                    var logMsg = $"用户{fromUserId}转出{amount}元";
                    _logger.LogInformation($"转出本地事务-执行：{logMsg}");
                    await _context.Database.UseTransactionAsync(tx);
                    var bankAccount = await _context.BankAccount.FindAsync(fromUserId);
                    if (bankAccount == null || bankAccount.Balance < amount)
                    {
                        _logger.LogInformation($"转出本地事务-失败：{logMsg}");
                        throw new InvalidDataException("账户不存在或余额不足！");
                    }
                    bankAccount.Balance -= amount;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"转出本地事务-成功：{logMsg}");
                }, cancellationToken);
        }
        catch (Exception ex) // 7. 如果开启了`EnableWaitResult()`，则可通过捕获异常的方式，捕获事务失败的结果。
        {
            _logger.LogError($"转账事务-失败：{info}");
            return new BadRequestObjectResult($"转账失败:{ex.Message}");
        }
        _logger.LogInformation($"转账事务-完成：{info}");
        return Ok($"转账事务-完成：{info}");
    }

    [HttpPost("TransferIn")]
    public async Task<IActionResult> TransferIn([FromBody] TransferRequest request)
    {
        var msg = $"用户{request.UserId}转入{request.Amount}元";
        _logger.LogInformation($"转入子事务-启动：{msg}");
        var branchBarrier = _barrierFactory.CreateBranchBarrier(Request.Query);
        using (var conn = _context.Database.GetDbConnection())
        {
            await branchBarrier.Call(conn, async (tx) =>
            {
                _logger.LogInformation($"转入子事务-执行：{msg}");
                await _context.Database.UseTransactionAsync(tx);
                var bankAccount = await _context.BankAccount.FindAsync(request.UserId);
                bankAccount.Balance += request.Amount;
                await _context.SaveChangesAsync();
            });
        }
        _logger.LogInformation($"转入子事务-成功：{msg}");
        return Ok();
    }

    /// <summary>
    /// MSG QueryPrepared(mysql)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("msg-queryprepared")]
    public async Task<IActionResult> MsgMySqlQueryPrepared(CancellationToken cancellationToken)
    {
        var bb = _barrierFactory.CreateBranchBarrier(Request.Query);
        _logger.LogInformation("执行回查： {0}", bb);
        using (var conn = _context.Database.GetDbConnection())
        {
            var res = await bb.QueryPrepared(conn);
            return Ok(new { dtm_result = res });
        }
    }
}
