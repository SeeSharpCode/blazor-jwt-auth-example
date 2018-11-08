using BlazorJwtAuthentication.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorJwtAuthentication.Server.Controllers
{
    [Route("api/accounts")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] LoginViewModel loginViewModel)
        {
            IdentityUser user = new IdentityUser(loginViewModel.UserName);
            var result = await _userManager.CreateAsync(user, loginViewModel.Password);
            if (!result.Succeeded) return StatusCode(500);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LogIn([FromBody] LoginViewModel loginViewModel)
        {
            IdentityUser user = await _userManager.FindByNameAsync(loginViewModel.UserName);
            if (user == null) return BadRequest();
            
            var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
            if (!result.Succeeded) return StatusCode(500);

            var claims = new[] 
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my-secret-key-which-is-so-big-123123123213123"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("http://localhost:57080/", "http://localhost:57080/", claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);            
            
            return Ok(new LoginResponse { Token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}