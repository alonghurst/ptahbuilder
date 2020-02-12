using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Helpers;

namespace PtahBuilder.BuildSystem
{
    public class DataGeneratorFactory
    {
        public Logger Logger { get; }
        public PathResolver PathResolver { get; }
        private Type[] _typesToGenerate;

        public DataGeneratorFactory(Logger logger, PathResolver pathResolver, params Type[] typesToGenerate)
        {
            Logger = logger;
            PathResolver = pathResolver;
            _typesToGenerate = typesToGenerate;
        }

        public void Process()
        {
            var generatorTypes = _typesToGenerate.Select(t => new { BaseData = t, generatorType = Helpers.ReflectionHelper.FindBaseDataGeneratorType(t) })
                .ToDictionary(t => t.BaseData, t => t.generatorType);

            var metadataResolverTypes = _typesToGenerate.Select(t => new { BaseData = t, metadataResolverType = Helpers.ReflectionHelper.FindBaseDataMetadataResolverType(t) })
                .ToDictionary(t => t.BaseData, t => t.metadataResolverType);

            var typesToProcess = _typesToGenerate.ToList();
            var processedTypes = new Dictionary<Type, object>();

            var baseTypeConstructorArguments = typeof(DataGenerator<>).GetConstructors().First().GetParameters().Length;

            bool doneAny;
            do
            {
                doneAny = false;
                for (int i = 0; i < typesToProcess.Count; i++)
                {
                    var type = typesToProcess[i];

                    var generatorType = generatorTypes[type];

                    var constructorToUse = generatorType.GetConstructors().First();
                    var parameters = constructorToUse.GetParameters();
                    var additionalArguments = new List<object?>();

                    for (int j = baseTypeConstructorArguments; j < parameters.Length; j++)
                    {
                        if (GetProcessedTypeForParameter(parameters[j], processedTypes, out var argument))
                        {
                            additionalArguments.Add(argument);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (baseTypeConstructorArguments + additionalArguments.Count == parameters.Length)
                    {
                        var metadataResolver = metadataResolverTypes[type].GetConstructors().First().Invoke(null);

                        var arguments = new[]
                        {
                            Logger,
                            PathResolver,
                            metadataResolver
                        }.Union(additionalArguments).ToArray();

                        dynamic baseDataMetadataResolver = constructorToUse.Invoke(arguments);

                        var result = baseDataMetadataResolver.Generate();

                        processedTypes.Add(type, result);

                        doneAny = true;
                        typesToProcess.RemoveAt(i);
                        i--;
                    }
                }
            } while (typesToProcess.Count > 0 && doneAny);

            Logger.LogSection("Types where a BaseDataGenerator could not be instantiated", typesToProcess.Select(t => t.FullName ?? t.Name));

            Logger.LogSection("Type Summary", processedTypes.Select(t => $"{t.Key.Name} {((dynamic)t.Value).Count}"));

            ProcessSecondaryGenerators(processedTypes);
        }

        private bool GetProcessedTypeForParameter(ParameterInfo parameter, Dictionary<Type, object> processedTypes, out object[]? argument)
        {
            var parameterType = parameter.ParameterType.GetTypeOrElementType();
            if (!processedTypes.ContainsKey(parameterType))
            {
                argument = null;
                return false;
            }

            argument = GetArgumentFromAdditionalData(processedTypes[parameterType]);
            return true;
        }

        private object[] GetArgumentFromAdditionalData(dynamic dictionary)
        {
            return Enumerable.ToArray(dictionary.Keys);
        }

        private void ProcessSecondaryGenerators(Dictionary<Type, object> processedTypes)
        {
            foreach (var processedType in processedTypes)
            {
                var secondaryGeneratorTypes = Helpers.ReflectionHelper.FindSecondaryGeneratorTypes(processedType.Key);

                foreach (var secondaryGeneratorType in secondaryGeneratorTypes)
                {
                    var secondaryGenerator = secondaryGeneratorType.GetConstructors().First().Invoke(new[] { Logger, PathResolver, processedType.Value });

                    var methods = secondaryGeneratorType.GetMethodsWithAttribute<GenerateAttribute>();

                    foreach (var method in methods)
                    {
                        var parameters = method.GetParameters();
                        var arguments = new List<object?>();

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (GetProcessedTypeForParameter(parameters[i], processedTypes, out var argument))
                            {
                                arguments.Add(argument);
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (arguments.Count == parameters.Length)
                        {
                            method.Invoke(secondaryGenerator, arguments.ToArray());

                            Logger.LogSection("Additional Generators", -1, $"{secondaryGeneratorType.Name}.{method.Name}");
                        }
                    }
                }
            }
        }
    }
}
