using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Services.Interfaces;
using BasicRedisChat.Controllers.Base;
using System.Linq;
using BasicRedisChat.BLL.Helpers;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Dtos;

namespace BasicRedisChat.Controllers
{
    /// <summary>
    /// Used to retrieve the room-related data.
    /// </summary>
    public class RoomsController : ApiController
    {
        private readonly IChatService _chatService;

        public RoomsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Get rooms for specific user id.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRoom(int userId = 0)
        {
            var rooms = await _chatService.GetRooms(userId);
            return Ok(rooms.Select(x => x.ToDto<ChatRoomDto>()));
        }

        /// <summary>
        /// Get Messages.
        /// </summary>
        [HttpGet("messages/{roomId}")]
        public async Task<IActionResult> GetMessages(string roomId = "0", int offset = 0, int size = 50)
        {
            var messages = await _chatService.GetMessages(roomId, offset, size);
            return Ok(messages.Select(x => x.ToDto<ChatRoomMessageDto>()));
        }
    }
}

