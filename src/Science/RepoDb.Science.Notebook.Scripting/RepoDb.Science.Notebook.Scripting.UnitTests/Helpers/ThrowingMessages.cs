namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// The texts of a host that fails to describe an exception.
/// </summary>
internal sealed class ThrowingMessages : NotebookScriptingMessages
{
    #region Private Variables

    private readonly Exception _failure;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the texts.
    /// </summary>
    /// <param name="failure">What describing an exception throws.</param>
    public ThrowingMessages(Exception failure)
    {
        _failure = failure;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Fails.
    /// </summary>
    /// <param name="exception">The exception of the script.</param>
    /// <returns>Never returns.</returns>
    public override string RuntimeError(Exception exception) => throw _failure;

    #endregion
}
