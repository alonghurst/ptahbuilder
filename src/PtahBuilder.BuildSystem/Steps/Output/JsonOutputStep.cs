using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Files;
using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class JsonOutputStep<T> : IStep<T>
{
    private readonly IJsonService _jsonService;
    private readonly IOutputFileService _outputFileService;

    public JsonOutputStep(IJsonService jsonService, IOutputFileService outputFileService)
    {
        _jsonService = jsonService;
        _outputFileService = outputFileService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var tasks = entities.Select(x =>
        {
            var file = _outputFileService.GetOutputFileForEntity(x, "json");

            var json = _jsonService.Serialize(x.Value);

            return File.WriteAllTextAsync(file, json);
        });

        await Task.WhenAll(tasks);
    }
}