using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace BasicRedisChat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Accept the PORT environment variable to enable Cloud Run/Heroku support.
            var customPort = Environment.GetEnvironmentVariable("PORT");
            if (customPort != null)
            {
                string url = String.Concat("http://0.0.0.0:", customPort);

                return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().UseUrls(url);
                });
            }
            else
            {

                return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            }
        }
    }
}
