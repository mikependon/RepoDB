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
    /// Computes the numeric standard deviation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="degreesOfFreedom">The nonnegative denominator adjustment.</param>
    /// <returns>The operation result.</returns>
    public static double? Std(this DataTable table,
        string column,
        int degreesOfFreedom = 1)
    {
        var variance = Variance(table,
            column,
            degreesOfFreedom);
        return variance.HasValue ? Math.Sqrt(variance.Value) : null;
    }

    #endregion
}
