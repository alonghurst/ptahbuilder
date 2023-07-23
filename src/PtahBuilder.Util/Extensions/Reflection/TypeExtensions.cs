using System.Text;

namespace PtahBuilder.Util.Extensions;

public static class TypeExtensions
{
    public static bool IsDictionaryType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
    }

    public static Type GetDictionaryKeyValuePairType(this Type dictionaryType)
    {
        Type keyType = dictionaryType.GetGenericArguments()[0];
        Type valueType = dictionaryType.GetGenericArguments()[1];

        return typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
    }

    public static Type? GetBaseGenericType(this Type type)
    {
        if (type.BaseType == null || type.BaseType == typeof(object))
        {
            return null;
        }

        if (type.BaseType.GenericTypeArguments.Any())
        {
            return type.BaseType.GenericTypeArguments[0];
        }

        return GetBaseGenericType(type.BaseType);
    }

    public static string GetTypeName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (!type.IsGenericType)
            return type.GetNestedTypeName();

        var stringBuilder = new StringBuilder();
        BuildClassNameRecursive(type, stringBuilder);
        return stringBuilder.ToString();
    }

    private static void BuildClassNameRecursive(Type type, StringBuilder classNameBuilder, int genericParameterIndex = 0)
    {
        if (type.IsGenericParameter)
            classNameBuilder.AppendFormat("T{0}", genericParameterIndex + 1);
        else if (type.IsGenericType)
        {
            classNameBuilder.Append(GetNestedTypeName(type) + "<");
            int subIndex = 0;
            foreach (Type genericTypeArgument in type.GetGenericArguments())
            {
                if (subIndex > 0)
                    classNameBuilder.Append(", ");

                BuildClassNameRecursive(genericTypeArgument, classNameBuilder, subIndex++);
            }
            classNameBuilder.Append(">");
        }
        else
            classNameBuilder.Append(type.GetNestedTypeName());
    }

    public static string GetNestedTypeName(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        if (!type.IsNested)
            return GetTypeNameInner(type);

        var nestedName = new StringBuilder();

        while (type != null)
        {
            if (nestedName.Length > 0)
                nestedName.Insert(0, '.');

            nestedName.Insert(0, GetTypeNameInner(type));

            if (type.DeclaringType == null)
            {
                break;
            }

            type = type.DeclaringType;
        }
        return nestedName.ToString();
    }

    private static string GetTypeNameInner(Type type)
    {
        return type.Name.Split('`')[0];
    }

    public static bool IsEnumerable(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
               || type.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
    }

    public static Type GetTypeOrElementType(this Type type)
    {
        var interfaceType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ? type : type.GetInterfaces().FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        if (interfaceType != null && type != typeof(string))
        {
            return interfaceType.GetGenericArguments()[0];
        }

        return type;
    }

    public static bool ImplementInterface(this Type type, Type @interface)
    {
        return type.GetInterfaces().Any(i => i == @interface || i.IsGenericType && i.GetGenericTypeDefinition() == @interface);
    }

    public static Type GetTypeOrElementType(this object o)
    {
        var type = o.GetType();
        return type.GetTypeOrElementType();
    }
}