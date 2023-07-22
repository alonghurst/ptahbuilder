namespace PtahBuilder.Util.Helpers;

public static class ConvertHelper
{
    public static double StringToDouble(object input)
    {
        if (double.TryParse(input.ToString(), out double d))
        {
            return d;
        }
        return 0;
    }

    public static float StringToFloat(object input)
    {
        if (float.TryParse(input.ToString(), out float d))
        {
            return d;
        }
        return 0;
    }

    public static int StringToInt(object input)
    {
        if (int.TryParse(input.ToString(), out int d))
        {
            return d;
        }
        return 0;
    }

    public static bool StringToBoolean(object? input)
    {
        if (input is string s)
        {
            if (s.Trim() == "1")
            {
                return true;
            }
            if (s.Trim() == "0")
            {
                return false;
            }
        }

        if (bool.TryParse(input?.ToString() ?? string.Empty, out bool d))
        {
            return d;
        }
        return false;
    }
}