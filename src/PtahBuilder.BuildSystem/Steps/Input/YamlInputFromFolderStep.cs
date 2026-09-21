using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class YamlInputFromFolderStep<T> : IStep<T>
{
    private readonly IYamlService _yamlService;
    private readonly IFilesConfig _files;
    private readonly ILogger _logger;
    private readonly string _relativeFolder;

    public YamlInputFromFolderStep(IYamlService yamlService, IFilesConfig files, ILogger logger, string relativeFolder)
    {
        _yamlService = yamlService;
        _files = files;
        _logger = logger;
        _relativeFolder = relativeFolder;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var file in ListYamlFiles(_files.DataDirectory, _relativeFolder))
        {
            _logger.Verbose($"Reading {file}");

            var text = await File.ReadAllTextAsync(file);

            if (string.IsNullOrWhiteSpace(text))
            {
                context.AddEntityFromFile(Activator.CreateInstance<T>(), file);
                continue;
            }

            var (entity, metadata) = _yamlService.DeserializeAndGetMetadata<T>(text);

            context.AddEntityFromFile(entity, file, metadata);
        }
    }

    public static IReadOnlyList<string> ListYamlFiles(string dataDirectory, string relativeFolder)
    {
        if (string.IsNullOrWhiteSpace(dataDirectory) || string.IsNullOrWhiteSpace(relativeFolder))
        {
            return Array.Empty<string>();
        }

        var directory = Path.Combine(dataDirectory, relativeFolder);

        if (!Directory.Exists(directory))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(directory, "*.yaml", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
