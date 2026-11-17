using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets a snapshot of column metadata and missing value counts.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>The table information.</returns>
    public static TableInfo Info(this DataTable table)
    {
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var columns = table.Columns
            .Cast<DataColumn>()
            .Select(column => InspectionHelper.GetColumnInfo(column,
                rows))
                    .ToList();
        return new TableInfo(rows.Length,
            columns);
    }

    #endregion
}
