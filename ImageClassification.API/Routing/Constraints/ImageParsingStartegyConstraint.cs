using ImageClassification.API.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.ComponentModel;

namespace ImageClassification.API.Routing.Constraints
{
    [Description("ImageParsingStrategy")]
    public class ImageParsingStartegyConstraint : IRouteConstraint
    {
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

            return values.TryGetValue(routeKey, out object value) && 
                   Enum.TryParse(value?.ToString(), out ImageParsingStrategy _);
        }
    }
}
