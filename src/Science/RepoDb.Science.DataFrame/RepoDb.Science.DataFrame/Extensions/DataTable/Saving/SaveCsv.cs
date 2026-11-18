using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Saves tables as CSV.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Saves the table to a CSV file.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="path">The output file.</param>
    /// <param name="separator">The field delimiter.</param>
    public static void SaveCsv(this DataTable table,
        string path,
        char separator = ',') =>
        TableSaveHelper.SaveFile(path,
            stream => table.SaveCsv(stream,
                separator));

    /// <summary>
    /// Writes CSV and leaves the stream open.
    /// </summary>
    /// <param name="table">The table to save.</param>
    /// <param name="stream">The output stream.</param>
    /// <param name="separator">The field delimiter.</param>
    public static void SaveCsv(this DataTable table,
        Stream stream,
        char separator = ',') =>
        CsvSaveHelper.Write(table,
            stream,
            separator);

    #endregion
}
