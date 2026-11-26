namespace RepoDb.Science;

/// <summary>
/// A series of a chart.
/// </summary>
public sealed class NotebookChartSeries
{
    #region Constructors

    /// <summary>
    /// Creates a chart series.
    /// </summary>
    /// <param name="name">The series name.</param>
    /// <param name="points">The values, one for each label.</param>
    /// <param name="color">The custom color, or null.</param>
    public NotebookChartSeries(string name,
        IReadOnlyList<double> points,
        string? color = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(points);
        Name = name;
        Points = points;
        Color = color;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the custom color, or null.
    /// </summary>
    public string? Color { get; }

    /// <summary>
    /// Gets the series name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the values, one for each label.
    /// </summary>
    public IReadOnlyList<double> Points { get; }

    #endregion
}
