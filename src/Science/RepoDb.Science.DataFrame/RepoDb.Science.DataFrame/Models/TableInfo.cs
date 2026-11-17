namespace RepoDb.Science;

/// <summary>
/// Describes a table snapshot.
/// </summary>
public sealed class TableInfo
{
    #region Constructors

    /// <summary>
    /// Creates a snapshot.
    /// </summary>
    /// <param name="rowCount">The number of current rows.</param>
    /// <param name="columns">The column summaries.</param>
    public TableInfo(int rowCount,
        IReadOnlyList<ColumnInfo> columns)
    {
        RowCount = rowCount;
        Columns = columns
            .ToList()
            .AsReadOnly();
    }

    #endregion

    #region Properties

    /// <summary>
    /// The number of current rows.
    /// </summary>
    public int RowCount { get; }

    /// <summary>
    /// The column summaries.
    /// </summary>
    public IReadOnlyList<ColumnInfo> Columns { get; }

    #endregion
}
