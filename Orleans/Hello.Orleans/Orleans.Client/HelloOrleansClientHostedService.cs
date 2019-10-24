using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            Promotion promotion = await CreateNewPromotion();

            if (DateTime.Now < promotion.StartTime)
            {
                Console.WriteLine("秒杀尚未开始！");
                int startSeconds = (promotion.StartTime - DateTime.Now).Seconds;
                while (startSeconds > 0)
                {
                    Console.Write($"\r倒计时：{--startSeconds}s！");
                    await Task.Delay(1000);
                }
            }

            Console.WriteLine("秒杀正式开始！");

            var productRandom = new Random();

            ConcurrentBag<string> orderHistory = new ConcurrentBag<string>();


            //模拟并行秒杀
            var result = Parallel.For(1, 100, async (index, state) =>
            {
                if (DateTime.Now > promotion.EndTime)
                {
                    _logger.LogInformation("秒杀结束！");
                    state.Stop();
                }
                
                var userId = $"User{index}";
                //全局单例
                var sessionControl = this._client.GetGrain<ISessionControl>(Guid.Empty);

                var loginSucceed = await sessionControl.Login(userId);
                if (!loginSucceed)
                {
                    _logger.LogWarning($"{userId} 你好，当前参与秒杀人员过多，请稍后再试！");
                    return;
                }

                //随机购买一样商品
                var productAddToCart = promotion.PromotionProducts[productRandom.Next(promotion.PromotionProducts.Count - 1)];
                //随机购买数量
                var buyNum = productRandom.Next(1, productAddToCart.MaxBuy);

                var productManager = this._client.GetGrain<IPromotionProductManager>(productAddToCart.ProductId);
                var isSucceed = await productManager.Minus(buyNum);

                if (isSucceed)
                {
                    var message = $"{userId} 你好，你成功秒杀了 {buyNum} 件 {productAddToCart.Name}";

                    orderHistory.Add(message);
                    _logger.LogInformation(message);

                    //秒杀成功就退出秒杀系统
                    await sessionControl.Logout(userId);
                }
                else
                {
                    var productStatus = await productManager.GetStatus();
                    var message = $"{userId} 你好，你尝试秒杀{buyNum}件{productAddToCart.Name}失败， 剩余库存 {productStatus.Qty}";
                    _logger.LogWarning(message);
                }

            });

            if (result.IsCompleted)
            {
                //promotionProducts.ForEach(async product =>
                //{
                //    var manager = this._client.GetGrain<IPromotionProductManager>(product.ProductId);
                //    var promotionProduct = await manager.GetStatus();
                //    _logger.LogInformation($"{promotionProduct.Name} 剩余 {promotionProduct.Qty} 件");
                //});


                foreach (var order in orderHistory)
                {
                    _logger.LogError(order);
                }
            }
        }

        /// <summary>
        /// 创建一个5分钟的秒杀活动，10秒后开始
        /// </summary>
        /// <returns></returns>
        private async Task<Promotion> CreateNewPromotion()
        {
            var promotion = new Promotion(DateTime.Now.AddSeconds(10), DateTime.Now.AddMinutes(5));

            var promotionProducts = new List<PromotionProduct>()
            {
                new PromotionProduct(1, "iPhone x", 10, 3888,1),
                new PromotionProduct(2, "Huawei meta 30", 20, 4500,2),
                new PromotionProduct(3, "XiaoMi Note7 Pro", 30, 1288,3),
                new PromotionProduct(4, "Oppo R7", 40, 2000,4),
            };

            promotionProducts.ForEach(async product =>
            {
                promotion.AddPromotionProduct(product);

                var productManager = this._client.GetGrain<IPromotionProductManager>(product.ProductId);
                await productManager.InitialProduct(product);
            });


            _logger.LogInformation(promotion.ToString());

            return await Task.FromResult(promotion);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}