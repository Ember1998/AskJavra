using AskJavra.Models.Root;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : Controller
    {
        private readonly TagService _tagService;
        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {           
            return Ok(await _tagService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //var errorResponse = new ResponseDto<MyEntity>(false, "Entity not found", null);
            //return NotFound(errorResponse);
            var result =await _tagService.GetByIdAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Success == false && result.Message == "not found")
                return NotFound(result);
            else return StatusCode( 500,result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagDto dto)
        {
            if (ModelState.IsValid)
            {
                var result =  await _tagService.AddAsync(dto);

                // Ensure result.Data is not null before accessing Id
                if (result.Data == null)
                {
                    return StatusCode(500, result); ;
                }

                return CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result);
            }else
             return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TagDto entity)
        {
            if (id < 1 || !ModelState.IsValid)
            {
                var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", null);
                return BadRequest(errorResponse);
            }
            var tag = new Tag(id, entity.Name, entity.TagDescription);
            
            var response = await _tagService.UpdateAsync(tag);
            
            if (response.Success == false && response.Message == "not found") return NotFound(response);
            else if (response.Data != null && response.Success) return Ok(response);
            else return StatusCode(500, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
             var result = await _tagService.DeleteAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Message == "not found")
                return NotFound(result);
            else
                return StatusCode(500, result);

        }
    }
}
