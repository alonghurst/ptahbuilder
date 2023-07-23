using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps;

internal class FindAdditionalTypesToDocumentStep : IStep<TypeToDocument>
{
    private readonly ILogger _logger;

    public FindAdditionalTypesToDocumentStep(ILogger logger)
    {
        _logger = logger;
    }

    public Task Execute(IPipelineContext<TypeToDocument> context, IReadOnlyCollection<Entity<TypeToDocument>> entities)
    {
        var discovered = new List<TypeToDocument>();

        foreach (var entity in entities)
        {
            var type = entity.Value.Type;

            var prefix = (type.Namespace ?? string.Empty).Split('.').FirstOrDefault() ?? "?";

            var relevantProperties = type.GetWritableProperties();

            foreach (var relevantProperty in relevantProperties)
            {
                var relevantType = relevantProperty.PropertyType.GetTypeOrElementType();

                if ((relevantType.Namespace ?? string.Empty).StartsWith(prefix))
                {
                    _logger.Info($"Discovered {relevantType.GetTypeName()} on {type.GetTypeName()}");

                    discovered.Add(new (relevantType));
                }
            }
        }

        context.AddEntities(discovered);

        return Task.CompletedTask;
    }
}