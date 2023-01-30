using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.CodeGeneration;
using PtahBuilder.LegacyBuildSystem.Metadata;

namespace PtahBuilder.LegacyBuildSystem.Syntax;

public abstract class InstanceToTypeFactoryBase<T>
{
    public InstanceToTypeFactoryBase(Logger logger, BaseDataMetadataResolver<T> metadataResolver)
    {
        Logger = logger;
        Mapper = new InstanceToSyntaxMapper(Logger);
        MetadataResolver = metadataResolver;
    }

    public Logger Logger { get; }
    public InstanceToSyntaxMapper Mapper { get; }
    public BaseDataMetadataResolver<T> MetadataResolver { get; }

    public void Generate(string definedInNamespace, IEnumerable<T> instances, string file)
    {
        var instancesAr = instances.ToArray();

        var member = new[]
        {
            Constructs.Class(MetadataResolver.DataDirectoryToOperateIn, new[]
                {
                    SyntaxKind.PublicKeyword,
                    SyntaxKind.StaticKeyword,
                    SyntaxKind.PartialKeyword
                },
                () => SyntaxFactory.List(SubClass(instancesAr))
            )
        };

        var namespaces = instancesAr.Select(i => i.GetType().Namespace)
            .Union(Mapper.FoundTypes.SelectMany(ExtractNamespaces))
            .Except(new[] { "System" })
            .Distinct();

        var codeFile = new CodeFile
        {
            DefinedInNamespace = definedInNamespace,
            UsingNamespaces = namespaces,
            ClassName = "Factory",
            AccessModifiers = new[]
            {
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            },
            Members = () => member
        };

        File.WriteAllText(file, codeFile.Definition());
    }

    private IEnumerable<string> ExtractNamespaces(Type type)
    {
        if (type.Namespace != null)
        {
            yield return type.Namespace;
        }
        if (type.IsGenericType)
        {
            foreach (var genericArgument in type.GetGenericArguments())
            {
                var namespaces = ExtractNamespaces(genericArgument);
                foreach (var ns in namespaces)
                {
                    yield return ns;
                }
            }
        }
        foreach (var property in type.GetProperties())
        {
            var namespaces = ExtractNamespaces(property.PropertyType);
            foreach (var ns in namespaces)
            {
                yield return ns;
            }
        }
    }

    protected abstract string SubClassName { get; }

    private IEnumerable<MemberDeclarationSyntax> SubClass(T[] instancesAr)
    {
        yield return Constructs.Class(SubClassName, new[]
            {
                SyntaxKind.PublicKeyword,
                SyntaxKind.StaticKeyword,
                SyntaxKind.PartialKeyword
            },
            () => SyntaxFactory.List(Members(instancesAr))
        );
    }

    protected abstract IEnumerable<MemberDeclarationSyntax> Members(T[] instancesAr);
}