using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Executes typed scalar and grouped numeric summaries.
/// </summary>
internal static class AggregationHelper
{
    #region Public Methods

    /// <summary>
    /// Summarizes a current numeric column.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="name">The numeric column.</param>
    /// <param name="function">The summary function.</param>
    /// <returns>The scalar or null when no values exist.</returns>
    public static object? Scalar(DataTable table,
        string name,
        AggregateFunction function)
    {
        var column = InspectionHelper.Column(table,
            name);
        ValidateFunction(column,
            function);
        return Compute(InspectionHelper.Rows(table),
            column,
            function);
    }

    /// <summary>
    /// Converts a scalar summary to a numeric output type.
    /// </summary>
    /// <typeparam name="T">The numeric output type.</typeparam>
    /// <param name="value">The scalar summary.</param>
    /// <returns>The converted summary.</returns>
    public static T? ConvertResult<T>(object? value) where T : struct
    {
        if (!InspectionHelper.IsNumeric(typeof(T)))
        {
            throw new ArgumentException("The result type must be numeric.",
                nameof(value));
        }
        return value is null ? null : (T)Convert.ChangeType(value,
            typeof(T),
            CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Summarizes grouped or global rows.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="keys">The grouping names.</param>
    /// <param name="definitions">The summary definitions.</param>
    /// <returns>The summary table.</returns>
    public static DataTable Execute(DataTable table,
        string[] keys,
        AggregateDefinition[] definitions)
    {
        var columns = TableHelper.Columns(table,
            keys);
        var result = OutputSchema(table,
            columns,
            definitions);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var groups = columns.Length == 0 ? new[] { rows } : rows
            .GroupBy(row => new RowKey(columns
            .Select(column => row[column])))
            .Select(group => group
            .ToArray());
        foreach (var group in groups)
        {
            var values = columns
                .Select(column => group[0][column])
                .Concat(definitions
                .Select(definition => Compute(group,
                    table.Columns[definition.Column]!,
                    definition.Function) ?? DBNull.Value))
                        .ToArray();
            result.Rows.Add(values);
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates definitions and creates their result columns.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="keys">The grouping columns.</param>
    /// <param name="definitions">The summary definitions.</param>
    /// <returns>The output schema.</returns>
    private static DataTable OutputSchema(DataTable table,
        DataColumn[] keys,
        AggregateDefinition[] definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        if (definitions.Length == 0)
        {
            throw new ArgumentException("At least one aggregation is required.",
                nameof(definitions));
        }
        var result = TableHelper.Schema(table,
            keys);
        foreach (var definition in definitions)
        {
            ArgumentNullException.ThrowIfNull(definition,
                nameof(definitions));
            var column = InspectionHelper.Column(table,
                definition.Column);
            ValidateFunction(column,
                definition.Function);
            TableHelper.AddColumn(result,
                definition.OutputColumn,
                ResultType(column,
                    definition.Function));
        }
        return result;
    }

    /// <summary>
    /// Resolves the summary output type.
    /// </summary>
    /// <param name="column">The source column.</param>
    /// <param name="function">The summary function.</param>
    /// <returns>The declared output type.</returns>
    private static Type ResultType(DataColumn column,
        AggregateFunction function)
    {
        if (function == AggregateFunction.Count)
        {
            return typeof(int);
        }
        if (function == AggregateFunction.Sum)
        {
            return typeof(decimal);
        }
        return function == AggregateFunction.Mean ? typeof(double) : column.DataType;
    }

    /// <summary>
    /// Validates a function and its input type.
    /// </summary>
    /// <param name="column">The source column.</param>
    /// <param name="function">The summary function.</param>
    private static void ValidateFunction(DataColumn column,
        AggregateFunction function)
    {
        if (!Enum.IsDefined(function))
        {
            throw new ArgumentOutOfRangeException(nameof(function));
        }
        if (function != AggregateFunction.Count)
        {
            FeatureHelper.RequireNumeric(column);
        }
    }

    /// <summary>
    /// Computes a summary from present values.
    /// </summary>
    /// <param name="rows">The group rows.</param>
    /// <param name="column">The source column.</param>
    /// <param name="function">The summary function.</param>
    /// <returns>The summary or null.</returns>
    private static object? Compute(IEnumerable<DataRow> rows,
        DataColumn column,
        AggregateFunction function)
    {
        var values = rows
            .Select(row => row[column])
            .Where(value => !InspectionHelper.IsMissing(value))
            .ToArray();
        if (function == AggregateFunction.Count)
        {
            return values.Length;
        }
        foreach (var value in values)
        {
            FeatureHelper.Number(value);
        }
        return values.Length == 0 ? null : PresentSummary(values,
            function);
    }

    /// <summary>
    /// Computes a summary for nonempty finite values.
    /// </summary>
    /// <param name="values">The present values.</param>
    /// <param name="function">The validated function.</param>
    /// <returns>The numeric summary.</returns>
    private static object PresentSummary(object[] values,
        AggregateFunction function)
    {
        return function switch
        {
            AggregateFunction.Sum => values
                .Sum(value => Convert.ToDecimal(value,
                    CultureInfo.InvariantCulture)),
            AggregateFunction.Mean => values
                .Average(FeatureHelper.Number),
            AggregateFunction.Min => values
                .Min(Comparer<object>.Default)!,
            _ => values
                .Max(Comparer<object>.Default)!
        };
    }

    #endregion
}
