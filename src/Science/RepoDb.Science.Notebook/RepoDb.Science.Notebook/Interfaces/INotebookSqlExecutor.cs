namespace RepoDb.Science;

/// <summary>
/// Runs the SQL scripts of a notebook.
/// </summary>
public interface INotebookSqlExecutor
{
    #region Public Methods

    /// <summary>
    /// Runs a SQL script.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the script.</returns>
    Task<NotebookSqlResult> ExecuteAsync(NotebookSqlRequest request,
        CancellationToken cancellationToken = default);

    #endregion
}
