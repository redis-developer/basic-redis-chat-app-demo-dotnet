using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

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

        private readonly LinksObject _links;
        public LinksController(IFileProvider fileProvider)
        {
            var contents = fileProvider.GetFileInfo("repo.json");
            var stream = contents.CreateReadStream();
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            stream.Close();
            _links = JsonSerializer.Deserialize<LinksObject>(System.Text.Encoding.UTF8.GetString(bytes));
        }

        /// <summary>
        /// This one outputs the url this demo is hosted at, for specifying the GitHub link url.
        /// </summary>
        [HttpGet]
        public LinksObject Get()
        {
            return _links;
        }
    }
}
