namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A SQL executor that answers from a function.
/// </summary>
internal sealed class FakeSqlExecutor : INotebookSqlExecutor
{
    #region Private Variables

    private readonly Func<NotebookSqlRequest, NotebookSqlResult> _run;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an executor that returns tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    public FakeSqlExecutor(params NotebookTable[] tables)
    {
        _run = _ => new NotebookSqlResult { Tables = tables };
    }

    /// <summary>
    /// Creates an executor that gets its tables from a function.
    /// </summary>
    /// <param name="run">The function.</param>
    public FakeSqlExecutor(Func<NotebookSqlRequest, IReadOnlyList<NotebookTable>> run)
    {
        _run = request => new NotebookSqlResult { Tables = run(request) };
    }

    /// <summary>
    /// Creates an executor that gets its outcome from a function.
    /// </summary>
    /// <param name="run">The function.</param>
    /// <param name="_">Tells this constructor apart from the table one.</param>
    private FakeSqlExecutor(Func<NotebookSqlRequest, NotebookSqlResult> run,
        bool _)
    {
        _run = run;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the requests received.
    /// </summary>
    public List<NotebookSqlRequest> Requests { get; } = [];

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates an executor that gets its outcome from a function.
    /// </summary>
    /// <param name="run">The function.</param>
    /// <returns>The executor.</returns>
    public static FakeSqlExecutor Answering(Func<NotebookSqlRequest, NotebookSqlResult> run) => new(run,
        true);

    /// <summary>
    /// Records the request and answers it.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome.</returns>
    public Task<NotebookSqlResult> ExecuteAsync(NotebookSqlRequest request,
        CancellationToken cancellationToken = default)
    {
        Requests.Add(request);
        return Task.FromResult(_run(request));
    }

    #endregion
}
