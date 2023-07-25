using PtahBuilder.Util.Services.Logging;
using System.Reflection;
using PtahBuilder.Util.Extensions.Reflection;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace PtahBuilder.BuildSystem.Services.Serialization;

public class YamlService : IYamlService
{
    private readonly ILogger _logger;
    private readonly IScalarValueService _scalarValueService;
    private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new();
    private readonly ISerializer _serializer;

    public YamlService(ILogger logger, IScalarValueService scalarValueService)
    {
        _logger = logger;
        _scalarValueService = scalarValueService;
        _serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull | DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .Build();
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

    public string Serialize<T>(T entity) => entity == null ? string.Empty : _serializer.Serialize(entity);

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

            value = _scalarValueService.ConvertScalarValue(property.PropertyType, value);

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
                yield return _scalarValueService.ConvertScalarValue(type, scalar.Value);
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

                    var keyValue = _scalarValueService.ConvertScalarValue(type.GetGenericArguments()[0], stringyValues[0].Trim());
                    var valueValue = _scalarValueService.ConvertScalarValue(type.GetGenericArguments()[1], value.Trim());

                    yield return Activator.CreateInstance(type, keyValue, valueValue);
                }
                else
                {
                    var key = mappingNode.Children[new YamlScalarNode("Key")];
                    var value = mappingNode.Children[new YamlScalarNode("Value")];

                    var keyValue = _scalarValueService.ConvertScalarValue(type.GetGenericArguments()[0], ((YamlScalarNode)key).Value);
                    object? valueValue;

                    if (value is YamlScalarNode scalar)
                    {
                        valueValue = _scalarValueService.ConvertScalarValue(type.GetGenericArguments()[1], scalar.Value);
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