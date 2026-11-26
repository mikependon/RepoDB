namespace RepoDb.Science;

/// <summary>
/// The level of a log entry.
/// </summary>
public enum NotebookLogLevel
{
    /// <summary>
    /// A diagnostic entry.
    /// </summary>
    Debug,

    /// <summary>
    /// An informational entry.
    /// </summary>
    Information,

    /// <summary>
    /// A warning.
    /// </summary>
    Warning,

    /// <summary>
    /// An error.
    /// </summary>
    Error,

    /// <summary>
    /// A critical error.
    /// </summary>
    Critical
}
