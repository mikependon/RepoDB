namespace RepoDb.Science;

/// <summary>
/// Runs the C# scripts of a notebook.
/// </summary>
public interface INotebookCSharpExecutor
{
    #region Public Methods

    /// <summary>
    /// Runs a C# script.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the script.</returns>
    Task<NotebookScriptResult> ExecuteAsync(NotebookScriptRequest request,
        CancellationToken cancellationToken = default);

    #endregion
}
