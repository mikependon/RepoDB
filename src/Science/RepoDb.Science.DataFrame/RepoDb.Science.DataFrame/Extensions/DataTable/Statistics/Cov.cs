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
    /// Computes pairwise-complete numeric covariance.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="left">The first numeric column.</param>
    /// <param name="right">The second numeric column.</param>
    /// <param name="degreesOfFreedom">The nonnegative covariance denominator adjustment.</param>
    /// <returns>The operation result.</returns>
    public static double? Cov(this DataTable table,
        string left,
        string right,
        int degreesOfFreedom = 1)
    {
        return CorrelationHelper.Calculate(SeriesHelper.Read(table,
            left),
            SeriesHelper.Read(table,
                right),
            false,
            degreesOfFreedom);
    }

    #endregion
}
