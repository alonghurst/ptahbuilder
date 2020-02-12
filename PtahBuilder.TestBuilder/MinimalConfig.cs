using PtahBuilder.BuildSystem.FileManagement;
using PtahBuilder.BuildSystem.Metadata;
using PtahBuilder.TestBuilder.Types;

namespace PtahBuilder.TestBuilder
{
   public static class MinimalConfig
    {
        public class Files : IFiles
        {
            public string Root => ".";
            public string OutputForCode => "./Output";
        }

        public class MetadataResolver<T> : BaseDataMetadataResolver<T> where T : BaseTypeData
        {
            public override string AbsoluteNamespaceForOutput => "PtahBuilder.TestBuilder.Output";

            public override string GetEntityId(T entity) => entity.TypeName;
            public override void SetEntityId(T entity, string entityId)
            {
                entity.TypeName = entityId;
            }
        }
    }
}
