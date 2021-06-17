using BasicRedisChat.Base.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace BasicRedisChat.Configs
{
    public class ServiceAutoConfig
    {
        public static void Configure(IServiceCollection services)
        {
            var service = typeof(IService);
            var singletonService = typeof(ISingletonService);
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => (service.IsAssignableFrom(p) || singletonService.IsAssignableFrom(p)) && p.IsClass && !p.IsAbstract).ToList();
            types.ForEach(c =>
            {
                var originInterfaces = c.GetInterfaces();
                var isSingleton = originInterfaces.Any(i => singletonService.IsAssignableFrom(i));
                var interfaces = originInterfaces.Where(x =>
                        x.Name != service.Name ||
                        x.Name != singletonService.Name
                    ).ToList();

                interfaces.ForEach(i =>
                {
                    if (!isSingleton)
                    {
                        services.AddTransient(i, c);
                    }
                    else
                    {
                        services.AddSingleton(i, c);
                    }
                });
            });
        }
    }
}
