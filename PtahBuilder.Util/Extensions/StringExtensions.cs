namespace PtahBuilder.Util.Extensions;

public static class StringExtensions
{
    public static string ToFileTypeWildCard(this string fileType)
    {
        if (!fileType.StartsWith("*."))
        {
            fileType = fileType.StartsWith(".") ? $"*{fileType}" : $"*.{fileType}";
        }

        return fileType;
    }
}