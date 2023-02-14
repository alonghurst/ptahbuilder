namespace PtahBuilder.BuildSystem.Entities;

public class Metadata
{
    public Metadata() : this(Enumerable.Empty<KeyValuePair<string, object>>())
    {

    }

    public Metadata(IEnumerable<KeyValuePair<string, object>> data)
    {
        foreach (var pair in data)
        {
            Add(pair.Key, pair.Value);
        }
    }

    public string[]? GetArray(string key) => Values.ContainsKey(key) ? Values[key] is string ? new[] { Values[key].ToString()! } : Values[key] as string[] : null;
    public string? GetString(string key) => Values.ContainsKey(key) && Values[key] is string ? Values[key].ToString() : null;

    public T Get<T>(string key) where T : class => Values[key] as T ?? throw new InvalidOperationException($"Unable to parse {key} as {typeof(T).Name}");

    public Dictionary<string, object> Values { get; } = new();

    public void Add(string key, object value)
    {
        Values.Add(key, value);
    }
    public void Set(string key, object value)
    {
        Values[key] = value;
    }

    public bool TryGetValue(string sourceFile, out object? o) => Values.TryGetValue(sourceFile, out o);
}
