namespace RepoDb.Science;

/// <summary>
/// What the sections of a notebook need to run.
/// </summary>
public sealed class NotebookExecutionContext
{
    #region Public Variables

    /// <summary>
    /// The default number of lines of script output kept.
    /// </summary>
    public const int DefaultOutputLimit = 20;

    /// <summary>
    /// The default number of rows kept for each table.
    /// </summary>
    public const int DefaultRowLimit = 1000;

    #endregion

    #region Private Variables

    private readonly int _outputLimit = DefaultOutputLimit;
    private readonly int _rowLimit = DefaultRowLimit;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the C# executor, or null when C# cannot run.
    /// </summary>
    public INotebookCSharpExecutor? CSharpExecutor { get; init; }

    /// <summary>
    /// Gets the guard checked before a script runs.
    /// </summary>
    public INotebookExecutionGuard? Guard { get; init; }

    /// <summary>
    /// Gets the number of lines of script output kept.
    /// </summary>
    public int OutputLimit
    {
        get => _outputLimit;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            _outputLimit = value;
        }
    }

    /// <summary>
    /// Gets the number of rows kept for each table.
    /// </summary>
    public int RowLimit
    {
        get => _rowLimit;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            _rowLimit = value;
        }
    }

    /// <summary>
    /// Gets the SQL executor, or null when SQL cannot run.
    /// </summary>
    public INotebookSqlExecutor? SqlExecutor { get; init; }

    #endregion
}
