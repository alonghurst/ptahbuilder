using Humanizer;

namespace PtahBuilder.BuildSystem.Metadata
{
    public abstract class BaseDataMetadataResolver<T> where T : TypeData
    {
        public virtual string DataDirectoryToOperateIn => EntityName.Replace("Type", string.Empty).Pluralize();

        public string NamespaceForOutput => "Factories";

        public virtual string EntityName => typeof(T).Name;

        public virtual string GetEntityCategory(T typeEntity) => string.Empty;

        public virtual string NamespacePart => DataDirectoryToOperateIn;

        public abstract string AbsoluteNamespaceForOutput { get; }
    }
}
