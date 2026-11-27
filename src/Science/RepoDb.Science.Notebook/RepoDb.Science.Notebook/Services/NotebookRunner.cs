using System.Diagnostics;

namespace RepoDb.Science.Services;

/// <summary>
/// Plans and runs the sections of a notebook one after the other.
/// </summary>
internal static class NotebookRunner
{
    #region Public Methods

    /// <summary>
    /// Orders the sections that run, with the imports first, and names them.
    /// </summary>
    /// <param name="sections">The sections of the notebook.</param>
    /// <param name="messages">The texts of the notebook.</param>
    /// <returns>The steps.</returns>
    public static IReadOnlyList<NotebookRunStep> Plan(IEnumerable<NotebookSection> sections,
        NotebookMessages messages) => sections
        .OfType<NotebookExecutableSection>()
        .OrderBy(section => section is NotebookImportsSection ? 0 : 1)
        .Select((section,
            index) => new NotebookRunStep(section,
            NameOf(section,
                index + 1,
                messages)))
        .ToList();

    /// <summary>
    /// Runs the steps, skipping the ones after a failure.
    /// </summary>
    /// <param name="steps">The planned steps.</param>
    /// <param name="context">What the run needs.</param>
    /// <param name="messages">The texts of the notebook.</param>
    /// <param name="progress">Receives a report after each section.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The outcome.</returns>
    public static async Task<NotebookRunResult> RunAsync(IReadOnlyList<NotebookRunStep> steps,
        NotebookExecutionContext context,
        NotebookMessages messages,
        IProgress<NotebookRunProgress>? progress,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);
        var stopwatch = Stopwatch.StartNew();
        var items = new List<NotebookRunItem>();
        string? failure = null;
        foreach (var step in steps)
        {
            failure ??= StopReason(messages,
                cancellationToken);
            var item = failure is null
                ? await RunStepAsync(step,
                    context,
                    cancellationToken)
                    .ConfigureAwait(true)
                : Skipped(step,
                    failure);
            failure ??= FailureOf(item,
                messages,
                cancellationToken);
            items.Add(item);
            progress?.Report(new NotebookRunProgress(items.Count,
                steps.Count,
                item));
        }
        return new NotebookRunResult(items,
            failure is null,
            failure ?? string.Empty,
            stopwatch.Elapsed);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets why the run must stop after a section.
    /// </summary>
    /// <param name="item">The section just run.</param>
    /// <param name="messages">The texts of the notebook.</param>
    /// <param name="cancellationToken">The token that stops the run.</param>
    /// <returns>The reason, or null when the run goes on.</returns>
    private static string? FailureOf(NotebookRunItem item,
        NotebookMessages messages,
        CancellationToken cancellationToken) => StopReason(messages,
            cancellationToken) ?? (item.State == NotebookRunState.Failed ? messages.RunFailed(item.Name) : null);

    /// <summary>
    /// Gets the name of a section in the run.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="number">The position in the run, from one.</param>
    /// <param name="messages">The texts of the notebook.</param>
    /// <returns>The name.</returns>
    private static string NameOf(NotebookExecutableSection section,
        int number,
        NotebookMessages messages) => section switch
        {
            NotebookImportsSection => messages.ImportsStepName(),
            NotebookChartSection => messages.ChartStepName(number),
            NotebookCodeSection { Language: NotebookCodeLanguage.CSharp } => messages.CSharpStepName(number),
            _ => ((NotebookCodeSection)section).Name
        };

    /// <summary>
    /// Runs the section of a step.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The outcome of the section.</returns>
    private static async Task<NotebookRunItem> RunStepAsync(NotebookRunStep step,
        NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            await step.Section.ExecuteAsync(context,
                cancellationToken)
                .ConfigureAwait(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // The section recorded that it was stopped.
        }
        return new NotebookRunItem(step.Section,
            step.Name,
            step.Section.State,
            step.Section.StatusMessage,
            step.Section.Elapsed);
    }

    /// <summary>
    /// Records a step that does not run.
    /// </summary>
    /// <param name="step">The step.</param>
    /// <param name="reason">Why it does not run.</param>
    /// <returns>The outcome of the section.</returns>
    private static NotebookRunItem Skipped(NotebookRunStep step,
        string reason) => new(step.Section,
            step.Name,
            NotebookRunState.Skipped,
            reason,
            TimeSpan.Zero);

    /// <summary>
    /// Gets why a cancelled run stops.
    /// </summary>
    /// <param name="messages">The texts of the notebook.</param>
    /// <param name="cancellationToken">The token that stops the run.</param>
    /// <returns>The reason, or null while it is not cancelled.</returns>
    private static string? StopReason(NotebookMessages messages,
        CancellationToken cancellationToken) => cancellationToken.IsCancellationRequested ? messages.RunStopped() : null;

    #endregion
}
