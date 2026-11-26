using System.Runtime.CompilerServices;

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
    /// <param name="expression">The text of the condition.</param>
    public static void Check(bool condition,
        [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"Assertion failed: {expression}");
        }
    }

    /// <summary>
    /// Requires an expected exception.
    /// </summary>
    /// <typeparam name="T">The exception type.</typeparam>
    /// <param name="operation">The tested operation.</param>
    /// <returns>The observed exception.</returns>
    public static T Throws<T>(Action operation)
        where T : Exception
    {
        try
        {
            operation();
        }
        catch (T exception)
        {
            return exception;
        }
        throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown.");
    }

    /// <summary>
    /// Requires an expected exception from an asynchronous operation.
    /// </summary>
    /// <typeparam name="T">The exception type.</typeparam>
    /// <param name="operation">The tested operation.</param>
    /// <returns>The observed exception.</returns>
    public static async Task<T> ThrowsAsync<T>(Func<Task> operation)
        where T : Exception
    {
        try
        {
            await operation();
        }
        catch (T exception)
        {
            return exception;
        }
        throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown.");
    }

    #endregion
}
