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
    /// Broadcasts one group summary to each original row.
    /// </summary>
    /// <param name="column">The summarized column.</param>
    /// <param name="function">The summary function.</param>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table in original row order.</returns>
    public DataTable Transform(string column,
        AggregateFunction function,
        string? outputColumn = null,
        bool inPlace = false)
    {
        return GroupTransformationHelper.Execute(_source,
            _columns,
            column,
            function,
            outputColumn ?? column,
            inPlace);
    }

    /// <summary>
    /// Filters grouped summaries using their named output columns.
    /// </summary>
    /// <param name="predicate">The summary-row predicate.</param>
    /// <param name="aggregates">The named summary definitions.</param>
    /// <returns>The matching summary rows.</returns>
    public DataTable Having(Func<DataRow, bool> predicate,
        params AggregateDefinition[] aggregates)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Aggregate(aggregates)
            .Filter(predicate);
    }

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
