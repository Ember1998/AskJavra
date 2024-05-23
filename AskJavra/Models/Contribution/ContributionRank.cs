using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Contribution
{
    public class ContributionRank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; }
        public string? Description { get; set; }
        public int RankMinPoint { get; set; }
        public int RankMaxPoint { get; set; }
    }
}
