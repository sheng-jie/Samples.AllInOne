using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Interfaces;

namespace Orleans.Silo1
{
    /// <summary>
    /// Orleans grain implementation class HelloGrain.
    /// </summary>
    public class HelloGrain : Orleans.Grain, IHello
    {
        private readonly ILogger logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        Task<string> IHello.SayHello(string greeting)
        {
            logger.LogInformation($"SayHello message received: greeting = '{greeting}'");
            return Task.FromResult($"You said: '{greeting}', I say: Hello!");
        }

        public Task<User> GetUser(int id) => Task.FromResult(new User() { Id = id, No = $"No.{id}" });
    }
}
