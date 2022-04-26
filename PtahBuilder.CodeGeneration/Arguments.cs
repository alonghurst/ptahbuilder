using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Arguments
{
    public static ArgumentSyntax False => SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)
        .WithToken(Tokens.False));

    public static ArgumentSyntax True => SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
        .WithToken(Tokens.True));

    public static ArgumentSyntax This => SyntaxFactory.Argument(SyntaxFactory.ThisExpression().WithToken(Tokens.This));

    public static ArgumentSyntax Null => SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
        .WithToken(Tokens.Null));

    public static ArgumentSyntax String(string literal)
    {
        return SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(literal)));
    }

    public static ArgumentListSyntax AsArgumentList(this ArgumentSyntax argument)
    {
        return SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(argument));
    }

    public static ArgumentListSyntax AsArgumentList(this string[] arguments)
    {
        var args = arguments.Select(VariableArgumentNamed);

        return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(args));
    }

    public static ArgumentListSyntax AsArgumentList(this IEnumerable<ArgumentSyntax> arguments)
    {
        return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));
    }

    public static ArgumentSyntax VariableArgumentNamed(string variableName)
    {
        return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(variableName));
    }

    public static ArgumentListSyntax AsArgumentList(this IEnumerable<ExpressionSyntax> expressions)
    {
        return expressions.Select(Wrap).AsArgumentList();
    }
        
    public static ArgumentSyntax AsArgument(this ExpressionSyntax expression)
    {
        return SyntaxFactory.Argument(expression);
    }

    public static ArgumentSyntax Wrap(ExpressionSyntax lambda)
    {
        return SyntaxFactory.Argument(lambda);
    }

    public static TypeArgumentListSyntax GenericTypeArguments(string[] genericTypes)
    {
        var generics = new List<SyntaxNodeOrToken>();

        foreach (var g in genericTypes)
        {
            generics.Add(SyntaxFactory.IdentifierName(g));
            if (g != genericTypes.Last())
            {
                generics.Add(Tokens.Comma);
            }
        }

        return SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(generics));
    }

    public static ArgumentSyntax Integer(int v)
    {
        return SyntaxFactory.Argument(Literals.Integer(v));
    }

    public static TypeArgumentListSyntax GenericTypeArguments(Type[] genericTypes)
    {
        var generics = new List<SyntaxNodeOrToken>();

        foreach (var g in genericTypes)
        {
            generics.Add(Types.Type(g));
            if (g != genericTypes.Last())
            {
                generics.Add(Tokens.Comma);
            }
        }

        return SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList<TypeSyntax>(generics));
    }
}