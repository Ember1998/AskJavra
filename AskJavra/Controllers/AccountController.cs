using AskJavra.DataContext;
using AskJavra.Enums;
using AskJavra.Models.Contribution;
using AskJavra.ViewModels;
using AskJavra.ViewModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AskJavra.Controllers
{
    [ApiController]
    [Route("api/Account")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDBContext _dbContext;
        private readonly DbSet<ContributionRank> _dbSetRank;
        private readonly DbSet<ContributionPoint> _dbSetPoint;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDBContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _dbContext = dbContext;
            _dbSetRank = _dbContext.Set<ContributionRank>();
            _dbSetPoint = _dbContext.Set<ContributionPoint>();
        }
        [AllowAnonymous]
        [HttpPost("login")]
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
                        role = role.First(),
                        firstLogin = !user.Active
                    };
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task AddTokenExpirationInfo(ApplicationUser user, int span = 1 * 24 * 60)
        {
            var expiresAt = DateTime.Now.Add(TimeSpan.FromMinutes(span));
            var tokenExpiredAtClaim = new Claim("ActivtationTokenExpiredAt", expiresAt.ToUniversalTime().Ticks.ToString());
            await userManager.AddClaimAsync(user, tokenExpiredAtClaim);
        }

        private async Task<bool> TokenExpiredValidate(ApplicationUser user)
        {
            var claims = (await userManager.GetClaimsAsync(user))
                .Where(c => c.Type == "ActivtationTokenExpiredAt");
            var expiredAt = claims.FirstOrDefault()?.Value;
            bool expired = true;    // default value
            if (expiredAt != null)
            {
                var expires = Convert.ToInt64(expiredAt);
                var now = DateTime.Now.Ticks;
                expired = now <= expires ? false : true;
            }
            else
            {
                expired = false;
            }
            // clear claims
            await userManager.RemoveClaimsAsync(user, claims);
            return expired;
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
                    await AddTokenExpirationInfo(user);

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
        [HttpGet("GetAll")]
        public IActionResult GetAllUsers()
        {
            return Ok(userManager.Users.Select(x => new UserApiModel { 
                Id = x.Id, FullName = x.FullName, UserName = x.UserName, Active = x.Active, PhoneNumber = x.PhoneNumber, Department = x.Department, Email = x.Email }));
        }

        [AllowAnonymous]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            UserWithRankDto dto = new UserWithRankDto();

            dto.User =  await userManager.FindByIdAsync(id);
            dto.RankDetails = await GetTotalrank();
            dto.UserRank = await GetUserTotalPoint(id);

            return Ok(dto);
        }
        private async Task<List<RankDetails>> GetTotalrank()
        {
            try
            {
                return await _dbSetRank.Select(x => new RankDetails
                {
                    MaxPoint = x.RankMaxPoint,
                    MinPoint = x.RankMinPoint,
                    RankName = x.RankName
                }).ToListAsync();
            }
            catch
            {
                return new List<RankDetails>();
            }
        }
        private async Task<UserRankDetails> GetUserTotalPoint(string userId)
        {
            try
            {
                var result = _dbSetPoint.Where(x => x.UserId == userId).Select(x => x.Point).ToArray();
                int total_point = result.Sum();
                //var resultttt = await _dbSetRank.Where(x => x.RankMinPoint <= total_point && x.RankMaxPoint >= total_point).Select(y => new UserRankDetails
                //{
                //    RankName = y.RankName,
                //    TotalPoint = total_point
                //}).FirstOrDefaultAsync();
                return await _dbSetRank.Where(x => x.RankMinPoint <= total_point && x.RankMaxPoint >= total_point).Select(y => new UserRankDetails
                {
                    RankName = y.RankName,
                    TotalPoint = total_point
                }).FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                return new UserRankDetails();
            }
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
            user.Active = true;
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
