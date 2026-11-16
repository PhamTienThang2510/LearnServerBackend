using System;
using Microsoft.Extensions.DependencyInjection;
using LearnBackendSource;
using LearnBackendSource.Packets;
using LearnBackendSource.Handlers;

namespace Infrastructure
{
    public static class Container
    {
        public static IServiceProvider Configure()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            return services.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<NetworkServer>();
            services.AddSingleton<PacketRegistry>();
            
            // Handlers
            services.AddScoped<AuthRequestHandler>();
        }
    }
}
