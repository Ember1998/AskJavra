using AskJavra.Models.Post;
using AskJavra.Models.Root;
using AskJavra.Repositories.Interface;
using AskJavra.Repositories.Service;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedController : Controller
    {
        private readonly PostService _postService;
        private readonly PostTagService _postTagService;
        public FeedController(PostService postService, PostTagService postTagService)
        {
            _postService = postService;
            _postTagService = postTagService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] FeedRequestDto request)
        {
            var result = await _postService.GetAllAsync(request);
            return Ok(result);
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
                if(result.Success && result.Data.PostId != Guid.Empty)
                {
                    var post = await _postService.GetByIdAsync(result.Data.PostId);
                    var postMod = new Post(post.Data.PostId, post.Data.Title, post.Data.Description, post.Data.PostType, post.Data.CreatedBy, post.Data.IsAnonymous);
                    if(dto.TagIds != null && dto.TagIds.Length > 0 )                       
                        await _postTagService.AddPostTagAsync(dto.TagIds, postMod);

                    return Ok(result);

                }
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
            var post = new Post(id, entity.Title, entity.Description, entity.PostType, entity.CreatedBy, entity.IsAnonymous);
            post.LastModifiedBy = entity.UpdatedBy;
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
        [HttpPost("RevokeOrUpVoteFeed{postId}/{upvoteBy}")]
        public async Task<IActionResult> RevokeOrUpVoteFeed(Guid postId, string upvoteBy)
        {
            try
            {
                var result = await _postService.UpvoteFeed(postId, upvoteBy);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
