namespace RepoDb.Science;

/// <summary>
/// The progress of the run of a notebook.
/// </summary>
public sealed class NotebookRunProgress
{
    #region Constructors

    /// <summary>
    /// Creates a progress report.
    /// </summary>
    /// <param name="completed">The sections done.</param>
    /// <param name="total">The sections to run.</param>
    /// <param name="item">The section just done.</param>
    public NotebookRunProgress(int completed,
        int total,
        NotebookRunItem item)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(total);
        ArgumentNullException.ThrowIfNull(item);
        Completed = completed;
        Total = total;
        Item = item;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the sections done.
    /// </summary>
    public int Completed { get; }

    /// <summary>
    /// Gets the section just done.
    /// </summary>
    public NotebookRunItem Item { get; }

    /// <summary>
    /// Gets the percentage of sections done.
    /// </summary>
    public double Percentage => Completed * 100d / Total;

    /// <summary>
    /// Gets the sections to run.
    /// </summary>
    public int Total { get; }

    #endregion
}
