using System;
using System.Threading;
using Grpc.Core;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Grpc.Demo.Server;

namespace Grpc.Demo.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await BathTheCats();
            //            await HelloGrpc();
            Console.ReadKey();
        }

        private static async Task HelloGrpc()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                new HelloRequest { Name = "Grpc!" });
            Console.WriteLine("Greeter Return: " + reply.Message);

            var catClient = new LuCat.LuCatClient(channel);

            var catReply = await catClient.SuckingCatAsync(new Empty());
            Console.WriteLine("调用撸猫服务：" + catReply.Message);
        }

        private static async Task BathTheCats()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var catClient = new LuCat.LuCatClient(channel);
            var catCount = await catClient.CountAsync(new Empty());

            Console.WriteLine($"猫：{catCount}");

            var cts = new CancellationTokenSource();

            cts.CancelAfter(TimeSpan.FromSeconds(2.5));

            var bathCat = catClient.BathTheCat(cancellationToken: cts.Token);

            var bathCatRespTask = Task.Run(async () =>
            {
                try
                {
                    await foreach (var resp in bathCat.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
                    {
                        Console.WriteLine(resp.Message);
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
                {
                    Console.WriteLine("Stream cancelled.");
                }
            });

            var rand = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < 10; i++)
            {
                var id = rand.Next(0, catCount.Count);
                await bathCat.RequestStream.WriteAsync(new BathTheCatReq() { Id = id });
                Console.WriteLine($"{i}. No.{id} 已进行排队洗澡！");
            }

            await bathCat.RequestStream.CompleteAsync();

            await bathCatRespTask;

            Console.WriteLine();
        }
    }
}
