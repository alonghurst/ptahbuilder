﻿using System;
using System.Collections.Generic;

namespace PtahBuilder.BuildSystem.Helpers
{
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Converts the value using the convert function if the output type matches the property.
        /// Otherwise returns the value
        /// </summary>
        public static object LazyConvertForType<T>(this Type type, object value, Func<object, T> convert)
        {
            if (type == typeof(T))
            {
                return convert(value);
            }
            return value;
        }

        public static object LazyConvertEnumForProperty(this Type type, object value)
        {
            if (type.IsEnum)
            {
                var valueToString = value.ToString();
                if (valueToString != null)
                {
                    return Convert.ChangeType(Enum.Parse(type, valueToString), type);
                }
            }

            return value;
        }

        public static bool IsDictionaryType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        public static Type GetDictionaryKeyValuePairType(this Type dictionaryType)
        {
            Type keyType = dictionaryType.GetGenericArguments()[0];
            Type valueType = dictionaryType.GetGenericArguments()[1];

            return typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
        }
    }
}
