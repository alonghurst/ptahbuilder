using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Instantiations
{
    public static LocalDeclarationStatementSyntax DeclareAndInstantiate(string name, ExpressionSyntax initialValue, string type = "var")
    {
        return SyntaxFactory.LocalDeclarationStatement(SyntaxFactory.VariableDeclaration(Types.Type(type))
            .WithVariables(
                SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                    SyntaxFactory.VariableDeclarator(
                            SyntaxFactory.Identifier(name))
                        .WithInitializer(SyntaxFactory.EqualsValueClause(initialValue)))));
    }

    public static ObjectCreationExpressionSyntax NewUp(string type, params AssignmentExpressionSyntax[] assignments)
    {
        var obj = SyntaxFactory.ObjectCreationExpression(Types.Type(type));

        if (assignments.Any())
        {
            var syntax = new List<SyntaxNodeOrToken>();

            foreach (var assignment in assignments)
            {
                syntax.Add(assignment);
                if (assignment != assignments.Last())
                {
                    syntax.Add(Tokens.Comma.WithTrailingTrivia(Tokens.LineBreak));
                }
            }

            obj = obj.WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                SyntaxFactory.SeparatedList<ExpressionSyntax>(syntax)));
        }

        return obj;
    }
    public static ObjectCreationExpressionSyntax NewUp(string type, ArgumentListSyntax arguments, params AssignmentExpressionSyntax[] assignments)
    {
        var obj = SyntaxFactory.ObjectCreationExpression(Types.Type(type));

        if (assignments.Any())
        {
            var syntax = new List<SyntaxNodeOrToken>();

            foreach (var assignment in assignments)
            {
                syntax.Add(assignment);
                if (assignment != assignments.Last())
                {
                    syntax.Add(Tokens.Comma.WithTrailingTrivia(Tokens.LineBreak));
                }
            }

            obj = obj
                .WithArgumentList(arguments)
                .WithInitializer(SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression,
                    SyntaxFactory.SeparatedList<ExpressionSyntax>(syntax)));
        }

        return obj;
    }


    public static ObjectCreationExpressionSyntax NewUp(string className, params ArgumentSyntax[] arguments)
    {
        var argumentList = arguments.AsEnumerable()
            .AsArgumentList();
        return NewUp(className, argumentList);
    }

    public static ObjectCreationExpressionSyntax NewUp(string className)
    {
        return NewUp(className, SyntaxFactory.ArgumentList());
    }

    public static ObjectCreationExpressionSyntax NewUp(string className, ArgumentListSyntax argumentList)
    {
        return NewUpBase(className)
            .WithArgumentList(argumentList
                .WithOpenParenToken(Tokens.OpenParen)
                .WithCloseParenToken(Tokens.CloseParen));
    }

    public static ObjectCreationExpressionSyntax NewUp(NameSyntax className, params ArgumentSyntax[] arguments)
    {
        var argumentList = arguments.AsEnumerable()
            .AsArgumentList();
        return SyntaxFactory.ObjectCreationExpression(className)
            .WithArgumentList(argumentList
                .WithOpenParenToken(Tokens.OpenParen)
                .WithCloseParenToken(Tokens.CloseParen));
    }

    public static ObjectCreationExpressionSyntax NewUp(NameSyntax className, ArgumentListSyntax argumentList)
    {
        return SyntaxFactory.ObjectCreationExpression(className)
            .WithArgumentList(argumentList
                .WithOpenParenToken(Tokens.OpenParen)
                .WithCloseParenToken(Tokens.CloseParen));
    }

    public static ObjectCreationExpressionSyntax NewUpBase(string className)
    {
        return SyntaxFactory.ObjectCreationExpression(SyntaxFactory.IdentifierName(className))
            .WithNewKeyword(Tokens.New);
    }

    public static InitializerExpressionSyntax ObjectInitializer(params ExpressionSyntax[] expressions)
    {
        return ObjectInitializer(expressions.AsEnumerable());
    }

    public static InitializerExpressionSyntax ObjectInitializer(IEnumerable<ExpressionSyntax> expressions)
    {
        return SyntaxFactory.InitializerExpression(
                SyntaxKind.ObjectInitializerExpression,
                SyntaxFactory.SeparatedList(expressions))
            .WithOpenBraceToken(Tokens.OpenBrace)
            .WithCloseBraceToken(Tokens.CloseBrace);
    }

}