using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The base of the sections that run a SQL or a C# script.
/// </summary>
public abstract class NotebookScriptSection : NotebookExecutableSection
{
    #region Private Variables

    private Guid? _connectionId;
    private string _output = string.Empty;
    private IReadOnlyList<NotebookTable> _results = [];
    private string _resultStatus = string.Empty;
    private double _tableHeight;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a script section.
    /// </summary>
    private protected NotebookScriptSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the identifier of the connection of the SQL script.
    /// </summary>
    public Guid? ConnectionId
    {
        get => _connectionId;
        set => SetContent(ref _connectionId,
            value);
    }

    /// <summary>
    /// Gets what the last C# script wrote and returned.
    /// </summary>
    public string Output
    {
        get => _output;
        private set => SetProperty(ref _output,
            value);
    }

    /// <summary>
    /// Gets the tables of the last run.
    /// </summary>
    public IReadOnlyList<NotebookTable> Results
    {
        get => _results;
        private protected set
        {
            if (SetProperty(ref _results,
                value))
            {
                OnResultsChanged();
            }
        }
    }

    /// <summary>
    /// Gets the summary of the tables of the last run.
    /// </summary>
    public string ResultStatus
    {
        get => _resultStatus;
        private protected set => SetProperty(ref _resultStatus,
            value);
    }

    /// <summary>
    /// Gets the language of the script that runs.
    /// </summary>
    public abstract NotebookCodeLanguage ScriptLanguage { get; }

    /// <summary>
    /// Gets the script that runs.
    /// </summary>
    public abstract string ScriptToRun { get; }

    /// <summary>
    /// Gets or sets the table height, where zero means automatic.
    /// </summary>
    public double TableHeight
    {
        get => _tableHeight;
        set => SetProperty(ref _tableHeight,
            NotebookValues.Height(value));
    }

    /// <summary>
    /// Gets the text the preview comes from.
    /// </summary>
    private protected override string PreviewSource => ScriptToRun;

    #endregion

    #region Internal Methods

    /// <summary>
    /// Restores the saved results.
    /// </summary>
    /// <param name="output">The script output.</param>
    /// <param name="results">The tables.</param>
    /// <param name="status">The summary.</param>
    internal void RestoreResults(string output,
        IReadOnlyList<NotebookTable> results,
        string status)
    {
        Output = output;
        Results = results;
        ResultStatus = status;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Uses the outcome of a successful C# script.
    /// </summary>
    /// <param name="chart">The table given to the chart, already limited, or null.</param>
    /// <returns>The result.</returns>
    private protected virtual NotebookSectionResult CompleteCSharp(NotebookTable? chart)
    {
        Log.Add(NotebookLogLevel.Information,
            "The script ran well.");
        return NotebookSectionResult.Succeeded();
    }

    /// <summary>
    /// Uses the tables of a successful SQL script.
    /// </summary>
    /// <param name="tables">The tables, already limited.</param>
    /// <returns>The result.</returns>
    private protected virtual NotebookSectionResult CompleteSql(IReadOnlyList<NotebookTable> tables)
    {
        Show(tables);
        if (tables
            .Any(table => table.IsTruncated))
        {
            Log.Add(NotebookLogLevel.Warning,
                "The rows were limited to the row limit.");
        }
        else
        {
            Log.Add(NotebookLogLevel.Information,
                $"Done: {ResultStatus}");
        }
        return NotebookSectionResult.Succeeded(ResultStatus);
    }

    /// <summary>
    /// Reacts to new results.
    /// </summary>
    private protected virtual void OnResultsChanged()
    {
    }

    /// <summary>
    /// Removes the output of a run.
    /// </summary>
    private protected override void ResetOutput()
    {
        base.ResetOutput();
        Output = string.Empty;
        ResultStatus = string.Empty;
        Results = [];
    }

    /// <summary>
    /// Runs the script after the guard allows it.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private protected override async Task<NotebookSectionResult> RunAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        if (context.Guard is { } guard && await guard.CheckAsync(this,
            cancellationToken)
            .ConfigureAwait(true) is { } reason)
        {
            return Fail(reason);
        }
        var run = ScriptLanguage == NotebookCodeLanguage.Sql
            ? RunSqlAsync(context,
                cancellationToken)
            : RunCSharpAsync(context,
                cancellationToken);
        return await run.ConfigureAwait(true);
    }

    /// <summary>
    /// Applies the outcome of a C# script.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <param name="context">What the run needs.</param>
    /// <returns>The result.</returns>
    private NotebookSectionResult ApplyCSharp(NotebookScriptResult result,
        NotebookExecutionContext context)
    {
        foreach (var entry in result.Logs)
        {
            Log.Add(entry.Level,
                entry.Message);
        }
        if (result.IsUnavailable)
        {
            return Skip(result.Error ?? "The C# executor is not available.");
        }
        Output = OutputOf(result,
            context.OutputLimit);
        if (result.Tables.Count > 0)
        {
            Show(NotebookTableLimiter.Limit(result.Tables,
                context.RowLimit));
        }
        if (!result.Succeeded)
        {
            return Fail(result.Error ?? "The script failed.");
        }
        return CompleteCSharp(result.Chart is { } chart
            ? NotebookTableLimiter.Limit([chart],
                context.RowLimit)[0]
            : null);
    }

    /// <summary>
    /// Writes the output and the returned value of a script.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <param name="limit">The output lines kept.</param>
    /// <returns>The text.</returns>
    private static string OutputOf(NotebookScriptResult result,
        int limit)
    {
        var parts = new[]
        {
            NotebookOutputTrimmer.LastLines(result.Output.TrimEnd(),
                limit),
            result.ReturnValue is { } value ? $"Returned: {value}" : string.Empty
        };
        return string.Join(Environment.NewLine,
            parts
                .Where(part => part.Length > 0));
    }

    /// <summary>
    /// Runs the C# script, or skips it without an executor.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private async Task<NotebookSectionResult> RunCSharpAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        if (context.CSharpExecutor is not { } executor)
        {
            return Skip("C# scripts cannot be run yet.");
        }
        var sqlResults = Owner?.GetSqlResults() ?? new Dictionary<string, IReadOnlyList<NotebookTable>>(StringComparer.OrdinalIgnoreCase);
        var request = new NotebookScriptRequest(ScriptToRun,
            Owner?.ImportsScript ?? string.Empty,
            sqlResults,
            context.RowLimit);
        var result = await executor.ExecuteAsync(request,
            cancellationToken)
            .ConfigureAwait(true);
        return ApplyCSharp(result,
            context);
    }

    /// <summary>
    /// Runs the SQL script.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private async Task<NotebookSectionResult> RunSqlAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        if (context.SqlExecutor is not { } executor)
        {
            return Fail("No SQL executor is configured.");
        }
        if (string.IsNullOrWhiteSpace(ScriptToRun))
        {
            return Fail("Type the SQL script to run.");
        }
        Log.Add(NotebookLogLevel.Information,
            "Running the script.");
        var request = new NotebookSqlRequest(ScriptToRun,
            ConnectionId,
            context.RowLimit);
        var tables = await executor.ExecuteAsync(request,
            cancellationToken)
            .ConfigureAwait(true);
        return CompleteSql(NotebookTableLimiter.Limit(tables,
            context.RowLimit));
    }

    /// <summary>
    /// Shows tables as the results.
    /// </summary>
    /// <param name="tables">The tables.</param>
    private void Show(IReadOnlyList<NotebookTable> tables)
    {
        var rowCount = tables
            .Sum(table => table.Rows.Count);
        ResultStatus = $"{rowCount} row(s)";
        Results = tables;
    }

    #endregion
}
