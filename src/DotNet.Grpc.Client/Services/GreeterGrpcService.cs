using DotNet.Grpc.Server.Protos;
using System.Threading.Tasks;

namespace DotNet.Grpc.Server.Services
{
    public interface IGreeterGrpcService
    {
        Task<HelloReply> SayHello(string name);
    }

    public class GreeterGrpcService : IGreeterGrpcService
    {
        private readonly Greeter.GreeterClient _greeterClient;

        public GreeterGrpcService(Greeter.GreeterClient greeterClient)
        {
            _greeterClient = greeterClient;
        }

        public async Task<HelloReply> SayHello(string name)
        {
            var response = await _greeterClient.SayHelloAsync(new HelloRequest() { Name = name });
            return response;
        }
    }
}
