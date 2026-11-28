namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// An exception whose message cannot be read.
/// </summary>
internal sealed class FailingMessage : Exception
{
    #region Private Variables

    private readonly Exception _failure;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the exception.
    /// </summary>
    /// <param name="failure">What reading its message throws.</param>
    public FailingMessage(Exception failure)
    {
        _failure = failure;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Fails.
    /// </summary>
    public override string Message => throw _failure;

    #endregion
}
