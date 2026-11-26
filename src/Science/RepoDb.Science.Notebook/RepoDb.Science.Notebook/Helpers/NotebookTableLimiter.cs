namespace RepoDb.Science.Helpers;

/// <summary>
/// Cuts tables to the row limit.
/// </summary>
internal static class NotebookTableLimiter
{
    #region Public Methods

    /// <summary>
    /// Cuts each table to the row limit.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <param name="rowLimit">The most rows kept.</param>
    /// <returns>The tables.</returns>
    public static IReadOnlyList<NotebookTable> Limit(IReadOnlyList<NotebookTable> tables,
        int rowLimit) => tables
        .Select(table => Limit(table,
            rowLimit))
        .ToList();

    #endregion

    #region Private Methods

    /// <summary>
    /// Cuts a table to the row limit.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="rowLimit">The most rows kept.</param>
    /// <returns>The table.</returns>
    private static NotebookTable Limit(NotebookTable table,
        int rowLimit) => table.Rows.Count <= rowLimit
        ? table
        : new NotebookTable(table.Columns,
            table.Rows
                .Take(rowLimit)
                .ToList(),
            true,
            table.Title);

    #endregion
}
