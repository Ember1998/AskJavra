using AskJavra.Repositories.Service;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }
        [HttpPost("UpdateContributionRankPoint/{id}")]
        public async Task<IActionResult> UpdateContributionRankPoint(int id, int  minPoint, int maxPoint)
        {
            try
            {
                var result = await _adminService.UpdateRankPoint(id, minPoint, maxPoint);
                if (result)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
