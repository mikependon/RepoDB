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
    /// Computes a running numeric summary in current row order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="skipNulls">Whether accumulation resumes after missing values.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable CumMin(this DataTable table,
        string column,
        bool skipNulls = true,
        bool inPlace = false)
    {
        var values = SeriesHelper.Read(table,
            column);
        var result = SequenceHelper.Accumulate(values,
            Math.Min,
            skipNulls);
        return SeriesHelper.Write(table,
            column,
            result,
            inPlace);
    }

    #endregion
}
