using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Syntax;

public static class Collections
{
    public static ImplicitArrayCreationExpressionSyntax AsArraySyntax(this IEnumerable<ExpressionSyntax> expressions)
    {
        var withCommas = new List<SyntaxNodeOrToken>();
        var expressionsAr = expressions.ToArray();

        for (int i = 0; i < expressionsAr.Length; i++)
        {
            withCommas.Add(expressionsAr[i]);
            if (i < expressionsAr.Length - 1)
            {
                withCommas.Add(Tokens.Comma);
            }
        }

        return SyntaxFactory.ImplicitArrayCreationExpression(
            SyntaxFactory.InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                SyntaxFactory.SeparatedList<ExpressionSyntax>(withCommas)));
    }

    public static ObjectCreationExpressionSyntax InstantiateDictionary(TypeSyntax keyType, TypeSyntax valueType, IEnumerable<ExpressionSyntax> expressions)
    {
        var withCommas = new List<SyntaxNodeOrToken>();
        var expressionsAr = expressions.ToArray();

        for (int i = 0; i < expressionsAr.Length; i++)
        {
            withCommas.Add(expressionsAr[i]);
            if (i < expressionsAr.Length - 1)
            {
                withCommas.Add(Tokens.Comma);
            }
        }

        var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.CollectionInitializerExpression, SyntaxFactory.SeparatedList<ExpressionSyntax>(withCommas));

        return SyntaxFactory.ObjectCreationExpression(
                SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("Dictionary"))
                    .WithTypeArgumentList(
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList<TypeSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    keyType,
                                    Tokens.Comma,
                                    valueType
                                }))))
            .WithInitializer(initializer);
    }

    public static InitializerExpressionSyntax KeyValuePairInitializer(ExpressionSyntax key, ExpressionSyntax value)
    {
        return SyntaxFactory.InitializerExpression(
            SyntaxKind.ComplexElementInitializerExpression,
            SyntaxFactory.SeparatedList<ExpressionSyntax>(
                new SyntaxNodeOrToken[]
                {
                    key,
                    Tokens.Comma,
                    value
                }));
    }
}