using BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Services.Interfaces;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Hubs
{
    public class ChatHub : BaseHub
    {
        private readonly IChatService chatService;
        public ChatHub(IUserService userService, IChatService chatService) : base(userService)
        {
            this.chatService = chatService;
        }

        public async Task SendMessage(string userString, string messageString)
        {
            var message = JsonConvert.DeserializeObject<ChatRoomMessage>(messageString);

            var user = JsonConvert.DeserializeObject<UserDto>(userString);
            
            await chatService.SendMessage(user, message);
        }
    }
}
