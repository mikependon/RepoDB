using System.Text;
using System.Xml;
using System.Xml.Schema;
using RepoDb.Science.Models;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Selects and projects records from XML.
/// </summary>
internal static class XmlLoadHelper
{
    #region Private Variables

    private static readonly object _repeated = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Reports selected XML records.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="rowElement">The row name or absolute path.</param>
    /// <param name="columns">The output names and scalar paths.</param>
    /// <param name="schemas">The trusted schemas for validation and typing.</param>
    /// <param name="column">The column callback.</param>
    /// <param name="row">The record callback.</param>
    public static void Read(Stream stream,
        string rowElement,
        IReadOnlyDictionary<string, string>? columns,
        XmlSchemaSet? schemas,
        Action<string> column,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        var path = SplitPath(rowElement);
        var mappings = PrepareMappings(columns,
            column);
        using var reader = CreateReader(stream,
            schemas);
        ReadRows(reader,
            path,
            mappings,
            row);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a reader with external resolution disabled.
    /// </summary>
    /// <param name="stream">The input stream.</param>
    /// <param name="schemas">The trusted validation schemas.</param>
    /// <returns>The configured reader.</returns>
    private static XmlReader CreateReader(Stream stream,
        XmlSchemaSet? schemas)
    {
        var settings = new XmlReaderSettings
        {
            CloseInput = false,
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        };
        if (schemas is not null)
        {
            settings.Schemas = schemas;
            settings.ValidationType = ValidationType.Schema;
        }
        return XmlReader.Create(stream,
            settings);
    }

    /// <summary>
    /// Registers output mappings before reading records.
    /// </summary>
    /// <param name="columns">The output names and scalar paths.</param>
    /// <param name="column">The column callback.</param>
    /// <returns>The prepared mappings.</returns>
    private static Dictionary<string, string[]>? PrepareMappings(IReadOnlyDictionary<string, string>? columns,
        Action<string> column)
    {
        if (columns is null)
        {
            return null;
        }
        var mappings = new Dictionary<string, string[]>(StringComparer.Ordinal);
        foreach (var (name, path) in columns)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Column names must be nonempty.",
                    nameof(columns));
            }
            mappings.Add(name,
                SplitPath(path));
            column(name);
        }
        return mappings;
    }

    /// <summary>
    /// Parses a path of XML local names.
    /// </summary>
    /// <param name="path">The element path.</param>
    /// <returns>The local names.</returns>
    private static string[] SplitPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var parts = path.Split('/');
        foreach (var part in parts)
        {
            XmlConvert.VerifyNCName(part);
        }
        return parts;
    }

    /// <summary>
    /// Locates row elements while reading the document forward.
    /// </summary>
    /// <param name="reader">The XML reader.</param>
    /// <param name="path">The selected row path.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <param name="row">The record callback.</param>
    private static void ReadRows(XmlReader reader,
        string[] path,
        Dictionary<string, string[]>? mappings,
        Action<IReadOnlyDictionary<string, object?>> row)
    {
        var names = new List<string>();
        var levels = mappings is { Count: > 0 } ? mappings.Values
            .Max(path => path.Length) : 1;
        while (reader.Read())
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                continue;
            }
            if (names.Count == reader.Depth)
            {
                names.Add(reader.LocalName);
            }
            else
            {
                names[reader.Depth] = reader.LocalName;
            }
            if (Matches(reader,
                path,
                names))
            {
                var value = ReadElement(reader,
                    levels);
                var fields = value as Dictionary<string, object?> ?? new(StringComparer.Ordinal);
                row(Project(fields,
                    mappings));
            }
        }
    }

    /// <summary>
    /// Matches a local name or a path from the document root.
    /// </summary>
    /// <param name="reader">The current element.</param>
    /// <param name="path">The selected path.</param>
    /// <param name="names">The element names by depth.</param>
    /// <returns>Whether the element starts a record.</returns>
    private static bool Matches(XmlReader reader,
        string[] path,
        List<string> names) =>
        path.Length == 1 ? string.Equals(reader.LocalName,
                path[0],
                StringComparison.Ordinal) :
            path.Length == reader.Depth + 1 && names
                .Take(path.Length)
                .SequenceEqual(path,
                    StringComparer.Ordinal);

    /// <summary>
    /// Reads one scalar or object and leaves the reader on its last token.
    /// </summary>
    /// <param name="reader">The current element.</param>
    /// <param name="levels">The remaining levels of projected elements.</param>
    /// <returns>The element value.</returns>
    private static object? ReadElement(XmlReader reader,
        int levels)
    {
        var nil = reader.SchemaInfo?.IsNil == true ||
            reader.GetAttribute("nil",
                XmlSchema.InstanceNamespace) is "true" or "1";
        if (reader.IsEmptyElement)
        {
            return Scalar(reader,
                string.Empty,
                nil);
        }
        return ReadContent(reader,
            levels,
            nil);
    }

    /// <summary>
    /// Reads the content of a nonempty element.
    /// </summary>
    /// <param name="reader">The current element.</param>
    /// <param name="levels">The remaining levels of projected elements.</param>
    /// <param name="nil">Whether the element represents null.</param>
    /// <returns>The element value.</returns>
    private static object? ReadContent(XmlReader reader,
        int levels,
        bool nil)
    {
        Dictionary<string, object?>? fields = null;
        var text = new StringBuilder();
        while (reader.Read() && reader.NodeType != XmlNodeType.EndElement)
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                fields ??= new(StringComparer.Ordinal);
                ReadChild(reader,
                    levels,
                    fields);
            }
            else if (fields is null && IsText(reader.NodeType))
            {
                text.Append(reader.Value);
            }
        }
        return fields is null ? Scalar(reader,
            text.ToString(),
            nil) : fields;
    }

    /// <summary>
    /// Reads a child and tracks repeated names.
    /// </summary>
    /// <param name="reader">The child element.</param>
    /// <param name="levels">The remaining levels of projected elements.</param>
    /// <param name="fields">The fields already read.</param>
    private static void ReadChild(XmlReader reader,
        int levels,
        Dictionary<string, object?> fields)
    {
        if (levels == 0)
        {
            SkipElement(reader);
            return;
        }
        var name = reader.LocalName;
        var value = ReadElement(reader,
            levels - 1);
        if (!fields.TryAdd(name,
            value))
        {
            fields[name] = _repeated;
        }
    }

    /// <summary>
    /// Consumes an unprojected element through its closing token.
    /// </summary>
    /// <param name="reader">The current element.</param>
    private static void SkipElement(XmlReader reader)
    {
        using var subtree = reader.ReadSubtree();
        while (subtree.Read())
        {
        }
    }

    /// <summary>
    /// Identifies XML character data.
    /// </summary>
    /// <param name="kind">The node kind.</param>
    /// <returns>Whether the node contains text.</returns>
    private static bool IsText(XmlNodeType kind) =>
        kind is XmlNodeType.Text or XmlNodeType.CDATA or XmlNodeType.Whitespace or XmlNodeType.SignificantWhitespace;

    /// <summary>
    /// Reads text or a supported scalar declared by an XML schema.
    /// </summary>
    /// <param name="reader">The current element or end element.</param>
    /// <param name="text">The element text.</param>
    /// <param name="nil">Whether the element represents null.</param>
    /// <returns>The scalar value.</returns>
    private static object? Scalar(XmlReader reader,
        string text,
        bool nil)
    {
        var info = reader.SchemaInfo;
        var datatype = (info?.MemberType ?? info?.SchemaType)?.Datatype;
        if (datatype is null || !IsSupported(datatype.ValueType))
        {
            return nil ? null : text;
        }
        if (info?.IsDefault == true)
        {
            text = info.SchemaElement?.FixedValue ?? info.SchemaElement?.DefaultValue ?? text;
        }
        return new XmlLoadValue(datatype.ValueType,
            text,
            nil ? null : datatype.ParseValue(text,
                reader.NameTable,
                reader as IXmlNamespaceResolver));
    }

    /// <summary>
    /// Identifies XML schema types that can be loaded as scalars.
    /// </summary>
    /// <param name="type">The XML schema value type.</param>
    /// <returns>Whether the value can be stored directly.</returns>
    private static bool IsSupported(Type type) =>
        type.IsPrimitive || type == typeof(string) || type == typeof(decimal) ||
            type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(byte[]);

    /// <summary>
    /// Projects explicit mappings or direct scalar fields.
    /// </summary>
    /// <param name="fields">The record fields.</param>
    /// <param name="mappings">The output mappings.</param>
    /// <returns>The projected record.</returns>
    private static Dictionary<string, object?> Project(Dictionary<string, object?> fields,
        Dictionary<string, string[]>? mappings)
    {
        if (mappings is null)
        {
            return fields
                .Where(field => field.Value != _repeated && field.Value is not Dictionary<string, object?>)
                .ToDictionary(field => field.Key,
                    field => field.Value,
                    StringComparer.Ordinal);
        }
        var result = new Dictionary<string, object?>(mappings.Count,
            StringComparer.Ordinal);
        foreach (var (name, path) in mappings)
        {
            result.Add(name,
                Resolve(fields,
                    path));
        }
        return result;
    }

    /// <summary>
    /// Resolves a scalar path relative to a record.
    /// </summary>
    /// <param name="fields">The record fields.</param>
    /// <param name="path">The scalar path.</param>
    /// <returns>The selected scalar or null.</returns>
    private static object? Resolve(Dictionary<string, object?> fields,
        string[] path)
    {
        object? current = fields;
        foreach (var segment in path)
        {
            if (current is not Dictionary<string, object?> children ||
                !children.TryGetValue(segment,
                    out current))
            {
                return null;
            }
            if (current == _repeated)
            {
                throw new XmlException($"XML column path contains a repeating element '{segment}'.");
            }
        }
        return current is Dictionary<string, object?> ?
            throw new XmlException("A mapped XML column must select a scalar.") : current;
    }

    #endregion
}
