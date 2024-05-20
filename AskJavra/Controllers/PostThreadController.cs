﻿using AskJavra.Models.Post;
using AskJavra.Models.Root;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    public class PostThreadController : Controller
    {
        private readonly PostThreadService _postThreadService;
        public PostThreadController(PostThreadService postThreadService)
        {
            _postThreadService = postThreadService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postThreadService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //var errorResponse = new ResponseDto<MyEntity>(false, "Entity not found", null);
            //return NotFound(errorResponse);
            var result = await _postThreadService.GetByIdAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Success == false && result.Message == "not found")
                return NotFound(result);
            else return StatusCode(500, result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostThreadDto dto)
        {
            if (ModelState.IsValid)
            {
                var result = await _postThreadService.AddAsync(dto);

                // Ensure result.Data is not null before accessing Id
                if (result.Data == null)
                {
                    return StatusCode(500, result); ;
                }

                return CreatedAtAction(nameof(Create), new { id = result.Data.Id }, result);
            }
            else
                return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PostThreadDto entity)
        {
            if (id != Guid.Empty || !ModelState.IsValid)
            {
                var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", null);
                return BadRequest(errorResponse);
            }
            var postThread = new PostThread(id, entity.ThreadTitle, entity.ThreadDescription, entity.PostId);
            var response = await _postThreadService.UpdateAsync(postThread);
           
            if (response.Success == false && response.Message == "not found") return NotFound(response);
            else if (response.Data != null && response.Success) return Ok(response);
            else return StatusCode(500, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _postThreadService.DeleteAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Message == "not found")
                return NotFound(result);
            else
                return StatusCode(500, result);

        }
    }
}