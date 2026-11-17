using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Provides analytical DataTable operations.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Keeps the first occurrence of each row key.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The key names, or all columns.</param>
    /// <returns>The operation result.</returns>
    public static DataTable DropDuplicates(this DataTable table,
        params string[] columns)
    {
        return DropDuplicates(table,
            columns,
            false);
    }

    /// <summary>
    /// Keeps the first row per key with optional source mutation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The key names, or all columns.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The distinct rows.</returns>
    public static DataTable DropDuplicates(this DataTable table,
        string[] columns,
        bool inPlace)
    {
        var selected = TableHelper.Columns(table,
            columns,
            true);
        var seen = new HashSet<RowKey>();
        return Filter(table,
            row => seen.Add(new RowKey(selected
                .Select(column => row[column]))),
            inPlace);
    }

    #endregion
}
