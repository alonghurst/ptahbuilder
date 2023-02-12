using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Literals
{
    public static LiteralExpressionSyntax Integer(int i)
    {
        return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(i));
    }

    public static ExpressionSyntax String(string text)
    {
        return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(text));
    }

    public static ExpressionSyntax AsLiteral(this string text)
    {
        return SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(text));
    }

    public static ExpressionSyntax StringEmpty()
    {
        return Invocations.InvokeProperty("string", "Empty");
    }

    public static ExpressionSyntax True => SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
        .WithToken(Tokens.True);

    public static ExpressionSyntax False => SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)
        .WithToken(Tokens.False);

    public static ExpressionSyntax Boolean(bool value)
    {
        return value ? True : False;
    }

    public static LiteralExpressionSyntax Float(float value)
    {
        return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value));
    }

    public static ExpressionSyntax Null => SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
        .WithToken(Tokens.Null);
}