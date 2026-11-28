using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes the value a script returned.
/// </summary>
internal static class NotebookScriptValueFormatter
{
    #region Public Methods

    /// <summary>
    /// Writes a returned value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The text, or null when there is no value.</returns>
    public static string? Format(object? value) => value switch
    {
        null => null,
        DataTable table => string.Create(CultureInfo.CurrentCulture,
            $"DataTable '{table.TableName}': {table.Columns.Count:N0} columns, {table.Rows.Count:N0} rows"),
        IFormattable formattable => formattable.ToString(null,
            CultureInfo.InvariantCulture),
        _ => value.ToString()
    };

    #endregion
}
