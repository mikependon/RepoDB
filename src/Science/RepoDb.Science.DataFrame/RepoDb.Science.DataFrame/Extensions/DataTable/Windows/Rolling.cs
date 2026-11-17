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
    /// <param name="window">The positive trailing row count.</param>
    /// <param name="minPeriods">The minimum present values, or the full width.</param>
    /// <returns>The operation result.</returns>
    public static DataTableWindow Rolling(this DataTable table,
        string column,
        int window,
        int? minPeriods = null)
    {
        return new DataTableWindow(table,
            column,
            window,
            minPeriods ?? window);
    }

    #endregion
}
