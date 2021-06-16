using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using StackExchange.Redis;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.Controllers.Base;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using BasicRedisChat.BLL.Helpers;

namespace BasicRedisChat.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ISecurityService securityService;

        public AuthController(ISecurityService securityService)
        {
            this.securityService = securityService;
        }

        public class LoginData
        {
            [Required]
            public string username { get; set; }
            [Required]
            public string password { get; set; }
        }

        /// <summary>
        /// Create user session by username and password.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            
            var user = await securityService.Login(userLoginDto);
            if (user == null)
            {
                return Unauthorized();
            }

            var res = user.ToDto<UserDto>();

            await HttpContext.Session.LoadAsync();
            
            var userString = JsonSerializer.Serialize(res);
            HttpContext.Session.SetString("user", userString);
            await HttpContext.Session.CommitAsync();

            return Ok(res);
        }

        /// <summary>
        /// Dispose the user session.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await securityService.Logout(HttpContext);
            return Ok();
        }
    }
}
