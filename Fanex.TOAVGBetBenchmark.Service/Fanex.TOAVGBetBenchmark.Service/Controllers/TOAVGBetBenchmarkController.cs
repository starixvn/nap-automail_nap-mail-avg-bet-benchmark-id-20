using Fanex.MailDelivery.Core.Models;
using Fanex.TOAVGBetBenchmark.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.TOAVGBetBenchmark.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToavgBetBenchmarkController(MainService service) : ControllerBase
    {
        [HttpGet]
        public JobMessage Get()
        {
            return service.Execute();
        }
    }
}