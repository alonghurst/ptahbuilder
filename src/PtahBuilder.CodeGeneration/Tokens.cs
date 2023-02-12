using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace PtahBuilder.CodeGeneration;

public static class Tokens
{
    public static SyntaxToken False => SyntaxFactory.Token(SyntaxKind.FalseKeyword);

    public static SyntaxToken True => SyntaxFactory.Token(SyntaxKind.TrueKeyword);

    public static SyntaxToken SemiColon => SyntaxFactory.Token(SyntaxKind.SemicolonToken);

    public static SyntaxToken Equals => SyntaxFactory.Token(SyntaxKind.EqualsToken);

    public static SyntaxTokenList PrivateReadOnlyModifiers => SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));

    public static SyntaxToken ReadOnly => SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword);

    public static SyntaxTokenList PublicModifier => List(Public);

    public static SyntaxTokenList ProtectedModifier => List(Protected);

    public static SyntaxTokenList InternalModifier => List(Internal);

    public static SyntaxTokenList PrivateModifier => List(Private);

    public static SyntaxTokenList StaticModifier => List(Static);

    public static SyntaxTokenList AsyncModifier => List(Async);

    public static SyntaxTokenList ParamsModifier => List(Params);

    public static SyntaxToken Dot => SyntaxFactory.Token(SyntaxKind.DotToken);

    public static SyntaxToken Comma => SyntaxFactory.Token(SyntaxKind.CommaToken);


    public static SyntaxToken OpenParen => SyntaxFactory.Token(SyntaxKind.OpenParenToken);

    public static SyntaxToken CloseParen => SyntaxFactory.Token(SyntaxKind.CloseParenToken);

    public static SyntaxToken OpenBrace => SyntaxFactory.Token(SyntaxKind.OpenBraceToken);

    public static SyntaxToken CloseBrace => SyntaxFactory.Token(SyntaxKind.CloseBraceToken);

    public static SyntaxToken LessThan => SyntaxFactory.Token(SyntaxKind.LessThanToken);

    public static SyntaxToken GreaterThan => SyntaxFactory.Token(SyntaxKind.GreaterThanToken);

    public static SyntaxToken New => SyntaxFactory.Token(SyntaxKind.NewKeyword);

    public static SyntaxToken This => SyntaxFactory.Token(SyntaxKind.ThisKeyword);

    public static SyntaxToken Return => SyntaxFactory.Token(SyntaxKind.ReturnKeyword);

    public static SyntaxToken Abstract => SyntaxFactory.Token(SyntaxKind.AbstractKeyword);

    public static SyntaxToken Virtual => SyntaxFactory.Token(SyntaxKind.VirtualKeyword);

    public static SyntaxToken Public => SyntaxFactory.Token(SyntaxKind.PublicKeyword);

    public static SyntaxToken Protected => SyntaxFactory.Token(SyntaxKind.ProtectedKeyword);

    public static SyntaxToken Private => SyntaxFactory.Token(SyntaxKind.PrivateKeyword);

    public static SyntaxToken Internal => SyntaxFactory.Token(SyntaxKind.InternalKeyword);

    public static SyntaxToken Async => SyntaxFactory.Token(SyntaxKind.AsyncKeyword);
    public static SyntaxToken Params => SyntaxFactory.Token(SyntaxKind.ParamsKeyword);

    public static SyntaxToken Get => SyntaxFactory.Token(SyntaxKind.GetKeyword);

    public static SyntaxToken Set => SyntaxFactory.Token(SyntaxKind.SetKeyword);

    public static SyntaxToken Null => SyntaxFactory.Token(SyntaxKind.NullKeyword);

    public static SyntaxToken Question => SyntaxFactory.Token(SyntaxKind.QuestionToken);

    public static SyntaxToken LambdaArrow => SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken);

    public static SyntaxToken Yield => SyntaxFactory.Token(SyntaxKind.YieldKeyword);

    public static SyntaxToken Foreach => SyntaxFactory.Token(SyntaxKind.ForEachKeyword);

    public static SyntaxToken In => SyntaxFactory.Token(SyntaxKind.InKeyword);

    public static SyntaxToken Static => SyntaxFactory.Token(SyntaxKind.StaticKeyword);

    public static SyntaxToken Override => SyntaxFactory.Token(SyntaxKind.OverrideKeyword);

    public static SyntaxToken Partial => SyntaxFactory.Token(SyntaxKind.PartialKeyword);

    public static SyntaxTriviaList LineBreak => SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(Environment.NewLine));

    public static SyntaxTokenList List(IEnumerable<SyntaxKind> tokens)
    {
        return SyntaxFactory.TokenList(tokens.Select(SyntaxFactory.Token));
    }

    public static SyntaxTokenList AsList(this IEnumerable<SyntaxKind> tokens)
    {
        return List(tokens);
    }

    public static SyntaxTokenList List(IEnumerable<SyntaxToken> tokens)
    {
        return SyntaxFactory.TokenList(tokens);
    }

    public static SyntaxTokenList List(params SyntaxToken[] tokens)
    {
        return List(tokens.AsEnumerable());
    }

    public static SyntaxToken IfStatement(SyntaxKind comparison)
    {
        if (comparison == SyntaxKind.NotEqualsExpression)
        {
            return SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken);
        }

        if (comparison == SyntaxKind.EqualsExpression)
        {
            return SyntaxFactory.Token(SyntaxKind.EqualsEqualsToken);
        }

        throw new System.Exception($"Unknown if comparision {comparison}");
    }
}