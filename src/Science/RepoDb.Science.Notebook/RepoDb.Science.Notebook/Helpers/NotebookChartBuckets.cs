using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Groups chart points into time buckets.
/// </summary>
internal static class NotebookChartBuckets
{
    #region Public Variables

    /// <summary>
    /// The most buckets a chart draws.
    /// </summary>
    public const int MaxBuckets = 5000;

    #endregion

    #region Private Variables

    private static readonly int[] Widths = [60, 300, 600, 3600, 43200, 86400];

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds or averages the values of each bucket.
    /// </summary>
    /// <param name="points">The points, ordered by time.</param>
    /// <param name="seriesCount">The number of series.</param>
    /// <param name="widthSeconds">The bucket width.</param>
    /// <param name="average">Whether to average instead of add.</param>
    /// <returns>The values of each series.</returns>
    public static double[][] Aggregate(IReadOnlyList<NotebookChartPoint> points,
        int seriesCount,
        int widthSeconds,
        bool average)
    {
        var count = CountOf(points,
            widthSeconds);
        var sums = Grid<double>(seriesCount,
            count);
        var counts = Grid<int>(seriesCount,
            count);
        foreach (var point in points)
        {
            Add(point,
                (int)((point.At - points[0].At).TotalSeconds / widthSeconds),
                sums,
                counts);
        }
        return average
            ? Averaged(sums,
                counts)
            : sums;
    }

    /// <summary>
    /// Gets the number of buckets that cover the points.
    /// </summary>
    /// <param name="points">The points, ordered by time.</param>
    /// <param name="widthSeconds">The bucket width.</param>
    /// <returns>The number of buckets.</returns>
    public static int CountOf(IReadOnlyList<NotebookChartPoint> points,
        int widthSeconds) => (int)((points[^1].At - points[0].At).TotalSeconds / widthSeconds) + 1;

    /// <summary>
    /// Gets the label of each bucket.
    /// </summary>
    /// <param name="points">The points, ordered by time.</param>
    /// <param name="widthSeconds">The bucket width.</param>
    /// <returns>The labels.</returns>
    public static List<string> LabelsOf(IReadOnlyList<NotebookChartPoint> points,
        int widthSeconds)
    {
        var format = FormatOf(points[^1].At - points[0].At,
            widthSeconds);
        return Enumerable
            .Range(0,
                CountOf(points,
                    widthSeconds))
            .Select(index => points[0].At
                .AddSeconds((double)index * widthSeconds)
                .ToString(format,
                    CultureInfo.InvariantCulture))
            .ToList();
    }

    /// <summary>
    /// Gets the width to use, widening it when there would be too many buckets.
    /// </summary>
    /// <param name="bucket">The requested bucket.</param>
    /// <param name="span">The time covered.</param>
    /// <returns>The width in seconds.</returns>
    public static int WidthOf(NotebookChartBucket bucket,
        TimeSpan span)
    {
        var seconds = Math.Max(1,
            span.TotalSeconds);
        foreach (var width in Widths
            .Skip((int)bucket))
        {
            if (seconds / width <= MaxBuckets)
            {
                return width;
            }
        }
        return (int)Math.Ceiling(seconds / MaxBuckets / 60) * 60;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a point to its bucket.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="bucket">The bucket position.</param>
    /// <param name="sums">The sums.</param>
    /// <param name="counts">The counts.</param>
    private static void Add(NotebookChartPoint point,
        int bucket,
        double[][] sums,
        int[][] counts)
    {
        for (var series = 0; series < sums.Length; series++)
        {
            if (point.Values[series] is { } value)
            {
                sums[series][bucket] += value;
                counts[series][bucket]++;
            }
        }
    }

    /// <summary>
    /// Divides each sum by its count.
    /// </summary>
    /// <param name="sums">The sums.</param>
    /// <param name="counts">The counts.</param>
    /// <returns>The averages.</returns>
    private static double[][] Averaged(double[][] sums,
        int[][] counts) => sums
        .Select((series,
            index) => series
            .Select((sum,
                bucket) => counts[index][bucket] == 0 ? 0 : sum / counts[index][bucket])
            .ToArray())
        .ToArray();

    /// <summary>
    /// Gets the time format of the labels.
    /// </summary>
    /// <param name="span">The time covered.</param>
    /// <param name="widthSeconds">The bucket width.</param>
    /// <returns>The format.</returns>
    private static string FormatOf(TimeSpan span,
        int widthSeconds)
    {
        if (span >= TimeSpan.FromDays(1))
        {
            return "MM-dd HH:mm";
        }
        return widthSeconds < 60 ? "HH:mm:ss" : "HH:mm";
    }

    /// <summary>
    /// Creates a row of buckets for each series.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="series">The number of series.</param>
    /// <param name="buckets">The number of buckets.</param>
    /// <returns>The grid.</returns>
    private static T[][] Grid<T>(int series,
        int buckets) => Enumerable
        .Range(0,
            series)
        .Select(_ => new T[buckets])
        .ToArray();

    #endregion
}
