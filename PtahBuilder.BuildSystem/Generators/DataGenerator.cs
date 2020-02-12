using System.Collections.Generic;
using System.IO;
using System.Linq;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Helpers;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;
using PtahBuilder.BuildSystem.Syntax;

namespace PtahBuilder.BuildSystem.Generators
{
    public class DataGenerator<T> where T :  new()
    {
        public PathResolver PathResolver { get; }
        public Logger Logger { get; }
        public BaseDataMetadataResolver<T> MetadataResolver { get; }

        public DataGenerator(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<T> metadataResolver)
        {
            Logger = logger;
            PathResolver = pathResolver;
            MetadataResolver = metadataResolver;
        }

        public Dictionary<T, MetadataCollection> Generate()
        {
            var dataDirectory = Path.GetFullPath(PathResolver.DataDirectory(MetadataResolver.DataDirectoryToOperateIn));
            if (!Directory.Exists(dataDirectory))
            {
                Logger.Warning($"Unable to find directory {dataDirectory}");
                return new Dictionary<T, MetadataCollection>();
            }

            var tidier = new FileTidier();
            tidier.ParseDirectory(dataDirectory);

            Logger.Info($"Parsing {MetadataResolver.EntityTypeName} data");

            var yaml = new YamlToBaseDataMapper<T>(Logger, MetadataResolver);
            yaml.ParseDirectory(dataDirectory);

            var baseData = yaml.ParsedEntitiesMetadata;

            var operations = GetOperations().ToArray();

            foreach (var operation in operations)
            {
                baseData = operation.Operate(baseData);
            }

            var toSyntax = new InstanceToTypeFactoryDefinitionsFileWriter<T>(Logger, MetadataResolver);

            toSyntax.Generate(MetadataResolver.AbsoluteNamespaceForOutput,
                baseData.WhereIsNotBuildOnly(),
                PathResolver.FactoryOutputFile(MetadataResolver, "Types"));

            return baseData;
        }

        protected virtual IEnumerable<IOperation<T>> GetOperations()
        {
            yield return new FileMover<T>(Logger, PathResolver, MetadataResolver);
        }
    }
}
