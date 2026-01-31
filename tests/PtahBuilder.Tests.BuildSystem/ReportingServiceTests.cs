using Microsoft.Extensions.DependencyInjection;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Reporting;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class ReportingServiceTests
{
    private static IReportingService CreateService(string? reportsDirectory = null)
    {
        reportsDirectory ??= Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var services = new ServiceCollection()
            .AddSingleton<IFilesConfig>(new TestFilesConfig(reportsDirectory))
            .AddPtahBuildSystemServices();
        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<IReportingService>();
    }

    private sealed class TestFilesConfig : IFilesConfig
    {
        public TestFilesConfig(string reportsDirectory) => ReportsDirectory = reportsDirectory;
        public string WorkingDirectory => ".";
        public string DataDirectory => "./Data";
        public string OutputDirectory => "./Output";
        public string ReportsDirectory { get; }
        public Dictionary<string, string> AdditionalDirectories => new();
    }

    [Fact]
    public void GetReport_WithSameKey_ReturnsSameInstance()
    {
        var service = CreateService();
        var key = "TestReport";

        var report1 = service.GetReport(key);
        var report2 = service.GetReport(key);

        Assert.Same(report1, report2);
    }

    [Fact]
    public void GetReport_WithDifferentKeys_ReturnsDifferentInstances()
    {
        var service = CreateService();
        var report1 = service.GetReport("ReportA");
        var report2 = service.GetReport("ReportB");

        Assert.NotSame(report1, report2);
    }

    [Fact]
    public async Task GetReport_AppendLine_AccumulatesContent()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        var report = service.GetReport("Summary");
        report.AppendLine("Line 1");
        report.AppendLine("Line 2");
        report.AppendLine("Line 3");

        await service.WriteAllReportsAsync();

        var filePath = Path.Combine(reportsDir, "Summary.txt");
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Line 1", content);
        Assert.Contains("Line 2", content);
        Assert.Contains("Line 3", content);

        try { Directory.Delete(reportsDir, true); } catch { /* ignore cleanup */ }
    }

    [Fact]
    public async Task WriteAllReportsAsync_CreatesReportsDirectory_WhenMissing()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        Assert.False(Directory.Exists(reportsDir));

        var service = CreateService(reportsDir);
        service.GetReport("Dummy").AppendLine("x");
        await service.WriteAllReportsAsync();

        Assert.True(Directory.Exists(reportsDir));

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }

    [Fact]
    public async Task WriteAllReportsAsync_WritesKeyAsFileName()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        service.GetReport("MyReport").AppendLine("Content");
        service.GetReport("Another").AppendLine("Other");

        await service.WriteAllReportsAsync();

        Assert.True(File.Exists(Path.Combine(reportsDir, "MyReport.txt")));
        Assert.True(File.Exists(Path.Combine(reportsDir, "Another.txt")));

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }

    [Fact]
    public void GetReport_T_Extension_UsesTypeFullNameAsKey()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        var report = service.GetReport<string>();
        report.AppendLine("String report");

        Assert.Same(report, service.GetReport<string>());

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }

    [Fact]
    public async Task GetReport_T_Extension_WritesToFileNamedAfterType()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        service.GetReport<int>().AppendLine("Int report");
        await service.WriteAllReportsAsync();

        var expectedFileName = "System.Int32.txt";
        var filePath = Path.Combine(reportsDir, expectedFileName);
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Int report", content);

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }

    [Fact]
    public void GetReport_Type_Extension_ReturnsReportForKeyedByTypeFullName()
    {
        var service = CreateService();
        var report = service.GetReport(typeof(ReportingServiceTests));
        var report2 = service.GetReport(typeof(ReportingServiceTests));

        Assert.Same(report, report2);
    }

    [Fact]
    public void GetReportFor_T_Extension_ReturnsReportForKeyedByType()
    {
        var service = CreateService();
        var dummy = new { Id = 1 };
        var report = service.GetReportFor(dummy);

        Assert.NotNull(report);
        report.AppendLine("test");
        Assert.Same(report, service.GetReportFor(dummy));
    }

    [Fact]
    public async Task WriteAllReportsAsync_WithNoReports_DoesNotThrow()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        await service.WriteAllReportsAsync();

        Assert.True(Directory.Exists(reportsDir));
        Assert.Empty(Directory.GetFiles(reportsDir));

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }

    [Fact]
    public async Task WriteAllReportsAsync_SanitizesInvalidFileNameChars()
    {
        var reportsDir = Path.Combine(Path.GetTempPath(), "PtahBuilder.Reports", Guid.NewGuid().ToString("N"));
        var service = CreateService(reportsDir);

        var invalidKey = "Report:With<Invalid>Chars|In*Name?";
        service.GetReport(invalidKey).AppendLine("Content");
        await service.WriteAllReportsAsync();

        var files = Directory.GetFiles(reportsDir);
        Assert.Single(files);
        Assert.Contains("Content", await File.ReadAllTextAsync(files[0]));

        try { Directory.Delete(reportsDir, true); } catch { /* ignore */ }
    }
}
