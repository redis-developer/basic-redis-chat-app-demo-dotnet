using BasicRedisChat.Base.Interfaces;
using BasicRedisChat.BLL.Base.Service.Interfaces;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces
{
    public interface ISecurityService : IBaseService
    {
        Task<User> Login(UserLoginDto userLoginDto);
        Task Logout(HttpContext httpContext);
    }
}
