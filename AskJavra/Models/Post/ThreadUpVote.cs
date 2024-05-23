using AskJavra.DataContext;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Post
{
    public class ThreadUpVote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public Guid ThreadId { get; set; }
        public virtual PostThread Thread { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
