namespace RepoDb.Science.Helpers;

/// <summary>
/// Computes position-based nullable series operations.
/// </summary>
internal static class SequenceHelper
{
    #region Public Methods

    /// <summary>
    /// Fills only numeric gaps bounded by present values.
    /// </summary>
    /// <param name="values">The mutable numeric series.</param>
    public static void Interpolate(double?[] values)
    {
        var left = -1;
        for (var right = 0; right < values.Length; right++)
        {
            if (!values[right].HasValue)
            {
                continue;
            }
            if (left >= 0)
            {
                FillGap(values,
                    left,
                    right);
            }
            left = right;
        }
    }

    /// <summary>
    /// Accumulates present values while preserving missing positions.
    /// </summary>
    /// <param name="values">The numeric series.</param>
    /// <param name="combine">The running computation.</param>
    /// <param name="skipNulls">Whether to resume after gaps.</param>
    /// <returns>The accumulated series.</returns>
    public static double?[] Accumulate(double?[] values,
        Func<double, double, double> combine,
        bool skipNulls)
    {
        var result = new double?[values.Length];
        double? running = null;
        var blocked = false;
        for (var index = 0; index < values.Length; index++)
        {
            if (!values[index].HasValue)
            {
                blocked = !skipNulls;
                continue;
            }
            if (!blocked)
            {
                running = running.HasValue ? combine(running.Value,
                    values[index]!.Value) : values[index];
                result[index] = SeriesHelper.Finite(running!.Value);
            }
        }
        return result;
    }

    /// <summary>
    /// Shifts values using an overflow-safe signed offset.
    /// </summary>
    /// <param name="values">The current values.</param>
    /// <param name="periods">The signed offset.</param>
    /// <returns>The shifted values.</returns>
    public static object?[] Shift(object?[] values,
        int periods)
    {
        var result = new object?[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            var source = (long)index - periods;
            result[index] = source >= 0 && source < values.Length ? values[(int)source] : DBNull.Value;
        }
        return result;
    }

    /// <summary>
    /// Computes differences or fractional changes.
    /// </summary>
    /// <param name="values">The numeric series.</param>
    /// <param name="periods">The signed offset.</param>
    /// <param name="fractional">Whether to compute fractional changes.</param>
    /// <returns>The comparison series.</returns>
    public static double?[] Compare(double?[] values,
        int periods,
        bool fractional)
    {
        var result = new double?[values.Length];
        for (var index = 0; index < values.Length; index++)
        {
            var prior = (long)index - periods;
            if (prior >= 0 && prior < values.Length && values[index].HasValue && values[(int)prior].HasValue)
            {
                var previous = values[(int)prior]!.Value;
                result[index] = fractional ? previous == 0 ? null : values[index]!.Value / previous - 1 :
                    values[index]!.Value - previous;
            }
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Interpolates values strictly inside a bounded interval.
    /// </summary>
    /// <param name="values">The mutable series.</param>
    /// <param name="left">The preceding present position.</param>
    /// <param name="right">The following present position.</param>
    private static void FillGap(double?[] values,
        int left,
        int right)
    {
        for (var index = left + 1; index < right; index++)
        {
            var fraction = (double)(index - left) / (right - left);
            values[index] = values[left]!.Value * (1 - fraction) + values[right]!.Value * fraction;
        }
    }

    #endregion
}
