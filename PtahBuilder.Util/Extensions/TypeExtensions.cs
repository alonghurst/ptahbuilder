﻿using System.Text;

namespace PtahBuilder.Util.Extensions
{
    public static class TypeExtensions
    {
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
    }
}
