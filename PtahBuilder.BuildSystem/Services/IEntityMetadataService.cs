namespace PtahBuilder.BuildSystem.Services;

public interface IEntityMetadataService
{
    string GetSimpleName<T>();
    string GetSimpleName(Type type);
    string GetSimpleNamePlural<T>();
    string GetSimpleNamePlural(Type type);
}