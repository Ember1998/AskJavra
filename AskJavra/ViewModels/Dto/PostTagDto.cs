using AskJavra.Enums;
using AskJavra.Models.Post;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels.Dto
{
    public class PostTagDto
    {
        public int? TagId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? PostThreadId { get; set; }
        public PostTagDto(int? tagId, Guid? postId, Guid? postThreadId)
        {
            TagId = tagId;
            PostId = postId;
            PostThreadId = postThreadId;
        }
        public PostTagDto() {  }
    }
    public class PostTagAndIdDto : PostThreadDto
    {
        public Guid Id { get; set; }
    }
}
