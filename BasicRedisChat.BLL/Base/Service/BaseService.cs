using BasicRedisChat.BLL.Base.Service.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisChat.BLL.Base.Service
{
    public abstract class BaseService : IBaseService
    {
        protected readonly IDatabase _database;
        protected readonly IConnectionMultiplexer _redis;

        public BaseService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        protected async Task PublishMessage<T>(string type, T data)
        {
            // That's a very quick and dirty way to handle the json type serialization...
            var jsonData = JsonConvert.SerializeObject(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            //var jsonData = JsonConvert.DeserializeObject<JsonDocument>(dataString);

            var pubSubMessage = new PubSubMessage()
            {
                Type = type,
                Data = jsonData
            };

            await PublishMessage(pubSubMessage);
        }

        private async Task PublishMessage(PubSubMessage pubSubMessage)
        {
            await _database.PublishAsync("MESSAGES", JsonConvert.SerializeObject(pubSubMessage));
        }
    }

    public class PubSubMessage
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public string ServerId { get; set; } = "123";
    }

}

