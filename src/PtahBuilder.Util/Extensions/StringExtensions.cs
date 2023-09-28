using System.Text.RegularExpressions;

namespace PtahBuilder.Util.Extensions;

public static class StringExtensions
{
    private static Regex _slugRegex = new("[^a-zA-Z0-9 -]");

    public static string ToFileTypeWildCard(this string fileType)
    {
        if (!fileType.StartsWith("*."))
        {
            fileType = fileType.StartsWith(".") ? $"*{fileType}" : $"*.{fileType}";
        }

        return fileType;
    }

    public static string WithDot(this string extension)
    {
        if (!extension.StartsWith("."))
        {
            return $".{extension}";
        }

        return extension;
    }

    /// <summary>
    /// Splits all members of a string array by a given separator and returns a 1 dimensional array containing all
    /// non-empty trimmed values
    /// </summary>
    public static string[] SplitAndFlatten(this string[] array, string separator = ",")
    {
        return array.SelectMany(x => x.Split(separator, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    public static string ToSlug(this string text)
    {
        text = text.ToLower().Replace(" ", "-");
        
        text = _slugRegex.Replace(text, "");

        while (text.Contains("--"))
        {
            text = text.Replace("--", "-");
        }

        return text;
    }
}