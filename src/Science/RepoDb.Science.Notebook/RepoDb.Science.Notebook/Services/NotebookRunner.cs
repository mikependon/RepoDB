using System.Diagnostics;

namespace RepoDb.Science.Services;

/// <summary>
/// Runs the sections of a notebook one after the other.
/// </summary>
internal static class NotebookRunner
{
    #region Private Variables

    private const string Stopped = "Stopped by the user.";

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the imports and then the other sections, skipping the ones after a failure.
    /// </summary>
    /// <param name="sections">The sections of the notebook.</param>
    /// <param name="context">What the run needs.</param>
    /// <param name="progress">Receives a report after each section.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The outcome.</returns>
    public static async Task<NotebookRunResult> RunAsync(IEnumerable<NotebookSection> sections,
        NotebookExecutionContext context,
        IProgress<NotebookRunProgress>? progress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        var stopwatch = Stopwatch.StartNew();
        var planned = Plan(sections);
        var items = new List<NotebookRunItem>();
        var failure = string.Empty;
        foreach (var section in planned)
        {
            failure = failure.Length == 0 && cancellationToken.IsCancellationRequested ? Stopped : failure;
            var name = NameOf(section,
                items.Count);
            var item = failure.Length > 0
                ? Skipped(section,
                    name,
                    failure)
                : await RunSectionAsync(section,
                    name,
                    context,
                    cancellationToken)
                    .ConfigureAwait(true);
            failure = failure.Length > 0 ? failure : FailureOf(item,
                cancellationToken);
            items.Add(item);
            progress?.Report(new NotebookRunProgress(items.Count,
                planned.Count,
                item));
        }
        return new NotebookRunResult(items,
            failure,
            stopwatch.Elapsed);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets why the run must stop after a section.
    /// </summary>
    /// <param name="item">The section just run.</param>
    /// <param name="cancellationToken">The token that stops the run.</param>
    /// <returns>The reason, or empty.</returns>
    private static string FailureOf(NotebookRunItem item,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Stopped;
        }
        return item.State == NotebookRunState.Failed ? $"Stopped: {item.Name} failed." : string.Empty;
    }

    /// <summary>
    /// Gets the name of a section in the run.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="index">The position in the run.</param>
    /// <returns>The name.</returns>
    private static string NameOf(NotebookExecutableSection section,
        int index) => section switch
        {
            NotebookImportsSection => "Imports",
            NotebookChartSection => $"CHART {index + 1}",
            NotebookCodeSection { Language: NotebookCodeLanguage.CSharp } => $"C# {index + 1}",
            _ => ((NotebookCodeSection)section).Name
        };

    /// <summary>
    /// Orders the sections that run, with the imports first.
    /// </summary>
    /// <param name="sections">The sections of the notebook.</param>
    /// <returns>The sections to run.</returns>
    private static List<NotebookExecutableSection> Plan(IEnumerable<NotebookSection> sections) => sections
        .OfType<NotebookExecutableSection>()
        .OrderBy(section => section is NotebookImportsSection ? 0 : 1)
        .ToList();

    /// <summary>
    /// Runs a section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="name">The name in the run.</param>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The outcome of the section.</returns>
    private static async Task<NotebookRunItem> RunSectionAsync(NotebookExecutableSection section,
        string name,
        NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            await section.ExecuteAsync(context,
                cancellationToken)
                .ConfigureAwait(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The section recorded that it was stopped.
        }
        return new NotebookRunItem(section,
            name,
            section.State,
            section.StatusMessage,
            section.Elapsed);
    }

    /// <summary>
    /// Records a section that does not run.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="name">The name in the run.</param>
    /// <param name="reason">Why it does not run.</param>
    /// <returns>The outcome of the section.</returns>
    private static NotebookRunItem Skipped(NotebookExecutableSection section,
        string name,
        string reason) => new(section,
            name,
            NotebookRunState.Skipped,
            reason,
            TimeSpan.Zero);

    #endregion
}
