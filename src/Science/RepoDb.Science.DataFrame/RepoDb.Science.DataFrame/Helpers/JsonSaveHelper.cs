using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Xml;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes table projections as UTF-8 JSON.
/// </summary>
internal static class JsonSaveHelper
{
    #region Private Variables

    private const int FlushThreshold = 64 * 1024;
    private const string PandasSchemaVersion = "1.4.0";

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes a JSON projection to an open stream.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="orientation">The output layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void Write(DataTable table,
        Stream stream,
        DataOrientation orientation,
        bool indented)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(stream);
        TableSaveHelper.ValidateOrientation(orientation);
        using var writer = new Utf8JsonWriter(stream,
            new JsonWriterOptions { Indented = indented });
        writer.WriteStartObject();
        writer.WriteString("producer",
            "RepoDB");
        writer.WriteString("orientation",
            orientation.ToString());
        writer.WritePropertyName("data");
        WriteProjection(writer,
            table,
            orientation);
        writer.WriteEndObject();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Writes the selected projection.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="orientation">The output layout.</param>
    private static void WriteProjection(Utf8JsonWriter writer,
        DataTable table,
        DataOrientation orientation)
    {
        switch (orientation)
        {
            case DataOrientation.Columns:
                WriteColumns(writer,
                    table);
                break;
            case DataOrientation.Index:
                WriteIndexedRows(writer,
                    table);
                break;
            case DataOrientation.Split:
                WriteSplit(writer,
                    table);
                break;
            case DataOrientation.Table:
                WriteTable(writer,
                    table);
                break;
            default:
                WriteRows(writer,
                    table,
                    positional: orientation == DataOrientation.Values);
                break;
        }
    }

    /// <summary>
    /// Writes column names, row positions, and values separately.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    private static void WriteSplit(Utf8JsonWriter writer,
        DataTable table)
    {
        writer.WriteStartObject();
        writer.WriteStartArray("columns");
        foreach (DataColumn column in table.Columns)
        {
            writer.WriteStringValue(column.ColumnName);
            FlushIfNeeded(writer);
        }
        writer.WriteEndArray();
        writer.WriteStartArray("index");
        var index = 0;
        foreach (var _ in InspectionHelper.Rows(table))
        {
            writer.WriteNumberValue(index++);
            FlushIfNeeded(writer);
        }
        writer.WriteEndArray();
        writer.WritePropertyName("data");
        WriteRows(writer,
            table,
            positional: true);
        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes an object keyed by column names.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    private static void WriteColumns(Utf8JsonWriter writer,
        DataTable table)
    {
        writer.WriteStartObject();
        foreach (DataColumn column in table.Columns)
        {
            writer.WriteStartObject(column.ColumnName);
            var index = 0;
            foreach (var row in InspectionHelper.Rows(table))
            {
                writer.WritePropertyName(IndexText(index++));
                WriteValue(writer,
                    row[column]);
            }
            writer.WriteEndObject();
            FlushIfNeeded(writer);
        }
        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes row objects keyed by position.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    private static void WriteIndexedRows(Utf8JsonWriter writer,
        DataTable table)
    {
        writer.WriteStartObject();
        var index = 0;
        foreach (var row in InspectionHelper.Rows(table))
        {
            writer.WritePropertyName(IndexText(index++));
            WriteRecord(writer,
                row,
                indexName: null,
                index: 0);
        }
        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes schema metadata and named records.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    private static void WriteTable(Utf8JsonWriter writer,
        DataTable table)
    {
        var indexName = TableSaveHelper.IndexName(table);
        writer.WriteStartObject();
        writer.WriteStartObject("schema");
        writer.WriteStartArray("fields");
        WriteSchemaField(writer,
            indexName,
            "integer");
        foreach (DataColumn column in table.Columns)
        {
            WriteSchemaField(writer,
                column.ColumnName,
                TableSaveHelper.SchemaType(column),
                ExtensionType(column));
        }
        writer.WriteEndArray();
        writer.WriteStartArray("primaryKey");
        writer.WriteStringValue(indexName);
        writer.WriteEndArray();
        writer.WriteString("pandas_version",
            PandasSchemaVersion);
        writer.WriteEndObject();
        writer.WritePropertyName("data");
        WriteRows(writer,
            table,
            positional: false,
            indexName);
        writer.WriteEndObject();
    }

    /// <summary>
    /// Writes a field descriptor.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="name">The field name.</param>
    /// <param name="type">The serialized field type.</param>
    /// <param name="extensionType">The nullable pandas dtype, if needed.</param>
    private static void WriteSchemaField(Utf8JsonWriter writer,
        string name,
        string type,
        string? extensionType = null)
    {
        writer.WriteStartObject();
        writer.WriteString("name",
            name);
        writer.WriteString("type",
            type);
        if (extensionType is not null)
        {
            writer.WriteString("extDtype",
                extensionType);
        }
        writer.WriteEndObject();
        FlushIfNeeded(writer);
    }

    /// <summary>
    /// Maps nullable integer and boolean columns to pandas dtypes.
    /// </summary>
    /// <param name="column">The source column.</param>
    /// <returns>The nullable dtype or null.</returns>
    private static string? ExtensionType(DataColumn column)
    {
        if (!column.AllowDBNull || column.DataType.IsEnum)
        {
            return null;
        }
        return Type.GetTypeCode(column.DataType) switch
        {
            TypeCode.Boolean => "boolean",
            TypeCode.SByte => "Int8",
            TypeCode.Byte => "UInt8",
            >= TypeCode.Int16 and <= TypeCode.UInt64 => column.DataType.Name,
            _ => null
        };
    }

    /// <summary>
    /// Writes the current rows as an array.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="table">The source table.</param>
    /// <param name="positional">Whether to write arrays of values.</param>
    /// <param name="indexName">The generated index name.</param>
    private static void WriteRows(Utf8JsonWriter writer,
        DataTable table,
        bool positional,
        string? indexName = null)
    {
        writer.WriteStartArray();
        var index = 0;
        foreach (var row in InspectionHelper.Rows(table))
        {
            if (positional)
            {
                WritePositionalRow(writer,
                    row);
            }
            else
            {
                WriteRecord(writer,
                    row,
                    indexName,
                    index++);
            }
            FlushIfNeeded(writer);
        }
        writer.WriteEndArray();
    }

    /// <summary>
    /// Writes one row as a positional array.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="row">The source row.</param>
    private static void WritePositionalRow(Utf8JsonWriter writer,
        DataRow row)
    {
        writer.WriteStartArray();
        foreach (DataColumn column in row.Table.Columns)
        {
            WriteValue(writer,
                row[column]);
        }
        writer.WriteEndArray();
    }

    /// <summary>
    /// Writes one row as a named object.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="row">The source row.</param>
    /// <param name="indexName">The generated index name.</param>
    /// <param name="index">The row position.</param>
    private static void WriteRecord(Utf8JsonWriter writer,
        DataRow row,
        string? indexName,
        int index)
    {
        writer.WriteStartObject();
        if (indexName is not null)
        {
            writer.WriteNumber(indexName,
                index);
        }
        foreach (DataColumn column in row.Table.Columns)
        {
            writer.WritePropertyName(column.ColumnName);
            WriteValue(writer,
                row[column]);
        }
        writer.WriteEndObject();
        FlushIfNeeded(writer);
    }

    /// <summary>
    /// Writes a scalar without reflection.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The cell value.</param>
    private static void WriteValue(Utf8JsonWriter writer,
        object value)
    {
        switch (value)
        {
            case DBNull:
                writer.WriteNullValue();
                break;
            case bool boolean:
                writer.WriteBooleanValue(boolean);
                break;
            case sbyte or short or int or long:
                writer.WriteNumberValue(Convert.ToInt64(value,
                    CultureInfo.InvariantCulture));
                break;
            case byte or ushort or uint or ulong:
                writer.WriteNumberValue(Convert.ToUInt64(value,
                    CultureInfo.InvariantCulture));
                break;
            case decimal number:
                writer.WriteNumberValue(number);
                break;
            case float number:
                writer.WriteNumberValue(number);
                break;
            case double number:
                writer.WriteNumberValue(number);
                break;
            case TimeSpan duration:
                writer.WriteStringValue(XmlConvert.ToString(duration));
                break;
            default:
                writer.WriteStringValue(TableSaveHelper.FormatValue(value));
                break;
        }
        FlushIfNeeded(writer);
    }

    /// <summary>
    /// Limits the buffer retained by the streaming writer.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    private static void FlushIfNeeded(Utf8JsonWriter writer)
    {
        if (writer.BytesPending >= FlushThreshold)
        {
            writer.Flush();
        }
    }

    /// <summary>
    /// Formats an index as a JSON property name.
    /// </summary>
    /// <param name="index">The row position.</param>
    private static string IndexText(int index) => index.ToString(CultureInfo.InvariantCulture);

    #endregion
}
