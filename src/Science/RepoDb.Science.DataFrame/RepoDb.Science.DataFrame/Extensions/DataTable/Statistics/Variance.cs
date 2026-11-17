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
    /// Computes numeric variance with an explicit denominator adjustment.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="degreesOfFreedom">The nonnegative denominator adjustment.</param>
    /// <returns>The operation result.</returns>
    public static double? Variance(this DataTable table,
        string column,
        int degreesOfFreedom = 1)
    {
        return StatisticsHelper.Variance(SeriesHelper.Present(SeriesHelper.Read(table,
            column)),
            degreesOfFreedom);
    }

    #endregion
}
