namespace RepoDb.Science.Helpers;

/// <summary>
/// Computes finite descriptive statistics.
/// </summary>
internal static class StatisticsHelper
{
    #region Public Methods

    /// <summary>
    /// Computes standard deviation for present values.
    /// </summary>
    /// <param name="values">The finite values.</param>
    /// <param name="degreesOfFreedom">The denominator adjustment.</param>
    /// <returns>The deviation or null for insufficient observations.</returns>
    public static double? Deviation(double[] values,
        int degreesOfFreedom)
    {
        var variance = Variance(values,
            degreesOfFreedom);
        return variance.HasValue ? Math.Sqrt(variance.Value) : null;
    }

    /// <summary>
    /// Computes a linearly interpolated quantile.
    /// </summary>
    /// <param name="values">The finite values.</param>
    /// <param name="probability">The probability between zero and one.</param>
    /// <returns>The quantile or null for empty input.</returns>
    public static double? Quantile(double[] values,
        double probability)
    {
        if (!double.IsFinite(probability) || probability < 0 || probability > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(probability));
        }
        if (values.Length == 0)
        {
            return null;
        }
        var sorted = values
            .Order()
            .ToArray();
        var position = (sorted.Length - 1) * probability;
        var lower = (int)position;
        var weight = position - lower;
        return lower == sorted.Length - 1 ? sorted[lower] :
            SeriesHelper.Finite(sorted[lower] * (1 - weight) + sorted[lower + 1] * weight);
    }

    /// <summary>
    /// Computes variance with an explicit degrees-of-freedom adjustment.
    /// </summary>
    /// <param name="values">The finite values.</param>
    /// <param name="degreesOfFreedom">The nonnegative denominator adjustment.</param>
    /// <returns>The variance or null for insufficient observations.</returns>
    public static double? Variance(double[] values,
        int degreesOfFreedom)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(degreesOfFreedom);
        var count = 0;
        double mean = 0, sum = 0;
        foreach (var value in values)
        {
            var delta = value - mean;
            mean += delta / ++count;
            sum += delta * (value - mean);
        }
        return count <= degreesOfFreedom ? null : SeriesHelper.Finite(Math.Max(0,
            sum / (count - degreesOfFreedom)));
    }

    /// <summary>
    /// Computes a basic window summary.
    /// </summary>
    /// <param name="values">The present finite values.</param>
    /// <param name="function">The summary function.</param>
    /// <returns>The summary or null for empty extrema and means.</returns>
    public static double? Summary(double[] values,
        AggregateFunction function)
    {
        if (!Enum.IsDefined(function))
        {
            throw new ArgumentOutOfRangeException(nameof(function));
        }
        if (function == AggregateFunction.Count)
        {
            return values.Length;
        }
        if (function == AggregateFunction.Sum)
        {
            return SeriesHelper.Finite(values
                .Sum());
        }
        return values.Length == 0 ? null : PresentSummary(values,
            function);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Computes a summary requiring present values.
    /// </summary>
    /// <param name="values">The present values.</param>
    /// <param name="function">The validated summary function.</param>
    /// <returns>The summary.</returns>
    private static double PresentSummary(double[] values,
        AggregateFunction function)
    {
        if (function == AggregateFunction.Mean)
        {
            return SeriesHelper.Finite(values
                .Average());
        }
        return function == AggregateFunction.Min ? values
            .Min() : values
            .Max();
    }

    #endregion
}
