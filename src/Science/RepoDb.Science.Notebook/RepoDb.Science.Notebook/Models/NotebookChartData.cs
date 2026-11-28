namespace RepoDb.Science;

/// <summary>
/// What a chart draws.
/// </summary>
public sealed class NotebookChartData
{
    #region Constructors

    /// <summary>
    /// Creates chart data.
    /// </summary>
    /// <param name="labels">The point labels.</param>
    /// <param name="series">The series.</param>
    /// <param name="isTimeBased">Whether the labels are times.</param>
    public NotebookChartData(IReadOnlyList<string> labels,
        IReadOnlyList<NotebookChartSeries> series,
        bool isTimeBased)
    {
        ArgumentNullException.ThrowIfNull(labels);
        ArgumentNullException.ThrowIfNull(series);
        Labels = labels;
        Series = series;
        IsTimeBased = isTimeBased;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether the labels are times.
    /// </summary>
    public bool IsTimeBased { get; }

    /// <summary>
    /// Gets the point labels.
    /// </summary>
    public IReadOnlyList<string> Labels { get; }

    /// <summary>
    /// Gets the series.
    /// </summary>
    public IReadOnlyList<NotebookChartSeries> Series { get; }

    #endregion
}
