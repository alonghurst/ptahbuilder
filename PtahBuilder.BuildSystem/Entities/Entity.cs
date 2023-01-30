namespace PtahBuilder.BuildSystem.Entities
{
    public class Entity<T>
    {
        public string Id { get; }
        public T Value { get; }

        public Metadata Metadata { get; }

        public Entity(string id, T value, Metadata metadata)
        {
            Id = id;
            Value = value;
            Metadata = metadata;
        }
    }
}
