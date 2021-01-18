using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImageClassification.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static ConstructorInfo GetConstructor(this Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new ArgumentException($"{type} doesn't have exactly one constructor");
            }

            var constructor = constructors[0];
            return constructor;
        }

        public static IReadOnlyDictionary<string, object> ToPropertiesDictionary(this object obj)
            => new RouteValueDictionary(obj);

        public static void AddSingletonWithConstructorParams<TService, TImplementation>(
            this IServiceCollection services,
            object paramsWithNames
        ) where TImplementation : class, TService where TService : class
            => services.AddSingletonWithConstructorParams(
                typeof(TService),
                typeof(TImplementation),
                paramsWithNames
            );

        public static void AddSingletonWithConstructorParams(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType,
            object paramsWithNames
        )
        {
            if (paramsWithNames == null)
            {
                throw new ArgumentException("null argument");
            }

            if (!paramsWithNames.GetType()
                .Name.Contains("AnonymousType"))
            {
                // due to ambiguous invocation of this method with a single arg
                services.AddSingletonWithConstructorParams(
                    serviceType,
                    implementationType,
                    new[] { paramsWithNames }
                );
                return;
            }

            var constructor = implementationType.GetConstructor();

            var overridingParams = paramsWithNames.ToPropertiesDictionary();
            foreach (var parameterInfo in constructor.GetParameters())
            {
                if (overridingParams.ContainsKey(parameterInfo.Name))
                {
                    var paramValue = overridingParams[parameterInfo.Name];
                    if (paramValue != null && !parameterInfo.ParameterType.IsInstanceOfType(paramValue))
                    {
                        throw new ArgumentException($"Argument of type {paramValue.GetType()} cannot be supplied as parameter named {parameterInfo.Name} for type {implementationType}");
                    }
                }
            }

            AddSingletonWithParamsCallback(
                services,
                serviceType,
                constructor,
                parameterInfo => overridingParams.ContainsKey(parameterInfo.Name),
                parameterInfo => overridingParams[parameterInfo.Name]
            );
        }

        public static void AddSingletonWithConstructorParams<TService, TImplementation>(
            this IServiceCollection services,
            params object[] parameters
        ) where TImplementation : class, TService where TService : class
            => services.AddSingletonWithConstructorParams(
                typeof(TService),
                typeof(TImplementation),
                parameters
            );

        public static void AddSingletonWithConstructorParams(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType,
            params object[] parameters
        )
        {
            var constructor = implementationType.GetConstructor();

            if (parameters.Any(param => param == null))
            {
                throw new ArgumentException($"Cannot supply null ctor params to {nameof(AddSingletonWithConstructorParams)}");
            }

            var overridingValuesByParameterName = new Dictionary<string, object>();
            foreach (var parameterInfo in constructor.GetParameters())
            {
                var matchingOverridingParameters = parameters.Where(param => parameterInfo.ParameterType.IsInstanceOfType(param))
                    .ToList();
                if (!matchingOverridingParameters.Any())
                {
                    continue;
                }

                if (matchingOverridingParameters.Count > 1)
                {
                    throw new ArgumentException($"More than one of the input parameters matches the ctor param {parameterInfo.Name} of type {parameterInfo.ParameterType}");
                }

                overridingValuesByParameterName[parameterInfo.Name] = matchingOverridingParameters[0];
            }

            services.AddSingletonWithParamsCallback(
                serviceType,
                constructor,
                parameterInfo => overridingValuesByParameterName.ContainsKey(parameterInfo.Name),
                parameterInfo => overridingValuesByParameterName[parameterInfo.Name]
            );
        }

        private static void AddSingletonWithParamsCallback(
            this IServiceCollection services,
            Type serviceType,
            ConstructorInfo constructor,
            Predicate<ParameterInfo> parameterPredicate,
            Func<ParameterInfo, object> parameterValueProvider
        )
            => services.AddSingleton(
                serviceType,
                svc => constructor.Invoke(
                    constructor.GetParameters()
                        .Select(
                            parameterInfo => parameterPredicate(parameterInfo)
                                ? parameterValueProvider(parameterInfo)
                                : svc.GetService(parameterInfo.ParameterType)
                        )
                        .ToArray()
                )
            );
    }
}
