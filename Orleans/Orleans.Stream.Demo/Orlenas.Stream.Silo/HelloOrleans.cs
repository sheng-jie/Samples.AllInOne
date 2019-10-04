using System;
using System.Threading.Tasks;
using Orleans.Stream.Grain;
using Orleans.Streams;
using Orleans.Timers;

namespace Orleans.Stream.Silo
{
    [ImplicitStreamSubscription("RANDOMDATA")]
    public class HelloOrleans : Orleans.Grain, IHelloOrleans
    {
        public Task<string> SayHi(string message)
        {
            //Create a GUID based on our GUID as a grain
            var guid = this.GetPrimaryKey();
            //Get one of the providers which we defined in config
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");

            RegisterTimer(obj => stream.OnNextAsync(new Random().Next()), null, 
                TimeSpan.FromMilliseconds(1000),
                TimeSpan.FromMilliseconds(1000));
            return Task.FromResult($"Hello Orleans:{message}");
        }

        public override async Task OnActivateAsync()
        {
            //Create a GUID based on our GUID as a grain
            var guid = this.GetPrimaryKey();
            //Get one of the providers which we defined in config
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<int>(guid, "RANDOMDATA");
            //Set our OnNext method to the lambda which simply prints the data, this doesn't make new subscription

            await stream.SubscribeAsync((data, token) =>
            {
                Console.WriteLine($"Received:{data}");
                return Task.CompletedTask;
            });
        }

    }


}
