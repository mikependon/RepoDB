using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Saves tables as JSON.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Saves the table to a JSON file.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="path">The output file.</param>
    /// <param name="orientation">The output layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void SaveJson(this DataTable table,
        string path,
        DataOrientation orientation = DataOrientation.Records,
        bool indented = false) =>
        TableSaveHelper.SaveFile(path,
            stream => table.SaveJson(stream,
                orientation,
                indented));

    /// <summary>
    /// Writes JSON and leaves the stream open.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="orientation">The output layout.</param>
    /// <param name="indented">Whether to indent the output.</param>
    public static void SaveJson(this DataTable table,
        Stream stream,
        DataOrientation orientation = DataOrientation.Records,
        bool indented = false) =>
        JsonSaveHelper.Write(table,
            stream,
            orientation,
            indented);

    #endregion
}
