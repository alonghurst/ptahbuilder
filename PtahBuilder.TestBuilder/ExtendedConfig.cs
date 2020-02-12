using System;
using System.Collections.Generic;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Generators;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.BuildSystem.Operations;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder
{
    public static class ExtendedConfig
    {
        public class SimpleTypeDataGenerator : DataGenerator<SimpleType>
        {
            public SimpleTypeDataGenerator(Logger logger, PathResolver pathResolver, BaseDataMetadataResolver<SimpleType> metadataResolver) : base(logger, pathResolver, metadataResolver)
            {
            }

            protected override IEnumerable<IOperation<SimpleType>> GetOperations()
            {
                foreach (var operation in base.GetOperations())
                {
                    yield return operation;
                }

                yield return new WrappedOperation<SimpleType>(ReverseName);
            }

            private void ReverseName(Dictionary<SimpleType, MetadataCollection> entities)
            {
                foreach (var entity in entities)
                {
                    if (entity.Value.ContainsKey("ReverseName"))
                    {
                        char[] arr = entity.Key.Name.ToCharArray();
                        Array.Reverse(arr);
                        entity.Key.Name = new string(arr);
                    }
                }
            }
        }
    }
}
