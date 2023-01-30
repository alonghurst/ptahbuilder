using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Execution.Pipelines;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services;
using PtahBuilder.Util.Services;

namespace PtahBuilder.BuildSystem.Stages.Input;

public class JsonInputStage<T> : IStage<T>
{
    private readonly IJsonService _jsonService;
    private readonly IInputFileService _inputFileService;

    public JsonInputStage(IJsonService jsonService, IInputFileService inputFileService)
    {
        _jsonService = jsonService;
        _inputFileService = inputFileService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var file in _inputFileService.GetInputFilesForEntity<T>("json"))
        {
            var text = await File.ReadAllTextAsync(file);

            var entity = _jsonService.Deserialize<T>(text);

            context.AddEntityFromFile(entity, file);
        }
    }
}