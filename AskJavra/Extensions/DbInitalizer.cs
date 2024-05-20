using AskJavra.DataContext;
using AskJavra.Enums;
using Microsoft.AspNetCore.Identity;

namespace AskJavra.Extensions
{
    public static class DbInitalizer
    {
        public static async Task InitalizeAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager
            )
        {
            //Seeding Role
            //var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var roles = Enum.GetValues(typeof(UserType)).Cast<UserType>();
            IdentityResult identityResult;
            foreach (var item in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(item.ToString());
                if (!roleExists)
                {
                    identityResult = await roleManager.CreateAsync(new IdentityRole(item.ToString()));
                }
            }

            //Seeding user
            //var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var existingUser = await userManager.FindByNameAsync("Javra");
            if (existingUser == null)
            {
                var user = new ApplicationUser()
                {
                    UserName = "Javra",
                    FullName = "Javra Solutions",
                    EmailConfirmed = true,
                    Department = "",
                    Email = "",
                    Active = true
                };
                var result = await userManager.CreateAsync(user, "Javra@123");
                await userManager.AddToRoleAsync(user, UserType.Admin.ToString());
            }
        }
    }
}
