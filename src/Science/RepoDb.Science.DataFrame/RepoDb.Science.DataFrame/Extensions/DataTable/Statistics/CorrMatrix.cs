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
    /// Computes a labeled pairwise-complete Pearson matrix.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected names, or all numeric columns.</param>
    /// <returns>The operation result.</returns>
    public static DataTable CorrMatrix(this DataTable table,
        params string[] columns)
    {
        return CorrelationHelper.Matrix(table,
            columns);
    }

    #endregion
}
