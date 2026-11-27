namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Messages that remove some lines.
/// </summary>
internal sealed class SilentMessages : NotebookMessages
{
    #region Public Methods

    /// <summary>
    /// Writes nothing before SQL runs.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>Empty.</returns>
    public override string SqlStarted(NotebookScriptSection section) => string.Empty;

    /// <summary>
    /// Says nothing about a cancelled run.
    /// </summary>
    /// <returns>Empty.</returns>
    public override string RunStopped() => string.Empty;

    /// <summary>
    /// Says nothing about a failed run.
    /// </summary>
    /// <param name="name">The step name.</param>
    /// <returns>Empty.</returns>
    public override string RunFailed(string name) => string.Empty;

    /// <summary>
    /// Gives an import problem no text.
    /// </summary>
    /// <returns>Empty.</returns>
    public override string ImportCodeNotAllowed() => string.Empty;

    #endregion
}
