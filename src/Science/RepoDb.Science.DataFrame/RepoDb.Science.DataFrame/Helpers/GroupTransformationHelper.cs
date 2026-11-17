using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Broadcasts validated group summaries back to source rows.
/// </summary>
internal static class GroupTransformationHelper
{
    #region Public Methods

    /// <summary>
    /// Repeats one summary for every row in its group.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="keys">The grouping names.</param>
    /// <param name="column">The summarized column.</param>
    /// <param name="function">The summary function.</param>
    /// <param name="output">The output column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table in source order.</returns>
    public static DataTable Execute(DataTable source,
        string[] keys,
        string column,
        AggregateFunction function,
        string output,
        bool inPlace)
    {
        var selected = TableHelper.Columns(source,
            keys);
        var value = InspectionHelper.Column(source,
            column);
        var type = AggregationHelper.OutputType(value,
            function);
        var rows = InspectionHelper.Rows(source)
            .ToArray();
        var summaries = rows
            .GroupBy(row => new RowKey(selected
            .Select(key => row[key])))
            .ToDictionary(group => group.Key,
                group => AggregationHelper.Summarize(group
                    .ToArray(),
                    value,
                    function));
        var values = rows
            .Select(row => summaries[new RowKey(selected
            .Select(key => row[key]))])
            .ToArray();
        return TransformationHelper.SetColumn(source,
            output,
            type,
            values,
            inPlace);
    }

    #endregion
}
