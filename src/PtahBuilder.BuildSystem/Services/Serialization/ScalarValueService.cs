﻿using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.BuildSystem.Services.Serialization;

public class ScalarValueService : IScalarValueService
{
    private readonly ICustomValueParserService _customValueParserService;

    public ScalarValueService(ICustomValueParserService customValueParserService)
    {
        _customValueParserService = customValueParserService;
    }

    public object? ConvertScalarValue(Type type, object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var argument = type.GetGenericArguments()[0];

            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                return null;
            }

            return ConvertScalarValue(argument, value);
        }

        if (_customValueParserService.TryParseValue(type, value, out var result))
        {
            return result!;
        }

        value = type.LazyConvertForValue(value, ConvertHelper.StringToBoolean);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToInt);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToDouble);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToFloat);
        value = type.LazyConvertForValue(value, LazyTimeSpan);
        value = type.LazyConvertEnumForProperty(value);

        if (type.IsArray && type.HasElementType)
        {
            var elementType = type.GetElementType();

            // If value is empty string then return empty array
            if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                value = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 0);
                return value;
            }

            // If the target property is an array but a scalar value was passed then simple wrap the result in array
            var arrValue = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 1);
            // ReSharper disable once RedundantCast
            ((dynamic)arrValue)[0] = (dynamic?)ConvertScalarValue(elementType, value);

            value = arrValue;
        }

        if (value is string text)
        {
            value = text.Trim();
        }

        return value;
    }

    private TimeSpan LazyTimeSpan(object v)
    {
        var toString = v.ToString() ?? string.Empty;

        if (toString.Contains(":"))
        {
            return TimeSpan.Parse(toString);
        }

        return TimeSpan.FromHours(Convert.ToDouble(toString));
    }
}