using BasicRedisChat.BLL.Components.Main.ChatСomponent.Entities;
using BasicRedisChat.BLL.Components.Main.UserСomponent.Entities;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.DbContext
{
    public static class DbInitializer
    {
        public static async Task Seed(IServiceScope serviceScope)
        {
            var redis = serviceScope.ServiceProvider.GetService<IConnectionMultiplexer>();
            var redisDatabase = redis.GetDatabase();
            // We store a counter for the total users and increment it on each register
            var totalUsersKeyExist = await redisDatabase.KeyExistsAsync("total_users");
            if (!totalUsersKeyExist)
            {
                // This counter is used for the id
                await redisDatabase.StringSetAsync("total_users", 0);
                // Some rooms have pre-defined names. When the clients attempts to fetch a room, an additional lookup
                // is handled to resolve the name.
                // Rooms with private messages don't have a name
                await redisDatabase.StringSetAsync("room:0:name", "General");

                // Create demo data with the default users
                {
                    var rnd = new Random();
                    Func<double> rand = () => rnd.NextDouble();
                    Func<int> getTimestamp = () => (int)DateTimeOffset.Now.ToUnixTimeSeconds();

                    var demoPassword = "password123";
                    var demoUsers = new List<string>() { "Pablo", "Joe", "Mary", "Alex" };

                    var greetings = new List<string>() { "Hello", "Hi", "Yo", "Hola" };

                    var messages = new List<string>() {
                        "Hello!",
                        "Hi, How are you? What about our next meeting?",
                        "Yeah everything is fine",
                        "Next meeting tomorrow 10.00AM",
                        "Wow that's great"
                    };
                    Func<string> getGreeting = () => greetings[(int)Math.Floor(rand() * greetings.Count)];
                    var addMessage = new Func<string, string, string, int, Task>(async (string roomId, string fromId, string content, int timeStamp) =>
                    {
                        var roomKey = $"room:{roomId}";
                        var message = new ChatRoomMessage()
                        {
                            From = fromId,
                            Date = timeStamp,
                            Message = content,
                            RoomId = roomId
                        };
                        await redisDatabase.SortedSetAddAsync(roomKey, JsonSerializer.Serialize(message), message.Date);
                    });

                    var createUser = new Func<string, string, Task<User>>(async (string username, string password) =>
                    {
                        var usernameKey = $"username:{username}";
                        // Yeah, bcrypt generally ins't used in .NET, this one is mainly added to be compatible with Node and Python demo servers.
                        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                        var nextId = await redisDatabase.StringIncrementAsync("total_users");
                        var userKey = $"user:{nextId}";
                        await redisDatabase.StringSetAsync(usernameKey, userKey);
                        await redisDatabase.HashSetAsync(userKey, new HashEntry[] {
                            new HashEntry("username", username),
                            new HashEntry("password", hashedPassword)
                        });

                        await redisDatabase.SetAddAsync($"user:{nextId}:rooms", "0");

                        return new User()
                        {
                            Id = (int)nextId,
                            Username = username,
                            Online = false
                        };
                    });

                    var getPrivateRoomId = new Func<int, int, string>((user1, user2) =>
                    {
                        var minUserId = user1 > user2 ? user2 : user1;
                        var maxUserId = user1 > user2 ? user1 : user2;
                        return $"{minUserId}:{maxUserId}";
                    });

                    var createPrivateRoom = new Func<int, int, Task<ChatRoom>>(async (user1, user2) =>
                    {
                        var roomId = getPrivateRoomId(user1, user2);

                        await redisDatabase.SetAddAsync($"user:{user1}:rooms", roomId);
                        await redisDatabase.SetAddAsync($"user:{user2}:rooms", roomId);

                        return new ChatRoom()
                        {
                            Id = roomId,
                            Names = new List<string>{
                                (await redisDatabase.HashGetAsync($"user:{user1}", "username")).ToString(),
                                (await redisDatabase.HashGetAsync($"user:{user2}", "username")).ToString(),
                            }
                        };
                    });


                    var users = new List<User>();
                    // For each name create a user.
                    foreach (var demoUser in demoUsers)
                    {
                        var user = await createUser(demoUser, demoPassword);
                        // This one should go to the session
                        users.Add(user);
                    }

                    var rooms = new Dictionary<string, ChatRoom>();
                    foreach (var user in users)
                    {
                        var otherUsers = users.Where(x => x.Id != user.Id);
                        foreach (var otherUser in otherUsers)
                        {
                            var privateRoomId = getPrivateRoomId(user.Id, otherUser.Id);
                            ChatRoom room = null;
                            if (!rooms.ContainsKey(privateRoomId))
                            {
                                room = await createPrivateRoom(user.Id, otherUser.Id);
                                rooms.Add(privateRoomId, room);
                            }
                            else
                            {
                                room = rooms[privateRoomId];
                            }
                            await addMessage(privateRoomId, otherUser.Id.ToString(), getGreeting(), (int)(getTimestamp() - rand() * 222));
                        }
                    }
                    var getRandomUserId = new Func<int>(() => users[(int)Math.Floor(users.Count * rand())].Id);
                    for (var messageIndex = 0; messageIndex < messages.Count; messageIndex++)
                    {
                        await addMessage("0", getRandomUserId().ToString(), messages[messageIndex], getTimestamp() - ((messages.Count - messageIndex) * 200));
                    }
                }

            }
        }
    }
}
