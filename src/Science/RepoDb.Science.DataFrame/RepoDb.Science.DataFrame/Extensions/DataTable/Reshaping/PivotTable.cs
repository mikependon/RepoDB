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
    /// <param name="values">The summarized value column.</param>
    /// <param name="function">The duplicate-key summary.</param>
    /// <returns>The operation result.</returns>
    public static DataTable PivotTable(this DataTable table,
        string index,
        string columns,
        string values,
        AggregateFunction function = AggregateFunction.Mean)
    {
        return PivotHelper.Execute(table,
            index,
            columns,
            values,
            function);
    }

    #endregion
}
