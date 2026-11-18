using System.Data;
using System.Globalization;
using RepoDb.Science.Models;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Discovers column types and materializes streamed records.
/// </summary>
internal static class TableLoadHelper
{
    #region Public Methods

    /// <summary>
    /// Loads records using supplied or discovered columns.
    /// </summary>
    /// <param name="table">The optional schema.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="inferTypes">Whether to scan for types.</param>
    /// <param name="read">The format reader.</param>
    /// <param name="inferText">Whether to infer types from text.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable Load(DataTable table,
        Stream stream,
        bool inferTypes,
        Action<Stream, Action<string>, Action<IReadOnlyDictionary<string, object?>>> read,
        bool inferText = false)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(stream);
        var result = table.Clone();
        if (result.Columns.Count != 0 || !inferTypes)
        {
            Fill(result,
                stream,
                read,
                discoverColumns: result.Columns.Count == 0);
            return result;
        }

        if (stream.CanSeek)
        {
            InferAndFill(result,
                stream,
                read,
                inferText);
            return result;
        }

        using var copy = CreateReplayStream();
        stream.CopyTo(copy);
        copy.Position = 0;
        InferAndFill(result,
            copy,
            read,
            inferText);
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Opens temporary storage for a non-seekable input.
    /// </summary>
    /// <returns>A stream that deletes its file when closed.</returns>
    private static FileStream CreateReplayStream() =>
        new(Path.Combine(Path.GetTempPath(),
                Path.GetRandomFileName()),
            FileMode.CreateNew,
            FileAccess.ReadWrite,
            FileShare.None,
            4096,
            FileOptions.DeleteOnClose | FileOptions.SequentialScan);

    /// <summary>
    /// Scans the input, then reloads it with a fixed schema.
    /// </summary>
    /// <param name="table">The destination table.</param>
    /// <param name="stream">The seekable input.</param>
    /// <param name="read">The format reader.</param>
    /// <param name="inferText">Whether to infer types from text.</param>
    private static void InferAndFill(DataTable table,
        Stream stream,
        Action<Stream, Action<string>, Action<IReadOnlyDictionary<string, object?>>> read,
        bool inferText)
    {
        var start = stream.Position;
        var types = new Dictionary<string, Type?>(StringComparer.Ordinal);
        read(stream,
            name => types.TryAdd(name,
                null),
            values => ObserveTypes(types,
                values,
                inferText));
        foreach (var (name, type) in types)
        {
            TableHelper.AddColumn(table,
                name,
                type ?? typeof(string));
        }
        stream.Position = start;
        Fill(table,
            stream,
            read,
            discoverColumns: false);
    }

    /// <summary>
    /// Updates candidate types from one record.
    /// </summary>
    /// <param name="types">The candidate types by column name.</param>
    /// <param name="values">The record values.</param>
    /// <param name="inferText">Whether to infer types from text.</param>
    private static void ObserveTypes(Dictionary<string, Type?> types,
        IReadOnlyDictionary<string, object?> values,
        bool inferText)
    {
        foreach (var (name, value) in values)
        {
            types.TryGetValue(name,
                out var previous);
            var current = value switch
            {
                XmlLoadValue xml => xml.Type,
                string text when inferText => TextType(text),
                _ => value?.GetType()
            };
            types[name] = CombineTypes(previous,
                current);
        }
    }

    /// <summary>
    /// Finds a compatible type for two candidates.
    /// </summary>
    /// <param name="previous">The earlier type.</param>
    /// <param name="current">The new type.</param>
    /// <returns>The combined type.</returns>
    private static Type? CombineTypes(Type? previous,
        Type? current)
    {
        if (previous is null || current is null || previous == current)
        {
            return previous ?? current;
        }
        if ((previous == typeof(long) && current == typeof(decimal)) ||
            (previous == typeof(decimal) && current == typeof(long)))
        {
            return typeof(decimal);
        }
        return typeof(string);
    }

    /// <summary>
    /// Identifies an invariant scalar type in text.
    /// </summary>
    /// <param name="text">The field text.</param>
    /// <returns>The inferred type.</returns>
    private static Type TextType(string text)
    {
        if (bool.TryParse(text,
            out _))
        {
            return typeof(bool);
        }
        var unsigned = text.AsSpan();
        if (!unsigned.IsEmpty && unsigned[0] is '+' or '-')
        {
            unsigned = unsigned[1..];
        }
        if (unsigned.Length > 1 && unsigned[0] == '0' && char.IsAsciiDigit(unsigned[1]))
        {
            return typeof(string);
        }
        if (long.TryParse(text,
            NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture,
            out _))
        {
            return typeof(long);
        }
        return IsExactDecimal(unsigned,
            text) ? typeof(decimal) : typeof(string);
    }

    /// <summary>
    /// Checks whether text represents an exact invariant decimal.
    /// </summary>
    /// <param name="unsigned">The text without a sign.</param>
    /// <param name="text">The original text.</param>
    /// <returns>Whether the value fits with its full precision.</returns>
    private static bool IsExactDecimal(ReadOnlySpan<char> unsigned,
        string text)
    {
        var point = unsigned.IndexOf('.');
        var scale = point < 0 ? 0 : unsigned[(point + 1)..]
            .TrimEnd('0')
            .Length;
        return decimal.TryParse(text,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out var number) &&
            scale <= number.Scale;
    }

    /// <summary>
    /// Loads records with deferred index maintenance and validation.
    /// </summary>
    /// <param name="table">The destination table.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="read">The format reader.</param>
    /// <param name="discoverColumns">Whether new columns can be added.</param>
    private static void Fill(DataTable table,
        Stream stream,
        Action<Stream, Action<string>, Action<IReadOnlyDictionary<string, object?>>> read,
        bool discoverColumns)
    {
        var number = 0L;
        var columns = table.Columns
            .Cast<DataColumn>()
            .ToDictionary(column => column.ColumnName,
                StringComparer.Ordinal);
        Action<string> addColumn = discoverColumns ? name => AddColumn(table,
            columns,
            name) : _ => { };

        table.BeginLoadData();
        read(stream,
            addColumn,
            values =>
            {
                if (discoverColumns)
                {
                    foreach (var name in values.Keys)
                    {
                        addColumn(name);
                    }
                }
                AddRow(table,
                    columns.Values,
                    values,
                    ++number);
            });
        table.EndLoadData();
    }

    /// <summary>
    /// Registers a discovered text column.
    /// </summary>
    /// <param name="table">The destination table.</param>
    /// <param name="columns">The registered columns.</param>
    /// <param name="name">The column name.</param>
    private static void AddColumn(DataTable table,
        Dictionary<string, DataColumn> columns,
        string name)
    {
        if (!columns.ContainsKey(name))
        {
            columns.Add(name,
                TableHelper.AddColumn(table,
                    name,
                    typeof(string)));
        }
    }

    /// <summary>
    /// Converts and inserts a record.
    /// </summary>
    /// <param name="table">The destination table.</param>
    /// <param name="columns">The destination columns.</param>
    /// <param name="values">The input fields.</param>
    /// <param name="number">The record number.</param>
    private static void AddRow(DataTable table,
        IEnumerable<DataColumn> columns,
        IReadOnlyDictionary<string, object?> values,
        long number)
    {
        var row = table.NewRow();
        foreach (var column in columns)
        {
            if (column.Expression.Length == 0)
            {
                values.TryGetValue(column.ColumnName,
                    out var value);
                SetValue(row,
                    column,
                    value,
                    number);
            }
        }
        table.Rows.Add(row);
    }

    /// <summary>
    /// Assigns one field and reports conversion failures.
    /// </summary>
    /// <param name="row">The destination row.</param>
    /// <param name="column">The destination column.</param>
    /// <param name="value">The source value.</param>
    /// <param name="number">The record number.</param>
    private static void SetValue(DataRow row,
        DataColumn column,
        object? value,
        long number)
    {
        try
        {
            row[column] = ConvertValue(value,
                column.DataType);
        }
        catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException or ArgumentException)
        {
            throw new FormatException($"Cannot load row {number}, column '{column.ColumnName}' as {column.DataType.Name}.",
                exception);
        }
    }

    /// <summary>
    /// Converts a scalar using invariant culture.
    /// </summary>
    /// <param name="value">The source scalar.</param>
    /// <param name="type">The destination type.</param>
    /// <returns>The converted value.</returns>
    private static object ConvertValue(object? value,
        Type type)
    {
        if (value is XmlLoadValue xml)
        {
            value = xml.Value is null || type == xml.Type ? xml.Value : xml.Text;
        }
        if (value is null)
        {
            return DBNull.Value;
        }
        if (type.IsInstanceOfType(value))
        {
            return value;
        }
        if (value is string text)
        {
            return ConvertText(text,
                type);
        }
        if (value is decimal number && decimal.Truncate(number) != number &&
            Type.GetTypeCode(type) is >= TypeCode.SByte and <= TypeCode.UInt64)
        {
            throw new FormatException("An integer column cannot store a fractional value.");
        }
        return Convert.ChangeType(value,
            type,
            CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a text field to its destination type.
    /// </summary>
    /// <param name="text">The input text.</param>
    /// <param name="type">The destination type.</param>
    /// <returns>The converted value.</returns>
    private static object ConvertText(string text,
        Type type)
    {
        if (type == typeof(Guid))
        {
            return Guid.Parse(text);
        }
        if (type == typeof(TimeSpan))
        {
            return TimeSpan.Parse(text,
                CultureInfo.InvariantCulture);
        }
        if (type == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(text,
                CultureInfo.InvariantCulture);
        }
        return Convert.ChangeType(text,
            type,
            CultureInfo.InvariantCulture);
    }

    #endregion
}
