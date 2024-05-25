using Microsoft.AspNetCore.Identity;

namespace AskJavra.DataContext
{
    public class ApplicationUser:IdentityUser
    {
        public ApplicationUser()
        {
            
        }
        public bool Active { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string? ProfilePicPath { get; set; }
    }
}
