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
    /// Fills missing values using the preceding present value in current row order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The filled table with unresolved leading gaps preserved.</returns>
    public static DataTable ForwardFill(this DataTable table,
        string column,
        bool inPlace = false)
    {
        return DirectionalFillHelper.Fill(table,
            column,
            true,
            inPlace);
    }

    #endregion
}
