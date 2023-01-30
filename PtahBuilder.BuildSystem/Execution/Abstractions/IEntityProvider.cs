using PtahBuilder.BuildSystem.Entities;

namespace PtahBuilder.BuildSystem.Execution.Abstractions;

public interface IEntityProvider<T>
{
    Dictionary<string, Entity<T>> Entities { get; }
}