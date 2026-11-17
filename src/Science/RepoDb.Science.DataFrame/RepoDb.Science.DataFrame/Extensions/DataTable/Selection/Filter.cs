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
    /// Keeps rows matching a predicate.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="predicate">The row predicate.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Filter(this DataTable table,
        Func<DataRow, bool> predicate,
        bool inPlace = false)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var included = rows
            .Where(predicate)
            .ToHashSet();
        if (inPlace)
        {
            TableHelper.RemoveRows(table,
                rows
                    .Where(row => !included.Contains(row)));
            return table;
        }
        return TableHelper.Project(table,
            table.Columns
                .Cast<DataColumn>(),
            rows
                .Where(included.Contains));
    }

    #endregion
}
