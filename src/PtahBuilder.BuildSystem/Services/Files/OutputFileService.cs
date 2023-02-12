using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Services.Files;

public class OutputFileService : IOutputFileService
{
    private readonly IFilesConfig _filesConfig;
    private readonly IEntityMetadataService _entityMetadataService;

    public OutputFileService(IFilesConfig filesConfig, IEntityMetadataService entityMetadataService)
    {
        _filesConfig = filesConfig;
        _entityMetadataService = entityMetadataService;
    }

    public string GetOutputDirectoryForEntity<T>()
    {
        var directory = Path.Combine(_filesConfig.OutputDirectory, _entityMetadataService.GetSimpleNamePlural<T>());

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
            fileType = $".{fileType}";
        }

        return Path.Combine(directory, $"{entity.Id}{fileType}");
    }
}