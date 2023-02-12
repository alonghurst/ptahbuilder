using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem.Services.Files;

public class InputFileService : IInputFileService
{
    private readonly IFilesConfig _filesConfig;
    private readonly IEntityMetadataService _entityMetadataService;

    public InputFileService(IFilesConfig filesConfig, IEntityMetadataService entityMetadataService)
    {
        _filesConfig = filesConfig;
        _entityMetadataService = entityMetadataService;
    }

    public IReadOnlyCollection<string> GetInputFilesForEntity<T>(string fileType)
    {
        var directory = Path.Combine(_filesConfig.DataDirectory, _entityMetadataService.GetSimpleNamePlural<T>());

        if (Directory.Exists(directory))
        {
            fileType = fileType.ToFileTypeWildCard();

            return Directory.GetFiles(directory, fileType, SearchOption.AllDirectories);
        }

        return Array.Empty<string>();
    }
}