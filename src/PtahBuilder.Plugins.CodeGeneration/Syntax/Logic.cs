using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Syntax;

public static class LogicHelper
{
    public static PrefixUnaryExpressionSyntax LogicalNot(ExpressionSyntax expression)
    {
        return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.LogicalNotExpression, expression);
    }

    public static BinaryExpressionSyntax BinaryLogic(string identifierA, string identifierB, SyntaxKind binaryKind)
    {
        return BinaryLogic(SyntaxFactory.IdentifierName(identifierA), SyntaxFactory.IdentifierName(identifierB), binaryKind);
    }

    public static BinaryExpressionSyntax BinaryLogic(ExpressionSyntax identifierA, ExpressionSyntax identifierB, SyntaxKind binaryKind)
    {
        return SyntaxFactory.BinaryExpression(binaryKind, identifierA, identifierB);

    }

    public static BinaryExpressionSyntax Concatenate(IEnumerable<BinaryExpressionSyntax> comparesEnumerable, bool isEquality)
    {
        var compares = comparesEnumerable.ToList();

        BinaryExpressionSyntax compare = compares[0];
        compares.RemoveAt(0);
        while (compares.Count > 0)
        {
            compare = Join(compare, compares[0], isEquality ? SyntaxKind.LogicalAndExpression : SyntaxKind.LogicalOrExpression);
            compares.RemoveAt(0);
        }

        return compare;
    }

    public static BinaryExpressionSyntax Concatenate(bool isEquality, params BinaryExpressionSyntax[] comparesEnumerable)
    {
        return Concatenate(comparesEnumerable, isEquality);
    }

    public static BinaryExpressionSyntax Join(ExpressionSyntax a, ExpressionSyntax b, SyntaxKind joinKind)
    {
        return SyntaxFactory.BinaryExpression(
            joinKind, a, b).WithOperatorToken(SyntaxFactory.Token(
            SyntaxKind.AmpersandAmpersandToken));
    }

    public static BinaryExpressionSyntax Comparison(string propertyName, string identifierA, string identifierB, bool isEquality)
    {
        ExpressionSyntax accessA = string.IsNullOrEmpty(identifierA) ?
            SyntaxFactory.IdentifierName(
                propertyName)
            :
            SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(identifierA),
                    SyntaxFactory.IdentifierName(
                        propertyName))
                .WithOperatorToken(
                    SyntaxFactory.Token(
                        SyntaxKind.DotToken));

        ExpressionSyntax accessB = string.IsNullOrEmpty(identifierB) ?
            SyntaxFactory.IdentifierName(
                propertyName)
            :
            SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(identifierB),
                    SyntaxFactory.IdentifierName(
                        propertyName))
                .WithOperatorToken(
                    SyntaxFactory.Token(
                        SyntaxKind.DotToken));

        return SyntaxFactory.BinaryExpression(
                SyntaxKind.EqualsExpression,
                accessA, accessB)

            .WithOperatorToken(
                SyntaxFactory.Token(
                    isEquality ? SyntaxKind.EqualsEqualsToken : SyntaxKind.ExclamationEqualsToken));
    }

    public static BinaryExpressionSyntax Equals(ExpressionSyntax a, ExpressionSyntax b)
    {
        return SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, a, b)
            .WithOperatorToken(SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken));
    }

    public static BinaryExpressionSyntax IsNull(ExpressionSyntax expression)
    {
        return SyntaxFactory.BinaryExpression(SyntaxKind.EqualsExpression, expression, Literals.Null);
    }

    public static BinaryExpressionSyntax IsNotNull(ExpressionSyntax expression)
    {
        return SyntaxFactory.BinaryExpression(SyntaxKind.NotEqualsExpression, expression, Literals.Null);
    }
}