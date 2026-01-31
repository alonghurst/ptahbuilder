using PtahBuilder.BuildSystem.Config;

namespace PtahBuilder.BuildSystem.Services.Reporting;

internal sealed class ReportingService : IReportingService
{
    private readonly IFilesConfig _filesConfig;
    private readonly Dictionary<string, Report> _reports = new();

    public ReportingService(IFilesConfig filesConfig)
    {
        _filesConfig = filesConfig;
    }

    public IReport GetReport(string key)
    {
        if (!_reports.TryGetValue(key, out var report))
        {
            report = new Report();
            _reports[key] = report;
        }

        return report;
    }

    public async Task WriteAllReportsAsync(CancellationToken cancellationToken = default)
    {
        var reportsDirectory = _filesConfig.ReportsDirectory;

        if (!Directory.Exists(reportsDirectory))
        {
            Directory.CreateDirectory(reportsDirectory);
        }

        foreach (var (key, report) in _reports)
        {
            var fileName = SanitizeFileName(key) + ".txt";
            var filePath = Path.Combine(reportsDirectory, fileName);
            var content = report.GetContent();

            await File.WriteAllTextAsync(filePath, content, cancellationToken);
        }
    }

    private static string SanitizeFileName(string key)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = key.ToCharArray();

        for (var i = 0; i < sanitized.Length; i++)
        {
            if (invalid.Contains(sanitized[i]))
            {
                sanitized[i] = '_';
            }
        }

        return new string(sanitized);
    }
}
