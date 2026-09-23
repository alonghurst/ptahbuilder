using PtahBuilder.BuildSystem.Services.Mapping;
using PtahBuilder.BuildSystem.Services.Parsing;
using PtahBuilder.BuildSystem.Services.Serialization;
using PtahBuilder.Util.Services.Logging;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class YamlServiceTests
{
    private class ClassWithStringStringArrayDictionary
    {
        public Dictionary<string, string[]> Map { get; set; } = new();
    }

    private class ClassWithStringArray
    {
        public string[] Items { get; set; } = Array.Empty<string>();
    }

    private class ClassWithNestedStringArray
    {
        public string[][] Items { get; set; } = Array.Empty<string[]>();
    }

    private sealed class ClassWithStringList
    {
        public List<string> Items { get; set; } = new();
    }

    private sealed class ClassWithNestedObjectList
    {
        public List<NamedEntry> Entries { get; set; } = new();
    }

    private sealed class NamedEntry
    {
        public string Id { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    private static YamlService CreateService()
    {
        var scalar = new ScalarValueService(new MockCustomValueParserService());
        return new YamlService(new NullLogger(), scalar);
    }

    [Fact]
    public void Deserialize_DictionaryWithSequenceValue_ParsesStringArray()
    {
        var yaml = """
            Map:
              - Key: A
                Value:
                  - apple
                  - cherry
              - Key: B
                Value:
                  - banana
            """;

        var result = CreateService().Deserialize<ClassWithStringStringArrayDictionary>(yaml);

        Assert.Equal(2, result.Map.Count);
        Assert.Equal(new[] { "apple", "cherry" }, result.Map["A"]);
        Assert.Equal(new[] { "banana" }, result.Map["B"]);
    }

    [Fact]
    public void Deserialize_ArrayProperty_ParsesSequence()
    {
        var yaml = """
            Items:
              - apple
              - cherry
            """;

        var result = CreateService().Deserialize<ClassWithStringArray>(yaml);

        Assert.Equal(new[] { "apple", "cherry" }, result.Items);
    }

    [Fact]
    public void Deserialize_NestedArrayProperty_ParsesSubSequences()
    {
        var yaml = """
            Items:
              - - apple
                - cherry
              - - banana
            """;

        var result = CreateService().Deserialize<ClassWithNestedStringArray>(yaml);

        Assert.Equal(2, result.Items.Length);
        Assert.Equal(new[] { "apple", "cherry" }, result.Items[0]);
        Assert.Equal(new[] { "banana" }, result.Items[1]);
    }

    [Fact]
    public void Deserialize_ListProperty_ParsesSequence()
    {
        var yaml = """
            Items:
              - apple
              - cherry
            """;

        var result = CreateService().Deserialize<ClassWithStringList>(yaml);

        Assert.Equal(new[] { "apple", "cherry" }, result.Items);
    }

    [Fact]
    public void Deserialize_SingleItemList_ParsesAsList()
    {
        var yaml = """
            Items:
              - apple
            """;

        var result = CreateService().Deserialize<ClassWithStringList>(yaml);

        Assert.Equal(new[] { "apple" }, result.Items);
    }

    [Fact]
    public void Deserialize_NestedObjectList_ParsesChildLists()
    {
        var yaml = """
            Entries:
              - Id: Return
                Tags:
                  - gold
                  - bone
              - Id: Favour
                Tags:
                  - clue
            """;

        var result = CreateService().Deserialize<ClassWithNestedObjectList>(yaml);

        Assert.Equal(2, result.Entries.Count);
        Assert.Equal("Return", result.Entries[0].Id);
        Assert.Equal(new[] { "gold", "bone" }, result.Entries[0].Tags);
        Assert.Equal("Favour", result.Entries[1].Id);
        Assert.Equal(new[] { "clue" }, result.Entries[1].Tags);
    }

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Success(string message) { }
        public void Verbose(string message) { }
    }
}
