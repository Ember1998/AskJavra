using AskJavra.Enums;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels.Dto
{
    public class PostThreadDto
    {
        [Required]
        public string ThreadTitle { get; set; }
        [Required]
        public Guid PostId { get; set; }
        public string? ThreadDescription { get; set; }
        public PostDto Post { get; set; }
        public PostThreadDto(string threadTitle, string? threadDescription, Guid postId)
        {
            ThreadTitle = threadTitle;
            ThreadDescription = threadDescription;
            PostId = postId;
        }
        public PostThreadDto() { }
    }
    public class PostThreadCreateDto
    {
        [Required]
        public string ThreadTitle { get; set; }
        [Required]
        public Guid PostId { get; set; }
        public string? ThreadDescription { get; set; }
        public string CreatedBy { get; set; }
    }
    public class PostThreadUpdateDto
    {
        [Required]
        public string ThreadTitle { get; set; }
        [Required]
        public Guid ThreadId { get; set; }
        [Required]
        public Guid PostId { get; set; }

        public string? ThreadDescription { get; set; }
    }

    public class PostThreadViewDto
    {
        public Guid ThreadId { get; set; }
        public string ThreadTitle { get; set; }
        [Required]
        public Guid PostId { get; set; }
        public string? ThreadDescription { get; set; }
        public PostViewDto Post { get; set; }
        public List<ThreadUpvoteResponseDto> ThreadUpVotes { get; set; } = new List<ThreadUpvoteResponseDto>();
        public int ThreadUpVoteCount { get; set; }
        public ApplicationUserViewDtocs CreatedByUser { get; set; }
    }
    public class ThreadUpvoteResponseDto
    {
        public string UpvoteBy { get; set; }
        public string ThreadTitle { get; set; }
        public string ThreadDescription { get; set; }
    }
}
