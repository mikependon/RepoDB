namespace RepoDb.Science.Helpers;

/// <summary>
/// Tests comparable present values.
/// </summary>
internal static class QualityHelper
{
    #region Public Methods

    /// <summary>
    /// Checks a value against compatible bounds using CLR comparison.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="lower">The lower bound.</param>
    /// <param name="upper">The upper bound.</param>
    /// <param name="inclusive">Whether endpoints match.</param>
    /// <returns>Whether the value is inside the bounds.</returns>
    public static bool Between(object value,
        object lower,
        object upper,
        bool inclusive)
    {
        if (InspectionHelper.IsMissing(value))
        {
            return false;
        }
        var left = Comparer<object>.Default.Compare(value,
            lower);
        var right = Comparer<object>.Default.Compare(value,
            upper);
        return inclusive ? left >= 0 && right <= 0 : left > 0 && right < 0;
    }

    #endregion
}
