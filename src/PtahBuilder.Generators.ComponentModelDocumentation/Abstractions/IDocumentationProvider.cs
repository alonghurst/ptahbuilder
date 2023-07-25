using System.Reflection;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Abstractions;

public interface IDocumentationProvider
{
    (string name, string description) DocumentType(Type type);
    EnumValueDocumentation DocumentEnumValue(Type type, object value);
    PropertyDocumentation DocumentProperty(Type type, PropertyInfo property);
}