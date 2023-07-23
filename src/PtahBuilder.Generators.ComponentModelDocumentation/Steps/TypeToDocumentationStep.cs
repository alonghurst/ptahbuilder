using System.ComponentModel.DataAnnotations;
using System.Reflection;
using PtahBuilder.BuildSystem.Entities;
using PtahBuilder.BuildSystem.Execution.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Steps;

internal class TypeToDocumentationStep : IStep<TypeDocumentation>
{
    private readonly IEntityProvider<TypeToDocument> _entityProvider;

    public TypeToDocumentationStep(IEntityProvider<TypeToDocument> entityProvider)
    {
        _entityProvider = entityProvider;
    }

    public Task Execute(IPipelineContext<TypeDocumentation> context, IReadOnlyCollection<Entity<TypeDocumentation>> entities)
    {
        foreach (var entity in _entityProvider.Entities)
        {
            var type = entity.Value.Value.Type;

            var properties = DocumentPropertiesForType(type);
            var enumValues = DocumentEnumValuesForType(type);

            var typeInfo = type.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            var (name, description) = NameAndDescriptionFromDisplayAttribute(typeInfo, type.GetTypeName());

            var typeDocumentation = new TypeDocumentation(type, name, description, properties, enumValues);

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

        var valuesWithAttribute = type.GetEnumValuesWithOptionalAttribute<DisplayAttribute>().ToArray();

        return valuesWithAttribute.Select(x =>
            {
                var value = x.value.ToString() ?? "";
                var (name, description) = NameAndDescriptionFromDisplayAttribute(x.attribute, value);

                return new EnumValueDocumentation(value, name, description);
            })
            .ToArray();
    }

    private PropertyDocumentation[] DocumentPropertiesForType(Type type)
    {
        var properties = type.GetWritableProperties()
            .Select(x =>
            {
                var propertyInfo = x.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;

                var (propertyName, propertyDescription) = NameAndDescriptionFromDisplayAttribute(propertyInfo, x.Name);

                return new PropertyDocumentation(x, propertyName, propertyDescription);
            })
            .ToArray();
        return properties;
    }

    private (string name, string description) NameAndDescriptionFromDisplayAttribute(DisplayAttribute? attribute, string name)
    {
        if (attribute?.Name is { } s)
        {
            name = s;
        }

        return (name, attribute?.Description ?? string.Empty);
    }
}