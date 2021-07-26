using DotNet.Grpc.Server.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.Grpc.Server.Services
{
    public class GreeterGrpcService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterGrpcService> _logger;
        public GreeterGrpcService(ILogger<GreeterGrpcService> logger)
        {
            _logger = logger;
        }

        public override async Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return await Task.FromResult(new HelloResponse
            {
                Message = $"Hello {request.Name}"
            });
        }

        public override async Task GetSayHelloStream(HelloRequest request, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
            var i = 1;
            while (!context.CancellationToken.IsCancellationRequested && i <= 5)
            {
                await Task.Delay(500);

                var helloResponse = new HelloResponse
                {
                    Message = $"Hello {request.Name} {i}"
                };

                await responseStream.WriteAsync(helloResponse);

                i++;
            }
        }

        public override async Task<HelloResponse> SendSayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, ServerCallContext context)
        {
            var result = new StringBuilder("Hello");

            await foreach (var request in requestStream.ReadAllAsync())
            {
                result.Append($", {request.Name}");
            };

            return await Task.FromResult(new HelloResponse
            {
                Message = result.ToString()
            });
        }

        public override async Task BiDirectionalSayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
        {
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
