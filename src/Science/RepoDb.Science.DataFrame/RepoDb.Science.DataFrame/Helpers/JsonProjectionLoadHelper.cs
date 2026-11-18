using System.Text.Json;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads watermarked JSON projections.
/// </summary>
internal static class JsonProjectionLoadHelper
{
    #region Public Methods

    /// <summary>
    /// Detects a RepoDB JSON watermark at the root.
    /// </summary>
    /// <param name="stream">The UTF-8 input stream.</param>
    public static DataOrientation? Detect(Stream stream)
    {
        var reader = new JsonTokenReader(stream);
        reader.MoveNext();
        if (reader.Kind != JsonTokenType.StartObject)
        {
            return null;
        }
        var marked = false;
        string? orientation = null;
        foreach (var name in Properties(reader))
        {
            switch (name)
            {
                case "producer":
                    if (reader.Value is not "RepoDB")
                    {
                        return null;
                    }
                    marked = true;
                    break;
                case "orientation":
                    orientation = reader.Value as string;
                    break;
            }
            if (marked && orientation is not null)
            {
                return ProjectionLoadHelper.Orientation(orientation);
            }
            reader.SkipChildren();
        }
        return marked ? ProjectionLoadHelper.Orientation(orientation) : null;
    }

    /// <summary>
    /// Reads an orientation from a watermarked JSON envelope.
    /// </summary>
    /// <param name="stream">The UTF-8 input stream.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="target">The destination or inference target.</param>
    public static void Read(Stream stream,
        DataOrientation orientation,
        ProjectionLoadTarget target)
    {
        var reader = new JsonTokenReader(stream);
        Next(reader);
        var found = false;
        foreach (var property in Properties(reader))
        {
            if (property is "data")
            {
                if (found)
                {
                    throw new FormatException("The RepoDB envelope contains duplicate data properties.");
                }
                ReadProjection(reader,
                    orientation,
                    target);
                found = true;
            }
            else
            {
                reader.SkipChildren();
            }
        }
        if (!found)
        {
            throw new FormatException("The RepoDB envelope requires a data property.");
        }
        reader.MoveNext();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Dispatches the payload by orientation.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadProjection(JsonTokenReader reader,
        DataOrientation orientation,
        ProjectionLoadTarget target)
    {
        switch (orientation)
        {
            case DataOrientation.Records:
                ReadRecords(reader,
                    target);
                break;
            case DataOrientation.Index:
                ReadIndex(reader,
                    target);
                break;
            case DataOrientation.Columns:
                ReadColumns(reader,
                    target);
                break;
            case DataOrientation.Split:
                ReadSplit(reader,
                    target);
                break;
            case DataOrientation.Values:
                ReadValues(reader,
                    target,
                    target.PositionNames,
                    null);
                break;
            case DataOrientation.Table:
                ReadTable(reader,
                    target);
                break;
        default:
            throw new ArgumentOutOfRangeException(nameof(orientation));
        }
    }

    /// <summary>
    /// Reads named records and extracts an optional generated index.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    /// <param name="indexName">The generated index field name.</param>
    private static void ReadRecords(JsonTokenReader reader,
        ProjectionLoadTarget target,
        string? indexName = null)
    {
        foreach (var _ in Elements(reader))
        {
            var values = ReadFields(reader);
            int? index = null;
            if (indexName is not null)
            {
                values.Remove(indexName,
                    out var value);
                index = ReadIndexValue(value);
            }
            target.AddRow(values,
                index);
        }
    }

    /// <summary>
    /// Reads records keyed by their saved positions.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadIndex(JsonTokenReader reader,
        ProjectionLoadTarget target)
    {
        foreach (var key in Properties(reader))
        {
            target.AddRow(ReadFields(reader),
                ProjectionLoadHelper.Index(key));
        }
    }

    /// <summary>
    /// Reads indexed values grouped by column.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadColumns(JsonTokenReader reader,
        ProjectionLoadTarget target)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in Properties(reader))
        {
            RequireUnique(names,
                name);
            target.AddColumn(name);
            var positions = new HashSet<int>();
            foreach (var key in Properties(reader))
            {
                var index = ProjectionLoadHelper.Index(key);
                if (!positions.Add(index))
                {
                    throw new FormatException($"Duplicate position {index} in column '{name}'.");
                }
                target.AddCell(index,
                    name,
                    Scalar(reader));
            }
        }
    }

    /// <summary>
    /// Reads the named columns, index, and positional records.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadSplit(JsonTokenReader reader,
        ProjectionLoadTarget target)
    {
        RequireSection(reader,
            "columns");
        var columns = ReadArray(reader,
            () => RequireText(reader.Value));
        RegisterColumns(target,
            columns);
        RequireSection(reader,
            "index");
        var positions = ReadArray(reader,
            () => ReadIndexValue(reader.Value));
        RequireSection(reader,
            "data");
        ReadValues(reader,
            target,
            columns,
            positions);
        Expect(Next(reader),
            JsonTokenType.EndObject);
    }

    /// <summary>
    /// Reads arrays of positional row values.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    /// <param name="columns">The saved column names or null.</param>
    /// <param name="positions">The saved row positions or null.</param>
    private static void ReadValues(JsonTokenReader reader,
        ProjectionLoadTarget target,
        IReadOnlyList<string>? columns,
        IReadOnlyList<int>? positions)
    {
        var count = 0;
        foreach (var _ in Elements(reader))
        {
            if (positions is not null && count >= positions.Count)
            {
                throw new FormatException("Split contains fewer positions than rows.");
            }
            target.AddPositional(ReadArray(reader,
                    () => Scalar(reader)),
                columns,
                positions?[count]);
            count++;
        }
        if (positions is not null && count != positions.Count)
        {
            throw new FormatException("Split contains more positions than rows.");
        }
    }

    /// <summary>
    /// Reads the schema and records of the Table layout.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadTable(JsonTokenReader reader,
        ProjectionLoadTarget target)
    {
        RequireSection(reader,
            "schema");
        var index = ReadSchema(reader,
            target);
        RequireSection(reader,
            "data");
        ReadRecords(reader,
            target,
            index);
        Expect(Next(reader),
            JsonTokenType.EndObject);
    }

    /// <summary>
    /// Registers schema fields and resolves the generated index name.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static string ReadSchema(JsonTokenReader reader,
        ProjectionLoadTarget target)
    {
        var fields = new Dictionary<string, Type>(StringComparer.Ordinal);
        string? index = null;
        foreach (var property in Properties(reader))
        {
            switch (property)
            {
                case "fields":
                    ReadSchemaFields(reader,
                        fields);
                    break;
                case "primaryKey":
                    var names = ReadArray(reader,
                        () => RequireText(reader.Value));
                    index = names.Count == 1 ? names[0] : null;
                    break;
                default:
                    reader.SkipChildren();
                    break;
            }
        }
        if (index is null || !fields.Remove(index))
        {
            throw new FormatException("Table requires one declared index field.");
        }
        foreach (var (name, type) in fields)
        {
            target.AddColumn(name,
                type);
        }
        return index;
    }

    /// <summary>
    /// Reads the named type of each schema field.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="fields">The schema fields by name.</param>
    private static void ReadSchemaFields(JsonTokenReader reader,
        Dictionary<string, Type> fields)
    {
        foreach (var _ in Elements(reader))
        {
            var field = ReadFields(reader);
            var name = RequireText(field.GetValueOrDefault("name"));
            var type = ProjectionLoadHelper.SchemaType(field.GetValueOrDefault("type") as string,
                field.GetValueOrDefault("extDtype") as string);
            if (!fields.TryAdd(name,
                type))
            {
                throw new FormatException($"Duplicate schema field '{name}'.");
            }
        }
    }

    /// <summary>
    /// Reads a record of named scalar values.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    private static Dictionary<string, object?> ReadFields(JsonTokenReader reader)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var name in Properties(reader))
        {
            if (!values.TryAdd(name,
                Scalar(reader)))
            {
                throw new FormatException($"Duplicate projected field '{name}'.");
            }
        }
        return values;
    }

    /// <summary>
    /// Reads an array without retaining surrounding JSON.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="read">The element reader.</param>
    /// <typeparam name="T">The element type.</typeparam>
    private static List<T> ReadArray<T>(JsonTokenReader reader,
        Func<T> read)
    {
        var values = new List<T>();
        foreach (var _ in Elements(reader))
        {
            values.Add(read());
        }
        return values;
    }

    /// <summary>
    /// Enumerates object properties and leaves each value ready to read.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    private static IEnumerable<string> Properties(JsonTokenReader reader)
    {
        Expect(reader.Kind,
            JsonTokenType.StartObject);
        while (Next(reader) != JsonTokenType.EndObject)
        {
            var name = (string)reader.Value!;
            Next(reader);
            yield return name;
        }
    }

    /// <summary>
    /// Enumerates array elements using the current token reader.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    private static IEnumerable<JsonTokenType> Elements(JsonTokenReader reader)
    {
        Expect(reader.Kind,
            JsonTokenType.StartArray);
        while (Next(reader) != JsonTokenType.EndArray)
        {
            yield return reader.Kind;
        }
    }

    /// <summary>
    /// Moves to a required named layout section.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    /// <param name="name">The expected property name.</param>
    private static void RequireSection(JsonTokenReader reader,
        string name)
    {
        Expect(Next(reader),
            JsonTokenType.PropertyName);
        if (!string.Equals(reader.Value as string,
            name,
            StringComparison.Ordinal))
        {
            throw new FormatException($"Expected the '{name}' projection section.");
        }
        Next(reader);
    }

    /// <summary>
    /// Reads a scalar token.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    private static object? Scalar(JsonTokenReader reader) =>
        reader.Kind is JsonTokenType.StartArray or JsonTokenType.StartObject ?
            throw new FormatException("Projected fields must be scalar values.") : reader.Value;

    /// <summary>
    /// Requires a string in a projected header.
    /// </summary>
    /// <param name="value">The header value.</param>
    private static string RequireText(object? value) =>
        value as string ?? throw new FormatException("Expected a projected column name.");

    /// <summary>
    /// Reads a generated numeric index.
    /// </summary>
    /// <param name="value">The index value.</param>
    private static int ReadIndexValue(object? value) =>
        value is long index and >= 0 and <= int.MaxValue ? (int)index :
            throw new FormatException("Expected a nonnegative numeric row position.");

    /// <summary>
    /// Registers unique named columns.
    /// </summary>
    /// <param name="target">The destination or inference target.</param>
    /// <param name="names">The projected column names.</param>
    private static void RegisterColumns(ProjectionLoadTarget target,
        IEnumerable<string> names)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var name in names)
        {
            RequireUnique(seen,
                name);
            target.AddColumn(name);
        }
    }

    /// <summary>
    /// Rejects duplicate projected column names.
    /// </summary>
    /// <param name="seen">The column names already visited.</param>
    /// <param name="name">The column name.</param>
    private static void RequireUnique(HashSet<string> seen,
        string name)
    {
        if (!seen.Add(name))
        {
            throw new FormatException($"Duplicate projected column '{name}'.");
        }
    }

    /// <summary>
    /// Moves to the next required token.
    /// </summary>
    /// <param name="reader">The JSON token reader.</param>
    private static JsonTokenType Next(JsonTokenReader reader)
    {
        reader.MoveNext();
        return reader.Kind;
    }

    /// <summary>
    /// Checks the current token kind.
    /// </summary>
    /// <param name="actual">The actual token.</param>
    /// <param name="expected">The required token.</param>
    private static void Expect(JsonTokenType actual,
        JsonTokenType expected)
    {
        if (actual != expected)
        {
            throw new FormatException($"Expected {expected} in RepoDB JSON, found {actual}.");
        }
    }

    #endregion
}
