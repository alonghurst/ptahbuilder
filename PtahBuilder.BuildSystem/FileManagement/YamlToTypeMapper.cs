using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PtahBuilder.BuildSystem.Exceptions;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.BuildSystem.Metadata;
using YamlDotNet.RepresentationModel;
using ReflectionHelper = PtahBuilder.BuildSystem.Helpers.ReflectionHelper;

namespace PtahBuilder.BuildSystem.FileManagement;

public class YamlToBaseDataMapper<T> : YamlToTypeMapper<T> where T : new()
{
    private BaseDataMetadataResolver<T> _metadataResolver;

    public YamlToBaseDataMapper(Logger logger, BaseDataMetadataResolver<T> metadataResolver) : base(logger)
    {
        _metadataResolver = metadataResolver;
    }

    protected override string GetEntityId(T entity) => _metadataResolver.GetEntityId(entity);

    protected override void OnEntityParsedFromFile(string filename, T entity)
    {
        if (string.IsNullOrWhiteSpace(GetEntityId(entity)))
        {
            _metadataResolver.SetEntityId(entity, Path.GetFileNameWithoutExtension(filename));
        }
    }
}

public abstract class YamlToTypeMapper<T> : DirectoryParser where T : new()
{
    public Dictionary<T, MetadataCollection> ParsedEntitiesMetadata { get; } = new Dictionary<T, MetadataCollection>();
    private readonly Dictionary<T, HashSet<string>> _explicitlySetPropertiesPerEntity = new Dictionary<T, HashSet<string>>();

    public Logger Logger { get; }

    private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

    protected abstract string GetEntityId(T entity);

    public YamlToTypeMapper(Logger logger)
    {
        Logger = logger;

        _properties.Add(typeof(T), typeof(T).GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p));
    }

    public override void ParseDirectory(string directoryPath)
    {
        base.ParseDirectory(directoryPath);

        ProcessBasedOn(ParsedEntitiesMetadata.Where(t => !string.IsNullOrEmpty(t.Value.BasedOn)).ToList());
    }

    protected override string FileFilter => "*.yaml";

    protected override void ParseFile(string filePath)
    {
        Logger.Info($"Parsing {filePath}");

        var entity = Activator.CreateInstance<T>();

        var metadata = new MetadataCollection
        {
            {MetadataKeys.SourceFile, filePath}
        };

        ParsedEntitiesMetadata.Add(entity, metadata);

        try
        {
            using (TextReader reader = File.OpenText(filePath))
            {
                var yaml = new YamlStream();
                yaml.Load(reader);

                if (yaml.Documents.Count > 0)
                {
                    var mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                    SetValuesFromYamlMapping(mapping, typeof(T), entity);
                }

                OnEntityParsedFromFile(filePath, entity);
            }
        }
        catch (YamlDotNet.Core.YamlException ex)
        {
            throw new BuilderException($"Error parsing yaml for file {filePath}", ex);
        }
    }

    private void SetValuesFromYamlMapping(YamlMappingNode mapping, Type type, object entity)
    {
        foreach (var entry in mapping.Children)
        {
            var key = ((YamlScalarNode)entry.Key).Value;

            if (key == "Meta" || key == "Metadata")
            {
                ProcessMetadata(entity, entry);
            }
            else
            {
                var property = FindProperty(type, key);
                if (property != null)
                {
                    SetValueFromYamlNode(entity, property, entry.Value);
                }
            }
        }
    }

    private void ProcessMetadata(object entity, KeyValuePair<YamlNode, YamlNode> entry)
    {
        if (entity is T)
        {
            var tEntity = (T)entity;

            var metadata = ParsedEntitiesMetadata[tEntity];
            if (entry.Value is YamlSequenceNode sequenceNode)
            {
                foreach (var n in sequenceNode.Children)
                {
                    if (n is YamlMappingNode node)
                    {
                        foreach (var e in node)
                        {
                            var mKey = ((YamlScalarNode)e.Key).Value;
                            var mValue = ((YamlScalarNode)e.Value).Value;

                            if (mKey != null && mValue != null)
                            {
                                if (metadata.ContainsKey(mKey))
                                {
                                    metadata[mKey] = mValue;
                                }
                                else
                                {
                                    metadata.Add(mKey, mValue);
                                }
                            }
                        }
                    }
                }
            }
            if (entry.Value is YamlMappingNode mappingNode)
            {
                foreach (var n in mappingNode.Children)
                {
                    var mKey = ((YamlScalarNode)n.Key).Value;
                    var mValue = ((YamlScalarNode)n.Value).Value;

                    if (mKey != null && mValue != null)
                    {
                        if (metadata.ContainsKey(mKey))
                        {
                            metadata[mKey] = mValue;
                        }
                        else
                        {
                            metadata.Add(mKey, mValue);
                        }
                    }
                }
            }
        }
    }

    private void SetValueFromYamlNode(object entity, PropertyInfo property, YamlNode yamlNode)
    {
        if (entity is T mainEntity)
        {
            if (!_explicitlySetPropertiesPerEntity.ContainsKey(mainEntity))
            {
                _explicitlySetPropertiesPerEntity.Add(mainEntity, new HashSet<string>());
            }
            if (!_explicitlySetPropertiesPerEntity[mainEntity].Contains(property.Name))
            {
                _explicitlySetPropertiesPerEntity[mainEntity].Add(property.Name);
            }
        }

        property = property.DeclaringType?.GetProperty(property.Name);

        if (property == null)
        {
            throw new NullReferenceException($"{nameof(property)} not found");
        }

        if (yamlNode is YamlSequenceNode sequenceNode)
        {
            if (property.PropertyType.IsArray)
            {
                var elementType = property.PropertyType.GetElementType();
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
                throw new NotImplementedException();
            }
        }
        else if (yamlNode is YamlMappingNode mappingNode)
        {
            var type = property.PropertyType;
            var subEntity = Activator.CreateInstance(type);
            SetValuesFromYamlMapping(mappingNode, type, subEntity);

            property.SetValue(entity, subEntity);
        }
        else
        {
            object value = ((YamlScalarNode)yamlNode).Value;

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

    private static object ConvertScalarValue(Type type, object value)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var argument = type.GetGenericArguments()[0];

            return ConvertScalarValue(argument, value);
        }

        foreach (var parser in ValueParsers.UserDefinedValueParsers)
        {
            if (type.IsAssignableFrom(parser.Key))
            {
                var result = parser.Value.Invoke(value);
                if (result != null)
                {
                    return result;
                }
            }
        }

        value = type.LazyConvertForType(value, Convert.ToBoolean);
        value = type.LazyConvertForType(value, Convert.ToInt32);
        value = type.LazyConvertForType(value, ConvertHelper.StringToDouble);
        value = type.LazyConvertForType(value, LazyTimeSpan);
        value = type.LazyConvertEnumForProperty(value);

        if (type.IsArray && type.HasElementType)
        {
            var elementType = type.GetElementType();

            // If the target property is an array but a scalar value was passed then simple wrap the result in array
            var arrValue = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), 1);
            // ReSharper disable once RedundantCast
            ((dynamic)arrValue)[0] = (dynamic)value;

            value = arrValue;
        }

        return value;
    }

    private static TimeSpan LazyTimeSpan(object v)
    {
        var toString = v.ToString();

        if (toString.Contains(":"))
        {
            return TimeSpan.Parse(toString);
        }

        return TimeSpan.FromHours(Convert.ToDouble(toString));
    }

    private IEnumerable<dynamic> GetSequenceValuesForArray(Type type, YamlSequenceNode sequenceNode)
    {
        foreach (var node in sequenceNode.Children)
        {
            if (node is YamlMappingNode mappingNode)
            {
                if (type == typeof(string))
                {
                    yield return ((YamlScalarNode)mappingNode.Children.First().Value).Value;
                }
                else
                {
                    var entity = Activator.CreateInstance(type);

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

    private IEnumerable<dynamic> GetSequenceValuesForDictionary(Type type, YamlSequenceNode sequenceNode)
    {
        foreach (var node in sequenceNode.Children)
        {
            if (node is YamlMappingNode mappingNode)
            {
                if (mappingNode.Children.First().Key is YamlScalarNode keyNode && keyNode.Value.ToUpper() == "KVP"
                                                                               && mappingNode.Children.First().Value is YamlScalarNode valueNode)
                {
                    var stringyValues = valueNode.Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

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
                    object valueValue;
                    if (value is YamlScalarNode scalar)
                    {
                        valueValue = ConvertScalarValue(type.GetGenericArguments()[1], scalar.Value);
                    }
                    else if (value is YamlMappingNode mapping)
                    {
                        var entity = Activator.CreateInstance(type.GetGenericArguments()[1]);
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
            _properties.Add(onType, onType.GetProperties().ToDictionary(p => p.Name, p => p));
        }

        if (!_properties[onType].ContainsKey(propertyName))
        {
            Logger.Warning($"Unable to find property {propertyName} for Type {onType}");
        }

        return _properties[onType][propertyName];
    }

    private void ProcessBasedOn(List<KeyValuePair<T, MetadataCollection>> setToProcess)
    {
        for (int i = 0; i < setToProcess.Count; i++)
        {
            var entityWithMetadata = setToProcess[i];

            var basedOn = ParsedEntitiesMetadata.Keys.FirstOrDefault(t => GetEntityId(t) == entityWithMetadata.Value.BasedOn);

            if (basedOn == null)
            {
                Logger.Warning($"{typeof(T).Name}: Unable to find based on \"{entityWithMetadata.Value.BasedOn}\" when processing entity \"{GetEntityId(entityWithMetadata.Key)}\"");
                setToProcess.RemoveAt(i);
                i--;
                continue;
            }

            if (setToProcess.Any(s => s.Key.Equals(basedOn)))
            {
                // The basedOn that was found is based on something else
                // Skip processing the entity until the basedOn entity has been processed
                continue;
            }

            if (ParsedEntitiesMetadata.TryGetValue(basedOn, out MetadataCollection basedOnMetadata))
            {
                entityWithMetadata.Value.TakeUnsetValuesFrom(basedOnMetadata);
            }

            // Get any properties in the current entity that are not at the default value
            var nonDefaultValues = ReflectionHelper.GetNonDefaultPropertyAndTheNewValue(entityWithMetadata.Key).ToArray();
            // Get any explicitly set properties
            var explicitlySet = _explicitlySetPropertiesPerEntity.ContainsKey(entityWithMetadata.Key) ? _explicitlySetPropertiesPerEntity[entityWithMetadata.Key] : new HashSet<string>();

            var propertiesToSkip = nonDefaultValues.Select(s => s.Key.Name).Union(explicitlySet).ToArray();

            foreach (var property in _properties[typeof(T)])
            {
                if (propertiesToSkip.Contains(property.Key))
                {
                    // The entity has a non-default value, don't do anything
                    continue;
                }

                var value = property.Value.GetValue(basedOn);
                property.Value.SetValue(entityWithMetadata.Key, value);
            }

            setToProcess.RemoveAt(i);
            i--;
        }

        if (setToProcess.Any())
        {
            ProcessBasedOn(setToProcess);
        }
    }

    protected virtual void OnEntityParsedFromFile(string filePath, T entity)
    {
    }
}