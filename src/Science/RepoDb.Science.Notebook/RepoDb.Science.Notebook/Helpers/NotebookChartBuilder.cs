namespace RepoDb.Science.Helpers;

/// <summary>
/// Builds what a chart draws from a checked table.
/// </summary>
internal static class NotebookChartBuilder
{
    #region Public Methods

    /// <summary>
    /// Builds the chart data.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The valid check of the table.</param>
    /// <param name="bucket">The requested bucket.</param>
    /// <param name="type">The chart type.</param>
    /// <param name="colors">The custom colors.</param>
    /// <returns>The chart data.</returns>
    public static NotebookChartData Build(NotebookTable table,
        NotebookChartCheck check,
        NotebookChartBucket bucket,
        NotebookChartType type,
        IReadOnlyDictionary<string, string> colors) => check.IsTimeBased
        ? Timed(table,
            check,
            bucket,
            type,
            colors)
        : Categories(table,
            check,
            colors);

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds a chart with a point for each row.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The check.</param>
    /// <param name="colors">The custom colors.</param>
    /// <returns>The chart data.</returns>
    private static NotebookChartData Categories(NotebookTable table,
        NotebookChartCheck check,
        IReadOnlyDictionary<string, string> colors)
    {
        var rows = table.Rows
            .Take(NotebookChartBuckets.MaxBuckets)
            .ToList();
        var values = check.ValueColumns
            .Select(column => rows
                .Select(row => ValueOf(row,
                    column))
                .ToArray())
            .ToArray();
        var labels = rows
            .Select(row => NotebookChartValues.TextAt(row,
                0) ?? "(null)")
            .ToList();
        return new NotebookChartData(labels,
            SeriesOf(table,
                check,
                values,
                colors),
            false);
    }

    /// <summary>
    /// Reads a timed row.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="check">The check.</param>
    /// <returns>The point, or null when the row has no time.</returns>
    private static NotebookChartPoint? PointOf(IReadOnlyList<string?> row,
        NotebookChartCheck check)
    {
        if (!NotebookChartValues.TryTimestamp(NotebookChartValues.TextAt(row,
                0),
            out var at))
        {
            return null;
        }
        var values = check.ValueColumns
            .Select(column => NotebookChartValues.TryNumber(NotebookChartValues.TextAt(row,
                    column),
                out var value) ? value : (double?)null)
            .ToList();
        return new NotebookChartPoint(at,
            values);
    }

    /// <summary>
    /// Reads the timed rows of a table, ordered by time.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The check.</param>
    /// <returns>The points.</returns>
    private static List<NotebookChartPoint> PointsOf(NotebookTable table,
        NotebookChartCheck check) => table.Rows
        .Select(row => PointOf(row,
            check))
        .OfType<NotebookChartPoint>()
        .OrderBy(point => point.At)
        .ToList();

    /// <summary>
    /// Names the series and gives them their colors.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The check.</param>
    /// <param name="values">The values of each series.</param>
    /// <param name="colors">The custom colors.</param>
    /// <returns>The series.</returns>
    private static List<NotebookChartSeries> SeriesOf(NotebookTable table,
        NotebookChartCheck check,
        double[][] values,
        IReadOnlyDictionary<string, string> colors) => check.ValueColumns
        .Select((column,
            index) => new NotebookChartSeries(table.Columns[column].Name,
            values[index],
            colors.GetValueOrDefault(table.Columns[column].Name)))
        .ToList();

    /// <summary>
    /// Builds a chart over time.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The check.</param>
    /// <param name="bucket">The requested bucket.</param>
    /// <param name="type">The chart type.</param>
    /// <param name="colors">The custom colors.</param>
    /// <returns>The chart data.</returns>
    private static NotebookChartData Timed(NotebookTable table,
        NotebookChartCheck check,
        NotebookChartBucket bucket,
        NotebookChartType type,
        IReadOnlyDictionary<string, string> colors)
    {
        var points = PointsOf(table,
            check);
        var width = NotebookChartBuckets.WidthOf(bucket,
            points[^1].At - points[0].At);
        var values = NotebookChartBuckets.Aggregate(points,
            check.ValueColumns.Count,
            width,
            type == NotebookChartType.Lines);
        return new NotebookChartData(NotebookChartBuckets.LabelsOf(points,
                width),
            SeriesOf(table,
                check,
                values,
                colors),
            true);
    }

    /// <summary>
    /// Reads the number of a row, or zero.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column position.</param>
    /// <returns>The number.</returns>
    private static double ValueOf(IReadOnlyList<string?> row,
        int column) => NotebookChartValues.TryNumber(NotebookChartValues.TextAt(row,
            column),
        out var value) ? value : 0;

    #endregion
}
