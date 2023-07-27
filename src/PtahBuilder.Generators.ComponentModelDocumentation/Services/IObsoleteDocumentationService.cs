using System.Reflection;
using PtahBuilder.Generators.ComponentModelDocumentation.Entities;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Services;

public interface IObsoleteDocumentationService
{
    ObsoleteDocumentation? TypeObsoleteDocumentation(Type type);
    ObsoleteDocumentation? EnumValueObsoleteDocumentation(Type type, object value);
    ObsoleteDocumentation? PropertyObsoleteDocumentation(PropertyInfo property);
}