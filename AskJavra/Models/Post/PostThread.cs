using AskJavra.Models.Root;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Post
{
    public class PostThread: RootAuditEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string ThreadTitle { get; set; }
        public string? ThreadDescription { get; set; }
        public bool IsSolution { get; set; } = false;
        public Guid PostId { get; set; }
        public Post Post { get; set; }
        public PostThread() {
            ThreadTitle = string.Empty;
            Post = new Post();
        }
        public PostThread(string threadTitle, string threadDescription,Guid postId, Post post)
        {
            Id = Guid.NewGuid();
            ThreadTitle = threadTitle;
            ThreadDescription = threadDescription;
            PostId = postId;
            Post = post;
        }
    }
}
