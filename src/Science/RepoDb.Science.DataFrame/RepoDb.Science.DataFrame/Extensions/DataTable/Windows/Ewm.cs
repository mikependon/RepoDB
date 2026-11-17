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
    /// <param name="alpha">The smoothing factor greater than zero and at most one.</param>
    /// <param name="minPeriods">The positive minimum present values.</param>
    /// <returns>The operation result.</returns>
    public static DataTableEwm Ewm(this DataTable table,
        string column,
        double alpha,
        int minPeriods = 1)
    {
        return new DataTableEwm(table,
            column,
            alpha,
            minPeriods);
    }

    #endregion
}
