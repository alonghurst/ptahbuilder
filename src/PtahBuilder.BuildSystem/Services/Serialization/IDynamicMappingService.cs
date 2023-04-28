namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IDynamicMappingService
{
    void Map<T>(T entity, string propertyName, object? rawValue) where T : class;
}