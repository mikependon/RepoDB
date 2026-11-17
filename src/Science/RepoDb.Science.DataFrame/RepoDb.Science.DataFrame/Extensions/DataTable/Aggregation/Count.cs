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
    /// Counts present values in a column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <returns>The operation result.</returns>
    public static int Count(this DataTable table,
        string column)
    {
        return (int)AggregationHelper.Scalar(table,
            column,
            AggregateFunction.Count)!;
    }

    #endregion
}
