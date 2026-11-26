using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads the values of a chart table.
/// </summary>
internal static class NotebookChartValues
{
    #region Private Variables

    private static readonly HashSet<string> NumericTypes = new(StringComparer.Ordinal)
    {
        "byte", "decimal", "double", "float", "int", "long", "short"
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Checks whether a type name is numeric.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>Whether it is numeric.</returns>
    public static bool IsNumeric(string typeName) => NumericTypes.Contains(typeName);

    /// <summary>
    /// Checks whether a type name is a time.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>Whether it is a time.</returns>
    public static bool IsTimestamp(string typeName) => typeName is "datetime" or "datetimeoffset";

    /// <summary>
    /// Gets the text of a row at a position.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="index">The position.</param>
    /// <returns>The text, or null when the row is shorter.</returns>
    public static string? TextAt(IReadOnlyList<string?> row,
        int index) => index < row.Count ? row[index] : null;

    /// <summary>
    /// Reads a finite number.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="value">The number.</param>
    /// <returns>Whether it was read.</returns>
    public static bool TryNumber(string? text,
        out double value) => double.TryParse(text,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out value) && double.IsFinite(value);

    /// <summary>
    /// Reads a time as universal time.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="value">The time.</param>
    /// <returns>Whether it was read.</returns>
    public static bool TryTimestamp(string? text,
        out DateTime value)
    {
        var read = DateTimeOffset.TryParse(text,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var parsed);
        value = parsed.UtcDateTime;
        return read;
    }

    #endregion
}
