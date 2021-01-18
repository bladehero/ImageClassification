using ImageClassification.API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ImageClassification.API.Extensions
{
    public static class ConfigurationOptionsExtensions
    {
        public static IServiceCollection ConfigureOptions<T>(this IServiceCollection services,
                                                             IConfiguration configuration) 
            where T : class, IConfigurationOptions, new()
        {
            var options = Activator.CreateInstance<T>();
            return services.Configure<T>(configuration.GetSection(options.SectionPath));
        }
    }
}
