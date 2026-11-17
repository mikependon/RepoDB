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
    /// Expands collection cells into rows using an object output column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The column to expand.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Explode(this DataTable table,
        string column)
    {
        return ReshapeHelper.Explode(table,
            column);
    }

    #endregion
}
