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
    /// Creates a grouping over current source values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The grouping key names.</param>
    /// <returns>The operation result.</returns>
    public static DataTableGroupBy GroupBy(this DataTable table,
        params string[] columns)
    {
        var keys = TableHelper.Columns(table,
            columns);
        if (keys.Length == 0)
        {
            throw new ArgumentException("At least one grouping column is required.",
                nameof(columns));
        }
        return new DataTableGroupBy(table,
            keys
                .Select(column => column.ColumnName)
                .ToArray());
    }

    #endregion
}
