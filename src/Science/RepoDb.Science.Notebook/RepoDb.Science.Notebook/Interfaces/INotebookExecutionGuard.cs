namespace RepoDb.Science;

/// <summary>
/// Decides whether a script may run.
/// </summary>
public interface INotebookExecutionGuard
{
    #region Public Methods

    /// <summary>
    /// Checks a section before its script runs.
    /// </summary>
    /// <param name="section">The section about to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Why the script must not run, or null when it may.</returns>
    Task<string?> CheckAsync(NotebookScriptSection section,
        CancellationToken cancellationToken = default);

    #endregion
}
