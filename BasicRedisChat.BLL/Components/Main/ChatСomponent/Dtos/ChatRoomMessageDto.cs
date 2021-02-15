using BasicRedisChat.BLL.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos
{
    public class ChatRoomMessageDto : BaseDto
    {
        public string From { get; set; }
        public int Date { get; set; }
        public string Message { get; set; }
        public string RoomId { get; set; }
    }
}
