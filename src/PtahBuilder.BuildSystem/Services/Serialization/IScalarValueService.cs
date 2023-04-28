namespace PtahBuilder.BuildSystem.Services.Serialization;

public interface IScalarValueService
{
    object? ConvertScalarValue(Type type, object? value);
}