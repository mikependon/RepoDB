namespace RepoDb.Science;

/// <summary>
/// Whether a table can be charted, and with which columns.
/// </summary>
internal sealed class NotebookChartCheck
{
    #region Constructors

    /// <summary>
    /// Creates a chart check.
    /// </summary>
    /// <param name="error">Why it cannot be charted.</param>
    /// <param name="valueColumns">The numeric column positions.</param>
    /// <param name="skipped">The skipped column names.</param>
    /// <param name="isTimeBased">Whether the first column is a time.</param>
    public NotebookChartCheck(string? error,
        IReadOnlyList<int> valueColumns,
        IReadOnlyList<string> skipped,
        bool isTimeBased)
    {
        Error = error;
        ValueColumns = valueColumns;
        Skipped = skipped;
        IsTimeBased = isTimeBased;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets why it cannot be charted, or null.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets whether the first column is a time.
    /// </summary>
    public bool IsTimeBased { get; }

    /// <summary>
    /// Gets whether the table can be charted.
    /// </summary>
    public bool IsValid => Error is null;

    /// <summary>
    /// Gets the skipped column names.
    /// </summary>
    public IReadOnlyList<string> Skipped { get; }

    /// <summary>
    /// Gets the numeric column positions.
    /// </summary>
    public IReadOnlyList<int> ValueColumns { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a failed check.
    /// </summary>
    /// <param name="error">Why it cannot be charted.</param>
    /// <returns>The check.</returns>
    public static NotebookChartCheck Failure(string error) => new(error,
        [],
        [],
        false);

    #endregion
}
