namespace RepoDb.Science;

/// <summary>
/// Contains numeric column statistics.
/// </summary>
public sealed class ColumnStatistics
{
    #region Constructors

    /// <summary>
    /// Creates a snapshot.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="count">The present value count.</param>
    /// <param name="mean">The arithmetic mean.</param>
    /// <param name="standardDeviation">The sample standard deviation.</param>
    /// <param name="min">The minimum.</param>
    /// <param name="firstQuartile">The first quartile.</param>
    /// <param name="median">The median.</param>
    /// <param name="thirdQuartile">The third quartile.</param>
    /// <param name="max">The maximum.</param>
    public ColumnStatistics(string name,
        int count,
        double? mean,
        double? standardDeviation,
        double? min,
        double? firstQuartile,
        double? median,
        double? thirdQuartile,
        double? max)
    {
        Name = name;
        Count = count;
        Mean = mean;
        StandardDeviation = standardDeviation;
        Min = min;
        FirstQuartile = firstQuartile;
        Median = median;
        ThirdQuartile = thirdQuartile;
        Max = max;
    }

    #endregion

    #region Properties

    /// <summary>
    /// The column name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The present value count.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The arithmetic mean.
    /// </summary>
    public double? Mean { get; }

    /// <summary>
    /// The sample standard deviation.
    /// </summary>
    public double? StandardDeviation { get; }

    /// <summary>
    /// The minimum.
    /// </summary>
    public double? Min { get; }

    /// <summary>
    /// The first quartile.
    /// </summary>
    public double? FirstQuartile { get; }

    /// <summary>
    /// The median.
    /// </summary>
    public double? Median { get; }

    /// <summary>
    /// The third quartile.
    /// </summary>
    public double? ThirdQuartile { get; }

    /// <summary>
    /// The maximum.
    /// </summary>
    public double? Max { get; }

    #endregion
}
