﻿namespace PtahBuilder.BuildSystem.Config
{
    public class FilesConfig : IFilesConfig
    {
        public string WorkingDirectory { get; set; } = ".";

        public string DataDirectory { get; set; } = "./Data";
        public string OutputDirectory { get; set; } = "./Output";

        public void Configure(string workingDirectory, string relativeDataDirectory = "Data", string relativeOutputDirectory = "Output")
        {
            WorkingDirectory = Path.GetFullPath(workingDirectory);
            DataDirectory = Path.Combine(WorkingDirectory, relativeDataDirectory);
            OutputDirectory = Path.Combine(WorkingDirectory, relativeOutputDirectory);
        }
    }
}
