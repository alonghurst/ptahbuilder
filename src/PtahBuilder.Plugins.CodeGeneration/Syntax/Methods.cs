using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Syntax;

public static class Methods
{
    public static MemberDeclarationSyntax WithSemicolonToken(this MethodDeclarationSyntax method)
    {
        return method.WithSemicolonToken(Tokens.SemiColon);
    }

    public static MethodDeclarationSyntax WithParameters(this MethodDeclarationSyntax method, ParameterSyntax parameter)
    {
        return method.WithParameters(new[] { parameter });
    }

    public static MethodDeclarationSyntax WithParameters(this MethodDeclarationSyntax method, IEnumerable<ParameterSyntax> args)
    {
        ParameterListSyntax parameters = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(args))
            .WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
            .WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        return method.WithParameterList(parameters);
    }

    public static MethodDeclarationSyntax WithBody(this MethodDeclarationSyntax method, params StatementSyntax[] syntax)
    {
        return method.WithBody(Statements.Block(syntax));
    }

    public static MethodDeclarationSyntax WithParameters(this MethodDeclarationSyntax method, IEnumerable<string> args = null)
    {
        ParameterListSyntax parameters = SyntaxFactory.ParameterList();
        if (args?.Any() ?? false)
        {
            parameters = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(args.Select(arg => SyntaxFactory.Parameter(SyntaxFactory.Identifier(arg))
                .WithType(Types.Type(typeof(int))))));
        }
        parameters.WithOpenParenToken(SyntaxFactory.Token(SyntaxKind.OpenParenToken))
            .WithCloseParenToken(SyntaxFactory.Token(SyntaxKind.CloseParenToken));

        return method.WithParameterList(parameters);
    }

    public static TypeSyntax WithGenericTypeArguments(this GenericNameSyntax theType, params string[] argumentTypes)
    {
        return theType.WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(argumentTypes.Select(SyntaxFactory.IdentifierName)))
            .WithLessThanToken(SyntaxFactory.Token(SyntaxKind.LessThanToken))
            .WithGreaterThanToken(SyntaxFactory.Token(SyntaxKind.GreaterThanToken)));
    }

    public static MethodDeclarationSyntax Header(string methodName, NameSyntax returnType)
    {
        return SyntaxFactory.MethodDeclaration(returnType, SyntaxFactory.Identifier(methodName));
    }

    public static MethodDeclarationSyntax Header(string methodName, TypeSyntax returnType)
    {
        return SyntaxFactory.MethodDeclaration(returnType, SyntaxFactory.Identifier(methodName));
    }

    public static MethodDeclarationSyntax Header(string methodName, Type returnType)
    {
        var type = Types.Type(returnType);
        return Header(methodName, type);
    }

    public static MethodDeclarationSyntax Header(string methodName, string returnType)
    {
        var type = SyntaxFactory.IdentifierName(returnType);
        return Header(methodName, type);
    }

    public static OperatorDeclarationSyntax OperatorHeader(SyntaxKind token, string returnType)
    {
        var type = SyntaxFactory.IdentifierName(returnType);
        return SyntaxFactory.OperatorDeclaration(type, SyntaxFactory.Token(token));
    }

    public static MethodDeclarationSyntax Header(string methodName, string returnType, string genericReturnType)
    {
        var genericType = Types.GenericType(returnType, genericReturnType);
        return Header(methodName, genericType);
    }

    public static ParameterSyntax Parameter(string parameterName, string parameterType)
    {
        var type = SyntaxFactory.IdentifierName(parameterType);
        return Parameter(parameterName, type);
    }

    public static ParameterSyntax Parameter(string parameterName, Type parameterType)
    {
        var type = Types.Type(parameterType);
        return Parameter(parameterName, type);
    }

    public static ParameterSyntax Parameter(string parameterName, TypeSyntax parameterType)
    {
        return SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameterName))
            .WithType(parameterType);
    }
}