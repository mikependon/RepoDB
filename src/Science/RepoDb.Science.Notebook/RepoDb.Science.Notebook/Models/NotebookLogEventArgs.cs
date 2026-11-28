namespace RepoDb.Science;

/// <summary>
/// The entry that was added to a log.
/// </summary>
public sealed class NotebookLogEventArgs : EventArgs
{
    #region Constructors

    /// <summary>
    /// Creates the event data.
    /// </summary>
    /// <param name="entry">The added entry.</param>
    public NotebookLogEventArgs(NotebookLogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        Entry = entry;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the added entry.
    /// </summary>
    public NotebookLogEntry Entry { get; }

    #endregion
}
