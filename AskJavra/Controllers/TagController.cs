using AskJavra.Models.Root;
using AskJavra.Repositories;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

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
            return Ok(_tagService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
                //var errorResponse = new ResponseDto<MyEntity>(false, "Entity not found", null);
                //return NotFound(errorResponse);
            return Ok(_tagService.GetByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TagDto dto)
        {
            if (ModelState.IsValid)
            {
                var entity = new Tag(dto.Name, dto.TagDescription);
                var result =  _tagService.AddAsync(dto);

                // Ensure result.Data is not null before accessing Id
                if (result.Data == null)
                {
                    return BadRequest(new ResponseDto<Tag>(false, "Entity creation failed", null));
                }

                var response = new ResponseDto<Tag>(true, "Entity created successfully", result.Data);
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, response);
            }

            var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", null);
            return BadRequest(errorResponse);
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
            await _tagService.UpdateAsync(tag);
            var response = new ResponseDto<Tag>(true, "Entity updated successfully", tag);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity =  _tagService.GetByIdAsync(id);
            if (entity == null)
            {
                var errorResponse = new ResponseDto<TagDto>(false, "Entity not found", null);
                return NotFound(errorResponse);
            }

             _tagService.DeleteAsync(id);
            return Ok(_tagService.DeleteAsync(id));
        }
    }
}
