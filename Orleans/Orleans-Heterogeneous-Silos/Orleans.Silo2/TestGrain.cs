using System.Threading.Tasks;
using Orleans.Interfaces;

namespace Orleans.Silo2
{
    public class TestGrain:Grain,ITest
    {
        public Task<int> GetNum(int num)
        {
            return Task.FromResult(num * num);
        }
    }
}