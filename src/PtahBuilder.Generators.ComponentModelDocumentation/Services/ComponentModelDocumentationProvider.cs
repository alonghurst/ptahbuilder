using System.ComponentModel.DataAnnotations;
using System.Reflection;
using PtahBuilder.Generators.ComponentModelDocumentation.Abstractions;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;
using PtahBuilder.Util.Extensions.Reflection;
using YamlDotNet.Core.Tokens;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Services;

internal class ComponentModelDocumentationProvider : IDocumentationProvider
{
    private readonly IObsoleteDocumentationService _obsoleteDocumentationService;

    public ComponentModelDocumentationProvider(IObsoleteDocumentationService obsoleteDocumentationService)
    {
        _obsoleteDocumentationService = obsoleteDocumentationService;
    }

    public (string name, string description) DocumentType(Type type)
    {
        var typeInfo = type.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        return NameAndDescriptionFromDisplayAttribute(typeInfo, type.GetTypeName());
    }

    public EnumValueDocumentation DocumentEnumValue(Type type, object value)
    {
        var attribute = type.GetCustomAttributeForEnumValue<DisplayAttribute>(value);
        var obsolete = _obsoleteDocumentationService.EnumValueObsoleteDocumentation(type, value);

        var (name, description) = NameAndDescriptionFromDisplayAttribute(attribute, value.ToString() ?? string.Empty);

        return new EnumValueDocumentation(value.ToString() ?? string.Empty, name, description, obsolete);
    }

    public PropertyDocumentation DocumentProperty(Type type, PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
        var obsolete = _obsoleteDocumentationService.PropertyObsoleteDocumentation(property);

        var (propertyName, propertyDescription) = NameAndDescriptionFromDisplayAttribute(attribute, property.Name);

        return new PropertyDocumentation(property, propertyName, propertyDescription, obsolete);
    }

    private (string name, string description) NameAndDescriptionFromDisplayAttribute(DisplayAttribute? attribute, string name)
    {
        if (attribute?.Name is { } s && !string.IsNullOrWhiteSpace(s))
        {
            name = s;
        }

        return (name, attribute?.Description ?? string.Empty);
    }
}