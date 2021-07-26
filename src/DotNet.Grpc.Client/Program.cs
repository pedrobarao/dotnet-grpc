using DotNet.Grpc.Server.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace DotNet.Grpc.Client.ConsoleApp
{
    class Program
    {
        static string _grpcServerUrl = "https://localhost:5201";

        static async Task Main(string[] args)
        {
            // Unary RPC
            await SayHello();

            // Server streaming RPC
            await SayHelloServerStreaming();

            // Client streaming RPC
            await SayHelloClientStreaming();

            // Bidirectional streaming RPC
            await SayHelloBidirectionalStreaming();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Unary RPC
        /// </summary>
        static async Task SayHello()
        {
            Console.WriteLine("=====> Unary RPC <=====");

            using var channel = GrpcChannel.ForAddress(_grpcServerUrl);
            var client = new Greeter.GreeterClient(channel);

            // Request-response
            var response = await client.SayHelloAsync(new HelloRequest { Name = "World" });
            Console.WriteLine($"Response: {response.Message}");
        }

        /// <summary>
        /// Server streaming RPC
        /// </summary>
        static async Task SayHelloServerStreaming()
        {
            Console.WriteLine("=====> Server streaming RPC <=====");

            using var channel = GrpcChannel.ForAddress(_grpcServerUrl);
            var client = new Greeter.GreeterClient(channel);

            // Send a request
            using var call = client.SayHelloServerStreaming(new HelloRequest { Name = "World" });

            // Receive many responses from server
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Response: {response.Message}");
            }
        }

        /// <summary>
        /// Client streaming RPC
        /// </summary>
        static async Task SayHelloClientStreaming()
        {
            Console.WriteLine("=====> Client streaming RPC <=====");

            using var channel = GrpcChannel.ForAddress(_grpcServerUrl);
            var client = new Greeter.GreeterClient(channel);

            string[] names = { "World 1", "World 2", "World 3", "World 4" };

            // Preparing request
            using (var call = client.SayHelloClientStreaming())
            {
                // Send many requests
                foreach (var name in names)
                {
                    await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }

                await call.RequestStream.CompleteAsync();

                // Receice a response
                HelloResponse response = await call.ResponseAsync;
                Console.WriteLine($"Response: {response.Message}");
            }
        }

        /// <summary>
        /// Bidirectional streaming RPC
        /// </summary>
        static async Task SayHelloBidirectionalStreaming()
        {
            Console.WriteLine("=====> Bidirectional streaming RPC <=====");

            using var channel = GrpcChannel.ForAddress(_grpcServerUrl);
            var client = new Greeter.GreeterClient(channel);

            // Preparing request
            using (var call = client.SayHelloBidirectionalStreaming())
            {
                // Get many responses in task
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var responseServer = call.ResponseStream.Current;
                        Console.WriteLine($"Response: {responseServer.Message}");
                    }
                });

                string[] namesBd = { "World 5", "World 6", "World 7", "World 8" };

                // Send many requests
                foreach (var name in namesBd)
                {
                    await call.RequestStream.WriteAsync(new HelloRequest { Name = name });
                }

                // Wait process all requests from client
                await call.RequestStream.CompleteAsync();

                // Wait process all responses from server
                await responseReaderTask;
            }
        }
    }
}
