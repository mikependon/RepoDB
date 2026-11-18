using System.Data;
using System.Text;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes CSV headers and records.
/// </summary>
internal static class CsvSaveHelper
{
    #region Public Methods

    /// <summary>
    /// Writes CSV to an open stream.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="separator">The field delimiter.</param>
    public static void Write(DataTable table,
        Stream stream,
        char separator)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(stream);
        Validate(table,
            separator);
        using var writer = new StreamWriter(stream,
            new UTF8Encoding(false,
                true),
            leaveOpen: true) { NewLine = "\r\n" };
        WriteRecord(writer,
            table,
            separator,
            row: null);
        foreach (var row in InspectionHelper.Rows(table))
        {
            WriteRecord(writer,
                table,
                separator,
                row);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates the delimiter and loader-compatible headers.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="separator">The field delimiter.</param>
    private static void Validate(DataTable table,
        char separator)
    {
        if (separator is '\r' or '\n' or '"' or '\0')
        {
            throw new ArgumentOutOfRangeException(nameof(separator));
        }
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (DataColumn column in table.Columns)
        {
            if (string.IsNullOrWhiteSpace(column.ColumnName) || !names.Add(column.ColumnName))
            {
                throw new ArgumentException($"Invalid or duplicate CSV header '{column.ColumnName}'.",
                    nameof(table));
            }
        }
        if (names.Count == 0)
        {
            throw new ArgumentException("CSV requires at least one column.",
                nameof(table));
        }
    }

    /// <summary>
    /// Writes the header or one data record.
    /// </summary>
    /// <param name="writer">The output writer.</param>
    /// <param name="table">The table to save.</param>
    /// <param name="separator">The field delimiter.</param>
    /// <param name="row">The row or null for the header.</param>
    private static void WriteRecord(TextWriter writer,
        DataTable table,
        char separator,
        DataRow? row)
    {
        foreach (DataColumn column in table.Columns)
        {
            if (column.Ordinal > 0)
            {
                writer.Write(separator);
            }
            var text = row is null ? column.ColumnName : TableSaveHelper.FormatValue(row[column]);
            WriteField(writer,
                text,
                separator);
        }
        writer.WriteLine();
    }

    /// <summary>
    /// Writes one CSV field with escaping.
    /// </summary>
    /// <param name="writer">The output writer.</param>
    /// <param name="text">The field text.</param>
    /// <param name="separator">The field delimiter.</param>
    private static void WriteField(TextWriter writer,
        string text,
        char separator)
    {
        if (!string.IsNullOrWhiteSpace(text) && !text.Contains(separator) &&
            !text.Contains('"') && !text.Contains('\r') && !text.Contains('\n'))
        {
            writer.Write(text);
            return;
        }
        writer.Write('"');
        writer.Write(text.Replace("\"",
            "\"\"",
            StringComparison.Ordinal));
        writer.Write('"');
    }

    #endregion
}
