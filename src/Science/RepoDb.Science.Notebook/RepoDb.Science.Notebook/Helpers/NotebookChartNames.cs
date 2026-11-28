using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The names of chart types and buckets, as shown to users and saved in files.
/// </summary>
public static class NotebookChartNames
{
    #region Private Variables

    private static readonly string[] Buckets = ["1min", "5min", "10min", "1h", "12h", "1d"];
    private static readonly string[] Types = ["Lines (Stacked)", "Lines", "Bars", "Bars (Stacked)"];

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the name of a bucket.
    /// </summary>
    /// <param name="bucket">The bucket.</param>
    /// <returns>The name.</returns>
    public static string NameOf(NotebookChartBucket bucket) => Buckets[(int)NotebookValues.Defined(bucket,
        nameof(bucket))];

    /// <summary>
    /// Gets the name of a chart type.
    /// </summary>
    /// <param name="type">The chart type.</param>
    /// <returns>The name.</returns>
    public static string NameOf(NotebookChartType type) => Types[(int)NotebookValues.Defined(type,
        nameof(type))];

    /// <summary>
    /// Gets the bucket of a name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The bucket, or one minute for an unknown name.</returns>
    public static NotebookChartBucket ParseBucket(string? name) => (NotebookChartBucket)Math.Max(0,
        Array.IndexOf(Buckets,
            name));

    /// <summary>
    /// Gets the chart type of a name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The chart type, or stacked lines for an unknown name.</returns>
    public static NotebookChartType ParseType(string? name) => (NotebookChartType)Math.Max(0,
        Array.IndexOf(Types,
            name));

    #endregion
}
