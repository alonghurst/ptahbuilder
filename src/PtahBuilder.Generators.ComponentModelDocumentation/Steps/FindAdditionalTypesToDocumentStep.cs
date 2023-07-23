using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Services.Logging;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps;

public class FindAdditionalTypesToDocumentStep : IStep<Type>
{
    private readonly ILogger _logger;

    public FindAdditionalTypesToDocumentStep(ILogger logger)
    {
        _logger = logger;
    }

    public Task Execute(IPipelineContext<Type> context, IReadOnlyCollection<Entity<Type>> entities)
    {
        foreach (var entity in entities)
        {
            var type = entity.Value;

            var prefix = (type.Namespace ?? string.Empty).Split('.').FirstOrDefault() ?? "?";

            var relevantProperties = type.GetWritableProperties();

            foreach (var relevantProperty in relevantProperties)
            {
                var relevantType = relevantProperty.PropertyType;

                if ((relevantType.Namespace ?? string.Empty).StartsWith(prefix))
                {
                    _logger.Info($"Discovered {relevantType.GetTypeName()} on {type.GetTypeName()}");

                    context.AddEntity(relevantType);
                }
            }
        }

        return Task.CompletedTask;
    }
}