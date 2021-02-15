using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Hubs
{
    public class BaseHub : Hub
    {

        //private readonly IParserManagerService parserManagerService;
        private readonly IUserService userService;

        public BaseHub(IUserService userService)
        {
            this.userService = userService;
        }

        public override async Task OnConnectedAsync()
        {

            var httpContext = Context.GetHttpContext();
            await httpContext.Session.LoadAsync();
            var userStr = httpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(userStr))
            {
                var user = JsonSerializer.Deserialize<UserDto>(userStr);

                await userService.OnStartSession(user);

            }
            else
            {
                await OnDisconnectedAsync(new Exception("Not Authorized"));
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var httpContext = Context.GetHttpContext();
            await httpContext.Session.LoadAsync();
            var userStr = httpContext.Session.GetString("user");
            if (!string.IsNullOrEmpty(userStr))
            {
                var user = JsonSerializer.Deserialize<UserDto>(userStr);

                await userService.OnStopSession(user);

            }

            await base.OnDisconnectedAsync(exception);
        }
    }

}
