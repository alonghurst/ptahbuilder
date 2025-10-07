using Xunit;
using PtahBuilder.BuildSystem;
using PtahBuilder.BuildSystem.Services.Mapping;
using PtahBuilder.BuildSystem.Services.Parsing;

namespace PtahBuilder.Tests.BuildSystem;

public class ScalarValueServiceTests
{
    private readonly ScalarValueService _service;

    public ScalarValueServiceTests()
    {
        // Create a simple mock that always returns false to use default conversion logic
        var mockCustomValueParser = new MockCustomValueParserService();
        _service = new ScalarValueService(mockCustomValueParser);
    }

    [Theory]
    [InlineData("123", typeof(int), 123)]
    [InlineData("456.78", typeof(double), 456.78)]
    [InlineData("789.12", typeof(float), 789.12f)]
    [InlineData("true", typeof(bool), true)]
    [InlineData("false", typeof(bool), false)]
    [InlineData("1", typeof(bool), true)]
    [InlineData("0", typeof(bool), false)]
    [InlineData("2023-12-25", typeof(DateTime), "2023-12-25")]
    [InlineData("25/12/2023", typeof(DateTime), "25/12/2023")]
    [InlineData("2:30:45", typeof(TimeSpan), "2:30:45")]
    [InlineData("2.5", typeof(TimeSpan), "2.5")]
    public void ConvertScalarValue_StringToPrimitiveTypes_ConvertsCorrectly(string input, Type targetType, object expected)
    {
        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        if (expected is string expectedStr)
        {
            // For DateTime and TimeSpan, we need to parse the expected string
            if (targetType == typeof(DateTime))
            {
                var expectedDateTime = DateTime.Parse(expectedStr);
                Assert.Equal(expectedDateTime, result);
            }
            else if (targetType == typeof(TimeSpan))
            {
                var expectedTimeSpan = expectedStr.Contains(":") 
                    ? TimeSpan.Parse(expectedStr) 
                    : TimeSpan.FromHours(double.Parse(expectedStr));
                Assert.Equal(expectedTimeSpan, result);
            }
        }
        else
        {
            Assert.Equal(expected, result);
        }
    }

    [Theory]
    [InlineData("123", typeof(int?), 123)]
    [InlineData("", typeof(int?), null)]
    [InlineData("   ", typeof(int?), null)]
    [InlineData(null, typeof(int?), null)]
    [InlineData("456.78", typeof(double?), 456.78)]
    [InlineData("", typeof(double?), null)]
    public void ConvertScalarValue_StringToNullableTypes_ConvertsCorrectly(string input, Type targetType, object? expected)
    {
        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1,2,3,4,5", typeof(int[]), new[] { 1, 2, 3, 4, 5 })]
    [InlineData("1.1,2.2,3.3", typeof(double[]), new[] { 1.1, 2.2, 3.3 })]
    [InlineData("true,false,true", typeof(bool[]), new[] { true, false, true })]
    public void ConvertScalarValue_StringToArray_ConvertsCorrectly(string input, Type targetType, object expected)
    {
        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        if (result is Array resultArray && expected is Array expectedArray)
        {
            Assert.Equal(expectedArray.Length, resultArray.Length);
            for (int i = 0; i < expectedArray.Length; i++)
            {
                Assert.Equal(expectedArray.GetValue(i), resultArray.GetValue(i));
            }
        }
        else
        {
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void ConvertScalarValue_EmptyStringToArray_ReturnsEmptyArray()
    {
        // Arrange
        var input = "";
        var targetType = typeof(int[]);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<int[]>(result);
        var array = (int[])result;
        Assert.Empty(array);
    }

    [Fact]
    public void ConvertScalarValue_SingleValueToArray_WrapsInArray()
    {
        // Arrange
        var input = "42";
        var targetType = typeof(int[]);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<int[]>(result);
        var array = (int[])result;
        Assert.Single(array);
        Assert.Equal(42, array[0]);
    }

    [Fact]
    public void ConvertScalarValue_StringTrimming_TrimsWhitespace()
    {
        // Arrange
        var input = "  hello world  ";
        var targetType = typeof(string);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal("hello world", result);
    }

    [Fact]
    public void ConvertScalarValue_TimeSpanFromHours_ConvertsCorrectly()
    {
        // Arrange
        var input = "2.5";
        var targetType = typeof(TimeSpan);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal(TimeSpan.FromHours(2.5), result);
    }

    [Fact]
    public void ConvertScalarValue_TimeSpanFromTimeFormat_ConvertsCorrectly()
    {
        // Arrange
        var input = "1:30:45";
        var targetType = typeof(TimeSpan);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal(TimeSpan.Parse("1:30:45"), result);
    }

    [Fact]
    public void ConvertScalarValue_StringToEnum_ConvertsCorrectly()
    {
        // Arrange
        var input = "Monday";
        var targetType = typeof(DayOfWeek);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void ConvertScalarValue_StringToEnumByValue_ConvertsCorrectly()
    {
        // Arrange
        var input = "1";
        var targetType = typeof(DayOfWeek);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void ConvertScalarValue_StringArray_ConvertsCorrectly()
    {
        // Arrange
        var input = "apple,banana,cherry";
        var targetType = typeof(string[]);

        // Act
        var result = _service.ConvertScalarValue(targetType, input);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string[]>(result);
        var array = (string[])result;
        Assert.Single(array);
        Assert.Equal("apple,banana,cherry", array[0]);
    }

    [Fact]
    public void ConvertScalarValue_StringToHashSetInt_ConvertsCorrectly()
    {
        var input = "1,2,3,4,5";
        var targetType = typeof(HashSet<int>);

        var result = _service.ConvertScalarValue(targetType, input);

        Assert.NotNull(result);
        Assert.IsType<HashSet<int>>(result);
        var hashSet = (HashSet<int>)result;
        Assert.Equal(5, hashSet.Count);
        Assert.Contains(1, hashSet);
        Assert.Contains(2, hashSet);
        Assert.Contains(3, hashSet);
        Assert.Contains(4, hashSet);
        Assert.Contains(5, hashSet);
    }

    [Fact]
    public void ConvertScalarValue_StringToHashSetString_ConvertsCorrectly()
    {
        var input = "apple,banana,cherry";
        var targetType = typeof(HashSet<string>);

        var result = _service.ConvertScalarValue(targetType, input);

        Assert.NotNull(result);
        Assert.IsType<HashSet<string>>(result);
        var hashSet = (HashSet<string>)result;
        Assert.Equal(3, hashSet.Count);
        Assert.Contains("apple", hashSet);
        Assert.Contains("banana", hashSet);
        Assert.Contains("cherry", hashSet);
    }

    [Fact]
    public void ConvertScalarValue_EmptyStringToHashSet_ReturnsEmptyHashSet()
    {
        var input = "";
        var targetType = typeof(HashSet<int>);

        var result = _service.ConvertScalarValue(targetType, input);

        Assert.NotNull(result);
        Assert.IsType<HashSet<int>>(result);
        var hashSet = (HashSet<int>)result;
        Assert.Empty(hashSet);
    }

    [Fact]
    public void ConvertScalarValue_SingleValueToHashSet_CreatesSingleElementHashSet()
    {
        var input = "42";
        var targetType = typeof(HashSet<int>);

        var result = _service.ConvertScalarValue(targetType, input);

        Assert.NotNull(result);
        Assert.IsType<HashSet<int>>(result);
        var hashSet = (HashSet<int>)result;
        Assert.Single(hashSet);
        Assert.Contains(42, hashSet);
    }

    [Fact]
    public void ConvertScalarValue_StringWithSpacesToHashSet_TrimsWhitespace()
    {
        var input = " apple , banana , cherry ";
        var targetType = typeof(HashSet<string>);

        var result = _service.ConvertScalarValue(targetType, input);

        Assert.NotNull(result);
        Assert.IsType<HashSet<string>>(result);
        var hashSet = (HashSet<string>)result;
        Assert.Equal(3, hashSet.Count);
        Assert.Contains("apple", hashSet);
        Assert.Contains("banana", hashSet);
        Assert.Contains("cherry", hashSet);
    }
}

// Mock implementation for testing
public class MockCustomValueParserService : ICustomValueParserService
{
    public bool TryParseValue(Type type, object value, out object? result)
    {
        // For testing purposes, we'll return false to use the default conversion logic
        result = null;
        return false;
    }
}
