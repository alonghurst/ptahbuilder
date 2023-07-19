using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Properties
{
    public static AccessorDeclarationSyntax Getter(ExpressionSyntax expression)
    {
        return Getter(expression.Return().AsBlock());
    }

    public static AccessorDeclarationSyntax Getter(BlockSyntax body)
    {
        var blockWithBraces = body.WithOpenBraceToken(Tokens.OpenBrace)
            .WithCloseBraceToken(Tokens.CloseBrace);
        return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration, blockWithBraces)
            .WithKeyword(Tokens.Get);
    }

    public static AccessorDeclarationSyntax Getter()
    {
        return SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
            .WithKeyword(Tokens.Get)
            .WithSemicolonToken(Tokens.SemiColon);
    }
    public static AccessorDeclarationSyntax Setter()
    {
        return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
            .WithKeyword(Tokens.Set)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static AccessorDeclarationSyntax Setter(ExpressionSyntax body)
    {
        return Setter(Statements.Block(body.AsStatement()));
    }

    public static AccessorDeclarationSyntax Setter(BlockSyntax body)
    {
        var blockWithBraces = body.WithOpenBraceToken(Tokens.OpenBrace)
            .WithCloseBraceToken(Tokens.CloseBrace);
        return SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration, blockWithBraces)
            .WithKeyword(Tokens.Set);
    }

    public static AccessorListSyntax Accessors(AccessorDeclarationSyntax accessor)
    {
        return Accessors(new[] { accessor });
    }

    public static AccessorListSyntax Accessors(params AccessorDeclarationSyntax[] accessors)
    {
        return SyntaxFactory.AccessorList(SyntaxFactory.List(accessors))
            .WithOpenBraceToken(Tokens.OpenBrace)
            .WithCloseBraceToken(Tokens.CloseBrace);
    }

    public static PropertyDeclarationSyntax Property(string propertyName, Type propertyType)
    {
        return Property(propertyName, Types.Type(propertyType));
    }

    public static PropertyDeclarationSyntax Property(string propertyName, string propertyType)
    {
        return Property(propertyName, Types.Type(propertyType));
    }

    public static PropertyDeclarationSyntax Property(string propertyName, TypeSyntax propertyType)
    {
        return SyntaxFactory.PropertyDeclaration(propertyType, SyntaxFactory.Identifier(propertyName));
    }

    public static PropertyDeclarationSyntax SimpleProperty(string propertyName, string propertyType)
    {
        return Property(propertyName, propertyType)
            .WithModifiers(Tokens.PublicModifier)
            .WithAccessorList(Accessors(Getter(), Setter()));
    }

    public static PropertyDeclarationSyntax SimpleProperty(string propertyName, Type propertyType)
    {
        return Property(propertyName, propertyType)
            .WithModifiers(Tokens.PublicModifier)
            .WithAccessorList(Accessors(Getter(), Setter()));
    }

    public static PropertyDeclarationSyntax WithPublic(this PropertyDeclarationSyntax syntax)
    {
        return syntax.WithModifiers(Tokens.PublicModifier);
    }

    public static PropertyDeclarationSyntax WithStatic(this PropertyDeclarationSyntax syntax)
    {
        return syntax.WithModifiers(Tokens.Static);
    }

    public static PropertyDeclarationSyntax WithGetter(this PropertyDeclarationSyntax syntax)
    {
        return syntax.WithAccessorList(Getter());
    }

    public static PropertyDeclarationSyntax WithLiteralInitializer(this PropertyDeclarationSyntax syntax, string literalValue)
    {
        return syntax
            .WithInitializer(SyntaxFactory.EqualsValueClause(Literals.String(literalValue)))
            .WithSemicolonToken();
    }

    public static PropertyDeclarationSyntax WithLambda(this PropertyDeclarationSyntax property, ExpressionSyntax expression)
    {
        return property.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(expression))
            .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    public static PropertyDeclarationSyntax WithModifiers(this PropertyDeclarationSyntax property, params SyntaxToken[] modifiers)
    {
        return property.WithModifiers(SyntaxFactory.TokenList(modifiers));
    }

    public static PropertyDeclarationSyntax WithAccessorList(this PropertyDeclarationSyntax syntax, params AccessorDeclarationSyntax[] accessor)
    {
        var accessors = Accessors(accessor);
        return syntax.WithAccessorList(accessors);
    }

    public static PropertyDeclarationSyntax GetterSetter(string propertyName, string theType)
    {
        var type = Types.Type(theType);
        return GetterSetter(propertyName, type);
    }

    public static PropertyDeclarationSyntax GetterSetter(string propertyName, Type theType)
    {
        var type = Types.Type(theType);
        return GetterSetter(propertyName, type);
    }

    public static PropertyDeclarationSyntax GetterSetter(string propertyName, TypeSyntax theType)
    {
        var property = SyntaxFactory.PropertyDeclaration(
                theType,
                SyntaxFactory.Identifier(propertyName))
            .WithAccessorList(GetterSetterAccessor);

        return property;
    }
        
    public static PropertyDeclarationSyntax WithAttributes(this PropertyDeclarationSyntax property, params AttributeSyntax[] attributes)
    {
        var attributeSingletons = attributes.Select(a => SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(a))).ToArray();

        return property.WithAttributeLists(
            SyntaxFactory.List
            (
                attributeSingletons
            ));
    }

    private static AccessorListSyntax GetterSetterAccessor => SyntaxFactory.AccessorList(SyntaxFactory.List(new[]
        {
            GetterAccessorDeclaration,
            SetterAccessorDeclaration
        })).WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

    private static AccessorListSyntax GetterAccessor => SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(GetterAccessorDeclaration))
        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

    private static AccessorDeclarationSyntax GetterAccessorDeclaration => SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
        .WithKeyword(SyntaxFactory.Token(SyntaxKind.GetKeyword))
        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

    private static AccessorListSyntax SetterAccessor => SyntaxFactory.AccessorList(SyntaxFactory.SingletonList(SetterAccessorDeclaration))
        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken));

    private static AccessorDeclarationSyntax SetterAccessorDeclaration => SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
        .WithKeyword(SyntaxFactory.Token(SyntaxKind.SetKeyword))
        .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

    public static PropertyDeclarationSyntax WithSemicolonToken(this PropertyDeclarationSyntax property)
    {
        return property.WithSemicolonToken(Tokens.SemiColon);
    }
}