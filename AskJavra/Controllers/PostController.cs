﻿using AskJavra.Models.Post;
using AskJavra.Models.Root;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly PostService _postService;
        public PostController(PostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _postService.GetByIdAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Success == false && result.Message == "not found")
                return NotFound(result);
            else return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _postService.AddAsync(dto);

                // Ensure result.Data is not null before accessing Id
                if (result.Data == null)
                {
                    return StatusCode(500,new ResponseDto<Tag>(false, "Entity creation failed", new Tag()));
                }

                return CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result);
            }

            var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", new Tag());
            return BadRequest(errorResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PostDto entity)
        {
            if (id == Guid.Empty || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = new Post(id, entity.Title, entity.Description, entity.PostType);
            var response = await _postService.UpdateAsync(post);
            if (response.Success == false && response.Message == "not found") return NotFound(response);
            else if (response.Data != null && response.Success) return Ok(response);
            else return StatusCode(500, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
           
            var result = await _postService.DeleteAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Message == "not found")
                return NotFound(result);
            else
                return StatusCode(500, result);

        }
    }
}
