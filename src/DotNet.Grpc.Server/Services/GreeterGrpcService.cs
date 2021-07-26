using DotNet.Grpc.Server.Protos;
using Grpc.Core;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Grpc.Server.Services
{
    public class GreeterGrpcService : Greeter.GreeterBase
    {
        /// <summary>
        /// Unary RPC
        /// </summary>
        public override async Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new HelloResponse
            {
                Message = $"Hello {request.Name}"
            });
        }

        /// <summary>
        /// Server streaming RPC
        /// </summary>
        public override async Task SayHelloServerStreaming(HelloRequest request, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
            var i = 1;

            // Send many responses
            while (!context.CancellationToken.IsCancellationRequested && i <= 5)
            {
                await Task.Delay(500);
                var helloResponse = new HelloResponse
                {
                    Message = $"Hello {request.Name} {i}"
                };

                // Send response
                await responseStream.WriteAsync(helloResponse);

                i++;
            }
        }

        /// <summary>
        /// Client streaming RPC
        /// </summary>
        public override async Task<HelloResponse> SayHelloClientStreaming(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var result = new StringBuilder();

            // Receive and process some requests
            await foreach (var request in requestStream.ReadAllAsync())
            {
                result.Append($"Hello {request.Name}");
            }

            // Send a response
            return await Task.FromResult(new HelloResponse
            {
                Message = result.ToString()
            });
        }

        /// <summary>
        /// Bidirectional streaming RPC
        /// </summary>
        public override async Task SayHelloBidirectionalStreaming(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
            // Receive some requests, process and returns a response per request
            await foreach (var message in requestStream.ReadAllAsync())
            {
                var helloResponse = new HelloResponse
                {
                    Message = $"Hello {message.Name}"
                };
                await responseStream.WriteAsync(helloResponse);
            }
        }
    }
}
