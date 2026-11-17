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
    /// Drops rows missing any selected value.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected names, or all columns.</param>
    /// <returns>The operation result.</returns>
    public static DataTable DropNulls(this DataTable table,
        params string[] columns)
    {
        return DropNulls(table,
            columns,
            false);
    }

    /// <summary>
    /// Drops rows missing selected values with optional source mutation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected names, or all columns.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The cleaned table.</returns>
    public static DataTable DropNulls(this DataTable table,
        string[] columns,
        bool inPlace)
    {
        var selected = TableHelper.Columns(table,
            columns,
            true);
        return Filter(table,
            row => selected
                .All(column => !InspectionHelper.IsMissing(row[column])),
            inPlace);
    }

    #endregion
}
