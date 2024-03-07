using System.Diagnostics;

namespace PtahBuilder.BuildSystem.Entities;

[DebuggerDisplay("{Id}: {Value}")]
public class Entity<T>
{
    public string Id { get; }
    public T Value { get; }

    public Metadata Metadata { get; }

    public Validation Validation { get; } = new();

    public Entity(string id, T value, Metadata metadata)
    {
        Id = id;
        Value = value;
        Metadata = metadata;
    }
}