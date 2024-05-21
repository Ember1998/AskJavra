using AskJavra.Enums;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels.Dto
{
    public class PostDto
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public FeedStatus FeedStatus { get; set; }       
        public List<PostTagDto>? Tags { get; set; }
        
        public PostDto(string title, string? description, PostType postType, List<PostTagDto> tags)
        {
            Title = title;
            Description = description;
            PostType = postType;
            Tags = tags;
        }
        public PostDto() {  }
    }
    public class PostAndIdDto : TagDto
    {
        public Guid Id { get; set; }
    }
    public class PostViewDto {
        public string Title { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public FeedStatus FeedStatus { get; set; }
        public List<PostTagDto>? Tags { get; set; }
        public Guid PostId { get; set; }
        public string PostTypeName { get; set; }
        public string FeedStatusName { get; set; }
        public DateTime CreationAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public List<UpvoteCountViewMode> UpVotes { get; set; }
        public List<PostThreadViewDto> postThreads { get; set; }
        public int TotalUpvoteCount { get; set; }
    }

}
