namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A value whose text cannot be written.
/// </summary>
internal sealed class FailingTexts
{
    #region Private Variables

    private readonly Exception _failure;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the value.
    /// </summary>
    /// <param name="failure">What writing its text throws.</param>
    public FailingTexts(Exception failure)
    {
        _failure = failure;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Fails.
    /// </summary>
    /// <returns>Never returns.</returns>
    public override string ToString() => throw _failure;

    #endregion
}
