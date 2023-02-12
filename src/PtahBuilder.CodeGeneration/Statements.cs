using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Statements
{
    public static UsingStatementSyntax Using(string variableName, ExpressionSyntax initializer, params StatementSyntax[] body)
    {
        var definition = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("var"))
            .WithVariables(SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variableName))
                    .WithInitializer(SyntaxFactory.EqualsValueClause(initializer))));

        return SyntaxFactory.UsingStatement(SyntaxFactory.Block(body)).WithDeclaration(definition);
    }

    public static UsingStatementSyntax Using(string variableName, ExpressionSyntax initializer, IEnumerable<StatementSyntax> body)
    {
        return Using(variableName, initializer, body.ToArray());
    }

    public static StatementSyntax If(StatementSyntax statement, params ExpressionSyntax[] clauses)
    {
        return If(new[] { statement }, clauses);
    }

    public static StatementSyntax If(IEnumerable<StatementSyntax> statements, params ExpressionSyntax[] clauses)
    {
        if (clauses.Length < 1)
            throw new InvalidOperationException("Provide at least 1 clause to an if statement");

        var condition = clauses[0];

        foreach (var other in clauses.Skip(1))
        {
            condition = LogicHelper.BinaryLogic(condition, other, SyntaxKind.LogicalOrExpression);
        }

        return SyntaxFactory.IfStatement(condition, Block(statements));
    }

    public static StatementSyntax If(ExpressionSyntax clause, params StatementSyntax[] statements)
    {
        return SyntaxFactory.IfStatement(clause, Block(statements));
    }

    public static BlockSyntax AsBlock(this StatementSyntax statement)
    {
        return Block(statement);
    }
        
    public static StatementSyntax Throw(string exceptionType)
    {
        return SyntaxFactory.ThrowStatement(Instantiations.NewUp(exceptionType));
    }

    public static CheckedStatementSyntax Unchecked(params StatementSyntax[] statements)
    {
        return SyntaxFactory.CheckedStatement(
            SyntaxKind.UncheckedStatement,
            Block(statements));
    }

    public static BlockSyntax Block(StatementSyntax statements)
    {
        return Block(new[] { statements });
    }

    public static BlockSyntax AsBlock(this IEnumerable<StatementSyntax> statements)
    {
        return Block(statements);
    }

    public static BlockSyntax Block(IEnumerable<StatementSyntax> statements)
    {
        return SyntaxFactory.Block(SyntaxFactory.List(statements))
            .WithOpenBraceToken(Tokens.OpenBrace)
            .WithCloseBraceToken(Tokens.CloseBrace);
    }
}