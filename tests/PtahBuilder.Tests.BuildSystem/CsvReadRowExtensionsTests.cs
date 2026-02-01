using PtahBuilder.BuildSystem.Extensions;
using PtahBuilder.BuildSystem.Steps.Input.Csv;
using Xunit;

namespace PtahBuilder.Tests.BuildSystem;

public class CsvReadRowExtensionsTests
{
    private static CsvReadStep<object>.ReadRow CreateRow(string[] columns, int rowNumber = 0, string filename = "file.csv", string rawLine = "")
    {
        rawLine = rawLine.Length > 0 ? rawLine : string.Join(",", columns);
        return new CsvReadStep<object>.ReadRow(rowNumber, filename, rawLine, columns);
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsValue_WhenColumnExists()
    {
        var row = CreateRow(new[] { "apple", "banana", "cherry" });
        Assert.Equal("apple", row.TryGetColumnValue("A"));
        Assert.Equal("banana", row.TryGetColumnValue("B"));
        Assert.Equal("cherry", row.TryGetColumnValue("C"));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsNull_WhenColumnIndexOutOfRange()
    {
        var row = CreateRow(new[] { "apple", "banana" });
        Assert.Null(row.TryGetColumnValue("C"));
        Assert.Null(row.TryGetColumnValue("Z"));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsNull_WhenRowIsNull()
    {
        CsvReadStep<object>.ReadRow? row = null;
        Assert.Null(row!.TryGetColumnValue("A"));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsNull_WhenColumnsIsNull()
    {
        var row = new CsvReadStep<object>.ReadRow(0, "f", "", null!);
        Assert.Null(row.TryGetColumnValue("A"));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsNull_WhenColumnLetterIsNullOrEmpty()
    {
        var row = CreateRow(new[] { "apple" });
        Assert.Null(row.TryGetColumnValue(""));
        Assert.Null(row.TryGetColumnValue(null!));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_ReturnsEmptyString_WhenCellIsEmpty()
    {
        var row = CreateRow(new[] { "", "banana" });
        Assert.Equal("", row.TryGetColumnValue("A"));
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_Out_ReturnsTrue_AndSetsValue_WhenColumnExists()
    {
        var row = CreateRow(new[] { "apple", "banana" });
        Assert.True(row.TryGetColumnValue("A", out var a));
        Assert.Equal("apple", a);
        Assert.True(row.TryGetColumnValue("B", out var b));
        Assert.Equal("banana", b);
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_Out_ReturnsTrue_WhenColumnExistsButValueIsEmpty()
    {
        var row = CreateRow(new[] { "" });
        Assert.True(row.TryGetColumnValue("A", out var value));
        Assert.Equal("", value);
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_Out_ReturnsFalse_WhenColumnOutOfRange()
    {
        var row = CreateRow(new[] { "apple" });
        Assert.False(row.TryGetColumnValue("B", out var value));
        Assert.Null(value);
    }

    [Fact]
    public void TryGetColumnValue_ByLetter_Out_ReturnsFalse_WhenRowIsNull()
    {
        CsvReadStep<object>.ReadRow? row = null;
        Assert.False(row!.TryGetColumnValue("A", out var value));
        Assert.Null(value);
    }

    [Fact]
    public void TryGetColumnValue_ByIndex_ReturnsValue_WhenIndexInRange()
    {
        var row = CreateRow(new[] { "apple", "banana", "cherry" });
        Assert.Equal("apple", row.TryGetColumnValue(0));
        Assert.Equal("banana", row.TryGetColumnValue(1));
        Assert.Equal("cherry", row.TryGetColumnValue(2));
    }

    [Fact]
    public void TryGetColumnValue_ByIndex_ReturnsNull_WhenIndexOutOfRange()
    {
        var row = CreateRow(new[] { "apple", "banana" });
        Assert.Null(row.TryGetColumnValue(-1));
        Assert.Null(row.TryGetColumnValue(2));
        Assert.Null(row.TryGetColumnValue(10));
    }

    [Fact]
    public void TryGetColumnValue_ByIndex_ReturnsNull_WhenRowOrColumnsNull()
    {
        var rowNull = new CsvReadStep<object>.ReadRow(0, "f", "", null!);
        Assert.Null(rowNull.TryGetColumnValue(0));
        CsvReadStep<object>.ReadRow? row = null;
        Assert.Null(row!.TryGetColumnValue(0));
    }

    [Fact]
    public void TryGetColumnValue_ByIndex_Out_ReturnsTrue_AndSetsValue_WhenIndexInRange()
    {
        var row = CreateRow(new[] { "apple", "banana" });
        Assert.True(row.TryGetColumnValue(0, out var v0));
        Assert.Equal("apple", v0);
        Assert.True(row.TryGetColumnValue(1, out var v1));
        Assert.Equal("banana", v1);
    }

    [Fact]
    public void TryGetColumnValue_ByIndex_Out_ReturnsFalse_WhenIndexOutOfRange()
    {
        var row = CreateRow(new[] { "apple" });
        Assert.False(row.TryGetColumnValue(-1, out var v));
        Assert.Null(v);
        Assert.False(row.TryGetColumnValue(1, out v));
        Assert.Null(v);
    }
}
