using AskJavra.Enums;

namespace AskJavra.ViewModels.Dto
{
    public class PostDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public PostType PostType { get; set; }
        public PostDto(string title, string? description, PostType postType)
        {
            Title = title;
            Description = description;
            PostType = postType;
        }
        public PostDto() {  }
    }
    public class PostAndIdDto : TagDto
    {
        public Guid Id { get; set; }
    }
}
