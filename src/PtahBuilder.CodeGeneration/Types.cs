using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PtahBuilder.CodeGeneration;

public static class Types
{
    public static TypeSyntax Type(string theType)
    {
        if (theType == "string")
        {
            return Type(typeof(string));
        }

        if (theType == "int")
        {
            return Type(typeof(int));
        }

        if (theType == "bool")
        {
            return Type(typeof(bool));
        }

        if (theType == "void")
        {
            return Type(typeof(void));
        }

        return SyntaxFactory.IdentifierName(theType);
    }

    public static TypeSyntax Void => Type(typeof(void));

    public static TypeSyntax Type(Type theType)
    {
        SyntaxKind predefinedType = SyntaxKind.None;
        if (theType == typeof(string))
        {
            predefinedType = SyntaxKind.StringKeyword;
        }
        else if (theType == typeof(int))
        {
            predefinedType = SyntaxKind.IntKeyword;
        }
        else if (theType == typeof(long))
        {
            predefinedType = SyntaxKind.LongKeyword;
        }
        else if (theType == typeof(bool))
        {
            predefinedType = SyntaxKind.BoolKeyword;
        }
        else if (theType == typeof(decimal))
        {
            predefinedType = SyntaxKind.DecimalKeyword;
        }
        else if (theType == typeof(double))
        {
            predefinedType = SyntaxKind.DoubleKeyword;
        }
        else if (theType == typeof(void))
        {
            predefinedType = SyntaxKind.VoidKeyword;
        }
        else if (theType == typeof(int?))
        {
            return NullableType(SyntaxKind.IntKeyword);
        }
        else if (theType == typeof(long?))
        {
            return NullableType(SyntaxKind.LongKeyword);
        }
        else if (theType == typeof(bool?))
        {
            return NullableType(SyntaxKind.BoolKeyword);
        }
        else if (theType == typeof(double?))
        {
            return NullableType(SyntaxKind.DoubleKeyword);
        }
        else if (theType == typeof(float?))
        {
            return NullableType(SyntaxKind.FloatKeyword);
        }
        else if (theType == typeof(decimal?))
        {
            return NullableType(SyntaxKind.DecimalKeyword);
        }
        else if (theType == typeof(DateTime?))
        {
            return SyntaxFactory.NullableType(SyntaxFactory.IdentifierName("DateTime"), Tokens.Question);

        }
        else if (theType == typeof(Guid?))
        {
            return SyntaxFactory.NullableType(SyntaxFactory.IdentifierName("Guid"), Tokens.Question);
        }

        if (predefinedType == SyntaxKind.None)
        {
            return SyntaxFactory.IdentifierName(theType.Name);
        }
        else
        {
            return SyntaxFactory.PredefinedType(SyntaxFactory.Token(predefinedType));
        }
    }

    public static ArrayTypeSyntax ArrayType(string baseTypeName)
    {
        var baseType = Type(baseTypeName);

        return ArrayType(baseType);
    }

    public static ArrayTypeSyntax ArrayType(TypeSyntax baseType)
    {
        return SyntaxFactory.ArrayType(baseType,
            SyntaxFactory.SingletonList(SyntaxFactory.ArrayRankSpecifier(SyntaxFactory.SingletonSeparatedList<ExpressionSyntax>(SyntaxFactory.OmittedArraySizeExpression()))));
    }

    public static TypeSyntax NullableTypeWithQuestion(string typeName)
    {
        return SyntaxFactory.NullableType(SyntaxFactory.IdentifierName(typeName), Tokens.Question);
    }

    public static TypeSyntax NullableType(Type theType)
    {
        if (theType == typeof(DateTime))
            return NullableTypeWithQuestion("DateTime");

        var typeSyntax = Type(theType);

        if (typeSyntax is IdentifierNameSyntax)
            throw new InvalidOperationException($"Type {theType} is of reference type and can't be nullable");

        if (typeSyntax is NullableTypeSyntax)
            return typeSyntax;

        var predefinedTypeSyntax = typeSyntax as PredefinedTypeSyntax;
        if (predefinedTypeSyntax != null)
            return SyntaxFactory.NullableType(predefinedTypeSyntax);

        throw new InvalidOperationException($"Type {theType} did not resolve to any syntax that can be made nullable");
    }

    public static TypeSyntax NullableType(SyntaxKind kind)
    {
        return SyntaxFactory.NullableType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(kind)));
    }

    public static GenericNameSyntax IEnumerableOfType(string type)
    {
        return GenericType("IEnumerable", type);
    }

    public static GenericNameSyntax GenericType(string genericType, params string[] subTypes)
    {
        return GenericType(SyntaxFactory.GenericName(genericType), subTypes);
    }
    public static GenericNameSyntax GenericType(string genericType, params Type[] subTypes)
    {
        return GenericType(SyntaxFactory.GenericName(genericType), subTypes.Select(Types.Type).ToArray());
    }

    public static GenericNameSyntax GenericType(GenericNameSyntax genericType, params string[] subTypes)
    {
        var generics = Arguments.GenericTypeArguments(subTypes);

        return genericType.WithTypeArgumentList(generics);
    }

    public static GenericNameSyntax GenericType(string genericType, params TypeSyntax[] subTypes)
    {
        return GenericType(SyntaxFactory.GenericName(genericType), subTypes);
    }

    public static GenericNameSyntax GenericType(GenericNameSyntax genericType, params TypeSyntax[] subTypes)
    {
        return genericType.WithTypeArgumentList(SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(subTypes)));
    }
}