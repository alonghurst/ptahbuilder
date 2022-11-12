using System.Text.Json;
using Humanizer;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.BuildSystem.Generators.Operations;

public class InstanceToJsonOperation<T> : Operation<T>
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override int Priority => int.MaxValue;


    public InstanceToJsonOperation(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public void Operate()
    {
        var directory = PathResolver.OutputDirectory(MetadataResolver.EntityShortName.Pluralize());

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        foreach (var entity in Entities.WhereIsNotBuildOnly())
        {
            var path = Path.Combine(directory, $"{MetadataResolver.GetEntityId(entity)}.json");

            var json = JsonSerializer.Serialize(entity, _options);

            File.WriteAllText(path, json);
        }
    }
}