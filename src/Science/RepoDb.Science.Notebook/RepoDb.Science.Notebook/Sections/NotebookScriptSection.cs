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

    #region Public Methods

    /// <summary>
    /// Cuts the output to its last lines.
    /// </summary>
    /// <param name="limit">The lines to keep.</param>
    public void ApplyOutputLimit(int limit)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);
        Output = NotebookOutputTrimmer.LastLines(Output,
            limit);
    }

    /// <summary>
    /// Replaces the result status, for example when the host shows another table. It is not a content change.
    /// </summary>
    /// <param name="status">The status.</param>
    public void SetResultStatus(string status)
    {
        ArgumentNullException.ThrowIfNull(status);
        ResultStatus = status;
    }

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
    /// <param name="status">The result status of the host, or null.</param>
    /// <returns>The result.</returns>
    private protected virtual NotebookSectionResult CompleteCSharp(NotebookTable? chart,
        string? status) => NotebookSectionResult.Succeeded();

    /// <summary>
    /// Uses the tables of a successful SQL script.
    /// </summary>
    /// <param name="tables">The tables, already limited.</param>
    /// <param name="status">The result status.</param>
    /// <returns>The result.</returns>
    private protected virtual NotebookSectionResult CompleteSql(IReadOnlyList<NotebookTable> tables,
        string status)
    {
        Results = tables;
        ResultStatus = status;
        return NotebookSectionResult.Succeeded(status);
    }

    /// <summary>
    /// Reacts to new results.
    /// </summary>
    private protected virtual void OnResultsChanged()
    {
    }

    /// <summary>
    /// Shows the tables a C# script gave, even when it failed afterwards.
    /// </summary>
    /// <param name="tables">The tables, already limited.</param>
    /// <param name="status">The result status of the host, or null.</param>
    private protected virtual void ShowCSharpTables(IReadOnlyList<NotebookTable> tables,
        string? status)
    {
        Results = tables;
        ResultStatus = status ?? (tables.Count > 0 ? Messages.RowCount(tables[0].Rows.Count) : string.Empty);
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
        WriteEntries(result.Logs);
        if (result.IsUnavailable)
        {
            return Skip(result.Error ?? Messages.CSharpUnavailable());
        }
        ShowCSharp(result,
            context);
        if (!result.Succeeded)
        {
            var error = result.Error ?? Messages.CSharpFailed();
            return FailExecution(error,
                error);
        }
        Write(NotebookLogLevel.Information,
            Messages.CSharpSucceeded());
        return CompleteCSharp(LimitChart(result.Chart,
                context.RowLimit),
            result.Status);
    }

    /// <summary>
    /// Applies the outcome of a SQL script.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <param name="rowLimit">The most rows kept for each table.</param>
    /// <returns>The result.</returns>
    private NotebookSectionResult ApplySql(NotebookSqlResult result,
        int rowLimit)
    {
        if (result.Error is { } error)
        {
            return FailExecution(Messages.SqlFailed(this,
                    error),
                error);
        }
        var tables = NotebookTableLimiter.Limit(result.Tables,
            rowLimit);
        var status = result.Status ?? DefaultStatus(tables,
            result.RowsAffected);
        WriteSqlEnd(tables,
            rowLimit,
            status);
        return CompleteSql(tables,
            status);
    }

    /// <summary>
    /// Builds the default status of a SQL result.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <param name="rowsAffected">The affected rows, or null.</param>
    /// <returns>The status.</returns>
    private string DefaultStatus(IReadOnlyList<NotebookTable> tables,
        int? rowsAffected)
    {
        if (tables.Count == 0 && rowsAffected is { } affected)
        {
            return Messages.RowsAffected(affected);
        }
        return Messages.RowCount(tables.Count > 0 ? tables[0].Rows.Count : 0);
    }

    /// <summary>
    /// Runs the SQL executor, turning its errors into a failed outcome.
    /// </summary>
    /// <param name="executor">The executor.</param>
    /// <param name="rowLimit">The most rows kept for each table.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome.</returns>
    private async Task<NotebookSqlResult> ExecuteSqlAsync(INotebookSqlExecutor executor,
        int rowLimit,
        CancellationToken cancellationToken)
    {
        var request = new NotebookSqlRequest(ScriptToRun,
            ConnectionId,
            rowLimit)
        {
            Log = Log,
            Messages = Messages
        };
        try
        {
            return await executor.ExecuteAsync(request,
                cancellationToken)
                .ConfigureAwait(true);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return new NotebookSqlResult { Error = exception.Message };
        }
    }

    /// <summary>
    /// Limits the table given to a chart.
    /// </summary>
    /// <param name="chart">The table, or null.</param>
    /// <param name="rowLimit">The most rows kept.</param>
    /// <returns>The limited table, or null.</returns>
    private static NotebookTable? LimitChart(NotebookTable? chart,
        int rowLimit) => chart is null
        ? null
        : NotebookTableLimiter.Limit([chart],
            rowLimit)[0];

    /// <summary>
    /// Writes the output and the returned value of a script.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <param name="limit">The output lines kept.</param>
    /// <returns>The text.</returns>
    private string OutputOf(NotebookScriptResult result,
        int limit)
    {
        var parts = new[]
        {
            NotebookOutputTrimmer.LastLines(result.Output.TrimEnd(),
                limit),
            result.ReturnValue is { } value ? Messages.ReturnedValue(value) : string.Empty
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
            return Skip(Messages.CSharpExecutorMissing());
        }
        var result = await executor.ExecuteAsync(ScriptRequest(context.RowLimit),
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
            return Fail(Messages.SqlExecutorMissing());
        }
        if (string.IsNullOrWhiteSpace(ScriptToRun))
        {
            return Fail(Messages.SqlScriptMissing());
        }
        Write(NotebookLogLevel.Information,
            Messages.SqlStarted(this));
        var result = await ExecuteSqlAsync(executor,
            context.RowLimit,
            cancellationToken)
            .ConfigureAwait(true);
        return ApplySql(result,
            context.RowLimit);
    }

    /// <summary>
    /// Builds the request of the C# script.
    /// </summary>
    /// <param name="rowLimit">The most rows kept for each table.</param>
    /// <returns>The request.</returns>
    private NotebookScriptRequest ScriptRequest(int rowLimit) => new(ScriptToRun,
        Owner?.ImportsScript ?? string.Empty,
        Owner?.GetSqlResults() ?? new Dictionary<string, IReadOnlyList<NotebookTable>>(StringComparer.OrdinalIgnoreCase),
        rowLimit);

    /// <summary>
    /// Shows the output and tables of a C# script.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <param name="context">What the run needs.</param>
    private void ShowCSharp(NotebookScriptResult result,
        NotebookExecutionContext context)
    {
        Output = OutputOf(result,
            context.OutputLimit);
        ShowCSharpTables(NotebookTableLimiter.Limit(result.Tables,
                context.RowLimit),
            result.Status);
    }

    /// <summary>
    /// Adds the log lines of a script.
    /// </summary>
    /// <param name="entries">The lines.</param>
    private void WriteEntries(IReadOnlyList<NotebookLogEntry> entries)
    {
        foreach (var entry in entries)
        {
            Write(entry.Level,
                entry.Message);
        }
    }

    /// <summary>
    /// Logs how a successful SQL script ended.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <param name="rowLimit">The row limit.</param>
    /// <param name="status">The result status.</param>
    private void WriteSqlEnd(IReadOnlyList<NotebookTable> tables,
        int rowLimit,
        string status)
    {
        if (tables
            .Any(table => table.IsTruncated))
        {
            Write(NotebookLogLevel.Warning,
                Messages.SqlRowsLimited(this,
                    rowLimit));
        }
        else
        {
            Write(NotebookLogLevel.Information,
                Messages.SqlCompleted(this,
                    status));
        }
    }

    #endregion
}
