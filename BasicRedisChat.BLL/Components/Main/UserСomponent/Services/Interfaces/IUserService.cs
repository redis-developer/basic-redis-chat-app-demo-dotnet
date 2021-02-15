using BasicRedisChat.Base.Interfaces;
using BasicRedisChat.BLL.Base.Service.Interfaces;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces
{
    public interface IUserService : IBaseService
    {
        Task<IDictionary<string, UserDto>> Get(int[] ids);
        Task<IDictionary<string, UserDto>> GetOnline();

        Task OnStartSession(UserDto user);
        Task OnStopSession(UserDto user);
    }
}
