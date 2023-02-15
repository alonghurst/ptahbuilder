using PtahBuilder.BuildSystem.Config;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using YamlDotNet.Serialization;

namespace PtahBuilder.BuildSystem.Steps.Output;

public class ReplaceYamlInputStep<T> : IStep<T>
{
    public async Task Execute(IPipelineContext<T> context, IReadOnlyCollection<Entity<T>> entities)
    {
        var serializer = new SerializerBuilder().Build();

        var tasks = entities.Select(x =>
        {
            if (x.Metadata.TryGetValue(MetadataKeys.SourceFile, out var value) && value is string file)
            {
                var yaml = serializer.Serialize(x.Value!);

                return File.WriteAllTextAsync(file, yaml);
            }

            return Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }
}