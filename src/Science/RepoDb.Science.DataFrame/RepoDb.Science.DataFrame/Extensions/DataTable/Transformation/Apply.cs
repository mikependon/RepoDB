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
    /// Computes multiple columns from the original rows.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="assignments">The output computations.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Apply(this DataTable table,
        params ColumnAssignment[] assignments)
    {
        return TransformationHelper.Apply(table,
            assignments,
            false);
    }

    /// <summary>
    /// Computes multiple columns with optional source mutation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="assignments">The column computations.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table.</returns>
    public static DataTable Apply(this DataTable table,
        ColumnAssignment[] assignments,
        bool inPlace)
    {
        return TransformationHelper.Apply(table,
            assignments,
            inPlace);
    }

    #endregion
}
