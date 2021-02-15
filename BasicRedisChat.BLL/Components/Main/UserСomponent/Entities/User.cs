using BasicRedisChat.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Online { get; set; } = false;
    }
}
