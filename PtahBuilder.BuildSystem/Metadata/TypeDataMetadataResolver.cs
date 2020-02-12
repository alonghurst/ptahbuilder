using Humanizer;

namespace PtahBuilder.BuildSystem.Metadata
{
    public abstract class BaseDataMetadataResolver<T>
    {
        public virtual string DataDirectoryToOperateIn => EntityTypeName.Replace("Type", string.Empty).Pluralize();

        public virtual string EntityTypeName => typeof(T).Name;

        public virtual string GetEntityCategory(T typeEntity) => string.Empty;

        public abstract string AbsoluteNamespaceForOutput { get; }

        public abstract string GetEntityId(T entity);
        public abstract void SetEntityId(T entity, string entityId);
    }
}
