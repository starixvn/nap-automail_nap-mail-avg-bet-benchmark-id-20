using Fanex.MailDelivery.Core.Models;
using Fanex.TOAVGBetBenchmark.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.TOAVGBetBenchmark.Service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TOAVGBetBenchmarkController : ControllerBase
    {
        private readonly MainService service;

        public TOAVGBetBenchmarkController(MainService service)
        {
            this.service = service;
        }

        [HttpGet]
        public JobMessage Get()
        {
            return service.Execute();
        }
    }
}