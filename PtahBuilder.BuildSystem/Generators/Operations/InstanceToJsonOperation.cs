using Humanizer;
using Newtonsoft.Json;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.BuildSystem.Generators.Operations;

public class InstanceToJsonOperation<T> : Operation<T>
{
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

            var json = JsonConvert.SerializeObject(entity, Settings.JsonSerializerSettings);

            File.WriteAllText(path, json);
        }
    }
}