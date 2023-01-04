using Newtonsoft.Json;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.BuildSystem.Generators.Operations;

public class InstanceToJsonArrayOperation<T> : Operation<T>
{

    public override int Priority => int.MaxValue;


    public InstanceToJsonArrayOperation(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public void Operate()
    {
        var path = PathResolver.OutputFile(MetadataResolver.EntityTypeName, ".json");

        var entities = Entities.WhereIsNotBuildOnly();

        var json = JsonConvert.SerializeObject(entities.ToArray(), Settings.JsonSerializerSettings);

        File.WriteAllText(path, json);
    }
}