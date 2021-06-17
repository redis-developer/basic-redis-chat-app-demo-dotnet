using BasicRedisChat.BLL.Base.Service;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Services
{
    public class SecurityService : BaseService, ISecurityService
    {
        public SecurityService(IConnectionMultiplexer redis) : base(redis)
        {
        }

        public async Task<User> Login(UserLoginDto userLoginDto)
        {
            var usernameKey = $"username:{userLoginDto.Username}";
            var userExists = await _database.KeyExistsAsync(usernameKey);
            if (userExists)
            {
                var userKey = (await _database.StringGetAsync(usernameKey)).ToString();
                var userId = int.Parse(userKey.Split(':').Last());
                return  new User()
                {
                    Username = userLoginDto.Username,
                    Id = userId,
                    Online = true
                };
            } else
            {
                return null;
            }
        }

        public async Task Logout(HttpContext httpContext)
        {
            httpContext.Session.Remove("user");
            await httpContext.Session.CommitAsync();
        }
    }
}
