using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PtahBuilder.Util.Extensions.Reflection
{
    public static class LazyConvertExtensions
    {/// <summary>
    /// Converts the value using the convert function if the output type matches the property.
    /// Otherwise returns the value
    /// </summary>
    public static object LazyConvertForReferenceType<T>(this Type type, object value, Func<object, T> convert) where T : class
    {
        if (type == typeof(T))
        {
            return convert(value);
        }
        return value;
    }

    /// <summary>
    /// Converts the value using the convert function if the output type matches the property.
    /// Otherwise returns the value
    /// </summary>
    public static object LazyConvertForValue<T>(this Type type, object value, Func<object, T> convert) where T : struct
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

}
}
