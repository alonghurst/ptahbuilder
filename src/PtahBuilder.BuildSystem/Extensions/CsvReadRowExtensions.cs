using PtahBuilder.BuildSystem.Steps.Input.Csv;

namespace PtahBuilder.BuildSystem.Extensions;

/// <summary>
/// Extension methods for CSV/TSV <see cref="CsvReadStep{T}.ReadRow"/> when operating on row columns.
/// </summary>
public static class CsvReadRowExtensions
{
    /// <summary>
    /// Gets the column value by column letter (e.g. "A", "B"). Returns null if the row/columns are null,
    /// the column letter is invalid, or the index is out of range.
    /// </summary>
    public static string? TryGetColumnValue<T>(this CsvReadStep<T>.ReadRow row, string columnLetter) where T : class
    {
        if (row?.Columns == null || string.IsNullOrEmpty(columnLetter))
            return null;
        var index = columnLetter.ToColumn();
        if (index < 0 || index >= row.Columns.Length)
            return null;
        return row.Columns[index];
    }

    /// <summary>
    /// Tries to get the column value by column letter (e.g. "A", "B"). Returns true and sets <paramref name="value"/>
    /// when the column exists; otherwise returns false and sets <paramref name="value"/> to null.
    /// </summary>
    public static bool TryGetColumnValue<T>(this CsvReadStep<T>.ReadRow row, string columnLetter, out string? value) where T : class
    {
        value = null;
        if (row?.Columns == null || string.IsNullOrEmpty(columnLetter))
            return false;
        var index = columnLetter.ToColumn();
        if (index < 0 || index >= row.Columns.Length)
            return false;
        value = row.Columns[index];
        return true;
    }

    /// <summary>
    /// Gets the column value by zero-based index. Returns null if the row/columns are null or the index is out of range.
    /// </summary>
    public static string? TryGetColumnValue<T>(this CsvReadStep<T>.ReadRow row, int columnIndex) where T : class
    {
        if (row?.Columns == null)
            return null;
        if (columnIndex < 0 || columnIndex >= row.Columns.Length)
            return null;
        return row.Columns[columnIndex];
    }

    /// <summary>
    /// Tries to get the column value by zero-based index. Returns true and sets <paramref name="value"/>
    /// when the column exists; otherwise returns false and sets <paramref name="value"/> to null.
    /// </summary>
    public static bool TryGetColumnValue<T>(this CsvReadStep<T>.ReadRow row, int columnIndex, out string? value) where T : class
    {
        value = null;
        if (row?.Columns == null)
            return false;
        if (columnIndex < 0 || columnIndex >= row.Columns.Length)
            return false;
        value = row.Columns[columnIndex];
        return true;
    }
}
