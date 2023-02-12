using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Files;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class YamlInputStep<T> : IStep<T>
{
    private readonly IYamlService _yamlService;
    private readonly IInputFileService _inputFileService;
    private readonly ILogger _logger;

    public YamlInputStep(IYamlService yamlService, IInputFileService inputFileService, ILogger logger)
    {
        _yamlService = yamlService;
        _inputFileService = inputFileService;
        _logger = logger;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var file in _inputFileService.GetInputFilesForEntity<T>("yaml"))
        {
            _logger.Info($"Reading {file}");

            var text = await File.ReadAllTextAsync(file);

            var entity = _yamlService.Deserialize<T>(text);

            context.AddEntityFromFile(entity, file);
        }
    }
}