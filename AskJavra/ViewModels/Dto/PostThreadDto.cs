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
        public PostThreadDto(string threadTitle, string? threadDescription, Guid postId)
        {
           ThreadTitle = threadTitle;
           ThreadDescription = threadDescription;                
           PostId = postId;
        }
        public PostThreadDto() {  }
    }
    public class PostThreadAndIdDto : PostThreadDto
    {
        public Guid Id { get; set; }
    }
}
