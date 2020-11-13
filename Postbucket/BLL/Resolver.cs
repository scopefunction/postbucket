using System;
using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Postbucket.BLL
{
    public static class Resolver
    {
        public static T Get<T>()
        {
            var provider = ConfigureContainer();
            return (T)provider.GetService(typeof(T));
        }

        static IServiceProvider ConfigureContainer()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            return services.BuildServiceProvider();
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DbContext>();
        }
    }
}