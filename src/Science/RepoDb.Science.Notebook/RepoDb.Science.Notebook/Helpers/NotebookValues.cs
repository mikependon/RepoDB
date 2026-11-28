namespace RepoDb.Science.Helpers;

/// <summary>
/// Keeps section values within their valid ranges.
/// </summary>
internal static class NotebookValues
{
    #region Public Variables

    /// <summary>
    /// The largest height a section keeps.
    /// </summary>
    public const double MaxHeight = 5000;

    #endregion

    #region Public Methods

    /// <summary>
    /// Requires a defined enumeration value.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="parameterName">The argument name.</param>
    /// <returns>The value.</returns>
    public static T Defined<T>(T value,
        string parameterName)
        where T : struct, Enum => Enum.IsDefined(value) ? value : throw new ArgumentOutOfRangeException(parameterName);

    /// <summary>
    /// Converts saved seconds to a duration.
    /// </summary>
    /// <param name="seconds">The seconds.</param>
    /// <returns>The duration, or zero when the seconds make no sense.</returns>
    public static TimeSpan Elapsed(double seconds) => seconds > 0 && seconds < TimeSpan.MaxValue.TotalSeconds
        ? TimeSpan.FromSeconds(seconds)
        : TimeSpan.Zero;

    /// <summary>
    /// Normalizes a user height, where zero means automatic.
    /// </summary>
    /// <param name="height">The height.</param>
    /// <returns>The normalized height.</returns>
    public static double Height(double height) => double.IsFinite(height) && height > 0
        ? Math.Min(height,
            MaxHeight)
        : 0;

    #endregion
}
