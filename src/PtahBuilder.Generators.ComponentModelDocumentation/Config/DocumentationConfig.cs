using PtahBuilder.Util.Helpers;

namespace PtahBuilder.Generators.ComponentModelDocumentation.Config;

public class DocumentationConfig
{
    public string  OutputDirectory { get; } 

    private readonly List<Type> _typesToDocument  = new();

    public DocumentationConfig AddType(Type type)
    {
        _typesToDocument.Add(type);
        return this;
    }

    public DocumentationConfig AddTypesInheritedFrom(Type type, bool includeAbstract = false)
    {
        var types = ReflectionHelper.GetLoadedTypesThatAreAssignableTo(type, !includeAbstract);

        foreach (var childType in types)
        {
            AddType(childType);
        }

        return this;
    }

    internal IReadOnlyCollection<Type> GetTypesToDocument() => _typesToDocument.Distinct().ToArray();
}