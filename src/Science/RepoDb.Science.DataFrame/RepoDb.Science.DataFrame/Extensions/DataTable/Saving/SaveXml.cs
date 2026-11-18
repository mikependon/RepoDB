using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Saves tables as XML.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Saves the table to a XML file.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="path">The output file.</param>
    /// <param name="rootElement">The root element name.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="orientation">The output layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void SaveXml(this DataTable table,
        string path,
        string rootElement = "rows",
        string rowElement = "row",
        DataOrientation orientation = DataOrientation.Records,
        bool indented = false) =>
        TableSaveHelper.SaveFile(path,
            stream => table.SaveXml(stream,
                rootElement,
                rowElement,
                orientation,
                indented));

    /// <summary>
    /// Writes XML and leaves the stream open.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="rootElement">The root element name.</param>
    /// <param name="rowElement">The row element name.</param>
    /// <param name="orientation">The output layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void SaveXml(this DataTable table,
        Stream stream,
        string rootElement = "rows",
        string rowElement = "row",
        DataOrientation orientation = DataOrientation.Records,
        bool indented = false) =>
        XmlSaveHelper.Write(table,
            stream,
            rootElement,
            rowElement,
            orientation,
            indented);

    #endregion
}
