using Humanizer;
using PtahBuilder.BuildSystem.Config;
using PtahBuilder.Util.Extensions;

namespace PtahBuilder.BuildSystem.Services;

public class InputFileService : IInputFileService
{
    private readonly IFilesConfig _filesConfig;

    public InputFileService(IFilesConfig filesConfig)
    {
        _filesConfig = filesConfig;
    }

    public IReadOnlyCollection<string> GetInputFilesForEntity<T>(string fileType)
    {
        var directory = Path.Combine(_filesConfig.DataDirectory, typeof(T).Name.Pluralize());

        if (Directory.Exists(directory))
        {
            fileType = fileType.ToFileTypeWildCard();

            return Directory.GetFiles(directory, fileType);
        }

        return Array.Empty<string>();
    }
}