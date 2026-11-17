using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Builds wide tables from explicit row and column keys.
/// </summary>
internal static class PivotHelper
{
    #region Public Methods

    /// <summary>
    /// Pivots values with optional duplicate-key aggregation.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="index">The row key name.</param>
    /// <param name="columns">The column key name.</param>
    /// <param name="values">The value name.</param>
    /// <param name="function">The optional duplicate-key summary.</param>
    /// <returns>The wide table.</returns>
    public static DataTable Execute(DataTable table,
        string index,
        string columns,
        string values,
        AggregateFunction? function)
    {
        var key = InspectionHelper.Column(table,
            index);
        var pivot = InspectionHelper.Column(table,
            columns);
        var value = InspectionHelper.Column(table,
            values);
        var type = function.HasValue ? AggregationHelper.OutputType(value,
            function.Value) : value.DataType;
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var names = Names(rows,
            pivot);
        var result = Schema(table,
            key,
            type,
            names.Values);
        Populate(result,
            rows,
            key,
            pivot,
            value,
            names,
            function);
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Maps distinct pivot keys to invariant output names.
    /// </summary>
    /// <param name="rows">The current rows.</param>
    /// <param name="pivot">The pivot column.</param>
    /// <returns>The ordered name mapping.</returns>
    private static Dictionary<RowKey, string> Names(DataRow[] rows,
        DataColumn pivot)
    {
        return rows
            .GroupBy(row => new RowKey(new[] { row[pivot] }))
            .ToDictionary(group => group.Key,
                group => Name(group
                    .First()[pivot]));
    }

    /// <summary>
    /// Creates an invariant name for a pivot key.
    /// </summary>
    /// <param name="value">The pivot key.</param>
    /// <returns>The output name.</returns>
    private static string Name(object value)
    {
        var name = InspectionHelper.IsMissing(value) ? "(null)" : Convert.ToString(value,
            CultureInfo.InvariantCulture);
        ArgumentException.ThrowIfNullOrWhiteSpace(name,
            nameof(value));
        return name;
    }

    /// <summary>
    /// Creates nullable pivot value columns.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="key">The row key column.</param>
    /// <param name="type">The output value type.</param>
    /// <param name="names">The ordered output names.</param>
    /// <returns>The pivot schema.</returns>
    private static DataTable Schema(DataTable table,
        DataColumn key,
        Type type,
        IEnumerable<string> names)
    {
        var result = TableHelper.Schema(table,
            new[] { key });
        foreach (var name in names)
        {
            TableHelper.AddColumn(result,
                name,
                type);
        }
        return result;
    }

    /// <summary>
    /// Populates ordered row groups and pivoted values.
    /// </summary>
    /// <param name="result">The output table.</param>
    /// <param name="rows">The current rows.</param>
    /// <param name="key">The row key column.</param>
    /// <param name="pivot">The pivot key column.</param>
    /// <param name="value">The value column.</param>
    /// <param name="names">The pivot output names.</param>
    /// <param name="function">The optional duplicate summary.</param>
    private static void Populate(DataTable result,
        DataRow[] rows,
        DataColumn key,
        DataColumn pivot,
        DataColumn value,
        Dictionary<RowKey, string> names,
        AggregateFunction? function)
    {
        foreach (var group in rows
            .GroupBy(row => new RowKey(new[] { row[key] })))
        {
            var output = result.NewRow();
            output[0] = group
                .First()[key];
            foreach (var cell in group
                .GroupBy(row => new RowKey(new[] { row[pivot] })))
            {
                output[names[cell.Key]] = Cell(cell
                    .ToArray(),
                    value,
                    function) ?? DBNull.Value;
            }
            result.Rows.Add(output);
        }
    }

    /// <summary>
    /// Resolves one pivot cell or rejects duplicate keys.
    /// </summary>
    /// <param name="rows">The matching rows.</param>
    /// <param name="column">The value column.</param>
    /// <param name="function">The optional aggregation.</param>
    /// <returns>The cell value.</returns>
    private static object? Cell(DataRow[] rows,
        DataColumn column,
        AggregateFunction? function)
    {
        if (function.HasValue)
        {
            return AggregationHelper.Summarize(rows,
                column,
                function.Value);
        }
        if (rows.Length != 1)
        {
            throw new InvalidOperationException("Pivot keys must be unique; use PivotTable for aggregation.");
        }
        return rows[0][column];
    }

    #endregion
}
