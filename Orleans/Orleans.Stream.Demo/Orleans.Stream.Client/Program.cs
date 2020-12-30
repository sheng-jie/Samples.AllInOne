using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Stream.Grain;

namespace Orleans.Stream.Client
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.Title = $"{typeof(Program).Namespace}";
            return RunMainAsync().Result;
        }
        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering()
                .AddSimpleMessageStreamProvider("SMSProvider")
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            // example of calling grains from the initialized client
            var friend = client.GetGrain<IHelloOrleans>(0);

            var guid = Guid.NewGuid();
            var response = await friend.SayHi("Good morning, HelloGrain!");

            
            Console.WriteLine(response);

            var timerGrain = client.GetGrain<ITimerGrain>(Guid.NewGuid());
            var timerTask = timerGrain.GetValueAsync();

            var reminderGrain = client.GetGrain<IReminderGrain>(Guid.NewGuid());
            var reminderTask = reminderGrain.GetValueAsync();
            
            Thread.Sleep(2000);

            var results = await Task.WhenAll(timerTask, reminderTask);


            Console.WriteLine($"Timer Result:{timerGrain.GetValueAsync().Result}");


            Console.WriteLine($"Reminder Result:{results}");


        }
    }

}
