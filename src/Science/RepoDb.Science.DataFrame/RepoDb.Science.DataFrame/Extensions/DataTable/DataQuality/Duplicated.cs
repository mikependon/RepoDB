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
    /// Marks repeated keys after their first occurrence.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The key names, or all columns.</param>
    /// <returns>The operation result.</returns>
    public static bool[] Duplicated(this DataTable table,
        params string[] columns)
    {
        var selected = TableHelper.Columns(table,
            columns,
            true);
        var seen = new HashSet<RowKey>();
        return InspectionHelper.Rows(table)
            .Select(row => !seen.Add(new RowKey(selected
            .Select(column => row[column]))))
            .ToArray();
    }

    #endregion
}
