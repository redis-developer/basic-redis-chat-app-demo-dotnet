using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos
{
    public class UserLoginDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
