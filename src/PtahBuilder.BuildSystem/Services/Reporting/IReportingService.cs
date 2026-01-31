namespace PtahBuilder.BuildSystem.Services.Reporting;

public interface IReportingService
{
    IReport GetReport(string key);

    Task WriteAllReportsAsync(CancellationToken cancellationToken = default);
}
