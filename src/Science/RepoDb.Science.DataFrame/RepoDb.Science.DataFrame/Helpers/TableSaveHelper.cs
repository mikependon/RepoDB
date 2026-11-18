using System.Data;
using System.Globalization;
using System.Xml;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Provides shared operations for table writers.
/// </summary>
internal static class TableSaveHelper
{
    #region Public Methods

    /// <summary>
    /// Writes a file before replacing its destination.
    /// </summary>
    /// <param name="path">The destination path.</param>
    /// <param name="write">The stream writer.</param>
    public static void SaveFile(string path,
        Action<Stream> write)
    {
        var destination = Path.GetFullPath(path);
        var temporary = Path.Combine(Path.GetDirectoryName(destination)!,
            Path.GetRandomFileName());
        var stream = new FileStream(temporary,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        var moved = false;
        try
        {
            using (stream)
            {
                write(stream);
            }
            File.Move(temporary,
                destination,
                overwrite: true);
            moved = true;
        }
        finally
        {
            if (!moved)
            {
                File.Delete(temporary);
            }
        }
    }

    /// <summary>
    /// Validates an output layout.
    /// </summary>
    /// <param name="orientation">The selected layout.</param>
    public static void ValidateOrientation(DataOrientation orientation)
    {
        if (orientation is < DataOrientation.Records or > DataOrientation.Table)
        {
            throw new ArgumentOutOfRangeException(nameof(orientation));
        }
    }

    /// <summary>
    /// Finds an index name that does not collide with a column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>The generated index name.</returns>
    public static string IndexName(DataTable table)
    {
        var name = "index";
        while (table.Columns.Contains(name))
        {
            name += "_";
        }
        return name;
    }

    /// <summary>
    /// Returns the schema type for a column.
    /// </summary>
    /// <param name="column">The source column.</param>
    /// <returns>The serialized field type.</returns>
    public static string SchemaType(DataColumn column) =>
        (column.DataType.IsEnum ? TypeCode.Object : Type.GetTypeCode(column.DataType)) switch
        {
            TypeCode.Boolean => "boolean",
            >= TypeCode.SByte and <= TypeCode.UInt64 => "integer",
            TypeCode.Single or TypeCode.Double or TypeCode.Decimal => "number",
            TypeCode.DateTime => "datetime",
            _ when column.DataType == typeof(DateTimeOffset) => "datetime",
            _ when column.DataType == typeof(TimeSpan) => "duration",
            _ => "string"
        };

    /// <summary>
    /// Formats a scalar using invariant conventions.
    /// </summary>
    /// <param name="value">The cell value.</param>
    /// <returns>The formatted scalar.</returns>
    public static string FormatValue(object value) =>
        value switch
        {
            DBNull => string.Empty,
            string text => text,
            char character => character.ToString(),
            bool boolean => XmlConvert.ToString(boolean),
            DateTime date => XmlConvert.ToString(date,
                XmlDateTimeSerializationMode.RoundtripKind),
            DateTimeOffset date => XmlConvert.ToString(date),
            DateOnly date => date.ToString("O",
                CultureInfo.InvariantCulture),
            TimeOnly time => time.ToString("O",
                CultureInfo.InvariantCulture),
            TimeSpan duration => duration.ToString("c",
                CultureInfo.InvariantCulture),
            byte[] bytes => Convert.ToBase64String(bytes),
            IFormattable scalar => scalar.ToString(null,
                CultureInfo.InvariantCulture),
            _ => throw new NotSupportedException($"Cannot save a value of type {value.GetType()}.")
        };

    #endregion
}
