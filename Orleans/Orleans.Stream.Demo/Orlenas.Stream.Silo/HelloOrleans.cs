using System;
using System.Threading.Tasks;
using Orleans.Stream.Grain;

namespace Orleans.Stream.Silo
{
    public class HelloOrleans : Orleans.Grain, IHelloOrleans
    {
        public Task<string> SayHi(string message)
        {
            return Task.FromResult($"Hello Orleans:{message}");
        }
    }
}
