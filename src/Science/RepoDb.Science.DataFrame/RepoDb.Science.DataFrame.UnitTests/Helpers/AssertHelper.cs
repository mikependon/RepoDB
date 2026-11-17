namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Provides method-driven assertions.
/// </summary>
internal static class AssertHelper
{
    #region Public Methods

    /// <summary>
    /// Requires an expected condition.
    /// </summary>
    /// <param name="condition">The expected condition.</param>
    public static void Check(bool condition)
    {
        if (!condition)
        {
            throw new InvalidOperationException("MVP assertion failed.");
        }
    }

    /// <summary>
    /// Requires an expected exception.
    /// </summary>
    /// <typeparam name="T">The exception type.</typeparam>
    /// <param name="operation">The tested operation.</param>
    /// <returns>The observed exception.</returns>
    public static T Throws<T>(Action operation) where T : Exception
    {
        try
        {
            operation();
        }
        catch (T exception)
        {
            return exception;
        }
        throw new InvalidOperationException("Expected exception was not thrown.");
    }

    #endregion
}
