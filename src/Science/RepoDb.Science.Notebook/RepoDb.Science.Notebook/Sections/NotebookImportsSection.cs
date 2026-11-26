using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The using directives of the C# scripts of a notebook.
/// </summary>
public sealed class NotebookImportsSection : NotebookExecutableSection
{
    #region Private Variables

    private NotebookImportCheck? _check;
    private string _script = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty imports section.
    /// </summary>
    public NotebookImportsSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the outcome of the last check, or null.
    /// </summary>
    public NotebookImportCheck? Check
    {
        get => _check;
        private set => SetProperty(ref _check,
            value);
    }

    /// <summary>
    /// Gets or sets the using directives.
    /// </summary>
    public string Script
    {
        get => _script;
        set
        {
            if (SetContent(ref _script,
                    value ?? string.Empty) && Check is not null)
            {
                Check = NotebookImportValidator.Check(Script);
            }
        }
    }

    /// <summary>
    /// Gets the kind of section.
    /// </summary>
    public override NotebookSectionType Type => NotebookSectionType.Imports;

    /// <summary>
    /// Gets the text the preview comes from.
    /// </summary>
    private protected override string PreviewSource => Script;

    #endregion

    #region Internal Methods

    /// <summary>
    /// Checks the directives of a loaded section.
    /// </summary>
    internal void RestoreCheck() => Check = string.IsNullOrWhiteSpace(Script) ? null : NotebookImportValidator.Check(Script);

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the authored values.
    /// </summary>
    /// <returns>The values.</returns>
    private protected override IEnumerable<string?> GetContent() => [Script];

    /// <summary>
    /// Removes the output of a run.
    /// </summary>
    private protected override void ResetOutput()
    {
        base.ResetOutput();
        Check = null;
    }

    /// <summary>
    /// Checks the directives.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private protected override Task<NotebookSectionResult> RunAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        var check = NotebookImportValidator.Check(Script);
        Check = check;
        LogIssues(check);
        return Task.FromResult(Conclude(check));
    }

    /// <summary>
    /// Logs the summary and builds the result.
    /// </summary>
    /// <param name="check">The check.</param>
    /// <returns>The result.</returns>
    private NotebookSectionResult Conclude(NotebookImportCheck check)
    {
        Log.Add(check.IsValid ? NotebookLogLevel.Information : NotebookLogLevel.Error,
            check.Summary);
        return check.IsValid
            ? NotebookSectionResult.Succeeded(check.Summary)
            : NotebookSectionResult.Failed(check.Errors[0].Text);
    }

    /// <summary>
    /// Logs the problems and warnings.
    /// </summary>
    /// <param name="check">The check.</param>
    private void LogIssues(NotebookImportCheck check)
    {
        foreach (var error in check.Errors)
        {
            Log.Add(NotebookLogLevel.Error,
                error.Text);
        }
        foreach (var warning in check.Warnings)
        {
            Log.Add(NotebookLogLevel.Warning,
                warning.Text);
        }
    }

    #endregion
}
