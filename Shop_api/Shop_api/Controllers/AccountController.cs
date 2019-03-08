using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shop_api.Data;
using Shop_api.Models;

namespace Shop_api.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        //private RoleManager<User> _roleManager;
        //private readonly IJwtFactory _jwtFactory;
        //private readonly JwtIssuerOptions _jwtOptions;
        IAuthenticationSchemeProvider _a;

        public AccountController(
                IAuthenticationSchemeProvider a,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                IConfiguration configuration
                //RoleManager<User> roleManager,
                //IJwtFactory jwtFactory,
                //JwtIssuerOptions jwtOptions
            )
        {
            _a = a;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            //_roleManager = roleManager;
            //_jwtFactory = jwtFactory;
        }

        [HttpPost("api/[controller]/Register")]
        public async Task<ActionResult<object>> Register([FromBody] RegisterViewModel model)
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return GenerateJwtToken(model.Email, user);
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("api/[controller]/Login/{provider?}")]
        public async Task<IActionResult> Login(string provider = "GitHub")
        {
            return Challenge(provider);
        }
                  
        [HttpGet("SignInGithub")]
        public async Task<IActionResult> SignInGithub(string code)
        {
            //return Challenge(new AuthenticationProperties { RedirectUri = "https://localhost:44397/api/account/SignInGithu" }, "GitHub");
            //return Challenge("GitHub");
            return Ok(code);
        }

        [HttpGet("api/Account/SignInGithub")]
        public async Task<IActionResult> SignInGithub2(string code)
        {
            //return Challenge(new AuthenticationProperties { RedirectUri = "https://localhost:44397/api/account/SignInGithu" }, "GitHub");
            //return Challenge("GitHub");
            return Ok();
        }

        [HttpGet("tst")]
        public async Task<IActionResult> tst(string t)
        {
            //List<AuthenticationScheme> listAS = new List<AuthenticationScheme>();
            //foreach (AuthenticationScheme p in await _a.GetRequestHandlerSchemesAsync())
            //{
            //    listAS.Add(p);
            //}
            //return Ok(listAS);
            //return Challenge(new AuthenticationProperties {RedirectUri = "/"}, "GitHub");
            return Ok(t);
        }

        /// <summary>
        /// JWT auth
        /// </summary>
        /// <param name="email"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        //[HttpPost("Login")]
        //public async Task<ActionResult> Login([FromBody] User model)
        //{
        //    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.PasswordHash, false /* remember me*/, false);
        //    if (result.Succeeded)
        //    {
        //        var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.UserName);
        //        return Ok(GenerateJwtToken(appUser.Email, appUser));
        //    }
        //    return BadRequest();
        //}

        [HttpGet("asdfasd")]
        public Object GenerateJwtToken(string email, User user)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


    // удаление пользователя по id
    // изменение пользователя по id
    // авторизация по имени пользователя и паролю
    // регистрация нового пользователя



}
