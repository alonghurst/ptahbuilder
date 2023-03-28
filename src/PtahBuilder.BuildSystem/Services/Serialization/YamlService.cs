using PtahBuilder.Util.Helpers;
using PtahBuilder.Util.Services.Logging;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace PtahBuilder.BuildSystem.Services.Serialization;

public class YamlService : IYamlService
{
    private readonly ILogger _logger;
    private readonly ICustomValueParserService _customValueParserService;
    private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new();

    public YamlService(ILogger logger, ICustomValueParserService customValueParserService)
    {
        _logger = logger;
        _customValueParserService = customValueParserService;
    }

    public T Deserialize<T>(string text)
    {
        using var input = new StringReader(text);

        var yaml = new YamlStream();
        yaml.Load(input);

        var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

        var entity = Activator.CreateInstance<T>()!;

        SetValuesFromYamlMapping(mapping, typeof(T), entity);

        return entity;
    }

    private void SetValuesFromYamlMapping(YamlMappingNode mapping, Type type, object entity)
    {
        foreach (var entry in mapping.Children)
        {
            var key = ((YamlScalarNode)entry.Key).Value!;

            var property = FindProperty(type, key);

            if (property != null)
            {
                try
                {
                    SetValueFromYamlNode(entity, property, entry.Value);
                }
                catch
                {
                    _logger.Error($"Error mapping on {property.Name} value {entry.Value}");
                    throw;
                }
            }
        }
    }

    private void SetValueFromYamlNode(object entity, PropertyInfo property, YamlNode yamlNode)
    {
        property = property.DeclaringType?.GetProperty(property.Name) ?? property;

        if (property == null)
        {
            throw new NullReferenceException($"{nameof(property)} not found");
        }

        if (yamlNode is YamlSequenceNode sequenceNode)
        {
            if (property.PropertyType.IsArray)
            {
                var elementType = property.PropertyType.GetElementType()!;

                dynamic values = GetSequenceValuesForArray(elementType, sequenceNode)
                    .Select(e => Convert.ChangeType(e, elementType))
                    .ToArray();

                var array = ValuesToArray(elementType, values);

                property.SetValue(entity, array);
            }
            else if (property.PropertyType.IsDictionaryType())
            {
                var kvpType = property.PropertyType.GetDictionaryKeyValuePairType();

                dynamic kvpValues = GetSequenceValuesForDictionary(kvpType, sequenceNode)
                    .Select(e => Convert.ChangeType(e, kvpType))
                    .ToArray();

                var array = ValuesToArray(kvpType, kvpValues);

                var dictionary = Activator.CreateInstance(property.PropertyType, array);

                property.SetValue(entity, dictionary);
            }
            else
            {
                throw new NotImplementedException($"Unable to parse yaml \"{yamlNode}\" for property \"{property.Name}\"");
            }
        }
        else if (yamlNode is YamlMappingNode mappingNode)
        {
            var type = property.PropertyType;
            var subEntity = Activator.CreateInstance(type)!;

            SetValuesFromYamlMapping(mappingNode, type, subEntity);

            property.SetValue(entity, subEntity);
        }
        else
        {
            object? value = ((YamlScalarNode)yamlNode).Value;

            value = ConvertScalarValue(property.PropertyType, value);

            property.SetValue(entity, value);
        }
    }

    private static dynamic ValuesToArray(Type elementType, dynamic values)
    {
        dynamic array = Array.CreateInstance(elementType, values.Length);

        for (var i = 0; i < array.Length; i++)
        {
            array[i] = values[i];
        }

        return array;
    }

    private object? ConvertScalarValue(Type type, object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var argument = type.GetGenericArguments()[0];

            return ConvertScalarValue(argument, value);
        }

        if (_customValueParserService.TryParseValue(type, value, out var result))
        {
            return result!;
        }

        value = type.LazyConvertForValue(value, Convert.ToBoolean);
        value = type.LazyConvertForValue(value, Convert.ToInt32);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToDouble);
        value = type.LazyConvertForValue(value, ConvertHelper.StringToFloat);
        value = type.LazyConvertForValue(value, LazyTimeSpan);
        value = type.LazyConvertEnumForProperty(value);

        if (type.IsArray && type.HasElementType)
        {
            var elementType = type.GetElementType();

            // If the target property is an array but a scalar value was passed then simple wrap the result in array
            var arrValue = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 1);
            // ReSharper disable once RedundantCast
            ((dynamic)arrValue)[0] = (dynamic?)ConvertScalarValue(elementType, value);

            value = arrValue;
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

    private IEnumerable<dynamic?> GetSequenceValuesForArray(Type type, YamlSequenceNode sequenceNode)
    {
        foreach (var node in sequenceNode.Children)
        {
            if (node is YamlMappingNode mappingNode)
            {
                if (type == typeof(string))
                {
                    var scalar = (YamlScalarNode)mappingNode.Children.First().Value;

                    yield return scalar.Value!;
                }
                else
                {
                    var entity = Activator.CreateInstance(type)!;

                    SetValuesFromYamlMapping(mappingNode, type, entity);

                    yield return entity;
                }
            }
            else if (node is YamlScalarNode scalar)
            {
                yield return ConvertScalarValue(type, scalar.Value);
            }
        }
    }

    private IEnumerable<dynamic?> GetSequenceValuesForDictionary(Type type, YamlSequenceNode sequenceNode)
    {
        foreach (var node in sequenceNode.Children)
        {
            if (node is YamlMappingNode mappingNode)
            {
                if (mappingNode.Children.First().Key is YamlScalarNode keyNode && keyNode.Value!.ToUpper() == "KVP"
                                                                               && mappingNode.Children.First().Value is YamlScalarNode valueNode)
                {
                    var stringyValues = valueNode.Value!.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                    var value = valueNode.Value.Substring(stringyValues[0].Length + 1);

                    var keyValue = ConvertScalarValue(type.GetGenericArguments()[0], stringyValues[0].Trim());
                    var valueValue = ConvertScalarValue(type.GetGenericArguments()[1], value.Trim());

                    yield return Activator.CreateInstance(type, keyValue, valueValue);
                }
                else
                {
                    var key = mappingNode.Children[new YamlScalarNode("Key")];
                    var value = mappingNode.Children[new YamlScalarNode("Value")];

                    var keyValue = ConvertScalarValue(type.GetGenericArguments()[0], ((YamlScalarNode)key).Value);
                    object? valueValue;

                    if (value is YamlScalarNode scalar)
                    {
                        valueValue = ConvertScalarValue(type.GetGenericArguments()[1], scalar.Value);
                    }
                    else if (value is YamlMappingNode mapping)
                    {
                        var entity = Activator.CreateInstance(type.GetGenericArguments()[1])!;

                        SetValuesFromYamlMapping(mapping, type.GetGenericArguments()[1], entity);
                        valueValue = entity;
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    yield return Activator.CreateInstance(type, keyValue, valueValue);
                }
            }
        }
    }

    private PropertyInfo FindProperty(Type onType, string propertyName)
    {
        if (!_properties.ContainsKey(onType))
        {
            _properties.Add(onType, onType.GetProperties().ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase));
        }

        if (!_properties[onType].ContainsKey(propertyName))
        {
            _logger.Warning($"Unable to find property {propertyName} for Type {onType}");
        }

        return _properties[onType][propertyName];
    }
}