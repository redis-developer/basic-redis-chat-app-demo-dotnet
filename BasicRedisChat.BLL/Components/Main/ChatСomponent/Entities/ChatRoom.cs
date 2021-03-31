using BasicRedisChat.DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities
{
    public class ChatRoom : BaseEntity
    {
        public string Id { get; set; }
        public IEnumerable<string> Names { get; set; }
    }
}
