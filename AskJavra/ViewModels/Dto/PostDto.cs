using AskJavra.Enums;

namespace AskJavra.ViewModels.Dto
{
    public class PostDto
    {
        public string Title { get; set; }
        public Guid PostId { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public List<PostTagDto>? Tags { get; set; }
        public DateTime CreationAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
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
}
