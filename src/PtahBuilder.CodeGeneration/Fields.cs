using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Fields
{
    public static FieldDeclarationSyntax PublicConstField(string name, Type type, ExpressionSyntax initialValue)
    {
        return Field(name, type, new[] { SyntaxKind.PublicKeyword, SyntaxKind.ConstKeyword }, initialValue);
    }

    public static FieldDeclarationSyntax Field(string name, Type type, SyntaxKind[] modifiers, ExpressionSyntax initialValue)
    {
        var typeDefinition = Types.Type(type);

        return SyntaxFactory.FieldDeclaration(
                SyntaxFactory.VariableDeclaration(typeDefinition)
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier(name))
                                .WithInitializer(
                                    SyntaxFactory.EqualsValueClause(initialValue)))))
            .WithModifiers(
                SyntaxFactory.TokenList(modifiers.AsList())
            );
    }
}