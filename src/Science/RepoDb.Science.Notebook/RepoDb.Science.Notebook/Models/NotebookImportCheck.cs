namespace RepoDb.Science;

/// <summary>
/// The outcome of checking the imports.
/// </summary>
public sealed class NotebookImportCheck
{
    #region Constructors

    /// <summary>
    /// Creates an import check.
    /// </summary>
    /// <param name="entries">The accepted directives.</param>
    /// <param name="errors">The rejected lines.</param>
    /// <param name="warnings">The suspicious lines.</param>
    public NotebookImportCheck(IReadOnlyList<NotebookImportEntry> entries,
        IReadOnlyList<NotebookImportIssue> errors,
        IReadOnlyList<NotebookImportIssue> warnings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(errors);
        ArgumentNullException.ThrowIfNull(warnings);
        Entries = entries;
        Errors = errors;
        Warnings = warnings;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the accepted directives.
    /// </summary>
    public IReadOnlyList<NotebookImportEntry> Entries { get; }

    /// <summary>
    /// Gets the rejected lines.
    /// </summary>
    public IReadOnlyList<NotebookImportIssue> Errors { get; }

    /// <summary>
    /// Gets whether every line is allowed.
    /// </summary>
    public bool IsValid => Errors.Count == 0;

    /// <summary>
    /// Gets the outcome in words.
    /// </summary>
    public string Summary => IsValid ? ValidSummary() : $"{Errors.Count} {(Errors.Count == 1 ? "problem" : "problems")} found.";

    /// <summary>
    /// Gets the suspicious lines.
    /// </summary>
    public IReadOnlyList<NotebookImportIssue> Warnings { get; }

    #endregion

    #region Private Methods

    /// <summary>
    /// Describes a check without errors.
    /// </summary>
    /// <returns>The description.</returns>
    private string ValidSummary() => Entries.Count == 0
        ? "No imports yet."
        : $"{Entries.Count} {(Entries.Count == 1 ? "import is" : "imports are")} valid.";

    #endregion
}
