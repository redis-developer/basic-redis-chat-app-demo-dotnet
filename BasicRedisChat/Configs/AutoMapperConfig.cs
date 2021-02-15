using AutoMapper;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicRedisChat.Configs
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<User, UserDto>();
            CreateMap<ChatRoom, ChatRoomDto>();
            CreateMap<ChatRoomMessage, ChatRoomMessageDto>();
        }
    }
}
