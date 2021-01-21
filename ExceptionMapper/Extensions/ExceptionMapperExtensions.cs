using ExceptionMapper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExceptionMapper.Extensions
{
    public static class ExceptionMapperExtensions
    {
        public static IExceptionMapper ConfigureExceptionMapper(this IServiceCollection services)
        {
            var mapper = new ExceptionMapper();
            services.AddSingleton<IExceptionMapper>(x => mapper);

            return mapper;
        }
        public static IServiceCollection ConfigureExceptionMapper(this IServiceCollection services, Action<IExceptionMappingBuilder> builder)
        {
            var mapper = new ExceptionMapper(builder);
            services.AddSingleton<IExceptionMapper>(x => mapper);

            return services;
        }
    }
}
