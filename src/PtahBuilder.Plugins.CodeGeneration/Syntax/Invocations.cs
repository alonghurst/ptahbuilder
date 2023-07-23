using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.Plugins.CodeGeneration.Syntax;

public static class Invocations
{
    public static InvocationExpressionSyntax InvokeWithNamedArguments(string target, string methodName, params string[] parameterNames)
    {
        var arguments = parameterNames.Select(pn => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(pn))).AsArgumentList();
        return Invoke(target, methodName, arguments);
    }

    public static InvocationExpressionSyntax Invoke(string target, string methodName, params ArgumentSyntax[] arguments)
    {
        return Invoke(target, methodName, arguments.AsArgumentList());
    }

    public static InvocationExpressionSyntax Invoke(string methodName)
    {
        return InvokeMethodOnSelf(methodName, Enumerable.Empty<ArgumentSyntax>());
    }

    public static IdentifierNameSyntax InvokeVariable(string variableName)
    {
        return SyntaxFactory.IdentifierName(variableName);
    }

    public static InvocationExpressionSyntax InvokeMethodOnSelf(string methodName, params ArgumentSyntax[] parameterNames)
    {
        return InvokeMethodOnSelf(methodName, parameterNames.AsEnumerable());
    }
    public static InvocationExpressionSyntax InvokeMethodOnSelf(string methodName, string[] typeArguments, params string[] parameterNames)
    {
        var argumentList = parameterNames
            .AsArgumentList()
            .WithOpenParenToken(Tokens.OpenParen)
            .WithCloseParenToken(Tokens.CloseParen);

        return SyntaxFactory.InvocationExpression(SyntaxFactory.GenericName(methodName).WithTypeArgumentList(Arguments.GenericTypeArguments(typeArguments)))
            .WithArgumentList(argumentList);
    }
    public static InvocationExpressionSyntax InvokeMethodOnSelf(string methodName, Type[] typeArguments, params string[] parameterNames)
    {
        var argumentList = parameterNames
            .AsArgumentList()
            .WithOpenParenToken(Tokens.OpenParen)
            .WithCloseParenToken(Tokens.CloseParen);

        return SyntaxFactory.InvocationExpression(SyntaxFactory.GenericName(methodName).WithTypeArgumentList(Arguments.GenericTypeArguments(typeArguments)))
            .WithArgumentList(argumentList);
    }

    public static InvocationExpressionSyntax InvokeMethodOnSelf(string methodName, IEnumerable<ArgumentSyntax> parameterNames)
    {
        var argumentList = parameterNames
            .AsArgumentList()
            .WithOpenParenToken(Tokens.OpenParen)
            .WithCloseParenToken(Tokens.CloseParen);

        return SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(methodName))
            .WithArgumentList(argumentList);
    }

    public static InvocationExpressionSyntax InvokeMethodOnSelf(string methodName, params string[] parameterNames)
    {
        var arguments = parameterNames.Select(pn => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(pn)));
        return InvokeMethodOnSelf(methodName, arguments);
    }

    public static InvocationExpressionSyntax Invoke(ExpressionSyntax previousExpression, string methodName, params ArgumentSyntax[] arguments)
    {
        var methodCall = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                previousExpression,
                SyntaxFactory.IdentifierName(methodName))
            .WithOperatorToken(Tokens.Dot);

        var invocationExpression = SyntaxFactory.InvocationExpression(methodCall)
            .WithArgumentList(arguments.AsEnumerable().AsArgumentList()
                .WithOpenParenToken(Tokens.OpenParen)
                .WithCloseParenToken(Tokens.CloseParen));

        return invocationExpression;
    }

    public static MemberAccessExpressionSyntax InvokeProperty(InvocationExpressionSyntax previousExpression, string propertyName)
    {
        var methodCall = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                previousExpression,
                SyntaxFactory.IdentifierName(propertyName))
            .WithOperatorToken(Tokens.Dot);

        return methodCall;
    }

    public static MemberAccessExpressionSyntax InvokeProperty(MemberAccessExpressionSyntax previousExpression, string propertyName)
    {
        var methodCall = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                previousExpression,
                SyntaxFactory.IdentifierName(propertyName))
            .WithOperatorToken(Tokens.Dot);

        return methodCall;
    }


    public static MemberAccessExpressionSyntax InvokeProperty(string variableName, string propertyName)
    {
        var methodCall = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(variableName),
                SyntaxFactory.IdentifierName(propertyName))
            .WithOperatorToken(Tokens.Dot);

        return methodCall;
    }

    public static InvocationExpressionSyntax Invoke(string target, string methodName, ArgumentSyntax argument)
    {
        return Invoke(target, methodName, new[] { argument }.AsArgumentList());
    }

    public static InvocationExpressionSyntax Invoke(string target, string methodName, ArgumentListSyntax arguments)
    {
        return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName(target),
                        SyntaxFactory.IdentifierName(methodName))
                    .WithOperatorToken(Tokens.Dot))
            .WithArgumentList(arguments.WithOpenParenToken(Tokens.OpenParen).WithCloseParenToken(Tokens.CloseParen));
    }


    public static InvocationExpressionSyntax InvokeMethodWithTypeArguments(string target, string methodName, params string[] genericTypes)
    {
        var generics = Arguments.GenericTypeArguments(genericTypes);

        return SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName(target),
                    SyntaxFactory.GenericName(methodName)
                        .WithTypeArgumentList(generics)
                )
                .WithOperatorToken(Tokens.Dot));
    }

    public static InvocationExpressionSyntax InvokeOnLocalMethod(string methodName, params string[] parameterNames)
    {
        var arguments = parameterNames.Select(pn => SyntaxFactory.Argument(SyntaxFactory.IdentifierName(pn))).AsArgumentList();
        return InvokeOnLocalMethod(methodName, arguments);
    }

    public static InvocationExpressionSyntax InvokeOnLocalMethod(string methodName, params ArgumentSyntax[] parameters)
    {
        var arguments = parameters.AsArgumentList();
        return InvokeOnLocalMethod(methodName, arguments);
    }

    public static InvocationExpressionSyntax InvokeOnLocalMethod(string methodName, IEnumerable<ArgumentSyntax> arguments)
    {
        var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(arguments));
        return InvokeOnLocalMethod(methodName, argumentList);
    }

    public static InvocationExpressionSyntax InvokeOnLocalMethod(string methodName, ArgumentListSyntax arguments)
    {
        return SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(methodName))
            .WithArgumentList(arguments.WithOpenParenToken(Tokens.OpenParen).WithCloseParenToken(Tokens.CloseParen));
    }

    public static ElementAccessExpressionSyntax AccessArrayElement(ExpressionSyntax previousExpression, int index)
    {
        return SyntaxFactory.ElementAccessExpression(previousExpression)
            .WithArgumentList(
                SyntaxFactory.BracketedArgumentList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Argument(Literals.Integer(index)))));
    }

    public static InvocationExpressionSyntax InvokeMethodOnExpression(this ExpressionSyntax expression, string methodName, params ArgumentSyntax[] arguments)
    {
        var argumentList = arguments.AsArgumentList();
        return expression.InvokeMethodOnExpression(methodName, argumentList);
    }

    public static InvocationExpressionSyntax InvokeMethodOnExpression(this ExpressionSyntax expression, string methodName, ArgumentListSyntax arguments)
    {
        var memberAccessExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            expression, SyntaxFactory.IdentifierName(methodName));

        return SyntaxFactory.InvocationExpression(memberAccessExpression).WithArgumentList(arguments);
    }

    public static InvocationExpressionSyntax InvokeMethodOnExpression(this ExpressionSyntax expression, string methodName, string[] genericTypes, params ArgumentSyntax[] arguments)
    {
        var argumentList = arguments.AsArgumentList();
        var generics = Arguments.GenericTypeArguments(genericTypes);

        return SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        expression,
                        SyntaxFactory.GenericName(methodName)
                            .WithTypeArgumentList(generics)
                    )
                    .WithOperatorToken(Tokens.Dot))
            .WithArgumentList(argumentList);
    }

    public static IdentifierNameSyntax FromIdentifierName(string name)
    {
        return SyntaxFactory.IdentifierName(name);
    }

    public static InvocationExpressionSyntax TaskFromResult(ExpressionSyntax previous)
    {
        return Invoke("Task", "FromResult", Arguments.Wrap(previous));
    }
}