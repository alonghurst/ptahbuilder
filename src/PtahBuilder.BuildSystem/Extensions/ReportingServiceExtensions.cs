using PtahBuilder.BuildSystem.Services.Reporting;

namespace PtahBuilder.BuildSystem.Extensions;

public static class ReportingServiceExtensions
{
    /// <summary>
    /// Gets a report using the full type name of <typeparamref name="T"/> as the key.
    /// </summary>
    public static IReport GetReport<T>(this IReportingService reportingService)
    {
        return reportingService.GetReport(GetReportKey(typeof(T)));
    }

    /// <summary>
    /// Gets a report using the full type name of <paramref name="type"/> as the key.
    /// </summary>
    public static IReport GetReport(this IReportingService reportingService, Type type)
    {
        return reportingService.GetReport(GetReportKey(type));
    }

    /// <summary>
    /// Gets a report using the type name of the given instance as the key.
    /// </summary>
    public static IReport GetReportFor<T>(this IReportingService reportingService, T _)
    {
        return reportingService.GetReport(GetReportKey(typeof(T)));
    }

    private static string GetReportKey(Type type)
    {
        return type.FullName ?? type.Name;
    }
}
