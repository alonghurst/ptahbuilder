﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;

namespace PtahBuilder.BuildSystem.Helpers
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, object> BlankInstances = new Dictionary<Type, object>();

        public static IEnumerable<KeyValuePair<PropertyInfo, object>> GetNonDefaultPropertyAndTheNewValue(object instance)
        {
            var type = instance.GetType();
            if (!BlankInstances.ContainsKey(type))
            {
                var blank = Activator.CreateInstance(type);

                if (blank == null)
                {
                    throw new InvalidOperationException($"Unable to instantiate a {type.Name}");
                }

                BlankInstances.Add(type, blank);
            }

            var blankInstance = BlankInstances[type];

            foreach (var property in type.GetProperties().Where(p => p.CanWrite))
            {
                var a = property.GetValue(instance);
                var b = property.GetValue(blankInstance);

                if (a == null && b == null)
                {
                    continue;
                }

                if (a != null && b == null)
                {
                    yield return new KeyValuePair<PropertyInfo, object>(property, a);
                    continue;
                }

                var propertyType = property.PropertyType;
                if (propertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    dynamic aEnumerable = a ?? Array.CreateInstance(propertyType, 0);
                    // ReSharper disable once ConstantNullCoalescingCondition
                    dynamic bEnumerable = b ?? Array.CreateInstance(propertyType, 0);

                    var equal = true;

                    foreach (var ae in aEnumerable)
                    {
                        equal = false;
                        foreach (var be in bEnumerable)
                        {
                            if (ae == be)
                            {
                                equal = true;
                                break;
                            }
                        }

                        if (!equal)
                        {
                            break;
                        }
                    }

                    if (equal)
                    {
                        continue;
                    }
                }

                if (a == null || !a.Equals(b))
                {
                    yield return new KeyValuePair<PropertyInfo, object>(property, a);
                }
            }
        }

        public static Type FindBaseDataGeneratorType(Type forType)
        {
            var generatorBaseType = typeof(DataGenerator<>).MakeGenericType(forType);

            var concreteType = GetLoadedTypesThatAreAssignableTo(generatorBaseType, possibleGenericArgument: forType)
                .OrderBy(t => t.IsGenericType ? 1 : 0)
                .FirstOrDefault();

            return concreteType ?? generatorBaseType;
        }

        public static Type FindBaseDataMetadataResolverType(Type forType)
        {
            var generatorBaseType = typeof(BaseDataMetadataResolver<>).MakeGenericType(forType);

            var concreteType = GetLoadedTypesThatAreAssignableTo(generatorBaseType, possibleGenericArgument: forType)
                .OrderBy(t => t.IsGenericType ? 1 : 0)
                .FirstOrDefault();

            return concreteType ?? generatorBaseType;
        }

        public static Type[] FindSecondaryGeneratorTypes(Type forType)
        {
            var generatorBaseType = typeof(SecondaryGenerator<>).MakeGenericType(forType);

            return GetLoadedTypesThatAreAssignableTo(generatorBaseType, possibleGenericArgument: forType).ToArray();
        }

        public static IEnumerable<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Type> GetAllLoadedTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
        }

        public static Type GetLoadedTypeByFullName(string fullTypeName)
        {
            return GetAllLoadedTypes().FirstOrDefault(t => t.FullName == fullTypeName);
        }

        public static IEnumerable<Type> GetLoadedTypesThatAreAssignableTo(Type type, bool instantiableOnly = true, Type possibleGenericArgument = null)
        {
            return GetAllLoadedTypes().Select(t =>
            {
                var name = t.Name;
                if (type.IsAssignableFrom(t) && (!instantiableOnly || !t.IsAbstract && !t.IsInterface))
                {
                    return (true, t);
                }

                if (t.IsGenericType && possibleGenericArgument != null)
                {
                    var genericArguments = t.GetGenericArguments();
                    if (genericArguments.Length == 1)
                    {
                        var genericConstraints = genericArguments[0].GetGenericParameterConstraints();

                        if (genericConstraints.Length == 0 || genericConstraints[0].IsAssignableFrom(possibleGenericArgument))
                        {
                            var generic = t.MakeGenericType(possibleGenericArgument);
                            if (type.IsAssignableFrom(generic) && (!instantiableOnly || !generic.IsAbstract && !generic.IsInterface))
                            {
                                return (true, generic);
                            }
                        }
                    }
                }
                return (false, t);
            })
                .Where(t => t.Item1)
                .Select(t => t.Item2);
        }

        public static IEnumerable<Type> GetLoadedTypesThatImplementInterfaceWithGenericArgumentsOfType(Type interfaceType, params Type[] genericTypes)
        {
            if (!interfaceType.IsInterface || !interfaceType.IsGenericType)
            {
                throw new InvalidOperationException($"Type \"{interfaceType.Name}\" is not an interface or is not a generic type");
            }

            var constructedType = interfaceType.MakeGenericType(genericTypes);

            return GetLoadedTypesThatAreAssignableTo(constructedType);
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
            return type.GetInterfaces().Any(i => i == @interface || (i.IsGenericType && i.GetGenericTypeDefinition() == @interface));
        }

        public static Type GetTypeOrElementType(this object o)
        {
            var type = o.GetType();
            return type.GetTypeOrElementType();
        }

        public static Type GetFirstGenericTypeOfNonGenericInterface(Type type, Type baseInterfaceType)
        {
            var firstInterface = type.GetInterfaces().FirstOrDefault(f => baseInterfaceType.IsAssignableFrom(f) && f.IsGenericType);

            return firstInterface?.GetGenericArguments()[0];
        }

        public static T GetAttributeOfType<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttribute<T>(true);
        }

        public static T GetAttributeOfType<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>(true);
        }
        public static bool HasAttributeOfType<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>(true) != null;
        }

        public static IEnumerable<Type> GetLoadedTypesWithAttribute<T>() where T : Attribute
        {
            return GetAllLoadedTypes().Where(t => GetAttributeOfType<T>(t) != null);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute<T>(true) != null);
        }

#nullable disable
        public static async Task<object> InvokeAsync(this MethodInfo method, object obj, params object[] parameters)
        {
            var task = (Task)method.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty?.GetValue(task);
        }
#nullable enable

        public static MethodInfo[] GetMethodsWithAttribute<T>(this Type type)
        {
            return type.GetMethods()
                .Where(p => p.GetCustomAttributes(true).Any(a => a is T))
                .ToArray();
        }
    }
}
