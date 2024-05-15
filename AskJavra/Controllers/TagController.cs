using AskJavra.Dto;
using AskJavra.Models.Root;
using AskJavra.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly TagRepository _tagRepository;
        public TagController(TagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]RequestDto request)
        {
            try
            {
                var result = await _tagRepository.GetAllPostAsync();
                if (result != null && result.Count() > 0)
                 return Ok(result);
                else return NotFound();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
    }
}
