using BasicRedisChat.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities
{
    public class ChatRoomMessage : BaseEntity
    {
        public string From { get; set; }
        public int Date { get; set; }
        public string Message { get; set; }
        public string RoomId { get; set; }
    }
}
