using DotNet.Grpc.Server.Protos;
using DotNet.Grpc.Server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DotNet.Grpc.Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GreetController : Controller
    {
        private readonly IGreeterGrpcService _greeterGrpcService;

        public GreetController(IGreeterGrpcService greeterGrpcService)
        {
            _greeterGrpcService = greeterGrpcService;
        }

        [HttpGet]
        public async Task<ActionResult<HelloReply>> Index(string name)
        {
            return await _greeterGrpcService.SayHello(name);
        }
    }
}
