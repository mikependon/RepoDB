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
    /// Summarizes the whole table into one row.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="aggregates">The named summary definitions.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Aggregate(this DataTable table,
        params AggregateDefinition[] aggregates)
    {
        return AggregationHelper.Execute(table,
            Array.Empty<string>(),
            aggregates);
    }

    #endregion
}
