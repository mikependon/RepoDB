namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A C# executor that returns a prepared outcome.
/// </summary>
internal sealed class FakeCSharpExecutor : INotebookCSharpExecutor
{
    #region Private Variables

    private readonly NotebookScriptResult _result;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an executor.
    /// </summary>
    /// <param name="result">The outcome to return.</param>
    public FakeCSharpExecutor(NotebookScriptResult result) => _result = result;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the requests received.
    /// </summary>
    public List<NotebookScriptRequest> Requests { get; } = [];

    #endregion

    #region Public Methods

    /// <summary>
    /// Records the request and returns the outcome.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome.</returns>
    public Task<NotebookScriptResult> ExecuteAsync(NotebookScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        return Task.FromResult(_result);
    }

    #endregion
}
