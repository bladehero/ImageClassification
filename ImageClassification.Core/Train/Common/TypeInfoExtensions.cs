using System;
using System.Linq;
using System.Reflection;

namespace ImageClassification.Core.Train.Common
{
    public static class TypeInfoExtensions
    {
        public static bool HasCustomAttribute(this TypeInfo typeInfo, Type attributeType)
        {
            if (attributeType is null)
            {
                ThrowHelper.ArgumentNull(nameof(attributeType));
            }

            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                ThrowHelper.Argument($"Custom attribute type should be inherited from {nameof(Attribute)}!");
            }

            return typeInfo.CustomAttributes.Any(x => x.AttributeType == attributeType);
        }

        public static bool HasCustomAttribute(this TypeInfo typeInfo, Type attributeType, Func<CustomAttributeData, bool> predicate)
        {
            if (attributeType is null)
            {
                ThrowHelper.ArgumentNull(nameof(attributeType));
            }

            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                ThrowHelper.Argument($"Custom attribute type should be inherited from {nameof(Attribute)}!");
            }

            if (predicate is null)
            {
                ThrowHelper.ArgumentNull(nameof(predicate));
            }

            return typeInfo.CustomAttributes.Any(x => x.AttributeType == attributeType && predicate(x));
        }

        public static bool ImplementsInterface(this TypeInfo typeInfo, Type interfaceType)
        {
            if (interfaceType is null)
            {
                ThrowHelper.ArgumentNull(nameof(interfaceType));
            }

            if (!interfaceType.IsInterface)
            {
                ThrowHelper.Argument($"Custom attribute type should be an interface!");
            }

            return typeInfo.ImplementedInterfaces.Any(x => x == interfaceType);
        }
    }
}
