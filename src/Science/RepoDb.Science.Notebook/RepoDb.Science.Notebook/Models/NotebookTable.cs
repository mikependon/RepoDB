namespace RepoDb.Science;

/// <summary>
/// A result table, with every value held as text.
/// </summary>
public sealed class NotebookTable
{
    #region Constructors

    /// <summary>
    /// Creates a table.
    /// </summary>
    /// <param name="columns">The columns.</param>
    /// <param name="rows">The rows.</param>
    /// <param name="isTruncated">Whether rows were left out.</param>
    /// <param name="title">The table title.</param>
    public NotebookTable(IReadOnlyList<NotebookColumn> columns,
        IReadOnlyList<IReadOnlyList<string?>> rows,
        bool isTruncated = false,
        string title = "")
    {
        ArgumentNullException.ThrowIfNull(columns);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(title);
        Columns = columns;
        Rows = rows;
        IsTruncated = isTruncated;
        Title = title;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the columns.
    /// </summary>
    public IReadOnlyList<NotebookColumn> Columns { get; }

    /// <summary>
    /// Gets whether rows were left out.
    /// </summary>
    public bool IsTruncated { get; }

    /// <summary>
    /// Gets the rows.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string?>> Rows { get; }

    /// <summary>
    /// Gets the table title.
    /// </summary>
    public string Title { get; }

    #endregion
}
