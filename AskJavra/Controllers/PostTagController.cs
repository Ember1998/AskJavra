using AskJavra.Models.Post;
using AskJavra.Models.Root;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostTagController : Controller
    {
        private readonly PostService _postService;
        private readonly PostTagService _postTagService;
        private readonly PostThreadService _postThreadService;
        public PostTagController(
            PostTagService postTagService
            , PostService postService
            , PostThreadService postThreadService
            )
        {
            _postTagService = postTagService;
            _postService = postService;
            _postThreadService = postThreadService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postTagService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            //var errorResponse = new ResponseDto<MyEntity>(false, "Entity not found", null);
            //return NotFound(errorResponse);
            var result = await _postTagService.GetByIdAsync(id);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Success == false && result.Message == "not found")
                return NotFound(result);
            else return StatusCode(500, result);
        }

        [HttpPost("PostTag")]
        public async Task<IActionResult> CreatePostTag(Guid postId, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var post = await _postService.GetByIdAsync(postId);
                var postMod = new Post(post.Data.PostId, post.Data.Title, post.Data.Description, post.Data.PostType, post.Data.CreatedBy, post.Data.IsAnonymous);
                var result = await _postTagService.AddPostTagAsync(tagIds, postMod);

                // Ensure result.Data is not null before accessing Id
                if (result.Data == null)
                {
                    return StatusCode(500, result); ;
                }

                return CreatedAtAction(nameof(CreatePostTag), new { id = result.Data[0].Id }, result);
            }
            else
                return BadRequest(ModelState);
        }

        //[HttpPost("ThreadTag")]
        //public async Task<IActionResult> CreateThreadTag(Guid threadId, [FromBody] List<ThreadTagDto> dto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var post = await _postThreadService.GetByIdAsync(threadId);
        //        var result = await _postTagService.AddPostThreadTagAsync(dto, post.Data);

        //        // Ensure result.Data is not null before accessing Id
        //        if (result.Data == null)
        //        {
        //            return StatusCode(500, result); ;
        //        }

        //        return CreatedAtAction(nameof(CreatePostTag), new { id = result.Data[0].Id }, result);
        //    }
        //    else
        //        return BadRequest(ModelState);
        //}
        [HttpPut("{postTagId}")]
        public async Task<IActionResult> UpdatePostTag(int postTagId, [FromBody] PostTagDto entity)
        {
            if (postTagId < 1 || !ModelState.IsValid || entity.PostId != null)
            {
                var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", null);
                return BadRequest(errorResponse);
            }
            var post = await _postService.GetByIdAsync((Guid)entity.PostId);
            var postMod = new Post(post.Data.PostId, post.Data.Title, post.Data.Description, post.Data.PostType, post.Data.CreatedBy, post.Data.IsAnonymous);
            var response = await _postTagService.UpdatePostTagAsync(postTagId, entity, postMod);

            if (response.Success == false && response.Message == "not found") return NotFound(response);
            else if (response.Data != null && response.Success) return Ok(response);
            else return StatusCode(500, response);
        }
        //[HttpPut("{threadTagId}")]
        //public async Task<IActionResult> UpdateThreadTag(int threadTagId, [FromBody] ThreadTagDto entity)
        //{
        //    if (threadTagId < 1 || !ModelState.IsValid || entity.PostThreadId != null)
        //    {
        //        var errorResponse = new ResponseDto<Tag>(false, "Invalid entity", null);
        //        return BadRequest(errorResponse);
        //    }
        //    var thread = await _postService.GetByIdAsync((Guid)entity.PostThreadId);
        //    var response = await _postTagService.UpdatePostThreadAsync(threadTagId, entity, thread.Data);

        //    if (response.Success == false && response.Message == "not found") return NotFound(response);
        //    else if (response.Data != null && response.Success) return Ok(response);
        //    else return StatusCode(500, response);
        //}
        [HttpDelete("{postTagId}")]
        public async Task<IActionResult> Delete(int postTagId)
        {
            var result = await _postTagService.DeleteAsync(postTagId);
            if (result != null && result.Success)
                return Ok(result);
            else if (result.Message == "not found")
                return NotFound(result);
            else
                return StatusCode(500, result);

        }
    }
}
