namespace PtahBuilder.BuildSystem.Services;

public interface ICustomValueParserService
{
    object? TryParseValue(Type destinationType, object value, out bool success);
}