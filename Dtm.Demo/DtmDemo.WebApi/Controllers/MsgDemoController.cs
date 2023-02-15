using Dtmcli;
using DtmCommon;
using DtmDemo.WebApi.Data;
using DtmDemo.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DtmDemo.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MsgDemoController : ControllerBase
    {
        private readonly IDtmTransFactory _dtmTransFactory;
        private readonly DtmDemoWebApiContext _context;
        private readonly IConfiguration _configuration;
        private readonly IBranchBarrierFactory _barrierFactory;
        private readonly ILogger<MsgDemoController> _logger;

        public MsgDemoController(IDtmTransFactory dtmTransFactory, DtmDemoWebApiContext dbContext, IConfiguration configuration,
            IBranchBarrierFactory branchBarrierFactory, ILogger<MsgDemoController> logger)
        {
            this._dtmTransFactory = dtmTransFactory;
            this._context = dbContext;
            this._configuration = configuration;
            this._barrierFactory = branchBarrierFactory;
            this._logger = logger;
        }
        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer(int fromUserId, int toUserId, decimal amount,
        CancellationToken cancellationToken)
        {
            try
            {
                var fromAccount = await _context.BankAccount.FindAsync(fromUserId);
                var toAccount = await _context.BankAccount.FindAsync(toUserId);
                if (toAccount == null)
                {
                    return new BadRequestObjectResult("Account does not exist or insufficient balance.");
                }

                _logger.LogInformation($"转账事务-启动：用户{fromUserId}转账{amount}元到用户{toUserId}");
                var gid = Guid.NewGuid().ToString();

                var svc = _configuration.GetValue<string>("MsgBaseURL");
                var msg = _dtmTransFactory.NewMsg(gid);
                msg.Add(svc + "/TransferIn", new TransferRequest(toUserId, amount));
                msg.EnableWaitResult();

                await msg.Submit();

                using var connection = _context.Database.GetDbConnection();
                await msg.DoAndSubmitDB(svc + "/msg-queryprepared", connection, async tx =>
                   {
                       await _context.Database.UseTransactionAsync(tx);
                       var bankAccount = await _context.BankAccount.FindAsync(fromUserId);
                       if (bankAccount == null || bankAccount.Balance < amount)
                           throw new InvalidDataException("账户不存在或余额不足！");
                       bankAccount.Balance -= amount;
                       await _context.SaveChangesAsync();
                   }, cancellationToken);
            }
            catch (Exception ex) // 6. 如果开启了`EnableWaitResult()`，则可通过捕获异常的方式，捕获事务失败的结果。
            {
                _logger.LogError($"转账事务-失败：用户{fromUserId}转账{amount}元到用户{toUserId}失败！");

                return new BadRequestObjectResult($"转账失败:{ex.Message}");
            }
            _logger.LogInformation($"转账事务-完成：用户{fromUserId}转账{amount}元到用户{toUserId}成功！");
            return Ok($"转账事务-完成：用户{fromUserId}转账{amount}元到用户{toUserId}成功！");
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

        /// <summary>
        /// MSG QueryPrepared(mysql)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("msg-queryprepared")]
        public async Task<IActionResult> MsgMySqlQueryPrepared(CancellationToken cancellationToken)
        {
            var bb = _barrierFactory.CreateBranchBarrier(Request.Query);
            _logger.LogInformation("bb {0}", bb);
            using (var conn = _context.Database.GetDbConnection())
            {
                var res = await bb.QueryPrepared(conn);

                return Ok(new { dtm_result = res });
            }
        }
    }
}
