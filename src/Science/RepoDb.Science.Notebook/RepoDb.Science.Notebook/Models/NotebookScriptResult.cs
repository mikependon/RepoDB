namespace RepoDb.Science;

/// <summary>
/// The outcome of a C# script.
/// </summary>
public sealed class NotebookScriptResult
{
    #region Properties

    /// <summary>
    /// Gets the table given to the chart.
    /// </summary>
    public NotebookTable? Chart { get; init; }

    /// <summary>
    /// Gets why the script failed or could not run.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets whether the script could not run at all.
    /// </summary>
    public bool IsUnavailable { get; init; }

    /// <summary>
    /// Gets the log entries of the script.
    /// </summary>
    public IReadOnlyList<NotebookLogEntry> Logs { get; init; } = [];

    /// <summary>
    /// Gets the console output.
    /// </summary>
    public string Output { get; init; } = string.Empty;

    /// <summary>
    /// Gets the returned value as text.
    /// </summary>
    public string? ReturnValue { get; init; }

    /// <summary>
    /// Gets whether the script ran to its end.
    /// </summary>
    public bool Succeeded { get; init; }

    /// <summary>
    /// Gets the tables the script showed.
    /// </summary>
    public IReadOnlyList<NotebookTable> Tables { get; init; } = [];

    #endregion
}
