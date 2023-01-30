using Humanizer;

namespace PtahBuilder.LegacyBuildSystem.Metadata;

public abstract class BaseDataMetadataResolver<T>
{
    public virtual string DataDirectoryToOperateIn => EntityShortName.Pluralize();

    public virtual string EntityTypeName => typeof(T).Name;

    public virtual string EntityShortName => EntityTypeName.Replace("Type", string.Empty);

    public virtual string GetEntityCategory(T typeEntity) => string.Empty;

    public abstract string AbsoluteNamespaceForOutput { get; }

    public abstract string GetEntityId(T entity);
    public abstract void SetEntityId(T entity, string entityId);
}