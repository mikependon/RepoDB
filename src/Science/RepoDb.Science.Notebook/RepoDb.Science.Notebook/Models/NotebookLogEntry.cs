namespace RepoDb.Science;

/// <summary>
/// A line of a section log.
/// </summary>
public sealed class NotebookLogEntry
{
    #region Constructors

    /// <summary>
    /// Creates a log entry.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    /// <param name="time">When it was written.</param>
    public NotebookLogEntry(NotebookLogLevel level,
        string message,
        DateTimeOffset time)
    {
        ArgumentNullException.ThrowIfNull(message);
        Level = level;
        Message = message;
        Time = time;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the level.
    /// </summary>
    public NotebookLogLevel Level { get; }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets when it was written.
    /// </summary>
    public DateTimeOffset Time { get; }

    #endregion
}
