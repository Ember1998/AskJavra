using AskJavra.DataContext;
using AskJavra.Enums;
using AskJavra.Models;
using AskJavra.Models.Employee;
using AskJavra.Services;
using AskJavra.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AskJavra.Repositories
{
    public class LMSSyncRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender _emailSender;

        public LMSSyncRepository(ApplicationDBContext applicationDBContext, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _applicationDBContext = applicationDBContext;
            this.userManager = userManager;
            _emailSender = emailSender;

        }
        public async Task SyncLMSdata(UserInfos userInfos)
        {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            foreach (var userInfo in userInfos.UserInfo)
            {
                var user = new ApplicationUser { UserName = userInfo.Username, FullName = userInfo.FullName, PhoneNumber = userInfo.Phone, Email = $"{userInfo.FirstName.ToLower()}.{userInfo.LastName.ToLower()}@javra.com", Department = userInfo.Department };
                var existinguser = await userManager.FindByNameAsync(userInfo.Username);
                if (existinguser == null)
                {
                    var response = await userManager.CreateAsync(user, $"{ti.ToTitleCase(userInfo.FirstName)}@{userInfo.EmployeeID}");
                    if (response.Succeeded)
                    {
                        if (userInfo.Username == "sasin113" || userInfo.Username == "shdha092" || userInfo.Username == "samah013" || userInfo.Username == "yamah022" || userInfo.Username == "susun022")
                        {
                            await ConfigureSyncEmail(user.Email, user.UserName, $"{ti.ToTitleCase(userInfo.FirstName)}@{userInfo.EmployeeID}");
                        }
                        await userManager.AddToRoleAsync(user, "Employee");
                    }
                }
               
            }
            var Employees = userInfos.UserInfo.Select(x => new Employee
            {
                Username = x.Username,
                Status = (EmpStatus)Enum.Parse(typeof(EmpStatus), x.Status, true),
                Country = x.Country,
                Department = x.Department,
                Name = x.FullName,
                Designation = x.Designation,
                LMSEmployeeId = x.EmployeeID,
                Email = x.Email
            });

            _applicationDBContext.AddRange(Employees.Where(x=> !_applicationDBContext.Employees.Any(y=>y.LMSEmployeeId == x.LMSEmployeeId)));
            _applicationDBContext.SaveChanges();

        }

        public Task ConfigureSyncEmail(string email, string Username, string password)
        {
            var subject = "AskJavra user credentials";
            var htmlMessage = $"Your username is : {Username} and password is: {password}";

            return _emailSender.SendEmailAsync(email, subject, htmlMessage);
        }

       
    }
}
