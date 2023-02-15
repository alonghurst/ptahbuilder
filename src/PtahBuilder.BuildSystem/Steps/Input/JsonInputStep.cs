using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Files;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class JsonInputStep<T> : IStep<T>
{
    private readonly ILogger _logger;
    private readonly IJsonService _jsonService;
    private readonly IInputFileService _inputFileService;

    public JsonInputStep(IJsonService jsonService, IInputFileService inputFileService, ILogger logger)
    {
        _jsonService = jsonService;
        _inputFileService = inputFileService;
        _logger = logger;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var file in _inputFileService.GetInputFilesForEntityType<T>("json"))
        {
            _logger.Verbose($"Reading {file}");

            var text = await File.ReadAllTextAsync(file);

            var entity = _jsonService.Deserialize<T>(text);

            context.AddEntityFromFile(entity, file);
        }
    }
}