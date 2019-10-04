using System.Threading.Tasks;

namespace Orleans.Stream.Grain
{
    public interface IReminderGrain : IGrainWithGuidKey
    {
        Task<int> GetValueAsync();
    }
}