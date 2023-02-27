using Microsoft.AspNetCore.Mvc;

namespace Fanex.TOAVGBetBenchmark.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "OK";
        }
    }
}