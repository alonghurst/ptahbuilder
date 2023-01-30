using PtahBuilder.LegacyBuildSystem.FileManagement;
using PtahBuilder.LegacyBuildSystem.Metadata;
using PtahBuilder.Tests.LegacyTestBuilder.Types;

namespace PtahBuilder.Tests.LegacyTestBuilder;

public static class MinimalConfig
{
    public class Files : IFiles
    {
        public static string ForcedRoot { get; set; }
        public string Root => ForcedRoot ?? "../../..";
        public string Output => Path.Combine(Root, "Output");
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