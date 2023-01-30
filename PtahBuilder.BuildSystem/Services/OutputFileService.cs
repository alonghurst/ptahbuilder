using Humanizer;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem.Services
{
    public class OutputFileService : IOutputFileService
    {
        private readonly IFilesConfig _filesConfig;

        public OutputFileService(IFilesConfig filesConfig)
        {
            _filesConfig = filesConfig;
        }

        public string GetOutputDirectoryForEntity<T>()
        {
            var directory = Path.Combine(_filesConfig.DataDirectory, typeof(T).Name.Pluralize());

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public string GetOutputFileForEntity<T>(Entity<T> entity, string fileType)
        {
            var directory = GetOutputDirectoryForEntity<T>();

            if (!fileType.StartsWith("."))
            {
                fileType = ".";
            }

            return Path.Combine(directory, $"{entity.Id}{fileType}");
        }
    }
}
