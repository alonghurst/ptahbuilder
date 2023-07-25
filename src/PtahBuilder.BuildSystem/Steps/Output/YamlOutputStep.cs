using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Files;
using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class YamlOutputStep<T> : IStep<T>
{
    private readonly IYamlService _yamlService;
    private readonly IOutputFileService _outputFileService;

    public YamlOutputStep(IYamlService yamlService, IOutputFileService outputFileService)
    {
        _yamlService = yamlService;
        _outputFileService = outputFileService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var tasks = entities.Select(x =>
        {
            var file = _outputFileService.GetOutputFileForEntity(x, "yaml");

            var json = _yamlService.Serialize(x.Value);

            return File.WriteAllTextAsync(file, json);
        });

        await Task.WhenAll(tasks);
    }
}