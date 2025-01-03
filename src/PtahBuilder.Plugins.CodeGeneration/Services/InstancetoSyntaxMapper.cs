﻿using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PtahBuilder.BuildSystem.Services;
using PtahBuilder.Plugins.CodeGeneration.Syntax;
using PtahBuilder.Util.Extensions.Reflection;
using PtahBuilder.Util.Services.Logging;
using Microsoft.CodeAnalysis.CSharp;

namespace PtahBuilder.Plugins.CodeGeneration.Services
{
    public class InstanceToSyntaxMapper
    {
        private readonly IDefaultValueService _defaultValueService;

        public InstanceToSyntaxMapper(ILogger logger, IDefaultValueService defaultValueService)
        {
            Logger = logger;
            _defaultValueService = defaultValueService;
        }

        public List<Type> FoundTypes { get; } = new();
        public ILogger Logger { get; }
        private Dictionary<Type, InstanceToSyntaxMapper> _childMappers = new();

        public ObjectCreationExpressionSyntax InstanceToSyntax(object instance)
        {
            return Instantiations.NewUp(instance.GetType().Name, CreateAssignments(instance).ToArray());
        }

        private IEnumerable<AssignmentExpressionSyntax> CreateAssignments(object instance)
        {
            var nonDefaultProperties = _defaultValueService.GetNonDefaultPropertyAndTheNewValue(instance).OrderBy(p => p.Key.Name);

            foreach (var property in nonDefaultProperties)
            {
                yield return Assign(property.Key, property.Value);
            }
        }

        private AssignmentExpressionSyntax Assign(PropertyInfo property, object value)
        {
            return Expressions.AssignExpression(property.Name, ValueToSyntax(value));
        }

        private ExpressionSyntax ValueToSyntax(object value)
        {
            if (value != null)
            {
                var type = value.GetType();

                if (type == typeof(int))
                {
                    return Literals.Integer((int)value);
                }
                if (type == typeof(string))
                {
                    return Literals.String((string)value);
                }
                if (type == typeof(bool))
                {
                    return Literals.Boolean((bool)value);
                }
                if (type == typeof(float))
                {
                    return Literals.Float((float)value);
                }
                if (type == typeof(double))
                {
                    return SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal((double)value));
                }

                if (type == typeof(TimeSpan))
                {
                    var timespan = (TimeSpan)value;
                    var expression = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(timespan.Ticks));

                    return Instantiations.NewUp("System.TimeSpan", expression.AsArgument());
                }

                if (type.IsEnum)
                {
                    RegisterFoundType(type);

                    return Invocations.InvokeProperty(type.Name, value.ToString());
                }

                if (type.IsArray)
                {
                    dynamic values = value;
                    var valuesSyntax = new List<ExpressionSyntax>();
                    foreach (var val in values)
                    {
                        valuesSyntax.Add(ValueToSyntax(val));
                    }
                    return valuesSyntax.AsArraySyntax();
                }

                if (type.IsDictionaryType())
                {
                    RegisterFoundType(type);

                    var kvpExpressions = new List<ExpressionSyntax>();

                    dynamic values = value;
                    foreach (dynamic kvp in values)
                    {
                        kvpExpressions.Add(Collections.KeyValuePairInitializer(ValueToSyntax(kvp.Key), ValueToSyntax(kvp.Value)));
                    }

                    var kvpType = type.GetDictionaryKeyValuePairType();
                    var keyType = Types.Type(kvpType.GetGenericArguments()[0]);
                    // Floats come through as System.Single instead of float. Hack but it fixes it
                    var valueType = kvpType.GetGenericArguments()[1] == typeof(float) ? Types.Type("float") : Types.Type(kvpType.GetGenericArguments()[1]);

                    return Collections.InstantiateDictionary(keyType, valueType, kvpExpressions);
                }

                if (!_childMappers.ContainsKey(type))
                {
                    RegisterFoundType(type);

                    _childMappers.Add(type, new(Logger, _defaultValueService));
                }

                return _childMappers[type].InstanceToSyntax(value);
            }

            return Literals.Null;
        }

        private void RegisterFoundType(Type type)
        {
            if (!FoundTypes.Contains(type))
            {
                FoundTypes.Add(type);
                if (type.IsGenericType)
                {
                    foreach (var genericArgument in type.GetGenericArguments())
                    {
                        RegisterFoundType(genericArgument);
                    }
                }
            }
        }
    }
}
