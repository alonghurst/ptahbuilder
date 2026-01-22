using System.Reflection;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.BuildSystem.Services.Mapping;

public class DynamicMappingService : IDynamicMappingService
{
    private readonly IScalarValueService _scalarValueService;
    private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _entityProperties = new();

    public DynamicMappingService(IScalarValueService scalarValueService)
    {
        _scalarValueService = scalarValueService;
    }

    public void Map<T>(T entity, string propertyName, object? rawValue) where T : class
    {
        object instance = entity;

        while (propertyName.Contains("."))
        {
            (instance, propertyName) = HandleSubObject(instance, propertyName);
        }

        MapToInstance(instance, propertyName, rawValue);
    }

    private (object instance, string propertyName) HandleSubObject(object instance, string propertyName)
    {
        var index = propertyName.IndexOf('.');

        var instancePropertyName = propertyName.Substring(0, index);
        var remainingPropertyName = propertyName.Substring(index + 1);

        var property = GetProperty(instance, instancePropertyName);

        var subObject = property.GetValue(instance);

        if (subObject == null)
        {
            subObject = Activator.CreateInstance(property.PropertyType) ?? throw new InvalidOperationException($"Unable to activate an instance of {property.PropertyType.Name}");
            property.SetValue(instance, subObject);
        }

        return (subObject, remainingPropertyName);
    }

    private void MapToInstance(object entity, string propertyName, object? rawValue)
    {
        var property = GetProperty(entity, propertyName);
        var value = _scalarValueService.ConvertScalarValue(property.PropertyType, rawValue);

        // If the property is a DictionaryType and the existing value is not null, merge the new dictionary into the existing one
        if (property.PropertyType.IsDictionaryType())
        {
            var existingValue = property.GetValue(entity);
            if (existingValue != null && value is System.Collections.IDictionary newDictionary)
            {
                var existingDictionary = (System.Collections.IDictionary)existingValue;
                
                // Merge new dictionary entries into existing dictionary
                foreach (System.Collections.DictionaryEntry entry in newDictionary)
                {
                    existingDictionary[entry.Key] = entry.Value;
                }
                
                // No need to set the property since we modified the existing dictionary in place
                return;
            }
        }

        property.SetValue(entity, value);
    }

    private PropertyInfo GetProperty(object entity, string propertyName)
    {
        propertyName = propertyName.Trim();

        var entityType = entity.GetType();

        if (!_entityProperties.ContainsKey(entityType))
        {
            _entityProperties.Add(entityType, entityType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToDictionary(x => x.Name, x => x));
        }

        if (_entityProperties[entityType].TryGetValue(propertyName, out var property))
        {
            return property;
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(propertyName), $"Unable to find a property named \"{propertyName}\" on type \"{entityType.GetTypeName()}\"");
        }
    }
}