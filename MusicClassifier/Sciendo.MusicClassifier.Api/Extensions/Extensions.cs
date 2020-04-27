using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sciendo.MusicClassifier.Api.Configuration;
using Sciendo.MusicClassifier.Api.Services;
using Sciendo.MusicClassifier.Api.Services.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sciendo.MusicClassifier.Api.Extensions
{
    public static class Extensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static void ConfigureLogger(this IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog(new LoggerConfiguration()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger()));

        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Sets>>();

            services.AddSingleton(configuration.GetSection("setsConfiguration").Get<SetsConfiguration>());
            services.AddScoped<ISets>(r => new Sets(r.GetRequiredService<ILogger<Sets>>(), r.GetRequiredService<SetsConfiguration>()));
            services.AddScoped<IArtists>(r => new Artists(r.GetRequiredService<ILogger<Artists>>()));
        }

    }
}
