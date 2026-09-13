namespace PtahBuilder.Plugins.GraphViz;

internal static class Require
{
    public static string NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be null or whitespace.", paramName);
        }

        return value;
    }
}
