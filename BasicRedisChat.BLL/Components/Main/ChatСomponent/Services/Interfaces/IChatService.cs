using BasicRedisChat.BLL.Base.Service.Interfaces;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Services.Interfaces
{
    public interface IChatService : IBaseService
    {
        Task<List<ChatRoom>> GetRooms(int userId = 0);
        Task<List<ChatRoomMessage>> GetMessages(string roomId = "0", int offset = 0, int size = 50);
        Task SendMessage(UserDto user, ChatRoomMessage message);
    }
}
