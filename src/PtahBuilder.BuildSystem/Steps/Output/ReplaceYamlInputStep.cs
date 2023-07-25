using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class ReplaceYamlInputStep<T> : IStep<T>
{
    private readonly IYamlService _yamlService;

    public ReplaceYamlInputStep(IYamlService yamlService)
    {
        _yamlService = yamlService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var tasks = entities.Select(x =>
        {
            if (x.Metadata.TryGetValue(MetadataKeys.SourceFile, out var value) && value is string file)
            {
                var yaml = _yamlService.Serialize(x.Value!);

                return File.WriteAllTextAsync(file, yaml);
            }

            return Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }
}