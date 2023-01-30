using Newtonsoft.Json;
using PtahBuilder.LegacyBuildSystem.Generators.Context;
using PtahBuilder.LegacyBuildSystem.Helpers;

namespace PtahBuilder.LegacyBuildSystem.Generators.Operations;

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