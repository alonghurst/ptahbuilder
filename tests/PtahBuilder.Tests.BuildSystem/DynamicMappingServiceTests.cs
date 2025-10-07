using System.Collections.Generic;
using PtahBuilder.BuildSystem.Services.Mapping;
using PtahBuilder.BuildSystem.Services.Parsing;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class DynamicMappingServiceTests
{
    private class ClassWithIntStringDictionary
    {
        public Dictionary<int, string> Map { get; set; } = new();
    }

    private class ClassWithStringStringArrayDictionary
    {
        public Dictionary<string, string[]> Map { get; set; } = new();
    }

    private static DynamicMappingService CreateService()
    {
        // Use a mock that always returns false so default conversion is used
        var scalar = new ScalarValueService(new MockCustomValueParserService());
        return new DynamicMappingService(scalar);
    }

    [Fact]
    public void Map_CanParseStringInto_DictionaryOfIntString()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();

        // single pair using '|' between key and value
        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "1|one");

        Assert.True(obj.Map.ContainsKey(1));
        Assert.Equal("one", obj.Map[1]);
    }

    [Fact]
    public void Map_CanParseStringInto_DictionaryOfStringToStringArray()
    {
        var svc = CreateService();
        var obj = new ClassWithStringStringArrayDictionary();

        // single pair; array element(s) parsed by scalar service (string[] wraps scalar)
        svc.Map(obj, nameof(ClassWithStringStringArrayDictionary.Map), "A|1");

        Assert.True(obj.Map.ContainsKey("A"));
        Assert.Equal(new[] { "1" }, obj.Map["A"]);
    }

    [Fact]
    public void Map_CanParseMultiplePairsInto_DictionaryOfIntString()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();

        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "1|one, 2|two");

        Assert.Equal(2, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("two", obj.Map[2]);
    }

    [Fact]
    public void Map_CanParseMultiplePairsInto_DictionaryOfStringToStringArray()
    {
        var svc = CreateService();
        var obj = new ClassWithStringStringArrayDictionary();

        // Each value becomes a single-element string[] under current scalar rules
        svc.Map(obj, nameof(ClassWithStringStringArrayDictionary.Map), "A|1, B|2");

        Assert.Equal(2, obj.Map.Count);
        Assert.Equal(new[] { "1" }, obj.Map["A"]);
        Assert.Equal(new[] { "2" }, obj.Map["B"]);
    }
}


