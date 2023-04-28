﻿namespace PtahBuilder.Util.Helpers;

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
}