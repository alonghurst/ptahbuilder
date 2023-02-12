using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace PtahBuilder.CodeGeneration;

public static partial class Constructs
{
    public static EnumDeclarationSyntax Enum(string name, IEnumerable<Tuple<int, string>> values)
    {
        var valuesList = new List<SyntaxNodeOrToken>();

        foreach (var v in values)
        {
            valuesList.Add(
                SyntaxFactory.EnumMemberDeclaration(SyntaxFactory.Identifier(v.Item2))
                    .WithEqualsValue(SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(v.Item1))))
            );
            if (v != values.Last())
                valuesList.Add(Tokens.Comma);
        }

        return SyntaxFactory.EnumDeclaration(name)
            .WithModifiers(Tokens.PublicModifier)
            .WithMembers(SyntaxFactory.SeparatedList<EnumMemberDeclarationSyntax>(
                SyntaxFactory.SeparatedList<EnumMemberDeclarationSyntax>(valuesList))
            );
    }

    public static void File(TextWriter writer, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        var workspace = new AdhocWorkspace();
        var options = workspace.Options
            .WithChangedOption(CSharpFormattingOptions.NewLineForMembersInObjectInit, true);

        var cu = SyntaxFactory.CompilationUnit()
            .WithMembers(members())
            .WithEndOfFileToken(SyntaxFactory.Token(SyntaxKind.EndOfFileToken));
        var formattedNode = Formatter.Format(cu, workspace, options);
        formattedNode.WriteTo(writer);
    }

    public static SyntaxList<MemberDeclarationSyntax> Interface(string name, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        return Interface(name, new string[0], members);
    }

    public static SyntaxList<MemberDeclarationSyntax> Interface(string name, IEnumerable<string> implements, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        var contents = SyntaxFactory.InterfaceDeclaration(name)
            .WithModifiers(
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(
                        SyntaxKind.PublicKeyword)))
            .WithKeyword(
                SyntaxFactory.Token(
                    SyntaxKind.InterfaceKeyword))
            .WithOpenBraceToken(
                SyntaxFactory.Token(
                    SyntaxKind.OpenBraceToken))
            .WithMembers(members())
            .WithCloseBraceToken(
                SyntaxFactory.Token(
                    SyntaxKind.CloseBraceToken));

        var implementing = implements.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(i)));
        if (implementing.Count() > 0)
            contents = contents.WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<BaseTypeSyntax>(implementing)));

        return SyntaxFactory.SingletonList<MemberDeclarationSyntax>(contents);
    }

    public static SyntaxList<MemberDeclarationSyntax> Namespace(string theNamespace, IEnumerable<string> namespaces, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        return Namespace(theNamespace, namespaces, null, members);
    }

    public static SyntaxList<MemberDeclarationSyntax> Namespace(string theNamespace, IEnumerable<string> namespaces, Dictionary<string, string> typesWithAliases, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        var contents = BaseNamespace(theNamespace)
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithMembers(members())
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        var usings = new List<UsingDirectiveSyntax>();

        if (namespaces != null)
        {
            usings.AddRange(namespaces.Select(s => Using(s, theNamespace)).Where(s => s != null));
        }
        if (typesWithAliases != null && typesWithAliases.Any())
        {
            usings.AddRange(typesWithAliases.Select(s => UsingTypeWithAlias(s.Key, s.Value)).Where(s => s != null));
        }

        if (usings.Any())
        {
            contents = contents.WithUsings(SyntaxFactory.List(usings));
        }

        return SyntaxFactory.SingletonList<MemberDeclarationSyntax>(contents);
    }

    public static MemberDeclarationSyntax Class(string name, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        return Class(name, new string[0], new[] { SyntaxKind.PublicKeyword }, members);
    }

    public static MemberDeclarationSyntax Class(string name, IEnumerable<SyntaxKind> accessModifiers, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        return Class(name, new string[0], accessModifiers ?? new[] { SyntaxKind.PublicKeyword }, members);
    }

    public static MemberDeclarationSyntax Class(string name, Func<SyntaxList<MemberDeclarationSyntax>> members, NameSyntax inherits)
    {
        return Class(name, new string[0], new[] { SyntaxKind.PublicKeyword }, members, inherits: inherits);
    }


    public static MemberDeclarationSyntax Class(string name, IEnumerable<string> implements, IEnumerable<SyntaxKind> accessModifiers, Func<SyntaxList<MemberDeclarationSyntax>> members, bool abstractClass = false, string inherits = null)
    {
        BaseTypeSyntax baseType = null;
        if (!string.IsNullOrEmpty(inherits))
            baseType = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(inherits));

        return Class(name, implements, accessModifiers, members, baseType, abstractClass);
    }

    public static MemberDeclarationSyntax Class(string name, IEnumerable<string> implements, IEnumerable<SyntaxKind> accessModifiers, Func<SyntaxList<MemberDeclarationSyntax>> members, NameSyntax inherits, bool abstractClass = false)
    {
        return Class(name, implements, accessModifiers, members, inherits != null ? SyntaxFactory.SimpleBaseType(inherits) : null, abstractClass);
    }

    public static MemberDeclarationSyntax Class(string name, IEnumerable<string> implements, IEnumerable<SyntaxKind> accessModifiers, Func<SyntaxList<MemberDeclarationSyntax>> members, BaseTypeSyntax inherits, bool abstractClass = false)
    {
        var contents = SyntaxFactory.ClassDeclaration(name)
            .WithModifiers(Tokens.List(accessModifiers))
            .WithKeyword(SyntaxFactory.Token(SyntaxKind.ClassKeyword))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithMembers(members())
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

        if (abstractClass)
            contents = contents.AddModifiers(SyntaxFactory.Token(SyntaxKind.AbstractKeyword));
        if (inherits != null)
            contents = contents.AddBaseListTypes(inherits);

        var implementing = implements.Select(i => SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName(i)));
        if (implementing.Count() > 0)
            contents = contents.AddBaseListTypes(implementing.ToArray());


        return contents;
    }

    public static SyntaxList<MemberDeclarationSyntax> Struct(string name, IEnumerable<SyntaxKind> accessModifiers, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        var implementing = SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName($"IEquatable<{name}>"));

        var contents = SyntaxFactory.StructDeclaration(name)
            .WithModifiers(SyntaxFactory.TokenList(accessModifiers.Select(SyntaxFactory.Token)))
            .WithKeyword(SyntaxFactory.Token(SyntaxKind.StructKeyword))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithMembers(members())
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
            .WithBaseList(SyntaxFactory.BaseList(SyntaxFactory.SeparatedList<BaseTypeSyntax>(new[] { implementing })));

        return SyntaxFactory.SingletonList<MemberDeclarationSyntax>(contents);
    }

    public static MemberDeclarationSyntax Class(string name, string inherits, IEnumerable<string> implements, bool abstractClass, Func<SyntaxList<MemberDeclarationSyntax>> members)
    {
        return Class(name, implements, new[] { SyntaxKind.InternalKeyword }, members, abstractClass, inherits);
    }

    private static NamespaceDeclarationSyntax BaseNamespace(string theNamespace)
    {
        if (theNamespace.EndsWith("."))
            theNamespace = theNamespace.Substring(0, theNamespace.Length - 1);

        return SyntaxFactory.NamespaceDeclaration(TurnStringIntoFullyQualifiedName(theNamespace));
    }

    private static UsingDirectiveSyntax UsingTypeWithAlias(string type, string fullyQualifiedType)
    {
        var fullyQualified = TurnStringIntoFullyQualifiedName(fullyQualifiedType);

        return SyntaxFactory.UsingDirective(fullyQualified).WithAlias(SyntaxFactory.NameEquals(type));
    }

    private static UsingDirectiveSyntax Using(string fullyQualifiedNamespace, string withinNamespace)
    {
        if (string.IsNullOrEmpty(fullyQualifiedNamespace) || fullyQualifiedNamespace.EndsWith("."))
            return null;

        return SyntaxFactory.UsingDirective(TurnStringIntoFullyQualifiedName(fullyQualifiedNamespace));
    }

    private static NameSyntax TurnStringIntoFullyQualifiedName(string theString)
    {
        var splitName = theString.Split('.');
        NameSyntax name = SyntaxFactory.IdentifierName(splitName.First());

        QualifiedNameSyntax fullyQualifiedName = null;
        foreach (var nextName in splitName.Skip(1))
        {
            fullyQualifiedName = SyntaxFactory.QualifiedName(fullyQualifiedName ?? name, SyntaxFactory.IdentifierName(nextName));
        }

        return fullyQualifiedName ?? name;
    }

    public static ConstructorDeclarationSyntax Constructor(
        string className,
        params ParameterSyntax[] parameters)
    {
        return Constructor(className, parameters, statements: null);
    }

    public static ConstructorDeclarationSyntax Constructor(
        string className,
        IEnumerable<ParameterSyntax> parameters,
        IEnumerable<StatementSyntax> statements = null)
    {
        return Constructor(className, Tokens.List(Tokens.PublicModifier), parameters, statements);
    }

    public static ConstructorDeclarationSyntax Constructor(
        string className,
        SyntaxTokenList modifiers,
        IEnumerable<ParameterSyntax> parameters,
        IEnumerable<StatementSyntax> statements = null)
    {
        var parameterList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters));
        return Constructor(className, modifiers, parameterList, statements);
    }

    private static ConstructorDeclarationSyntax Constructor(
        string className,
        ParameterListSyntax parameters = null,
        IEnumerable<StatementSyntax> statements = null)
    {
        return Constructor(className, Tokens.List(Tokens.PublicModifier), parameters, statements);
    }

    public static ConstructorDeclarationSyntax Constructor(
        string className,
        SyntaxTokenList modifiers,
        ParameterListSyntax parameters = null,
        IEnumerable<StatementSyntax> statements = null)
    {
        return SyntaxFactory.ConstructorDeclaration(className)
            .WithModifiers(modifiers)
            .WithParameterList(parameters)
            .WithBody(SyntaxFactory.Block(SyntaxFactory.List(statements))
                .WithOpenBraceToken(Tokens.OpenBrace)
                .WithCloseBraceToken(Tokens.CloseBrace));
    }

    public static ConstructorDeclarationSyntax WithBaseInitializer(
        this ConstructorDeclarationSyntax constructor, params string[] args)
    {
        return constructor.WithInitializer(
            SyntaxFactory.ConstructorInitializer(
                SyntaxKind.BaseConstructorInitializer,
                args.AsArgumentList()
            ));
    }

    public static SyntaxList<T> AsSingletonSyntaxList<T>(this T contents) where T : SyntaxNode
    {
        return SyntaxFactory.SingletonList<T>(contents);
    }

    public static SeparatedSyntaxList<T> AsSyntaxList<T>(this T lonesomeObject) where T : SyntaxNode
    {
        return SyntaxFactory.SingletonSeparatedList(lonesomeObject);
    }

    public static SeparatedSyntaxList<T> AsSyntaxList<T>(this IEnumerable<T> objects) where T : SyntaxNode
    {
        return SyntaxFactory.SeparatedList(objects);
    }
}