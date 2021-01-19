using ImageClassification.API.Configurations;
using ImageClassification.API.Enums;
using ImageClassification.API.Global;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.IO;

namespace ImageClassification.API.Routing.Constraints
{
    [Description("ClassifierName")]
    public class ClassifierNameConstraint : IRouteConstraint
    {
        public MLModelOptions Options { get; }

        public ClassifierNameConstraint(IOptions<MLModelOptions> options)
        {
            Options = options.Value;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (route == null)
                throw new ArgumentNullException(nameof(route));

            if (routeKey == null)
                throw new ArgumentNullException(nameof(routeKey));

            if (values == null)
                throw new ArgumentNullException(nameof(values));

            if (values.TryGetValue(routeKey, out object value) && value is string classifier)
            {
                return !string.IsNullOrWhiteSpace(classifier.ToString()) &&
                       File.Exists(Path.Combine(Options.MLModelFilePath, Path.ChangeExtension(classifier, Constants.Extensions.Zip)));
            }
            return false;
        }
    }
}
