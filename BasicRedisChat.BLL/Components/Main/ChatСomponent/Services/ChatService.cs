using BasicRedisChat.BLL.Base.Service;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.ChatСomponent.Services.Interfaces;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Dtos;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Components.Main.ChatСomponent.Services
{
    public class ChatService : BaseService, IChatService
    {
        public ChatService(IConnectionMultiplexer redis) : base(redis)
        {
        }

        public async Task<List<ChatRoomMessage>> GetMessages(string roomId = "0", int offset = 0, int size = 50)
        {
            var roomKey = $"room:{roomId}";
            var roomExists = await _database.KeyExistsAsync(roomKey);
            var messages = new List<ChatRoomMessage>();

            if (!roomExists)
            {
                return messages;
            }
            else
            {
                var values = await _database.SortedSetRangeByRankAsync(roomKey, offset, offset + size, Order.Descending);

                foreach (var valueRedisVal in values)
                {
                    var value = valueRedisVal.ToString();
                    try
                    {
                        messages.Add(JsonConvert.DeserializeObject<ChatRoomMessage>(value));
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // Console.WriteLine($"Couldn't deserialize json: {value}");
                    }
                }
                return messages;
            }
        }

        public async Task<List<ChatRoom>> GetRooms(int userId = 0)
        {
            var roomIds = await _database.SetMembersAsync($"user:{userId}:rooms");
            var rooms = new List<ChatRoom>();
            foreach (var roomIdRedisValue in roomIds)
            {
                var roomId = roomIdRedisValue.ToString();
                var name = await _database.StringGetAsync($"room:{roomId}:name");
                if (name.IsNullOrEmpty)
                {
                    // It's a room without a name, likey the one with private messages 
                    var roomExists = await _database.KeyExistsAsync($"room:{roomId}");
                    if (!roomExists)
                    {
                        continue;
                    }

                    var userIds = roomId.Split(':');
                    if (userIds.Length != 2)
                    {
                        throw new Exception("You don't have access to this room");
                    }

                    rooms.Add(new ChatRoom()
                    {
                        Id = roomId,
                        Names = new List<string>() {
                            (await _database.HashGetAsync($"user:{userIds[0]}", "username")).ToString(),
                            (await _database.HashGetAsync($"user:{userIds[1]}", "username")).ToString(),
                        }
                    });
                }
                else
                {
                    rooms.Add(new ChatRoom()
                    {
                        Id = roomId,
                        Names = new List<string>() {
                            name.ToString()
                        }
                    });
                }
            }
            return rooms;
        }

        public async Task SendMessage(UserDto user, ChatRoomMessage message)
        {
            await _database.SetAddAsync("online_users", message.From);
            var roomKey = $"room:{message.RoomId}";
            await _database.SortedSetAddAsync(roomKey, JsonConvert.SerializeObject(message), (double)message.Date);
            await PublishMessage("message", message);
        }
    }
}
