using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AskJavra.Models.Employee
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Name { get; set; }
        public Designation? Designation { get; set; }
        public Department? Department { get; set; }
        public required string Country { get; set; }

    }
}
