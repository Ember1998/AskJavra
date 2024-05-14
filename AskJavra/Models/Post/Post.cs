using AskJavra.Enums;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Post
{
    public class Post : RootAuditEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public PostType PostType { get; set; }
        public virtual ICollection<PostThread> Threads { get; set; }= new List<PostThread>();
        public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
