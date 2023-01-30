namespace PtahBuilder.BuildSystem.Config;

public interface IFilesConfig
{
    string WorkingDirectory { get; set; }
    string DataDirectory { get; set; }
    string OutputDirectory { get; set; }
}