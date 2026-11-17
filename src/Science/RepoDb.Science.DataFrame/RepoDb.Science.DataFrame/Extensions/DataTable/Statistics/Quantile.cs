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
    /// Computes a linearly interpolated numeric quantile.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="probability">The probability from zero through one.</param>
    /// <returns>The operation result.</returns>
    public static double? Quantile(this DataTable table,
        string column,
        double probability)
    {
        return StatisticsHelper.Quantile(SeriesHelper.Present(SeriesHelper.Read(table,
            column)),
            probability);
    }

    #endregion
}
