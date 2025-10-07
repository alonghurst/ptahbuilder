namespace PtahBuilder.BuildSystem.Services.Parsing;

public interface ICustomValueParserService
{
    bool TryParseValue(Type destinationType, object value, out object? result);
}