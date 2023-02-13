using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Services.Files;

namespace PtahBuilder.BuildSystem.Steps.Process;

public class MoveInputFileStep<T>:IStep<T>
{
    private readonly string _propertyName;
    private readonly IInputFileService _inputFileService;

    public MoveInputFileStep( IInputFileService inputFileService, string propertyName)
    {
        _propertyName = propertyName;
        _inputFileService = inputFileService;
    }

    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var property = typeof(T).GetProperty(_propertyName) ?? throw new InvalidOperationException($"Unable to find property named {_propertyName}");

        foreach (var entity in entities)
        {
            if (entity.Metadata.TryGetValue(MetadataKeys.SourceFile, out var value) && value is string filename)
            {
                var desiredCategory = property.GetValue(entity.Value)?.ToString();

                if (!string.IsNullOrWhiteSpace(desiredCategory))
                {
                    var currentDirectory = Path.GetFileName(Path.GetDirectoryName(filename) ?? string.Empty);

                    if (currentDirectory != desiredCategory)
                    {
                        var desiredDirectory = Path.Combine(_inputFileService.GetInputDirectoryForEntityType<T>(), desiredCategory);
                            
                        if (!Directory.Exists(desiredDirectory))
                        {
                            Directory.CreateDirectory(desiredDirectory);
                        }

                        var desiredFilename = Path.Combine(desiredDirectory, Path.GetFileName(filename));

                        var contents = await File.ReadAllTextAsync(filename);

                        await File.WriteAllTextAsync(desiredFilename, contents);

                        File.Delete(filename);
                    }
                }
            }
        }
    }
}