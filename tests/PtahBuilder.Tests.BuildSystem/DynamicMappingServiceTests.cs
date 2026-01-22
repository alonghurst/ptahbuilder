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

    private class ClassWithNullableDictionary
    {
        public Dictionary<int, string>? Map { get; set; }
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

    [Fact]
    public void Map_MergesNewDictionaryIntoExistingDictionary_WhenExistingIsNotNull()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();
        
        // First, set some initial values
        obj.Map[1] = "one";
        obj.Map[2] = "two";
        
        // Then add new values - should merge, not replace
        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "3|three, 4|four");
        
        // Should have all 4 values
        Assert.Equal(4, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("two", obj.Map[2]);
        Assert.Equal("three", obj.Map[3]);
        Assert.Equal("four", obj.Map[4]);
    }

    [Fact]
    public void Map_OverwritesExistingKeys_WhenMergingDictionaries()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();
        
        // First, set some initial values
        obj.Map[1] = "one";
        obj.Map[2] = "two";
        
        // Then add values with overlapping keys - should overwrite existing values
        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "2|TWO, 3|three");
        
        // Should have 3 values, with key 2 overwritten
        Assert.Equal(3, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("TWO", obj.Map[2]); // Overwritten
        Assert.Equal("three", obj.Map[3]);
    }

    [Fact]
    public void Map_ReplacesDictionary_WhenExistingIsEmpty()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();
        
        // Dictionary is initialized to empty, so this should work like a new dictionary
        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "1|one, 2|two");
        
        // Should have the new values
        Assert.NotNull(obj.Map);
        Assert.Equal(2, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("two", obj.Map[2]);
    }

    [Fact]
    public void Map_ReplacesDictionary_WhenExistingIsNull()
    {
        var svc = CreateService();
        var obj = new ClassWithNullableDictionary();
        
        // Dictionary is null, so this should create a new dictionary (not merge)
        svc.Map(obj, nameof(ClassWithNullableDictionary.Map), "1|one, 2|two");
        
        // Should have the new values
        Assert.NotNull(obj.Map);
        Assert.Equal(2, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("two", obj.Map[2]);
    }

    [Fact]
    public void Map_MergesNewDictionaryIntoExistingDictionary_ForStringStringArrayDictionary()
    {
        var svc = CreateService();
        var obj = new ClassWithStringStringArrayDictionary();
        
        // First, set some initial values
        obj.Map["A"] = new[] { "1" };
        obj.Map["B"] = new[] { "2" };
        
        // Then add new values - should merge
        svc.Map(obj, nameof(ClassWithStringStringArrayDictionary.Map), "C|3, D|4");
        
        // Should have all 4 values
        Assert.Equal(4, obj.Map.Count);
        Assert.Equal(new[] { "1" }, obj.Map["A"]);
        Assert.Equal(new[] { "2" }, obj.Map["B"]);
        Assert.Equal(new[] { "3" }, obj.Map["C"]);
        Assert.Equal(new[] { "4" }, obj.Map["D"]);
    }

    [Fact]
    public void Map_MergesEmptyDictionary_DoesNotAffectExistingDictionary()
    {
        var svc = CreateService();
        var obj = new ClassWithIntStringDictionary();
        
        // First, set some initial values
        obj.Map[1] = "one";
        obj.Map[2] = "two";
        
        // Then add empty dictionary - should not affect existing
        svc.Map(obj, nameof(ClassWithIntStringDictionary.Map), "");
        
        // Should still have the original 2 values
        Assert.Equal(2, obj.Map.Count);
        Assert.Equal("one", obj.Map[1]);
        Assert.Equal("two", obj.Map[2]);
    }
}


