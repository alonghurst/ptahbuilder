using Humanizer;

namespace PtahBuilder.BuildSystem.Metadata
{
    public abstract class BaseDataMetadataResolver<T>
    {
        public virtual string DataDirectoryToOperateIn => EntityName.Replace("Type", string.Empty).Pluralize();

        public virtual string EntityName => typeof(T).Name;

        public virtual string GetEntityCategory(T typeEntity) => string.Empty;

        public abstract string AbsoluteNamespaceForOutput { get; }
        public abstract string GetEntityTypeName(T entity);
    }
}
