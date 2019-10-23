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
            var promotion = new Promotion(DateTime.Now.AddMinutes(1), DateTime.Now.AddMinutes(5));

            var promotionProducts = new List<PromotionProduct>()
            {
                new PromotionProduct(1, "iPhone x", 10, 3888,1),
                new PromotionProduct(2, "Huawei meta 30", 20, 4500,2),
                new PromotionProduct(3, "XiaoMi Note7 Pro", 30, 1288,3),
                new PromotionProduct(4, "Oppo R7", 40, 2000,4),
            };

            promotionProducts.ForEach(product =>
            {
                promotion.AddPromotionProduct(product);

                var productManager = this._client.GetGrain<IPromotionProductManager>(product.ProductId);
                productManager.InitialProduct(product);
            });

            if (DateTime.Now < promotion.StartTime)
            {
                _logger.LogInformation($"秒杀未开始");

                _logger.LogInformation(promotion.ToString());

                await Task.Delay((promotion.StartTime - DateTime.Now).Milliseconds);
            }

            var productRandom = new Random();

            ConcurrentBag<string> orderHistory = new ConcurrentBag<string>();

            var result = Parallel.For(1, 100, async (index, state) =>
               {
                   if (DateTime.Now > promotion.EndTime)
                   {
                       _logger.LogInformation("秒杀结束！");
                       state.Stop();
                   }
                   var userId = $"User{index}";

                   //随机购买一样商品
                   var productAddToCart = promotion.PromotionProducts[productRandom.Next(promotion.PromotionProducts.Count - 1)];
                   //随机购买数量
                   var buyNum = productRandom.Next(1, productAddToCart.MaxBuy);

                   var productManager = this._client.GetGrain<IPromotionProductManager>(productAddToCart.ProductId);
                   var isSucceed = await productManager.Minus(buyNum);

                   if (isSucceed)
                   {
                       var message = $"{userId} 秒杀了 {buyNum} 件 {productAddToCart.Name}";
                       orderHistory.Add(message);
                       _logger.LogInformation(message);
                   }
                   else
                   {
                       var productStatus = await productManager.GetStatus();
                       var message = $"{productAddToCart.Name} 剩余库存 {productStatus.Qty}，{userId} 尝试购买{buyNum}件失败！";
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}