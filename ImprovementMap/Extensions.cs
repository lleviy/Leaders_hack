using ImprovementMap.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace ImprovementMap
{
    public static class Extensions
    {
        //public static IWebHost MigrateDatabase(this IWebHost webHost)
        //{
        //    // Manually run any pending migrations if configured to do so.
        //    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        //    if (env == "Production")
        //    {
        //        var serviceScopeFactory = (IServiceScopeFactory)webHost.Services.GetService(typeof(IServiceScopeFactory));
        //        using (var scope = serviceScopeFactory.CreateScope())
        //        {
        //            var services = scope.ServiceProvider;
        //            var dbContext = services.GetRequiredService<DataContext>();
        //            dbContext.Database.Migrate();
        //        }
        //    }
        //    return webHost;
        //}

        public static IHost MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Production")
            {
                // Create a scope to get scoped services.
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<TContext>>();
                    // get the service provider and db context.
                    var context = services.GetService<TContext>();

                    // do something you can customize.
                    // For example, I will migrate the database.
                    context.Database.Migrate();
                }
            }

            return host;
        }
    }
}
