namespace PtahBuilder.BuildSystem.Services.Mapping;

public interface IScalarValueService
{
    object? ConvertScalarValue(Type type, object? value);
}