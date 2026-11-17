using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Computes row-count windows and recursive weighted means.
/// </summary>
internal static class WindowHelper
{
    #region Public Methods

    /// <summary>
    /// Validates a numeric window definition.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="width">The positive row width.</param>
    /// <param name="minimum">The minimum present observations.</param>
    public static void Validate(DataTable source,
        string column,
        int width,
        int minimum)
    {
        FeatureHelper.RequireNumeric(InspectionHelper.Column(source,
            column));
        ArgumentOutOfRangeException.ThrowIfLessThan(width,
            1);
        ArgumentOutOfRangeException.ThrowIfNegative(minimum);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(minimum,
            width);
    }

    /// <summary>
    /// Computes trailing summaries at every current row.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="column">The input column.</param>
    /// <param name="width">The trailing row width.</param>
    /// <param name="minimum">The minimum present observations.</param>
    /// <param name="summary">The numeric summary.</param>
    /// <param name="output">The output column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The window result table.</returns>
    public static DataTable Calculate(DataTable source,
        string column,
        int width,
        int minimum,
        Func<double[], double?> summary,
        string output,
        bool inPlace)
    {
        var values = SeriesHelper.Read(source,
            column);
        var result = new double?[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            var start = Math.Max(0,
                index - width + 1);
            var present = SeriesHelper.Present(values
                .Skip(start)
                .Take(index - start + 1));
            result[index] = present.Length < minimum ? null : summary(present);
        }
        return SeriesHelper.Write(source,
            output,
            result,
            inPlace);
    }

    /// <summary>
    /// Computes recursive exponential means while ignoring missing observations.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="column">The input column.</param>
    /// <param name="alpha">The smoothing factor.</param>
    /// <param name="minimum">The minimum present observations.</param>
    /// <param name="output">The output column.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The weighted result table.</returns>
    public static DataTable Exponential(DataTable source,
        string column,
        double alpha,
        int minimum,
        string output,
        bool inPlace)
    {
        var values = SeriesHelper.Read(source,
            column);
        var result = new double?[values.Length];
        double? mean = null;
        var count = 0;
        for (var index = 0; index < values.Length; index++)
        {
            if (values[index].HasValue)
            {
                mean = mean.HasValue ? (1 - alpha) * mean.Value + alpha * values[index]!.Value : values[index];
                count++;
            }
            result[index] = count < minimum ? null : mean;
        }
        return SeriesHelper.Write(source,
            output,
            result,
            inPlace);
    }

    #endregion
}
