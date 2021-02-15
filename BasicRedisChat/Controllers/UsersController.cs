using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces;
using BasicRedisChat.Controllers.Base;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;

namespace BasicRedisChat.Controllers
{
    /// <summary>
    /// This controller handles user state. We don't apploy complete authorization with routes protection.
    /// The session simply stores the basic user data which is used for chat communication
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ApiController
    {
        private IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// Retrieve the user info based on ids sent.
        /// </summary>
        [HttpGet]
        public async Task<IDictionary<string, UserDto>> Get([FromQuery(Name = "ids[]")] int[] ids)
        {
            return await userService.Get(ids);
        }

        /// <summary>
        /// Check which users are online.
        /// </summary>
        [HttpGet("online")]
        public async Task<IDictionary<string, UserDto>> GetOnline()
        {
            return await userService.GetOnline();
        }

        /// <summary>
        /// The request the client sends to check if it has the user is cached.
        /// </summary>
        [HttpGet("me")]
        public async Task<User> GetMe()
        {

            await HttpContext.Session.LoadAsync();

            string userString = HttpContext.Session.GetString("user");

            if (userString != null && userString != "")
            {
                var user = JsonSerializer.Deserialize<User>(userString);
                if (user != null)
                {
                    return user;
                }
            }

            return null;
        }
    }
}
