using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Generators.ComponentModelDocumentation.Services;
using PtahBuilder.Util.Extensions.Reflection;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps;

internal class TypeToDocumentationStep : IStep<TypeDocumentation>
{
    private readonly IEntityProvider<TypeToDocument> _entityProvider;
    private readonly IDocumentationProvider _documentationProvider;
    private readonly IObsoleteDocumentationService _obsoleteDocumentationService;

    public TypeToDocumentationStep(IEntityProvider<TypeToDocument> entityProvider, IDocumentationProvider documentationProvider, IObsoleteDocumentationService obsoleteDocumentationService)
    {
        _entityProvider = entityProvider;
        _documentationProvider = documentationProvider;
        _obsoleteDocumentationService = obsoleteDocumentationService;
    }

    public Task Execute(IPipelineContext<TypeDocumentation> context, IReadOnlyCollection<Entity<TypeDocumentation>> entities)
    {
        foreach (var entity in _entityProvider.Entities)
        {
            var type = entity.Value.Value.Type;

            var properties = DocumentPropertiesForType(type);
            var enumValues = DocumentEnumValuesForType(type);
            var obsolete = _obsoleteDocumentationService.TypeObsoleteDocumentation(type);

            var (name, description) = _documentationProvider.DocumentType(type);

            var typeDocumentation = new TypeDocumentation(type, name, description, properties, enumValues, obsolete);

            context.AddEntity(typeDocumentation);
        }

        return Task.CompletedTask;
    }

    private IReadOnlyCollection<EnumValueDocumentation> DocumentEnumValuesForType(Type type)
    {
        if (!type.IsEnum)
        {
            return Array.Empty<EnumValueDocumentation>();
        }

        var documentedValues = new List<EnumValueDocumentation>();

        foreach (var value in Enum.GetValues(type))
        {
            var documentation = _documentationProvider.DocumentEnumValue(type, value);

            documentedValues.Add(documentation);
        }

        return documentedValues;
    }

    private PropertyDocumentation[] DocumentPropertiesForType(Type type)
    {
        var properties = type.GetWritableProperties()
            .Select(x => _documentationProvider.DocumentProperty(type, x))
            .ToArray();

        return properties;
    }
}