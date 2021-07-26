using DotNet.Grpc.Server.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace DotNet.Grpc.Client.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5201");
            var client = new Greeter.GreeterClient(channel);

            // Unary gRPCclient
            Console.WriteLine("=====> Unary gRPCclient <=====");
            var responseUnary = await client.SayHelloAsync(new HelloRequest { Name = "Unary call" });
            Console.WriteLine($"Response: {responseUnary.Message}");

            // Server streaming call
            Console.WriteLine("=====> Server streaming call <=====");
            using var serverStreamCall = client.GetSayHelloStream(new HelloRequest { Name = "Pedro" });
            await foreach (var responseServerStream in serverStreamCall.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Response: {responseServerStream.Message}");
            }

            // Client streaming call
            Console.WriteLine("=====> Client streaming call <=====");
            string[] names = { "Pedro", "Maria", "Ana", "Mariane" };

            using (var clientStreamCall = client.SendSayHelloStream())
            {
                foreach (var name in names)
                {
                    await clientStreamCall.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }
                await clientStreamCall.RequestStream.CompleteAsync();

                HelloResponse clientStreamResponse = await clientStreamCall.ResponseAsync;
                Console.WriteLine($"Response: {clientStreamResponse.Message}");
            }

            // Bi-directional streaming call
            Console.WriteLine("=====> Bi-directional streaming call <=====");
            using (var bidirectionalStreamingCall = client.BiDirectionalSayHelloStream())
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await bidirectionalStreamingCall.ResponseStream.MoveNext())
                    {
                        var responseServer = bidirectionalStreamingCall.ResponseStream.Current;
                        Console.WriteLine($"Response: {responseServer.Message}");
                    }
                });

                string[] namesBd = { "João", "Paulo", "Leandro", "Márcia" };
                foreach (var name in names)
                {
                    await bidirectionalStreamingCall.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }
                await bidirectionalStreamingCall.RequestStream.CompleteAsync();
                await responseReaderTask;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
