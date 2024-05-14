using AskJavra.DataContext;
using AskJavra.Enums;
using AskJavra.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AskJavra.Controllers
{
    [ApiController]
    [Route("api/Account")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        //private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<dynamic> LogIn(LoginModel model)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.UserName);
                    var role = await userManager.GetRolesAsync(user);

                    return new
                    {
                        Id = user.Id,
                        Email = user.Email,
                        role = role.First()
                    };
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<dynamic> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser { UserName = model.UserName, FullName = model.FullName, PhoneNumber = model.PhoneNumber };
                    var existinguser = await userManager.FindByNameAsync(model.UserName);
                    if (existinguser != null)
                    {
                        ModelState.AddModelError("User", "Already exists");
                        return BadRequest(ModelState);
                    }
                    var response = await userManager.CreateAsync(user, model.Password);
                    Enum.TryParse(model.UserType, out UserType myStatus);
                    var data = await userManager.AddToRoleAsync(user, myStatus.ToString());
                    return data.Succeeded;
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAll'")]
        public IActionResult GetAllUsers()
        {
            return Ok(userManager.Users.Select(x => new UserApiModel { Id = x.Id, FullName = x.FullName, UserName = x.UserName, Active = x.Active, PhoneNumber = x.PhoneNumber }));
        }

        [AllowAnonymous]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            return Ok(await userManager.FindByIdAsync(id));
        }


        [AllowAnonymous]
        [HttpPost("update")]
        public async Task<IActionResult> Update([FromBody] UserApiModel user)
        {
            var oldUser = await userManager.FindByIdAsync(user.Id);
            oldUser.UserName = user.UserName;
            oldUser.PhoneNumber = user.PhoneNumber;
            oldUser.Active = user.Active;
            oldUser.FullName = user.FullName;
            var result = await userManager.UpdateAsync(oldUser);
            return Ok(result.Succeeded);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetViewModel resetViewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 403;
                return new JsonResult(new SerializableError(ModelState).ToList());
            }
            var user = await userManager.FindByIdAsync(resetViewModel.Id);
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, resetViewModel.Password);
            return Ok(result.Succeeded);
        }

        [AllowAnonymous]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var result = await userManager.DeleteAsync(user);
            return Ok(result.Succeeded);
        }
    }
}
