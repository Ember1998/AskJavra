using System.ComponentModel.DataAnnotations;

namespace AskJavra.ViewModels
{
    public class ResetViewModel
    {
        public string Id { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Invalid password, min length : 6")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password doesn't match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

    }
}
