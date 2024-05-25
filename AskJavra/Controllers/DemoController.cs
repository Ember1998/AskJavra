using AskJavra.Models;
using AskJavra.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly DemoRepository _demoRepo;
        public DemoController(DemoRepository demoRepo)
        {
            _demoRepo = demoRepo;
        }
        [HttpGet("DemoMsg")]
        public IActionResult DemoMsg()
        {
            return Ok(_demoRepo.DemoMethod());
        }
        [HttpPost("Create")]
        public IActionResult Create(Demo demo)
        {
            _demoRepo.CreateDemo(demo);
            return Ok();
        }
    }
}
