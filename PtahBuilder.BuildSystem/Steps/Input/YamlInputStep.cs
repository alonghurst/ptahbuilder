using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Services.Files;
using PtahBuilder.BuildSystem.Services.Serialization;

namespace PtahBuilder.BuildSystem.Steps.Input;

public class YamlInputStep<T> : IStep<T>
{
    private readonly IYamlService _yamlService;
    private readonly IInputFileService _inputFileService;

    public YamlInputStep(IYamlService yamlService, IInputFileService inputFileService)
    {
        _yamlService = yamlService;
        _inputFileService = inputFileService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        foreach (var file in _inputFileService.GetInputFilesForEntity<T>("yaml"))
        {
            var text = await File.ReadAllTextAsync(file);

            var entity = _yamlService.Deserialize<T>(text);

            context.AddEntityFromFile(entity, file);
        }
    }
}