using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Loads records from JSON.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Loads a JSON file containing an array of records.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="path">The input file.</param>
    /// <param name="collectionPath">The optional dot-separated path to record arrays.</param>
    /// <param name="columns">The optional output names mapped to dot-separated scalar paths.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadJson(this DataTable table,
        string path,
        string? collectionPath = null,
        IReadOnlyDictionary<string, string>? columns = null)
    {
        using var stream = File.OpenRead(path);
        return table.LoadJson(stream,
            collectionPath,
            columns);
    }

    /// <summary>
    /// Loads JSON records and leaves the stream open.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="stream">The UTF-8 input stream.</param>
    /// <param name="collectionPath">The optional dot-separated path to record arrays.</param>
    /// <param name="columns">The optional output names mapped to dot-separated scalar paths.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadJson(this DataTable table,
        Stream stream,
        string? collectionPath = null,
        IReadOnlyDictionary<string, string>? columns = null) =>
        TableLoadHelper.Load(table,
            stream,
            true,
            (input, column, row) => JsonLoadHelper.Read(input,
                collectionPath,
                columns,
                column,
                row));

    #endregion
}
