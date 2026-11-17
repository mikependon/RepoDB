using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Computes trailing or expanding numeric summaries over current rows.
/// </summary>
public sealed class DataTableWindow
{
    #region Private Variables

    private readonly string _column;
    private readonly int _minPeriods;
    private readonly DataTable _source;
    private readonly int _width;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a row-count window.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="width">The positive row width.</param>
    /// <param name="minPeriods">The minimum present observations.</param>
    internal DataTableWindow(DataTable source,
        string column,
        int width,
        int minPeriods)
    {
        WindowHelper.Validate(source,
            column,
            width,
            minPeriods);
        _column = column;
        _minPeriods = minPeriods;
        _source = source;
        _width = width;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Computes a basic summary for each trailing window.
    /// </summary>
    /// <param name="function">The summary function.</param>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Aggregate(AggregateFunction function,
        string? outputColumn = null,
        bool inPlace = false)
    {
        if (!Enum.IsDefined(function))
        {
            throw new ArgumentOutOfRangeException(nameof(function));
        }
        return Execute(values => StatisticsHelper.Summary(values,
            function),
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes the sum for each window.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Sum(string? outputColumn = null,
        bool inPlace = false)
    {
        return Aggregate(AggregateFunction.Sum,
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes the mean for each window.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Mean(string? outputColumn = null,
        bool inPlace = false)
    {
        return Aggregate(AggregateFunction.Mean,
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes the min for each window.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Min(string? outputColumn = null,
        bool inPlace = false)
    {
        return Aggregate(AggregateFunction.Min,
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes the max for each window.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Max(string? outputColumn = null,
        bool inPlace = false)
    {
        return Aggregate(AggregateFunction.Max,
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes the count for each window.
    /// </summary>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The summary table.</returns>
    public DataTable Count(string? outputColumn = null,
        bool inPlace = false)
    {
        return Aggregate(AggregateFunction.Count,
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes variance for each window.
    /// </summary>
    /// <param name="degreesOfFreedom">The nonnegative denominator adjustment.</param>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The variance table.</returns>
    public DataTable Variance(int degreesOfFreedom = 1,
        string? outputColumn = null,
        bool inPlace = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(degreesOfFreedom);
        return Execute(values => StatisticsHelper.Variance(values,
            degreesOfFreedom),
            outputColumn,
            inPlace);
    }

    /// <summary>
    /// Computes standard deviation for each window.
    /// </summary>
    /// <param name="degreesOfFreedom">The nonnegative denominator adjustment.</param>
    /// <param name="outputColumn">The output name, or the input name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The deviation table.</returns>
    public DataTable Std(int degreesOfFreedom = 1,
        string? outputColumn = null,
        bool inPlace = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(degreesOfFreedom);
        return Execute(values => StatisticsHelper.Deviation(values,
            degreesOfFreedom),
            outputColumn,
            inPlace);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Executes a numeric computation over the current windows.
    /// </summary>
    /// <param name="summary">The summary computation.</param>
    /// <param name="outputColumn">The optional output name.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The transformed table.</returns>
    private DataTable Execute(Func<double[], double?> summary,
        string? outputColumn,
        bool inPlace)
    {
        return WindowHelper.Calculate(_source,
            _column,
            _width,
            _minPeriods,
            summary,
            outputColumn ?? _column,
            inPlace);
    }

    #endregion
}
