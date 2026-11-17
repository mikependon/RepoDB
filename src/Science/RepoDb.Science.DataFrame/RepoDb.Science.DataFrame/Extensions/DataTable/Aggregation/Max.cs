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
    /// Computes the max of present numeric values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <returns>The operation result.</returns>
    public static object? Max(this DataTable table,
        string column)
    {
        return AggregationHelper.Scalar(table,
            column,
            AggregateFunction.Max);
    }

    /// <summary>
    /// Converts a numeric summary to the requested numeric type.
    /// </summary>
    /// <typeparam name="T">The numeric output type.</typeparam>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <returns>The summary, or null for missing input.</returns>
    public static T? Max<T>(this DataTable table,
        string column) where T : struct
    {
        return AggregationHelper.ConvertResult<T>(Max(table,
            column));
    }

    #endregion
}
