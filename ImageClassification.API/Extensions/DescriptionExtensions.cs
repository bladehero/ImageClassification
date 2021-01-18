using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ImageClassification.API.Extensions
{
    public static class DescriptionExtensions
    {
        private static readonly Type _descriptionType = typeof(DescriptionAttribute);

        public static string GetDescription(this Type type)
        {
            var attributeData = type.CustomAttributes.FirstOrDefault(x => x.AttributeType.Equals(_descriptionType));

            if (attributeData is CustomAttributeData data)
            {
                var argument = data.ConstructorArguments.FirstOrDefault();
                return argument.Value?.ToString();
            }
            return null;
        }

        public static string GetDescription<T>(this T value)
        {
            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(_descriptionType, true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return description;
        }
    }
}
