namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Builds the tables the tests use.
/// </summary>
internal static class SampleTables
{
    #region Public Methods

    /// <summary>
    /// Builds a table whose first column is a text.
    /// </summary>
    /// <returns>The table.</returns>
    public static NotebookTable Categories() => Create(["Country:string", "Total:int"],
        ["PH", "2"],
        ["NL", "3"]);

    /// <summary>
    /// Builds a table.
    /// </summary>
    /// <param name="columns">The columns written as name:type.</param>
    /// <param name="rows">The rows.</param>
    /// <returns>The table.</returns>
    public static NotebookTable Create(string[] columns,
        params string?[][] rows) => new(columns
            .Select(column => column.Split(':'))
            .Select(parts => new NotebookColumn(parts[0],
                parts[1]))
            .ToList(),
        rows);

    /// <summary>
    /// Builds a table whose first column is a time, with two numeric columns and a text column.
    /// </summary>
    /// <returns>The table.</returns>
    public static NotebookTable Mixed() => Create(["At:datetime", "Total:int", "Note:string"],
        ["2026-01-01T00:00:00Z", "1", "a"],
        ["2026-01-01T00:01:00Z", "5", "b"]);

    /// <summary>
    /// Builds a table with three rows over two minutes.
    /// </summary>
    /// <returns>The table.</returns>
    public static NotebookTable Timed() => Create(["At:datetime", "Total:int"],
        ["2026-01-01T00:00:00Z", "2"],
        ["2026-01-01T00:00:30Z", "4"],
        ["2026-01-01T00:02:00Z", "6"]);

    #endregion
}
