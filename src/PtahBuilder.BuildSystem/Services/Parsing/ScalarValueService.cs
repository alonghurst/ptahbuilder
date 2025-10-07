using PtahBuilder.BuildSystem.Services.Parsing;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.BuildSystem.Services.Mapping;

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

        // Handle Dictionary<TKey, TValue> types parsed from strings
        if (type.IsDictionaryType())
        {
            var keyType = type.GetGenericArguments()[0];
            var valueType = type.GetGenericArguments()[1];

            if (value is string dictText)
            {
                dictText = dictText.Trim();

                var dictionary = (System.Collections.IDictionary?)Activator.CreateInstance(type);
                if (string.IsNullOrWhiteSpace(dictText))
                {
                    return dictionary;
                }

                // Expect KVPs separated by ',' and key/value separated by '|'
                var pairs = dictText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var pair in pairs)
                {
                    var kv = pair.Split('|', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (kv.Length != 2)
                    {
                        throw new InvalidOperationException($"Unable to split \"{pair}\" into key|value for dictionary parsing");
                    }

                    var keyObj = ConvertScalarValue(keyType, kv[0]);
                    var valueObj = ConvertScalarValue(valueType, kv[1]);

                    dictionary!.Add(keyObj!, valueObj);
                }

                return dictionary;
            }
        }

        value = type.LazyConvertForValue(value, ConvertHelper.StringToDateTime);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToBoolean);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToInt);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToDouble);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToFloat);
        value = type.LazyConvertForValue(value, LazyTimeSpan);
        value = type.LazyConvertEnumForProperty(value);

        if (type != typeof(string) && type.IsArray && type.HasElementType)
        {
            var elementType = type.GetElementType();

            // If value is empty string then return empty array
            if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                value = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 0);
                return value;
            }

            if (elementType != typeof(string) && elementType != null)
            {
                // Primitive elements are split by commas whereas non-primitives use commas to divide property assignment and semicolons to separate elements
                var splitBy = elementType.IsPrimitive || elementType.IsEnum ? ',' : ';';

                if (value is string toSplit && toSplit.Contains(splitBy))
                {
                    var splits = toSplit.Split(splitBy, StringSplitOptions.RemoveEmptyEntries);
                    
                    var rawData = splits.Select(x => (dynamic?)ConvertScalarValue(elementType, x))
                        //.Select(x => Convert.ChangeType(x, elementType))
                        .ToArray();

                    var valueData = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), rawData.Length);

                    for (int i = 0; i < rawData.Length; i++)
                    {
                        ((dynamic)valueData)[i] = rawData[i];
                    }

                    return valueData;
                }
            }

            // If the target property is an array but a scalar value was passed then simple wrap the result in array
            var arrValue = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 1);
            // ReSharper disable once RedundantCast
            ((dynamic)arrValue)[0] = (dynamic?)ConvertScalarValue(elementType, value);

            value = arrValue;
        }

        // Handle HashSet<T> types
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>))
        {
            var elementType = type.GetGenericArguments()[0];

            // If value is empty string then return empty HashSet
            if (value is string str && string.IsNullOrWhiteSpace(str))
            {
                var emptyHashSet = Activator.CreateInstance(type);
                return emptyHashSet;
            }

            if (value is string toSplit && toSplit.Contains(','))
            {
                var splits = toSplit.Split(',', StringSplitOptions.RemoveEmptyEntries);
                
                var convertedValues = splits.Select(x => ConvertScalarValue(elementType, x.Trim())).ToArray();
                
                var hashSet = Activator.CreateInstance(type);
                var addMethod = type.GetMethod("Add");
                
                foreach (var convertedValue in convertedValues)
                {
                    addMethod?.Invoke(hashSet, new[] { convertedValue });
                }

                return hashSet;
            }

            // If the target property is a HashSet but a scalar value was passed then create HashSet with single element
            var singleHashSet = Activator.CreateInstance(type);
            var singleAddMethod = type.GetMethod("Add");
            var convertedSingleValue = ConvertScalarValue(elementType, value);
            singleAddMethod?.Invoke(singleHashSet, new[] { convertedSingleValue });

            return singleHashSet;
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