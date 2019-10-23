using System.Threading.Tasks;

namespace Orleans.Grains
{
    public class PromotionProductManager : Grain<PromotionProduct>, IPromotionProductManager
    {
        public Task InitialProduct(PromotionProduct product)
        {
            this.State = product;
            return Task.CompletedTask;
        }

        public Task<bool> Minus(int qty)
        {
            if (this.State.Qty >= qty)
            {
                this.State.Qty -= qty;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<PromotionProduct> GetStatus()
        {
            return Task.FromResult(this.State);
        }

    }
}