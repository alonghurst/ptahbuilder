using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public class CodeFile
{
    public string DefinedInNamespace { get; set; }
    public IEnumerable<string> UsingNamespaces { get; set; }

    public SyntaxKind[] AccessModifiers { get; set; }

    public string ClassName { get; set; }

    public Func<IEnumerable<MemberDeclarationSyntax>> Members { get; set; }

    public string Definition()
    {
        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            Constructs.File(writer, () =>
            {
                return Constructs.Namespace(DefinedInNamespace,
                    (UsingNamespaces ?? Enumerable.Empty<string>()).Except(new[] { DefinedInNamespace }),
                    () =>
                    {
                        return Constructs.Class(ClassName, AccessModifiers, () =>
                        {
                            return SyntaxFactory.List(Members());
                        }).AsSingletonSyntaxList();
                    });
            });
        }

        return sb.ToString();
    }
}