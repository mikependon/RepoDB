namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A SQL executor that answers from a function.
/// </summary>
internal sealed class FakeSqlExecutor : INotebookSqlExecutor
{
    #region Private Variables

    private readonly Func<NotebookSqlRequest, IReadOnlyList<NotebookTable>> _run;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an executor that returns tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    public FakeSqlExecutor(params NotebookTable[] tables) => _run = _ => tables;

    /// <summary>
    /// Creates an executor that runs a function.
    /// </summary>
    /// <param name="run">The function.</param>
    public FakeSqlExecutor(Func<NotebookSqlRequest, IReadOnlyList<NotebookTable>> run) => _run = run;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the requests received.
    /// </summary>
    public List<NotebookSqlRequest> Requests { get; } = [];

    #endregion

    #region Public Methods

    /// <summary>
    /// Records the request and answers it.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The tables.</returns>
    public Task<IReadOnlyList<NotebookTable>> ExecuteAsync(NotebookSqlRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        return Task.FromResult(_run(request));
    }

    #endregion
}
