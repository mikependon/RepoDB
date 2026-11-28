using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// One run of a script, which builds its outcome.
/// </summary>
internal sealed class NotebookScriptRun
{
    #region Private Variables

    private readonly NotebookConsoleCapture _console;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a run.
    /// </summary>
    /// <param name="request">The script that runs.</param>
    /// <param name="host">What the script sees as Notebook.</param>
    /// <param name="console">The captured console.</param>
    public NotebookScriptRun(NotebookScriptRequest request,
        NotebookScriptHost host,
        NotebookConsoleCapture console)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(console);
        Request = request;
        Host = host;
        _console = console;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets what the script sees as Notebook.
    /// </summary>
    public NotebookScriptHost Host { get; }

    /// <summary>
    /// Gets the number of lines the imports take before the code.
    /// </summary>
    public int ImportLines => NotebookScriptFactory.ImportLinesOf(Request.Imports);

    /// <summary>
    /// Gets the script that runs.
    /// </summary>
    public NotebookScriptRequest Request { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Builds the outcome of a script that failed.
    /// </summary>
    /// <param name="logs">Why it failed, with the main reason first.</param>
    /// <returns>The outcome.</returns>
    public NotebookScriptResult Failure(IReadOnlyList<NotebookLogEntry> logs)
    {
        ArgumentNullException.ThrowIfNull(logs);
        return Result(logs,
            logs[0].Message,
            null);
    }

    /// <summary>
    /// Builds the outcome of a script that ran to its end, showing the tables of the returned value.
    /// </summary>
    /// <param name="returned">The value the script returned.</param>
    /// <returns>The outcome.</returns>
    public NotebookScriptResult Success(object? returned)
    {
        Host.Tables.AddFrom(returned);
        return Result([],
            null,
            NotebookScriptValueFormatter.Format(returned));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds an outcome, which succeeded when it has no error.
    /// </summary>
    /// <param name="logs">The log entries.</param>
    /// <param name="error">Why the script failed, or null.</param>
    /// <param name="returnValue">The returned value as text, or null.</param>
    /// <returns>The outcome.</returns>
    private NotebookScriptResult Result(IReadOnlyList<NotebookLogEntry> logs,
        string? error,
        string? returnValue) => new()
        {
            Succeeded = error is null,
            Error = error,
            Logs = logs,
            Output = _console.Output,
            ReturnValue = returnValue,
            Tables = Tables(),
            Chart = Chart()
        };

    /// <summary>
    /// Converts the shown tables.
    /// </summary>
    /// <returns>The tables.</returns>
    private List<NotebookTable> Tables() => Host.Tables.Items
        .Select(table => NotebookDataTableConverter.ToNotebookTable(table,
            Request.RowLimit))
        .ToList();

    /// <summary>
    /// Converts the charted table.
    /// </summary>
    /// <returns>The table, or null when the script gave none.</returns>
    private NotebookTable? Chart() => Host.Chart.Table is { } chart
        ? NotebookDataTableConverter.ToNotebookTable(chart,
            Request.RowLimit)
        : null;

    #endregion
}
