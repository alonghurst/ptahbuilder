namespace PtahBuilder.BuildSystem.Config;

public class FilesConfig : IFilesConfig
{
    public string WorkingDirectory { get; set; } = ".";

    public string DataDirectory { get; set; } = "./Data";
    public string OutputDirectory { get; set; } = "./Output";
    public string ReportsDirectory { get; set; } = "./Reports";

    public Dictionary<string, string> AdditionalDirectories { get; } = new();

    public void Configure(string workingDirectory, string relativeDataDirectory = "Data", string relativeOutputDirectory = "Output", string relativeReportsDirectory = "Reports")
    {
        WorkingDirectory = Path.GetFullPath(workingDirectory);
        DataDirectory = Path.Combine(WorkingDirectory, relativeDataDirectory);
        OutputDirectory = Path.Combine(WorkingDirectory, relativeOutputDirectory);
        ReportsDirectory = Path.Combine(WorkingDirectory, relativeReportsDirectory);

        if (!Directory.Exists(OutputDirectory))
        {
            Directory.CreateDirectory(OutputDirectory);
        }
    }
}