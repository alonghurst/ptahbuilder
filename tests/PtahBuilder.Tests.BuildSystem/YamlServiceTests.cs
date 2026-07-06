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

    private sealed class NullLogger : ILogger
    {
        public void Info(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
        public void Success(string message) { }
        public void Verbose(string message) { }
    }
}
