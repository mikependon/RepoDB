namespace RepoDb.Science;

/// <summary>
/// How a section run ended.
/// </summary>
internal sealed class NotebookSectionResult
{
    #region Constructors

    /// <summary>
    /// Creates a section result.
    /// </summary>
    /// <param name="state">The final state.</param>
    /// <param name="message">The status message.</param>
    private NotebookSectionResult(NotebookRunState state,
        string message)
    {
        State = state;
        Message = message;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the status message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the final state.
    /// </summary>
    public NotebookRunState State { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="message">Why it failed.</param>
    /// <returns>The result.</returns>
    public static NotebookSectionResult Failed(string message) => new(NotebookRunState.Failed,
        message);

    /// <summary>
    /// Creates a skipped result.
    /// </summary>
    /// <param name="message">Why it was skipped.</param>
    /// <returns>The result.</returns>
    public static NotebookSectionResult Skipped(string message) => new(NotebookRunState.Skipped,
        message);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="message">The status message.</param>
    /// <returns>The result.</returns>
    public static NotebookSectionResult Succeeded(string message = "") => new(NotebookRunState.Succeeded,
        message);

    #endregion
}
