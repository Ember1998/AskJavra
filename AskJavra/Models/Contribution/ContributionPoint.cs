using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AskJavra.DataContext;

namespace AskJavra.Models.Contribution
{
    public class ContributionPoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ContributionPointTypeId { get; set; }
        public virtual ContributionPointType ContributionPointType { get; set;}
        public virtual ApplicationUser User { get; set; }

        public int Point { get; set; }
    }
}
