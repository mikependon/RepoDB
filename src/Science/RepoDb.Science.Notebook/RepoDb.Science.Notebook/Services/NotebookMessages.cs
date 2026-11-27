namespace RepoDb.Science;

/// <summary>
/// The texts a notebook writes. A host overrides what differs; an empty log line is not written.
/// </summary>
public class NotebookMessages
{
    #region Constructors

    /// <summary>
    /// Creates the default messages.
    /// </summary>
    public NotebookMessages()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the shared default messages.
    /// </summary>
    public static NotebookMessages Default { get; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// The description of the header of a new notebook.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string NewNotebookDescription() => NotebookDocument.DefaultDescription;

    /// <summary>
    /// Written when a running section is cancelled.
    /// </summary>
    /// <param name="section">The cancelled section.</param>
    /// <returns>The text.</returns>
    public virtual string SectionStopped(NotebookExecutableSection section) => "Stopped by the user.";

    /// <summary>
    /// Why a run of all sections stopped after a cancellation.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string RunStopped() => "Stopped by the user.";

    /// <summary>
    /// Why a run of all sections stopped after a failure.
    /// </summary>
    /// <param name="name">The name of the failed step.</param>
    /// <returns>The text.</returns>
    public virtual string RunFailed(string name) => $"Stopped: {name} failed.";

    /// <summary>
    /// The name of the imports step of a run.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ImportsStepName() => "Imports";

    /// <summary>
    /// The name of a chart step of a run.
    /// </summary>
    /// <param name="number">The position of the step, from one.</param>
    /// <returns>The text.</returns>
    public virtual string ChartStepName(int number) => $"CHART {number}";

    /// <summary>
    /// The name of a C# step of a run.
    /// </summary>
    /// <param name="number">The position of the step, from one.</param>
    /// <returns>The text.</returns>
    public virtual string CSharpStepName(int number) => $"C# {number}";

    /// <summary>
    /// Written when SQL cannot run.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string SqlExecutorMissing() => "No SQL executor is configured.";

    /// <summary>
    /// Written when there is no SQL to run.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string SqlScriptMissing() => "Type the SQL script to run.";

    /// <summary>
    /// Written before a SQL script runs.
    /// </summary>
    /// <param name="section">The section that runs.</param>
    /// <returns>The text.</returns>
    public virtual string SqlStarted(NotebookScriptSection section) => "Running the script.";

    /// <summary>
    /// Written when a SQL script ran.
    /// </summary>
    /// <param name="section">The section that ran.</param>
    /// <param name="status">The result status.</param>
    /// <returns>The text.</returns>
    public virtual string SqlCompleted(NotebookScriptSection section,
        string status) => $"Done: {status}";

    /// <summary>
    /// Written when rows of a SQL result were left out.
    /// </summary>
    /// <param name="section">The section that ran.</param>
    /// <param name="rowLimit">The row limit.</param>
    /// <returns>The text.</returns>
    public virtual string SqlRowsLimited(NotebookScriptSection section,
        int rowLimit) => "The rows were limited to the row limit.";

    /// <summary>
    /// Written when a SQL script fails and its executor logged no error.
    /// </summary>
    /// <param name="section">The section that failed.</param>
    /// <param name="error">Why it failed.</param>
    /// <returns>The text.</returns>
    public virtual string SqlFailed(NotebookScriptSection section,
        string error) => error;

    /// <summary>
    /// The result status of a table.
    /// </summary>
    /// <param name="rowCount">The number of rows.</param>
    /// <returns>The text.</returns>
    public virtual string RowCount(int rowCount) => $"{rowCount} row(s)";

    /// <summary>
    /// The result status of a script that changed rows and returned no table.
    /// </summary>
    /// <param name="rowCount">The number of affected rows.</param>
    /// <returns>The text.</returns>
    public virtual string RowsAffected(int rowCount) => $"{rowCount} row(s) affected";

    /// <summary>
    /// Why the built-in executor refused a script.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string WritesNotAllowed() => "Only SELECT statements can run because writes are not allowed.";

    /// <summary>
    /// Written when C# cannot run.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string CSharpExecutorMissing() => "C# scripts cannot be run yet.";

    /// <summary>
    /// Written when the C# executor is unavailable and gives no reason.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string CSharpUnavailable() => "The C# executor is not available.";

    /// <summary>
    /// Written when a C# script ran.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string CSharpSucceeded() => "The script ran well.";

    /// <summary>
    /// Written when a C# script fails and gives no reason.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string CSharpFailed() => "The script failed.";

    /// <summary>
    /// The output line of the value a C# script returned.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    public virtual string ReturnedValue(string value) => $"Returned: {value}";

    /// <summary>
    /// Why a SQL chart has no table.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string SqlChartTableMissing() => "The script returned no result set to chart.";

    /// <summary>
    /// Why a C# chart has no table.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string CSharpChartTableMissing() => "The script did not give a table to the chart, so there is nothing to chart.";

    /// <summary>
    /// Written when a chart was drawn.
    /// </summary>
    /// <param name="rowCount">The number of rows.</param>
    /// <param name="seriesCount">The number of series.</param>
    /// <returns>The text.</returns>
    public virtual string Charted(int rowCount,
        int seriesCount) => $"Charted {rowCount} rows as {seriesCount} series.";

    /// <summary>
    /// Written when the first column labels the points instead of timing them.
    /// </summary>
    /// <param name="column">The column name.</param>
    /// <returns>The text.</returns>
    public virtual string ChartLabelColumn(string column) => $"The first column '{column}' is not a timestamp: each row is a point, labeled by it.";

    /// <summary>
    /// Written when columns that are not numeric were skipped.
    /// </summary>
    /// <param name="columns">The column names.</param>
    /// <returns>The text.</returns>
    public virtual string ChartColumnsSkipped(IReadOnlyList<string> columns)
    {
        var names = string.Join(", ",
            columns);
        return $"Skipped the columns that are not numeric: {names}.";
    }

    /// <summary>
    /// Why a table without rows cannot be charted.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ChartHasNoRows() => "The result has no rows to chart.";

    /// <summary>
    /// Why a table with one column cannot be charted.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ChartHasTooFewColumns() => "A chart needs a first column that labels its points and at least one numeric column after it.";

    /// <summary>
    /// Why a table without a readable time cannot be charted.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ChartHasNoTimestamp() => "No row has a valid timestamp in its first column.";

    /// <summary>
    /// Why a column that is not numeric fails the chart.
    /// </summary>
    /// <param name="column">The column name.</param>
    /// <param name="typeName">The column type name.</param>
    /// <returns>The text.</returns>
    public virtual string ChartColumnNotNumeric(string column,
        string typeName) => $"The column '{column}' is not numeric ({typeName}): every column after the first one must be numeric.";

    /// <summary>
    /// Why a table without numbers cannot be charted.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ChartHasNoNumericColumn() => "The result has no numeric column after the first one.";

    /// <summary>
    /// The problem of code that is not a using directive.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ImportCodeNotAllowed() => "Only 'using' directives are allowed here, no other code.";

    /// <summary>
    /// The problem of a using directive that imports nothing.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ImportTargetMissing() => "Write what to import after 'using' (for example: using System.Linq;).";

    /// <summary>
    /// The problem of a using statement.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string ImportStatementNotAllowed() => "Only 'using' directives (using System.Linq;) are allowed, not 'using' statements.";

    /// <summary>
    /// The problem of an invalid namespace or type name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The text.</returns>
    public virtual string ImportNameInvalid(string name) => $"'{name}' is not a valid namespace or type name.";

    /// <summary>
    /// The problem of an invalid alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>The text.</returns>
    public virtual string ImportAliasInvalid(string alias) => $"'{alias}' is not a valid alias (write it as: using Name = Namespace;).";

    /// <summary>
    /// The problem of a directive without a semicolon.
    /// </summary>
    /// <param name="statement">The directive.</param>
    /// <returns>The text.</returns>
    public virtual string ImportSemicolonMissing(string statement) => $"Missing ';' at the end of '{statement}'.";

    /// <summary>
    /// The warning of a repeated directive.
    /// </summary>
    /// <param name="directive">The directive.</param>
    /// <returns>The text.</returns>
    public virtual string ImportDuplicate(string directive) => $"'{directive}' is already imported.";

    /// <summary>
    /// A problem written with its line.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="message">The problem.</param>
    /// <returns>The text.</returns>
    public virtual string ImportIssue(int line,
        string message) => $"Line {line}: {message}";

    /// <summary>
    /// The summary of imports without directives.
    /// </summary>
    /// <returns>The text.</returns>
    public virtual string NoImports() => "No imports yet.";

    /// <summary>
    /// The summary of valid imports.
    /// </summary>
    /// <param name="count">The number of directives.</param>
    /// <returns>The text.</returns>
    public virtual string ImportsValid(int count) => $"{count} {(count == 1 ? "import is" : "imports are")} valid.";

    /// <summary>
    /// The summary of imports with problems.
    /// </summary>
    /// <param name="count">The number of problems.</param>
    /// <returns>The text.</returns>
    public virtual string ImportProblems(int count) => $"{count} {(count == 1 ? "problem" : "problems")} found.";

    #endregion
}
