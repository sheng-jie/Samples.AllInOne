using System.Threading.Tasks;

namespace Orleans.Grains
{
    public interface IPromotionProductManager : IGrainWithIntegerKey
    {
        Task InitialProduct(PromotionProduct product);
        Task<bool> Minus(int qty);
        Task<PromotionProduct> GetStatus();

    }
}