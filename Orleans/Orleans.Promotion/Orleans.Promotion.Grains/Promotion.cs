using System;
using System.Collections.Generic;
using System.Linq;

namespace Orleans.Promotion.Grains
{
    /// <summary>
    /// 秒杀
    /// </summary>
    public class Promotion
    {
        public Guid Id { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Promotion(DateTime startTime, DateTime endTime)
        {
            Id = Guid.NewGuid();
            StartTime = startTime;
            EndTime = endTime;
            PromotionProducts = new List<PromotionProduct>();
        }


        public List<PromotionProduct> PromotionProducts { get; private set; }

        /// <summary>
        /// 添加促销商品
        /// </summary>
        /// <param name="product"></param>
        public void AddPromotionProduct(PromotionProduct product)
        {
            product.PromotionId = this.Id;
            this.PromotionProducts.Add(product);
        }

        public override string ToString()
        {
            string list = string.Join(Environment.NewLine, PromotionProducts.Select(product => product.ToString()));
            return $"Promotion Id:{Id},StartTim:{StartTime},EndTime:{EndTime}" + Environment.NewLine
                                                                               + $"Promotion Products：" + Environment.NewLine + $"{list}";
        }
    }
}

