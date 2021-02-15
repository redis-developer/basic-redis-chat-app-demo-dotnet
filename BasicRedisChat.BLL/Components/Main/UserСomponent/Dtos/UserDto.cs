using BasicRedisChat.BLL.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos
{
    public class UserDto : BaseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool Online { get; set; } = false;
    }
}
