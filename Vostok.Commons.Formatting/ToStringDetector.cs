using System;
using System.Globalization;
using JetBrains.Annotations;
using Vostok.Commons.Collections;

// ReSharper disable ConvertClosureToMethodGroup

namespace Vostok.Commons.Formatting
{
    [PublicAPI]
    internal static class ToStringDetector
    {
        private const int CacheCapacity = 10000;

        private static readonly RecyclingBoundedCache<Type, Func<object, string>> Cache =
            new RecyclingBoundedCache<Type, Func<object, string>>(CacheCapacity);

        public static bool HasCustomToString(Type type) =>
            TryGetCustomToString(type) != null;
        
        [CanBeNull]
        public static Func<object, string> TryGetCustomToString(Type type) =>
            Cache.Obtain(type, t => TryGetCustomToStringInternal(t));

        private static Func<object, string> TryGetCustomToStringInternal(Type type)
        {
            // (iloktionov): Reject anonymous types:
            if (type.Name.StartsWith("<>"))
                return null;
            
            var toStringMethod = type.GetMethod("ToString", new []{typeof(CultureInfo)})
                                 ?? type.GetMethod("ToString", Array.Empty<Type>());
            
            if (toStringMethod == null)
                return null;

            var declaringType = toStringMethod.DeclaringType;

            if (declaringType == typeof(object) || declaringType == typeof(ValueType))
                return null;

            if (toStringMethod.GetParameters().Length == 1)
                return o => (string)toStringMethod.Invoke(o, new []{CultureInfo.InvariantCulture});
                
            return o => o.ToString();
        }
    }
}