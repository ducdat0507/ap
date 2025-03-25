using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RemoteCheckup.DTOs;
using RemoteCheckup.Models;

namespace RemoteCheckup.Controllers
{
    [Route("api/auth")] [ApiController] [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            var result = await _signInManager.PasswordSignInAsync(info.Username, info.Password, false, false);

            if (result.Succeeded) return Ok("Access granted");
            return Unauthorized("Access denied");
        }
    }
}
