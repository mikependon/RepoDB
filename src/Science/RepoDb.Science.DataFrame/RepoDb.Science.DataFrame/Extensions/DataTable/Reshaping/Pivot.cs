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
    /// <param name="index">The retained row key.</param>
    /// <param name="columns">The keys becoming output column names.</param>
    /// <param name="values">The cell value column.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Pivot(this DataTable table,
        string index,
        string columns,
        string values)
    {
        return PivotHelper.Execute(table,
            index,
            columns,
            values,
            null);
    }

    #endregion
}
