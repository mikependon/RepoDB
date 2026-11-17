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
    /// Materializes an explicitly keyed reshaped table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="rowName">The original-column label name.</param>
    /// <param name="columnPrefix">The output column prefix.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Transpose(this DataTable table,
        string rowName = "Column",
        string columnPrefix = "Row")
    {
        return ReshapeHelper.Transpose(table,
            rowName,
            columnPrefix);
    }

    #endregion
}
