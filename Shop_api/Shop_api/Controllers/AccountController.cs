using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private RoleManager<IdentityRole> _roleManager;
        //private readonly IJwtFactory _jwtFactory;
        //private readonly JwtIssuerOptions _jwtOptions;
        IAuthenticationSchemeProvider _a;

        public AccountController(
                IAuthenticationSchemeProvider a,
                UserManager<User> userManager,
                SignInManager<User> signInManager,
                IConfiguration configuration,
                RoleManager<IdentityRole> roleManager
            //IJwtFactory jwtFactory,
            //JwtIssuerOptions jwtOptions
            )
        {
            _a = a;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            //_jwtFactory = jwtFactory;
        }

        [HttpPost("api/[controller]/Register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
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
                return Ok(new {
                    access_token = GenerateJwtToken(model.Email, user),
                    user_name = user.UserName,
                    user_email = user.Email
                });
            }

            return BadRequest(result.Errors);
        }

        /// <summary>
        /// JWT auth
        /// </summary>
        /// <param name="email"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("api/Account/Login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false /* remember me*/, false);
            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return Ok(GenerateJwtToken(appUser.Email, appUser));
            }
            return BadRequest();
        }

        [HttpGet("api/Account/SignInGoogle/{access_token}")]
        public async Task<ActionResult<object>> _signInGoogle(string access_token)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={access_token}");
            using (HttpContent content = response.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    JObject dataObj = JsonConvert.DeserializeObject(data) as JObject;
                    User _user = await _userManager.FindByEmailAsync(dataObj.GetValue("email").ToString());
                    if (_user != null)
                    {
                        string password = dataObj.GetValue("id").ToString() + "123qweASD!@#";
                        // Залогинить _user по JWT
                        await _signInManager.SignInAsync(_user, true);
                        return Ok(new {
                            access_token = GenerateJwtToken(_user.Email, _user).ToString(),
                            user_name = _user.UserName,
                            user_email = _user.Email
                        });
                    }
                    else
                    {
                        _user = new User()
                        {
                            Email = dataObj.GetValue("email").ToString(),
                            UserName = dataObj.GetValue("name").ToString()
                        };
                        string password = dataObj.GetValue("id").ToString() + "123qweASD!@#";
                        await _userManager.CreateAsync(_user, password);
                        await _userManager.AddToRoleAsync(_user, "Client");
                        // Залогинить _user по JWT
                        await _signInManager.SignInAsync(_user, true);
                        return JsonConvert.SerializeObject(GenerateJwtToken(_user.Email, _user));
                    }
                }
            }
            return BadRequest("Ошибка");
        }



        //[Authorize]
        [HttpGet("test")]
        public async Task<IActionResult> tst()
        {
            //string token = HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "Authorization").Value.ToString().Split(" ")[1];
            //string email = tokenParse(token).Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value.ToString();
            //return Ok(email);

            return Content("created!!!");

        }

        [NonAction]
        public JwtSecurityToken tokenParse(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(token);

        }

        [NonAction]
        public ActionResult getNameByToken(JwtSecurityToken token)
        {
            return Ok(token);
        }

        [NonAction]
        public Object GenerateJwtToken(string email, User user)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
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
