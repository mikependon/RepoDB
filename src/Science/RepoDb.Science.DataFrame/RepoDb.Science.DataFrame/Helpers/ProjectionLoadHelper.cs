using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Dispatches watermarked files and materializes projected values.
/// </summary>
internal static class ProjectionLoadHelper
{
    #region Private Variables

    private static readonly Dictionary<string, Type> _integerTypes = new(StringComparer.Ordinal)
    {
        ["Int8"] = typeof(sbyte),
        ["UInt8"] = typeof(byte),
        ["Int16"] = typeof(short),
        ["UInt16"] = typeof(ushort),
        ["Int32"] = typeof(int),
        ["UInt32"] = typeof(uint),
        ["UInt64"] = typeof(ulong)
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Detects the watermark and selects the projected or flat reader.
    /// </summary>
    /// <param name="table">The caller's schema.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="detect">The watermark reader.</param>
    /// <param name="read">The projection reader.</param>
    /// <param name="flat">The legacy flat loader.</param>
    /// <param name="inferText">Whether to infer scalar types from plain text.</param>
    public static DataTable Load(DataTable table,
        Stream stream,
        Func<Stream, DataOrientation?> detect,
        Action<Stream, DataOrientation, ProjectionLoadTarget> read,
        Func<Stream, DataTable> flat,
        bool inferText = false)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(stream);
        if (stream.CanSeek)
        {
            return Read(table,
                stream,
                detect,
                read,
                flat,
                inferText);
        }
        using var copy = TableLoadHelper.CreateReplayStream();
        stream.CopyTo(copy);
        copy.Position = 0;
        return Read(table,
            copy,
            detect,
            read,
            flat,
            inferText);
    }

    /// <summary>
    /// Parses a saved orientation name.
    /// </summary>
    /// <param name="name">The exact orientation name.</param>
    public static DataOrientation Orientation(string? name) =>
        name switch
        {
            "Records" => DataOrientation.Records,
            "Split" => DataOrientation.Split,
            "Index" => DataOrientation.Index,
            "Columns" => DataOrientation.Columns,
            "Values" => DataOrientation.Values,
            "Table" => DataOrientation.Table,
            _ => throw new FormatException($"Missing or unsupported RepoDB orientation '{name}'.")
        };

    /// <summary>
    /// Parses a nonnegative saved row position.
    /// </summary>
    /// <param name="text">The serialized index.</param>
    public static int Index(string? text) =>
        int.TryParse(text,
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var index) ? index : throw new FormatException($"Invalid row position '{text}'.");

    /// <summary>
    /// Resolves a field type from the saved schema.
    /// </summary>
    /// <param name="type">The schema type.</param>
    /// <param name="extension">The optional pandas extension dtype.</param>
    public static Type SchemaType(string? type,
        string? extension = null) =>
        type switch
        {
            "integer" => IntegerType(extension),
            "boolean" => typeof(bool),
            "number" => typeof(decimal),
            "datetime" => typeof(DateTime),
            "duration" => typeof(TimeSpan),
            "string" => typeof(string),
            _ => throw new FormatException($"Unsupported field type '{type}'.")
        };

    #endregion

    #region Private Methods

    /// <summary>
    /// Resolves a pandas integer width.
    /// </summary>
    /// <param name="extension">The optional integer extension dtype.</param>
    /// <returns>The declared integer type.</returns>
    private static Type IntegerType(string? extension) =>
        extension is null ? typeof(long) : _integerTypes.GetValueOrDefault(extension) ?? typeof(long);

    /// <summary>
    /// Rewinds after detection and loads the selected layout.
    /// </summary>
    /// <param name="table">The caller's schema.</param>
    /// <param name="stream">The seekable stream.</param>
    /// <param name="detect">The watermark reader.</param>
    /// <param name="read">The projection reader.</param>
    /// <param name="flat">The legacy loader.</param>
    /// <param name="inferText">Whether to infer text types.</param>
    private static DataTable Read(DataTable table,
        Stream stream,
        Func<Stream, DataOrientation?> detect,
        Action<Stream, DataOrientation, ProjectionLoadTarget> read,
        Func<Stream, DataTable> flat,
        bool inferText)
    {
        var start = stream.Position;
        var orientation = detect(stream);
        stream.Position = start;
        return orientation is null ? flat(stream) : Fill(table,
            stream,
            orientation.Value,
            read,
            inferText);
    }

    /// <summary>
    /// Discovers the schema if necessary and fills a new table.
    /// </summary>
    /// <param name="table">The caller's schema.</param>
    /// <param name="stream">The seekable stream.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="read">The projection reader.</param>
    /// <param name="inferText">Whether to infer text types.</param>
    private static DataTable Fill(DataTable table,
        Stream stream,
        DataOrientation orientation,
        Action<Stream, DataOrientation, ProjectionLoadTarget> read,
        bool inferText)
    {
        var result = table.Clone();
        string[]? positionNames = result.Columns.Count == 0 ? null : result.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToArray();
        if (positionNames is null)
        {
            var start = stream.Position;
            var inference = new ProjectionLoadTarget(null,
                inferText,
                null);
            read(stream,
                orientation,
                inference);
            inference.ApplySchema(result);
            stream.Position = start;
        }
        var target = new ProjectionLoadTarget(result,
            inferText,
            positionNames);
        result.BeginLoadData();
        read(stream,
            orientation,
            target);
        target.Complete();
        result.EndLoadData();
        return result;
    }

    #endregion
}
