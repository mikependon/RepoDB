using System.Collections.ObjectModel;

namespace RepoDb.Science;

/// <summary>
/// The log of a section.
/// </summary>
public sealed class NotebookLog
{
    #region Public Variables

    /// <summary>
    /// The number of entries a log keeps by default.
    /// </summary>
    public const int DefaultLimit = 20;

    #endregion

    #region Private Variables

    private readonly ObservableCollection<NotebookLogEntry> _entries = [];
    private int _limit = DefaultLimit;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty log.
    /// </summary>
    public NotebookLog() => Entries = new ReadOnlyObservableCollection<NotebookLogEntry>(_entries);

    #endregion

    #region Properties

    /// <summary>
    /// Gets the entries, the oldest first.
    /// </summary>
    public ReadOnlyObservableCollection<NotebookLogEntry> Entries { get; }

    /// <summary>
    /// Gets or sets the number of entries the log keeps.
    /// </summary>
    public int Limit
    {
        get => _limit;
        set
        {
            _limit = Math.Max(1,
                value);
            Trim();
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when an entry is added.
    /// </summary>
    public event EventHandler<NotebookLogEventArgs>? EntryAdded;

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds an entry and drops the oldest ones above the limit.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    public void Add(NotebookLogLevel level,
        string message)
    {
        var entry = new NotebookLogEntry(level,
            message,
            DateTimeOffset.Now);
        _entries.Add(entry);
        Trim();
        EntryAdded?.Invoke(this,
            new NotebookLogEventArgs(entry));
    }

    /// <summary>
    /// Removes every entry.
    /// </summary>
    public void Clear() => _entries.Clear();

    #endregion

    #region Internal Methods

    /// <summary>
    /// Sets the limit without dropping the current entries; the next added entry applies it.
    /// </summary>
    /// <param name="limit">The limit, at least one.</param>
    internal void Adopt(int limit) => _limit = limit;

    /// <summary>
    /// Replaces the entries with saved ones, keeping all of them.
    /// </summary>
    /// <param name="entries">The saved entries.</param>
    internal void Load(IEnumerable<NotebookLogEntry> entries)
    {
        _entries.Clear();
        foreach (var entry in entries)
        {
            _entries.Add(entry);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Drops the oldest entries above the limit.
    /// </summary>
    private void Trim()
    {
        while (_entries.Count > _limit)
        {
            _entries.RemoveAt(0);
        }
    }

    #endregion
}
