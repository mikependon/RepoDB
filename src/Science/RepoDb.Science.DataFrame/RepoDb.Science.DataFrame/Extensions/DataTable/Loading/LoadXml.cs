using System.Data;
using System.Xml.Schema;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Loads records from XML.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Loads selected row elements or a RepoDB projection from XML.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="path">The input file.</param>
    /// <param name="rowElement">A local element name or slash-separated path from the root.</param>
    /// <param name="columns">The optional output names mapped to slash-separated scalar paths.</param>
    /// <param name="inferTypes">Whether to infer types from text for an empty schema.</param>
    /// <param name="schemas">The optional trusted XML schemas.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadXml(this DataTable table,
        string path,
        string rowElement = "row",
        IReadOnlyDictionary<string, string>? columns = null,
        bool inferTypes = false,
        XmlSchemaSet? schemas = null)
    {
        using var stream = File.OpenRead(path);
        return table.LoadXml(stream,
            rowElement,
            columns,
            inferTypes,
            schemas);
    }

    /// <summary>
    /// Loads XML records and leaves the stream open.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="rowElement">A local element name or slash-separated path from the root.</param>
    /// <param name="columns">The optional output names mapped to slash-separated scalar paths.</param>
    /// <param name="inferTypes">Whether to infer types from text for an empty schema.</param>
    /// <param name="schemas">The optional trusted XML schemas.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadXml(this DataTable table,
        Stream stream,
        string rowElement = "row",
        IReadOnlyDictionary<string, string>? columns = null,
        bool inferTypes = false,
        XmlSchemaSet? schemas = null)
    {
        DataTable LoadFlat(Stream input) => TableLoadHelper.Load(table,
            input,
            inferTypes || schemas is not null,
            (source, column, row) => XmlLoadHelper.Read(source,
                rowElement,
                columns,
                schemas,
                column,
                row),
            inferText: inferTypes);

        return columns is not null || schemas is not null ? LoadFlat(stream) :
            ProjectionLoadHelper.Load(table,
                stream,
                XmlProjectionLoadHelper.Detect,
                XmlProjectionLoadHelper.Read,
                LoadFlat,
                inferText: inferTypes);
    }

    #endregion
}
