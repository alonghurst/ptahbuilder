namespace PtahBuilder.BuildSystem.Services.Mapping;

public interface IDynamicMappingService
{
    void Map<T>(T entity, string propertyName, object? rawValue) where T : class;
}