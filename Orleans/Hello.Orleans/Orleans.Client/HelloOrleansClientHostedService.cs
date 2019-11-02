using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Grains;

namespace Orleans.Client
{
    public class HelloOrleansClientHostedService : IHostedService
    {
        private readonly IClusterClient _client;
        private readonly ILogger<HelloOrleansClientHostedService> _logger;

        public HelloOrleansClientHostedService(IClusterClient client, ILogger<HelloOrleansClientHostedService> logger)
        {
            _client = client;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // 模拟控制台终端用户登录
           await MockLogin("Hello.Orleans.Console");
           // 模拟网页终端用户登录
           await MockLogin("Hello.Orleans.Web");
        }


        /// <summary>
        /// 模拟指定应用的登录
        /// </summary>
        /// <param name="appName"></param>
        /// <returns></returns>
        public async Task MockLogin(string appName)
        {
            //假设我们需要支持不同端登录用户，则只需要将项目名称作为身份标识。
            //即可获取一个代表用来维护当前项目登录状态的的单例Grain。
            var sessionControl = _client.GetGrain<ISessionControlGrain>(appName);
            ParallelLoopResult result = Parallel.For(0, 100, (index) =>
            {
                var userId = $"User-{index}";
                sessionControl.Login(userId);
            });


            if (result.IsCompleted)
            {
                //ParallelLoopResult.IsCompleted 只是返回所有循环创建完毕，并不保证循环的内部任务创建并执行完毕
                //所以，此次手动延迟5秒后再去读取活动用户数。
                await Task.Delay(TimeSpan.FromSeconds(5));
                var activeUserCount = await sessionControl.GetActiveUserCount();

                _logger.LogInformation($"The Active Users Count of {appName} is {activeUserCount}");

            }
        }



        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Closed!");

            return Task.CompletedTask; ;
        }
    }
}