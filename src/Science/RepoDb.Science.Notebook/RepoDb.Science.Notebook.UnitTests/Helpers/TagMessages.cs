namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Messages that name themselves, so a test can tell which one was used.
/// </summary>
internal sealed class TagMessages : NotebookMessages
{
    #region Public Methods

    /// <summary>
    /// The description of a new notebook.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string NewNotebookDescription() => "description";

    /// <summary>
    /// A cancelled section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The tag.</returns>
    public override string SectionStopped(NotebookExecutableSection section) => $"stopped:{section.Type}";

    /// <summary>
    /// A cancelled run.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string RunStopped() => "run-stopped";

    /// <summary>
    /// A failed run.
    /// </summary>
    /// <param name="name">The step name.</param>
    /// <returns>The tag.</returns>
    public override string RunFailed(string name) => $"run-failed:{name}";

    /// <summary>
    /// The imports step.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ImportsStepName() => "imports-step";

    /// <summary>
    /// A chart step.
    /// </summary>
    /// <param name="number">The position.</param>
    /// <returns>The tag.</returns>
    public override string ChartStepName(int number) => $"chart-step:{number}";

    /// <summary>
    /// A C# step.
    /// </summary>
    /// <param name="number">The position.</param>
    /// <returns>The tag.</returns>
    public override string CSharpStepName(int number) => $"csharp-step:{number}";

    /// <summary>
    /// No SQL executor.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string SqlExecutorMissing() => "sql-executor-missing";

    /// <summary>
    /// No SQL script.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string SqlScriptMissing() => "sql-script-missing";

    /// <summary>
    /// SQL started.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The tag.</returns>
    public override string SqlStarted(NotebookScriptSection section) => $"sql-started:{section.ConnectionId}";

    /// <summary>
    /// SQL completed.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="status">The status.</param>
    /// <returns>The tag.</returns>
    public override string SqlCompleted(NotebookScriptSection section,
        string status) => $"sql-completed:{status}";

    /// <summary>
    /// Rows left out.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="rowLimit">The limit.</param>
    /// <returns>The tag.</returns>
    public override string SqlRowsLimited(NotebookScriptSection section,
        int rowLimit) => $"sql-limited:{rowLimit}";

    /// <summary>
    /// SQL failed.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="error">The error.</param>
    /// <returns>The tag.</returns>
    public override string SqlFailed(NotebookScriptSection section,
        string error) => $"sql-failed:{error}";

    /// <summary>
    /// A row count.
    /// </summary>
    /// <param name="rowCount">The rows.</param>
    /// <returns>The tag.</returns>
    public override string RowCount(int rowCount) => $"rows:{rowCount}";

    /// <summary>
    /// Affected rows.
    /// </summary>
    /// <param name="rowCount">The rows.</param>
    /// <returns>The tag.</returns>
    public override string RowsAffected(int rowCount) => $"affected:{rowCount}";

    /// <summary>
    /// Writes refused.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string WritesNotAllowed() => "writes-not-allowed";

    /// <summary>
    /// No C# executor.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string CSharpExecutorMissing() => "csharp-executor-missing";

    /// <summary>
    /// C# unavailable.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string CSharpUnavailable() => "csharp-unavailable";

    /// <summary>
    /// C# succeeded.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string CSharpSucceeded() => "csharp-succeeded";

    /// <summary>
    /// C# failed.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string CSharpFailed() => "csharp-failed";

    /// <summary>
    /// A returned value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The tag.</returns>
    public override string ReturnedValue(string value) => $"returned:{value}";

    /// <summary>
    /// No SQL chart table.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string SqlChartTableMissing() => "sql-chart-missing";

    /// <summary>
    /// No C# chart table.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string CSharpChartTableMissing() => "csharp-chart-missing";

    /// <summary>
    /// A drawn chart.
    /// </summary>
    /// <param name="rowCount">The rows.</param>
    /// <param name="seriesCount">The series.</param>
    /// <returns>The tag.</returns>
    public override string Charted(int rowCount,
        int seriesCount) => $"charted:{rowCount}:{seriesCount}";

    /// <summary>
    /// A label column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The tag.</returns>
    public override string ChartLabelColumn(string column) => $"label-column:{column}";

    /// <summary>
    /// Skipped columns.
    /// </summary>
    /// <param name="columns">The columns.</param>
    /// <returns>The tag.</returns>
    public override string ChartColumnsSkipped(IReadOnlyList<string> columns) => $"skipped:{columns[0]}";

    /// <summary>
    /// A chart without rows.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ChartHasNoRows() => "chart-no-rows";

    /// <summary>
    /// A chart with too few columns.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ChartHasTooFewColumns() => "chart-few-columns";

    /// <summary>
    /// A chart without times.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ChartHasNoTimestamp() => "chart-no-timestamp";

    /// <summary>
    /// A column that is not numeric.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="typeName">The type.</param>
    /// <returns>The tag.</returns>
    public override string ChartColumnNotNumeric(string column,
        string typeName) => $"not-numeric:{column}:{typeName}";

    /// <summary>
    /// A chart without numbers.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ChartHasNoNumericColumn() => "chart-no-numbers";

    /// <summary>
    /// Code in the imports.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ImportCodeNotAllowed() => "import-code";

    /// <summary>
    /// An empty using.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ImportTargetMissing() => "import-target";

    /// <summary>
    /// A using statement.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string ImportStatementNotAllowed() => "import-statement";

    /// <summary>
    /// An invalid name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The tag.</returns>
    public override string ImportNameInvalid(string name) => $"import-name:{name}";

    /// <summary>
    /// An invalid alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>The tag.</returns>
    public override string ImportAliasInvalid(string alias) => $"import-alias:{alias}";

    /// <summary>
    /// A missing semicolon.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>The tag.</returns>
    public override string ImportSemicolonMissing(string statement) => $"import-semicolon:{statement}";

    /// <summary>
    /// A repeated directive.
    /// </summary>
    /// <param name="directive">The directive.</param>
    /// <returns>The tag.</returns>
    public override string ImportDuplicate(string directive) => $"import-duplicate:{directive}";

    /// <summary>
    /// A problem with its line.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="message">The problem.</param>
    /// <returns>The tag.</returns>
    public override string ImportIssue(int line,
        string message) => $"{line}>{message}";

    /// <summary>
    /// No imports.
    /// </summary>
    /// <returns>The tag.</returns>
    public override string NoImports() => "no-imports";

    /// <summary>
    /// Valid imports.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <returns>The tag.</returns>
    public override string ImportsValid(int count) => $"imports-valid:{count}";

    /// <summary>
    /// Imports with problems.
    /// </summary>
    /// <param name="count">The count.</param>
    /// <returns>The tag.</returns>
    public override string ImportProblems(int count) => $"import-problems:{count}";

    #endregion
}
