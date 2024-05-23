using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Contribution
{
    public class ContributionPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ContributionPointId { get; set; }
        public ContributionPointType ContributionPointType { get; set;}
        public int Point { get; set; }
    }
}
