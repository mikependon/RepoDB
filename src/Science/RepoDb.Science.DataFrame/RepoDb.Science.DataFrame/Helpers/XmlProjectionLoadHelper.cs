using System.Xml;
using System.Xml.Schema;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads watermarked XML projections.
/// </summary>
internal static class XmlProjectionLoadHelper
{
    #region Public Methods

    /// <summary>
    /// Detects a RepoDB XML watermark on the root element.
    /// </summary>
    /// <param name="stream">The XML input stream.</param>
    public static DataOrientation? Detect(Stream stream)
    {
        using var reader = CreateReader(stream);
        reader.MoveToContent();
        return reader.GetAttribute("producer") is "RepoDB" ?
            ProjectionLoadHelper.Orientation(reader.GetAttribute("orientation")) : null;
    }

    /// <summary>
    /// Reads the selected projection from the root element.
    /// </summary>
    /// <param name="stream">The XML input stream.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="target">The destination or inference target.</param>
    public static void Read(Stream stream,
        DataOrientation orientation,
        ProjectionLoadTarget target)
    {
        using var reader = CreateReader(stream);
        reader.MoveToContent();
        switch (orientation)
        {
            case DataOrientation.Split:
                ReadSplit(reader,
                    target);
                break;
            case DataOrientation.Columns:
                ReadColumns(reader,
                    target);
                break;
            case DataOrientation.Table:
                ReadTable(reader,
                    target);
                break;
            default:
                ReadRows(reader,
                    orientation,
                    target,
                    orientation == DataOrientation.Index ? "index" : null);
                break;
        }
        reader.Read();
        reader.MoveToContent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates an XML reader with DTDs disabled.
    /// </summary>
    /// <param name="stream">The XML input stream.</param>
    private static XmlReader CreateReader(Stream stream) =>
        XmlReader.Create(stream,
            new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });

    /// <summary>
    /// Reads current records in document or saved index order.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="orientation">The selected layout.</param>
    /// <param name="target">The destination or inference target.</param>
    /// <param name="indexName">The saved index attribute name, if any.</param>
    private static void ReadRows(XmlReader reader,
        DataOrientation orientation,
        ProjectionLoadTarget target,
        string? indexName)
    {
        foreach (var row in Elements(reader))
        {
            if (orientation == DataOrientation.Values)
            {
                target.AddPositional(ReadValues(row),
                    target.PositionNames);
            }
            else
            {
                var index = indexName is null ? (int?)null :
                    ProjectionLoadHelper.Index(row.GetAttribute(indexName));
                target.AddRow(ReadFields(row),
                    index);
            }
        }
    }

    /// <summary>
    /// Reads named row fields.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    private static Dictionary<string, object?> ReadFields(XmlReader reader)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var field in Elements(reader))
        {
            var name = field.LocalName;
            if (!values.TryAdd(name,
                Scalar(field)))
            {
                throw new FormatException($"Duplicate projected field '{name}'.");
            }
        }
        return values;
    }

    /// <summary>
    /// Reads the column names, row index, and positional data.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadSplit(XmlReader reader,
        ProjectionLoadTarget target)
    {
        using var sections = Elements(reader)
            .GetEnumerator();
        var columns = ReadColumnNames(Section(sections,
                "columns"),
            target);
        var positions = Elements(Section(sections,
                "index"))
            .Select(element => ProjectionLoadHelper.Index(Scalar(element)))
            .ToList();
        ReadSplitRows(Section(sections,
                "data"),
            target,
            columns,
            positions);
        EndSections(sections);
    }

    /// <summary>
    /// Registers the Split column names in their saved order.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static List<string> ReadColumnNames(XmlReader reader,
        ProjectionLoadTarget target)
    {
        var columns = new List<string>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var field in Elements(reader))
        {
            var name = Required(Scalar(field),
                "column name");
            RequireUnique(names,
                name);
            target.AddColumn(name);
            columns.Add(name);
        }
        return columns;
    }

    /// <summary>
    /// Matches Split records to their saved positions.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    /// <param name="columns">The saved column names.</param>
    /// <param name="positions">The saved row positions.</param>
    private static void ReadSplitRows(XmlReader reader,
        ProjectionLoadTarget target,
        IReadOnlyList<string> columns,
        IReadOnlyList<int> positions)
    {
        var count = 0;
        foreach (var row in Elements(reader))
        {
            if (count >= positions.Count)
            {
                throw new FormatException("Split contains fewer positions than rows.");
            }
            target.AddPositional(ReadValues(row),
                columns,
                positions[count++]);
        }
        if (count != positions.Count)
        {
            throw new FormatException("Split contains more positions than rows.");
        }
    }

    /// <summary>
    /// Reads one positional row.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    private static List<object?> ReadValues(XmlReader reader) =>
        Elements(reader)
            .Select(element => (object?)Scalar(element))
            .ToList();

    /// <summary>
    /// Reads each indexed cell in a column-oriented projection.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadColumns(XmlReader reader,
        ProjectionLoadTarget target)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var column in Elements(reader))
        {
            var name = Required(column.GetAttribute("name"),
                "column name");
            RequireUnique(names,
                name);
            target.AddColumn(name);
            var positions = new HashSet<int>();
            foreach (var cell in Elements(column))
            {
                var index = ProjectionLoadHelper.Index(cell.GetAttribute("index"));
                if (!positions.Add(index))
                {
                    throw new FormatException($"Duplicate position {index} in column '{name}'.");
                }
                target.AddCell(index,
                    name,
                    Scalar(cell));
            }
        }
    }

    /// <summary>
    /// Reads the Table schema and its named records.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static void ReadTable(XmlReader reader,
        ProjectionLoadTarget target)
    {
        using var sections = Elements(reader)
            .GetEnumerator();
        var index = ReadSchema(Section(sections,
                "schema"),
            target);
        ReadRows(Section(sections,
                "data"),
            DataOrientation.Table,
            target,
            index);
        EndSections(sections);
    }

    /// <summary>
    /// Registers schema fields and returns the generated index name.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="target">The destination or inference target.</param>
    private static string ReadSchema(XmlReader reader,
        ProjectionLoadTarget target)
    {
        var fields = new Dictionary<string, Type>(StringComparer.Ordinal);
        string? index = null;
        foreach (var field in Elements(reader))
        {
            if (field.LocalName is "primaryKey")
            {
                index = Scalar(field);
            }
            else
            {
                ReadSchemaField(field,
                    fields);
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
    /// Reads a named schema field and its declared type.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    /// <param name="fields">The schema fields by name.</param>
    private static void ReadSchemaField(XmlReader reader,
        Dictionary<string, Type> fields)
    {
        var name = Required(reader.GetAttribute("name"),
            "field name");
        var type = ProjectionLoadHelper.SchemaType(reader.GetAttribute("type"));
        if (!fields.TryAdd(name,
            type))
        {
            throw new FormatException($"Duplicate schema field '{name}'.");
        }
    }

    /// <summary>
    /// Yields direct child elements without buffering their contents.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    private static IEnumerable<XmlReader> Elements(XmlReader reader)
    {
        using var parent = reader.ReadSubtree();
        parent.Read();
        while (parent.Read())
        {
            if (parent.NodeType == XmlNodeType.Element && parent.Depth == 1)
            {
                using var child = parent.ReadSubtree();
                child.Read();
                yield return child;
            }
        }
    }

    /// <summary>
    /// Selects the next required layout section.
    /// </summary>
    /// <param name="sections">The current section iterator.</param>
    /// <param name="name">The required section name.</param>
    private static XmlReader Section(IEnumerator<XmlReader> sections,
        string name)
    {
        if (!sections.MoveNext() || !string.Equals(sections.Current.LocalName,
            name,
            StringComparison.Ordinal))
        {
            throw new FormatException($"Expected the '{name}' projection section.");
        }
        return sections.Current;
    }

    /// <summary>
    /// Checks that all layout sections have been read.
    /// </summary>
    /// <param name="sections">The current section iterator.</param>
    private static void EndSections(IEnumerator<XmlReader> sections)
    {
        if (sections.MoveNext())
        {
            throw new FormatException("Unexpected section in the RepoDB projection.");
        }
    }

    /// <summary>
    /// Reads an XML field as scalar text or null.
    /// </summary>
    /// <param name="reader">The XML element reader.</param>
    private static string? Scalar(XmlReader reader)
    {
        var nil = reader.GetAttribute("nil",
            XmlSchema.InstanceNamespace) is "true" or "1";
        var text = reader.ReadElementContentAsString();
        return nil ? null : text;
    }

    /// <summary>
    /// Requires a non-null metadata value.
    /// </summary>
    /// <param name="text">The metadata value.</param>
    /// <param name="role">The expected value description.</param>
    private static string Required(string? text,
        string role) =>
        text ?? throw new FormatException($"Missing projected {role}.");

    /// <summary>
    /// Rejects duplicate column names.
    /// </summary>
    /// <param name="names">The column names already visited.</param>
    /// <param name="name">The new column name.</param>
    private static void RequireUnique(HashSet<string> names,
        string name)
    {
        if (!names.Add(name))
        {
            throw new FormatException($"Duplicate projected column '{name}'.");
        }
    }

    #endregion
}
