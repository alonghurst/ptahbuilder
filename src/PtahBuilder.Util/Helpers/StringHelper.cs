namespace PtahBuilder.Util.Helpers;

public static class StringHelper
{
    public static string LongestCommonPrefix(string a, string b)
    {
        var result = string.Empty;

        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            var c = a[i];
            if (c != b[i])
            {
                break;
            }
            result += c;
        }

        return result;
    }

    public static string WithPrefix(this string str, char prefix)
    {
        if (str[0] != prefix)
        {
            return $"{prefix}{str}";
        }

        return str;
    }
}