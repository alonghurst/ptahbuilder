using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Syntax;

public static class Variables
{
    public static VariableDeclarationSyntax DeclareVariableType(string variableType = "var")
    {
        return SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName(variableType));
    }

    public static VariableDeclaratorSyntax DeclareVariable(string variableName)
    {
        return SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier(variableName));
    }

    public static ExpressionSyntax VariableNamed(string variableName)
    {
        return SyntaxFactory.IdentifierName(variableName);
    }

    public static StatementSyntax AssignVariable(string variableName, ExpressionSyntax assignment)
    {
        return AssignVariable(VariableNamed(variableName), assignment);
    }

    public static StatementSyntax AssignVariable(ExpressionSyntax identifier, ExpressionSyntax assignment)
    {
        return SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, identifier, assignment));
    }

    public static AssignmentExpressionSyntax AssignmentSyntax(string name, ExpressionSyntax expression)
    {
        return SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName(name), expression);
    }

    public static LocalDeclarationStatementSyntax DeclareVariableWithInitializer(string variableName, ExpressionSyntax initializer, string type = "var")
    {
        var variable = DeclareVariable(variableName);
        var initializerExpression = SyntaxFactory.EqualsValueClause(initializer).WithEqualsToken(Tokens.Equals);

        var variables = variable.WithInitializer(initializerExpression).AsSyntaxList();

        var variableDeclaration = DeclareVariableType(type).WithVariables(variables);

        return SyntaxFactory.LocalDeclarationStatement(variableDeclaration);
    }

}