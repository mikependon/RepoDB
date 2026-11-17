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
    /// Computes the mean of present numeric values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <returns>The operation result.</returns>
    public static double? Mean(this DataTable table,
        string column)
    {
        return (double?)AggregationHelper.Scalar(table,
            column,
            AggregateFunction.Mean);
    }

    #endregion
}
