using System;

namespace Orleans.Grains
{
    public class PromotionProduct
    {
        public Guid PromotionId { get; set; }
        public int ProductId { get; private set; }
        public string Name { get; private set; }
        public int Qty { get; set; }
        public decimal Price { get; private set; }

        public int MaxBuy { get; set; }

        public PromotionProduct(int productId, string name, int qty, decimal price, int maxBuy)
        {
            ProductId = productId;
            Name = name;
            Qty = qty;
            Price = price;
            MaxBuy = maxBuy;
        }

        public PromotionProduct()
        {
        }

        public override string ToString()
        {
            return $"{ProductId}\t{Name}\t{MaxBuy}\t{Qty}";
        }
    }
}