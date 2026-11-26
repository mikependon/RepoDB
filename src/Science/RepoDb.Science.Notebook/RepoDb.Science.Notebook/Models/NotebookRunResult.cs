namespace RepoDb.Science;

/// <summary>
/// The outcome of the run of a notebook.
/// </summary>
public sealed class NotebookRunResult
{
    #region Constructors

    /// <summary>
    /// Creates a run result.
    /// </summary>
    /// <param name="items">The sections of the run.</param>
    /// <param name="failureMessage">Why the run stopped.</param>
    /// <param name="elapsed">How long it ran.</param>
    public NotebookRunResult(IReadOnlyList<NotebookRunItem> items,
        string failureMessage,
        TimeSpan elapsed)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(failureMessage);
        Items = items;
        FailureMessage = failureMessage;
        Elapsed = elapsed;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets how long it ran.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets why the run stopped, or empty.
    /// </summary>
    public string FailureMessage { get; }

    /// <summary>
    /// Gets the sections of the run.
    /// </summary>
    public IReadOnlyList<NotebookRunItem> Items { get; }

    /// <summary>
    /// Gets whether the run reached its end.
    /// </summary>
    public bool Succeeded => FailureMessage.Length == 0;

    #endregion
}
