using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PosSystem.Data;

namespace PosSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("phone-verify")]
        public async Task<IActionResult> PhoneVerify([FromBody] PhoneVerifyModel model)
        {
            // Find user (you can add real OTP check here later)
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            // In real app: validate OTP here

            await _signInManager.SignInAsync(user, isPersistent: model.RememberMe);
            return Ok(new { redirectUrl = "/dashboard" });
        }

        [HttpPost("email-login")]
        public async Task<IActionResult> EmailLogin([FromBody] EmailLoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { redirectUrl = "/dashboard" });
            }

            return BadRequest("Invalid login attempt");
        }
    }

    public class PhoneVerifyModel
    {
        public string PhoneNumber { get; set; } = "";
        public string Otp { get; set; } = "";
        public bool RememberMe { get; set; }
    }

    public class EmailLoginModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public bool RememberMe { get; set; }
    }
}
