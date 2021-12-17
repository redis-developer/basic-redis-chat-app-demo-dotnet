using AutoMapper;
using BasicRedisChat.BLL.Base.Service;
using BasicRedisChat.BLL.DbContext;
using BasicRedisChat.BLL.Helpers;
using BasicRedisChat.BLL.Hubs;
using BasicRedisChat.Configs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.IO;
using System.Reflection;

namespace BasicRedisChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasicRedisChat", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            ConnectionMultiplexer redis = null;
            string redisConnectionUrl = null;
            {
                var redisSettings = new RedisSettings();
                ConfigurationBinder.Bind(Configuration.GetSection("RedisSettings"), redisSettings);

                if (redisSettings != null)
                {
                    redisConnectionUrl = $"{redisSettings.Url}:{redisSettings.Port},password={redisSettings.Password}";
                }
                else
                {
                    var redisEndpointUrl = (Environment.GetEnvironmentVariable("REDIS_ENDPOINT_URL") ?? "127.0.0.1:6379").Split(':');
                    var redisHost = redisEndpointUrl[0];
                    var redisPort = redisEndpointUrl[1];

                    var redisPassword = Environment.GetEnvironmentVariable("REDIS_PASSWORD");
                    if (redisPassword != null)
                    {
                        redisConnectionUrl = $"{redisHost},password={redisPassword}";
                    }
                    else
                    {
                        redisConnectionUrl = $"{redisHost}:{redisPort}";
                    }
                }

                redis = ConnectionMultiplexer.Connect(redisConnectionUrl);
                services.AddSingleton<IConnectionMultiplexer>(redis);
            }

            services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtectionKeys");

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = redisConnectionUrl;
                option.InstanceName = "RedisInstance";
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = "AppTest";
            });

            Assembly.Load("BasicRedisChat.BLL");
            ServiceAutoConfig.Configure(services);
            services.AddAutoMapper(typeof(Startup));

            services.AddHttpContextAccessor();

            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));

            services.AddSignalR();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "dotnet_rest v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.Map(new PathString(""), client =>
            {
                var clientPath = Path.Combine(Directory.GetCurrentDirectory(), "./client/build");
                StaticFileOptions clientAppDist = new StaticFileOptions()
                {
                    FileProvider = new PhysicalFileProvider(clientPath)
                };
                client.UseSpaStaticFiles(clientAppDist);
                client.UseSpa(spa => { spa.Options.DefaultPageStaticFileOptions = clientAppDist; });

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<ChatHub>("/chat");
                    endpoints.MapControllers();
                });
            });


            IHubContext<ChatHub> chatHab = null;
            IConnectionMultiplexer redis = null;
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                DbInitializer.Seed(serviceScope).Wait();
                ConvertToDto.Mapper = serviceScope.ServiceProvider.GetService<IMapper>();
                chatHab = serviceScope.ServiceProvider.GetService<IHubContext<ChatHub>>();
                redis = serviceScope.ServiceProvider.GetService<IConnectionMultiplexer>();
            }

            var channel = redis.GetSubscriber().Subscribe("MESSAGES");
            channel.OnMessage(async message =>
            {
                try
                {
                    var mess = JsonConvert.DeserializeObject<PubSubMessage>(message.Message.ToString());
                    await chatHab.Clients.All.SendAsync(mess.Type, mess.Data);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e} ");
                }
            });

        }
    }
}
