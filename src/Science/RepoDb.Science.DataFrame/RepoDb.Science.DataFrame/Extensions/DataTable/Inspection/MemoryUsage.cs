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
    /// Estimates stored value bytes, excluding CLR and table overhead.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>The estimated payload bytes.</returns>
    public static long MemoryUsage(this DataTable table)
    {
        return InspectionHelper.Rows(table)
            .Sum(row => table.Columns
            .Cast<DataColumn>()
            .Sum(column => InspectionHelper.ValueSize(row[column])));
    }

    #endregion
}
