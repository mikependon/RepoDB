namespace RepoDb.Science;

/// <summary>
/// A section in the run of a notebook.
/// </summary>
public sealed class NotebookRunItem
{
    #region Constructors

    /// <summary>
    /// Creates a run item.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="name">The name in the run.</param>
    /// <param name="state">How it ended.</param>
    /// <param name="message">The status message.</param>
    /// <param name="elapsed">How long it ran.</param>
    public NotebookRunItem(NotebookExecutableSection section,
        string name,
        NotebookRunState state,
        string message,
        TimeSpan elapsed)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(message);
        Section = section;
        Name = name;
        State = state;
        Message = message;
        Elapsed = elapsed;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets how long it ran.
    /// </summary>
    public TimeSpan Elapsed { get; }

    /// <summary>
    /// Gets the status message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the name in the run.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the section.
    /// </summary>
    public NotebookExecutableSection Section { get; }

    /// <summary>
    /// Gets how it ended.
    /// </summary>
    public NotebookRunState State { get; }

    #endregion
}
