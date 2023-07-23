using System.Reflection;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.BuildSystem.Services.Serialization;

public class DynamicMappingService : IDynamicMappingService
{
    private readonly IScalarValueService _scalarValueService;
    private readonly Dictionary<Type, Dictionary<string, PropertyInfo>> _entityPropertyies = new();

    public DynamicMappingService(IScalarValueService scalarValueService)
    {
        _scalarValueService = scalarValueService;
    }

    public void Map<T>(T entity, string propertyName, object? rawValue) where T : class
    {
        var entityType = entity.GetType();

        if (!_entityPropertyies.ContainsKey(entityType))
        {
            _entityPropertyies.Add(entityType, entityType.GetProperties().ToDictionary(x => x.Name, x => x));
        }

        if (_entityPropertyies[entityType].TryGetValue(propertyName, out var property))
        {
            var value = _scalarValueService.ConvertScalarValue(property.PropertyType, rawValue);

            property.SetValue(entity, value);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(propertyName), $"Unable to find a property named \"{propertyName}\" on type \"{entityType.GetTypeName()}\"");
        }
    }
}