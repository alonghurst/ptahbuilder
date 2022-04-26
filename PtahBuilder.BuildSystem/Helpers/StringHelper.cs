namespace PtahBuilder.BuildSystem.Helpers;

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
}