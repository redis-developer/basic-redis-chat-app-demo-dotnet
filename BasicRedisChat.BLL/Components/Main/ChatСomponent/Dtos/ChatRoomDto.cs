using BasicRedisChat.BLL.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos
{
    public class ChatRoomDto : BaseDto
    {
        public string Id { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
