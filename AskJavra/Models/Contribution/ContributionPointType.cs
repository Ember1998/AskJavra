using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AskJavra.Models.Contribution
{
    public class ContributionPointType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Point { get; set; }
        public virtual ICollection<ContributionPoint> Points { get; set; }
    }
}
