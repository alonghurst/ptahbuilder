namespace PtahBuilder.BuildSystem.Config;

public interface IFilesConfig
{
    string WorkingDirectory { get; }
    string DataDirectory { get; }
    string OutputDirectory { get; }
    string ReportsDirectory { get; }
    Dictionary<string, string> AdditionalDirectories { get; }
}