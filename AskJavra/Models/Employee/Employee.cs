using AskJavra.Enums;
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
        public required string Designation { get; set; }
        public required string Department { get; set; }
        public required string Country { get; set; }
        public required EmpStatus Status { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required int LMSEmployeeId { get; set; }

    }
}
