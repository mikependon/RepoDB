using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads CSV records with a header.
/// </summary>
internal static class CsvLoadHelper
{
    #region Public Methods

    /// <summary>
    /// Reports columns and records from a CSV stream.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="separator">The field delimiter.</param>
    /// <param name="column">The column callback.</param>
    /// <param name="row">The record callback.</param>
    public static void Read(Stream stream,
        char separator,
        Action<string> column,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        if (separator is '\r' or '\n' or '"' or '\0')
        {
            throw new ArgumentOutOfRangeException(nameof(separator));
        }

        using var parser = new TextFieldParser(stream,
            new UTF8Encoding(false,
                true),
            detectEncoding: true,
            leaveOpen: true) { TrimWhiteSpace = false };
        parser.SetDelimiters(separator.ToString());
        var header = ReadFields(parser) ?? throw new FormatException("CSV requires a header.");
        ReadHeader(header,
            column);
        ReadRows(parser,
            header,
            row);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates and reports the header.
    /// </summary>
    /// <param name="header">The column names.</param>
    /// <param name="column">The column callback.</param>
    private static void ReadHeader(string[] header,
        Action<string> column)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in header)
        {
            if (string.IsNullOrWhiteSpace(name) || !names.Add(name))
            {
                throw new FormatException($"Invalid or duplicate CSV header '{name}'.");
            }
            column(name);
        }
    }

    /// <summary>
    /// Reports each CSV record.
    /// </summary>
    /// <param name="parser">The field parser.</param>
    /// <param name="header">The column names.</param>
    /// <param name="row">The record callback.</param>
    private static void ReadRows(TextFieldParser parser,
        string[] header,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        var number = 0L;
        var values = new Dictionary<string, object?>(header.Length,
            StringComparer.Ordinal);
        while (ReadFields(parser) is { } fields)
        {
            number++;
            if (fields.Length != header.Length)
            {
                throw new FormatException($"CSV data row {number} has {fields.Length} fields; expected {header.Length}.");
            }
            for (var index = 0; index < header.Length; index++)
            {
                values[header[index]] = fields[index];
            }
            row(values);
        }
    }

    /// <summary>
    /// Reads fields with a consistent format error.
    /// </summary>
    /// <param name="parser">The field parser.</param>
    /// <returns>The fields or end of input.</returns>
    private static string[]? ReadFields(TextFieldParser parser)
    {
        try
        {
            return parser.ReadFields();
        }
        catch (MalformedLineException exception)
        {
            throw new FormatException($"Invalid CSV record at line {exception.LineNumber}.",
                exception);
        }
    }

    #endregion
}
