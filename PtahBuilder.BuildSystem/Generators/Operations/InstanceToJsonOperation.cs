using System.Text.Json;
using PtahBuilder.BuildSystem.Generators.Context;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.BuildSystem.Generators.Operations;

public class InstanceToJsonProvider<T> : Operation<T>
{
    private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    }; 
    
    public override int Priority => int.MaxValue;


    public InstanceToJsonProvider(IOperationContext<T> context) : base(context)
    {
    }

    [Operate]
    public void Operate()
    {
        var path = PathResolver.OutputFile(MetadataResolver.EntityTypeName, ".json");

        var entities = Entities.WhereIsNotBuildOnly();

        var json = JsonSerializer.Serialize(entities.ToArray(), _options);

        File.WriteAllText(path, json);
    }
}