namespace PtahBuilder.BuildSystem.Helpers;

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
}