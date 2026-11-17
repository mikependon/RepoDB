using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Summarizes groups from current source values at execution time.
/// </summary>
public sealed class DataTableGroupBy
{
    #region Private Variables

    private readonly string[] _columns;
    private readonly DataTable _source;

    #endregion

    #region Constructors

    /// <summary>
    /// Captures a source and independent key names.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="columns">The grouping key names.</param>
    internal DataTableGroupBy(DataTable source,
        string[] columns)
    {
        _columns = columns
            .ToArray();
        _source = source;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Executes named summaries for each group.
    /// </summary>
    /// <param name="aggregates">The output definitions.</param>
    /// <returns>The grouped summaries.</returns>
    public DataTable Aggregate(params AggregateDefinition[] aggregates)
    {
        return AggregationHelper.Execute(_source,
            _columns,
            aggregates);
    }

    #endregion
}
