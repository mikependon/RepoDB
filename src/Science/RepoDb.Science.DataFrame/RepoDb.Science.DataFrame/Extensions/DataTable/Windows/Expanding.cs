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
    /// Creates a row-order numeric window over current source values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="minPeriods">The nonnegative minimum present values.</param>
    /// <returns>The operation result.</returns>
    public static DataTableWindow Expanding(this DataTable table,
        string column,
        int minPeriods = 1)
    {
        return new DataTableWindow(table,
            column,
            int.MaxValue,
            minPeriods);
    }

    #endregion
}
