namespace RepoDb.Science;

/// <summary>
/// A timed row of a chart table.
/// </summary>
internal sealed class NotebookChartPoint
{
    #region Constructors

    /// <summary>
    /// Creates a chart point.
    /// </summary>
    /// <param name="at">The time.</param>
    /// <param name="values">The values.</param>
    public NotebookChartPoint(DateTime at,
        IReadOnlyList<double?> values)
    {
        At = at;
        Values = values;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the time.
    /// </summary>
    public DateTime At { get; }

    /// <summary>
    /// Gets the values.
    /// </summary>
    public IReadOnlyList<double?> Values { get; }

    #endregion
}
