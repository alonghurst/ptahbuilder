using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Expressions
{
    public static ParenthesizedExpressionSyntax WrapInBrackets(ExpressionSyntax expression)
    {
        return SyntaxFactory.ParenthesizedExpression(expression);
    }

    public static BinaryExpressionSyntax CoalesceToEmptyString(ExpressionSyntax left)
    {
        return SyntaxFactory.BinaryExpression(SyntaxKind.CoalesceExpression, left, Literals.StringEmpty());
    }

    public static BinaryExpressionSyntax Coalesce(ExpressionSyntax left, ExpressionSyntax right)
    {
        return SyntaxFactory.BinaryExpression(SyntaxKind.CoalesceExpression, left, right);
    }

    public static BlockSyntax Block(StatementSyntax singleStatement)
    {
        return BlockFromStatements(new[] { singleStatement });
    }

    public static BlockSyntax Block(ExpressionStatementSyntax singleExpression)
    {
        return Block(new[] { singleExpression });
    }

    public static BlockSyntax Block(params ExpressionStatementSyntax[] expressions)
    {
        return Block(expressions.AsEnumerable());
    }

    public static BlockSyntax Block(params StatementSyntax[] expressions)
    {
        return Block(expressions.AsEnumerable());
    }

    public static BlockSyntax Block(IEnumerable<ExpressionStatementSyntax> statements)
    {
        var statementList = SyntaxFactory.List<StatementSyntax>(statements);
        return Block(statementList);
    }

    public static BlockSyntax BlockFromStatements(IEnumerable<StatementSyntax> statements)
    {
        var statementList = SyntaxFactory.List(statements);
        return Block(statementList);
    }

    public static BlockSyntax BlockFromStatements(params StatementSyntax[] statements)
    {
        return Block(statements.AsEnumerable());
    }

    private static BlockSyntax Block(IEnumerable<StatementSyntax> statements)
    {
        return SyntaxFactory.Block(statements);
    }

    public static BlockSyntax ReturnBlock(ExpressionSyntax expression)
    {
        var returnExpression = Return(expression);
        var block = Block(returnExpression);
        return block;
    }

    public static ReturnStatementSyntax Return()
    {
        return SyntaxFactory.ReturnStatement()
            .WithReturnKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ReturnStatementSyntax ReturnTrue()
    {
        return SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression))
            .WithReturnKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ReturnStatementSyntax ReturnFalse()
    {
        return SyntaxFactory.ReturnStatement(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
            .WithReturnKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ReturnStatementSyntax ReturnTaskFromResult(ExpressionSyntax expression)
    {
        return SyntaxFactory.ReturnStatement(Invocations.TaskFromResult(expression))
            .WithReturnKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ReturnStatementSyntax Return(this ExpressionSyntax expression)
    {
        return SyntaxFactory.ReturnStatement(expression)
            .WithReturnKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ExpressionStatementSyntax AssignVariable(string variableName, ExpressionSyntax value)
    {
        return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(variableName),
                    value)
                .WithOperatorToken(Tokens.Equals))
            .WithSemicolonToken(Tokens.SemiColon);

    }

    public static ExpressionStatementSyntax AssignStatement(ExpressionSyntax target, ExpressionSyntax value)
    {
        return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    target,
                    value)
                .WithOperatorToken(Tokens.Equals))
            .WithSemicolonToken(Tokens.SemiColon);

    }

    public static AssignmentExpressionSyntax AssignExpression(string variableName, ExpressionSyntax value)
    {
        return SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(variableName),
                value)
            .WithOperatorToken(Tokens.Equals);

    }


    public static AssignmentExpressionSyntax AssignExpression(string left, string right)
    {
        return SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(left),
                SyntaxFactory.IdentifierName(right))
            .WithOperatorToken(Tokens.Equals);
    }


    public static IfStatementSyntax If(SyntaxKind comparison, ExpressionSyntax left, ExpressionSyntax right, BlockSyntax todo)
    {
        var ifStatement = SyntaxFactory.BinaryExpression(comparison, left, right)
            .WithOperatorToken(Tokens.IfStatement(comparison));
        return SyntaxFactory.IfStatement(ifStatement, todo);
    }

    public static IfStatementSyntax If(SyntaxKind comparison, ExpressionSyntax left, ExpressionSyntax right, BlockSyntax truthBlock, BlockSyntax falseBlock)
    {
        return If(comparison, left, right, truthBlock).WithElse(SyntaxFactory.ElseClause(falseBlock));
    }

    public static AwaitExpressionSyntax AsAwait(this ExpressionSyntax expression, bool addConfigureAwait = true)
    {
        if (addConfigureAwait)
            expression = Invocations.Invoke(expression, "ConfigureAwait", Arguments.False);

        return SyntaxFactory.AwaitExpression(expression)
            .WithAwaitKeyword(SyntaxFactory.Token(SyntaxKind.AwaitKeyword));
    }

    public static ThrowStatementSyntax Throw(string exception, params string[] parameters)
    {
        var arguments = parameters.Select(pn => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(pn))).AsArgumentList();

        return SyntaxFactory.ThrowStatement(
            SyntaxFactory.ObjectCreationExpression(
                    SyntaxFactory.IdentifierName(
                        exception))
                .WithNewKeyword(
                    SyntaxFactory.Token(
                        SyntaxKind.NewKeyword))
                .WithArgumentList(
                    arguments
                        .WithOpenParenToken(
                            SyntaxFactory.Token(
                                SyntaxKind.OpenParenToken))
                        .WithCloseParenToken(
                            SyntaxFactory.Token(
                                SyntaxKind.CloseParenToken))));
    }

    public static StatementSyntax As(this InvocationExpressionSyntax invocation)
    {
        return SyntaxFactory.ExpressionStatement(invocation).WithSemicolonToken(Tokens.SemiColon);
    }

    public static StatementSyntax Return(string variableName)
    {
        return SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(variableName));
    }


    public static IfStatementSyntax If(MemberAccessExpressionSyntax property, BlockSyntax theBlock)
    {
        return SyntaxFactory.IfStatement(property, theBlock);
    }

    public static IfStatementSyntax If(string comparison, BlockSyntax block)
    {
        return SyntaxFactory.IfStatement(SyntaxFactory.IdentifierName(comparison), block);
    }


    public static YieldStatementSyntax YieldReturn(string variableName)
    {
        return SyntaxFactory.YieldStatement(
                SyntaxKind.YieldReturnStatement,
                SyntaxFactory.IdentifierName(variableName))
            .WithYieldKeyword(Tokens.Yield)
            .WithReturnOrBreakKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static YieldStatementSyntax YieldReturn(ExpressionSyntax expression)
    {
        return SyntaxFactory.YieldStatement(SyntaxKind.YieldReturnStatement, expression)
            .WithYieldKeyword(Tokens.Yield)
            .WithReturnOrBreakKeyword(Tokens.Return)
            .WithSemicolonToken(Tokens.SemiColon);
    }

    public static ForEachStatementSyntax Foreach(string singleDataObject, string multipleDataObjects, BlockSyntax foreachBlock)
    {
        return SyntaxFactory.ForEachStatement(
                SyntaxFactory.IdentifierName(@"var"),
                SyntaxFactory.Identifier(singleDataObject),
                SyntaxFactory.IdentifierName(multipleDataObjects),
                foreachBlock
                    .WithOpenBraceToken(Tokens.OpenBrace)
                    .WithCloseBraceToken(Tokens.CloseBrace))
            .WithForEachKeyword(Tokens.Foreach)
            .WithOpenParenToken(Tokens.OpenParen)
            .WithInKeyword(Tokens.In)
            .WithCloseParenToken(Tokens.CloseParen);
    }


    public static CastExpressionSyntax CastIdentifierAsType(string identifier, string typeName)
    {
        return SyntaxFactory.CastExpression(SyntaxFactory.IdentifierName(typeName), SyntaxFactory.IdentifierName(identifier));
    }

    public static CastExpressionSyntax CastExpressionAsType(ExpressionSyntax expression, string typeName)
    {
        return SyntaxFactory.CastExpression(SyntaxFactory.IdentifierName(typeName), expression);
    }

    public static StatementSyntax AsStatement(this AssignmentExpressionSyntax expression)
    {
        return SyntaxFactory.ExpressionStatement(expression);
    }

    public static BlockSyntax AsBlock(this ForEachStatementSyntax foreachStatement)
    {
        return Block(foreachStatement);
    }

    public static StatementSyntax AsStatement(this ExpressionSyntax syntax)
    {
        return SyntaxFactory.ExpressionStatement(syntax);
    }
    public static StatementSyntax AsExpression(this InvocationExpressionSyntax invocation)
    {
        return SyntaxFactory.ExpressionStatement(invocation).WithSemicolonToken(Tokens.SemiColon);
    }

    public static TypeOfExpressionSyntax TypeOf(string type)
    {
        return SyntaxFactory.TypeOfExpression(Types.Type(type));
    }

    public static TypeOfExpressionSyntax TypeOf(Type type)
    {
        return SyntaxFactory.TypeOfExpression(Types.Type(type));
    }

    public static ConditionalExpressionSyntax ConditionalExpression(ExpressionSyntax condition, ExpressionSyntax ifTrueExpression,ExpressionSyntax ifFalseExpression)
    {
        return ConditionalExpression(condition, ifTrueExpression, ifFalseExpression);
    }

    public static ExpressionSyntax ToArray(this ExpressionSyntax previous)
    {
        return Invocations.Invoke(previous, "ToArray");
    }
}