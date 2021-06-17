using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicRedisChat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LinksController
    {
        public class LinksObject
        {
            public string github { get; set; }
        }

        /// <summary>
        /// This one outputs the url this demo is hosted at, for specifying the GitHub link url.
        /// </summary>
        [HttpGet]
        public async Task<LinksObject> Get()
        {
            return JsonSerializer.Deserialize<LinksObject>(await File.ReadAllTextAsync("repo.json"));
        }
    }
}
