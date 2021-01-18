using ImageClassification.API.Configurations;
using ImageClassification.API.Delegates;
using ImageClassification.API.Enums;
using ImageClassification.API.Services.ImageParsingStrategies;
using ImageClassification.Core.Preparation;
using ImageClassification.Core.Preparation.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageClassification.API.Extensions
{
    public static class CustomServicesExtensions
    {
        public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureOptions<MLModelOptions>(configuration);
            services.ConfigureOptions<StorageOptions>(configuration);

            return services;
        }

        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddImageParsingStrategies();

            return services;
        }

        private static IServiceCollection AddImageParsingStrategies(this IServiceCollection services)
        {
            services.AddScoped<TestStrategy>();
            services.AddScoped<IParsingContext, ParsingContext>();
            services.AddScoped<ImageParsingResolver>(services => key =>
                    {
                        var context = services.GetService<IParsingContext>();
                        return key switch
                        {
                            ImageParsingStrategy.Test => services.GetService<TestStrategy>(),
                            _ => context.Default,
                        };
                    });

            return services;
        }
    }
}
