using System.Threading.Tasks;

namespace Orleans.Interfaces
{
    public interface ITest : IGrainWithIntegerKey
    {
        Task<int> GetNum(int num);
    }
}