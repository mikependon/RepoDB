namespace RepoDb.Science;

/// <summary>
/// The state of a section run.
/// </summary>
public enum NotebookRunState
{
    /// <summary>
    /// Not run yet.
    /// </summary>
    Pending,

    /// <summary>
    /// Running now.
    /// </summary>
    Running,

    /// <summary>
    /// Ran successfully.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Did not run.
    /// </summary>
    Skipped
}
