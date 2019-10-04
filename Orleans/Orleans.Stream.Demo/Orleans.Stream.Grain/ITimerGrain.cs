using System.Threading.Tasks;

namespace Orleans.Stream.Grain
{
    public interface ITimerGrain :IGrainWithGuidKey
    {
        Task<int> GetValueAsync();
    }
}