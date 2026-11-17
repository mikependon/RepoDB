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
    /// Identifies present values in current row order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <returns>The operation result.</returns>
    public static bool[] NotNull(this DataTable table,
        string column)
    {
        return IsNull(table,
            column)
            .Select(missing => !missing)
            .ToArray();
    }

    #endregion
}
