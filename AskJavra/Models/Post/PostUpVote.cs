using AskJavra.DataContext;
using AskJavra.Models.Root;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Post
{
    public class PostUpVote :RootAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
