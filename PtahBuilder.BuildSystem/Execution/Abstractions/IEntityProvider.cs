using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution;

public interface IEntityProvider<T>
{
    Dictionary<string, Entity<T>> Entities { get; }
}