using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class JsonDumpOutputStep<T> : IStep<T>
{
    private readonly IJsonService _jsonService;
    private readonly ILogger _logger;

    public JsonDumpOutputStep(IJsonService jsonService, ILogger logger)
    {
        _jsonService = jsonService;
        _logger = logger;
    }

    public Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var entity in entities)
        {
            var json = _jsonService.Serialize(entity.Value);

            _logger.Info(json);
        }

        return Task.CompletedTask;
    }
}