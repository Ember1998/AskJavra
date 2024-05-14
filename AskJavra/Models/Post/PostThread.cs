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
        public required string ThreadTitle { get; set; }
        public string? ThreadDescription { get; set; }
        public bool IsSolution { get; set; }
        public Guid PostId { get; set; }
        public required Post Post { get; set; }
    }
}
