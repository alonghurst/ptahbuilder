namespace PtahBuilder.BuildSystem.Services;

public interface ICustomValueParserService
{
    bool TryParseValue(Type destinationType, object value, out object? result);
}