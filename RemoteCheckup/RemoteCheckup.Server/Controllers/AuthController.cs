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

        [HttpGet("whoami")]
        [AllowAnonymous]
        public async Task<IActionResult> Lookup()
        {
            string name;
            if (
                HttpContext.User == null ||
                (name = (await _userManager.GetUserAsync(HttpContext.User!))?.UserName) == null
            ) {
                return Unauthorized(new {
                    loggedIn = false,
                    name = "",
                });
            }
            return Ok(new {
                loggedIn = true,
                name,
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            var result = await _signInManager.PasswordSignInAsync(info.Username, info.Password, info.RememberMe, false);

            if (result.Succeeded) return Ok(new {
                message = "Access granted"
            });
            return Unauthorized(new {
                message = "Access denied"
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new {
                message = "Goodbye"
            });
        }
    }
}
