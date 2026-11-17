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
    /// Projects columns in requested order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected column names.</param>
    /// <returns>The operation result.</returns>
    public static DataTable SelectColumns(this DataTable table,
        params string[] columns)
    {
        return TableHelper.Project(table,
            TableHelper.Columns(table,
                columns),
            InspectionHelper.Rows(table));
    }

    #endregion
}
