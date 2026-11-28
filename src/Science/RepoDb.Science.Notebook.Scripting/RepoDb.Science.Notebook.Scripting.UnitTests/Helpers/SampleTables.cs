namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Builds the tables the tests use.
/// </summary>
internal static class SampleTables
{
    #region Public Methods

    /// <summary>
    /// Creates a notebook table of customers.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <returns>The table.</returns>
    public static NotebookTable Customers(string title = "") => new([new NotebookColumn("Name"), new NotebookColumn("Orders",
            "long")],
        [["Alice", "12"], ["Bob", "7"]],
        title: title);

    /// <summary>
    /// Creates a data table of numbers.
    /// </summary>
    /// <returns>The table, with three rows.</returns>
    public static DataTable Numbers()
    {
        var table = new DataTable("Numbers");
        table.Columns.Add("Value",
            typeof(int));
        table.Rows.Add(1);
        table.Rows.Add(2);
        table.Rows.Add(3);
        table.AcceptChanges();
        return table;
    }

    /// <summary>
    /// Creates the SQL results of a request.
    /// </summary>
    /// <param name="sectionName">The name of the section.</param>
    /// <param name="tables">The tables of the section.</param>
    /// <returns>The results.</returns>
    public static Dictionary<string, IReadOnlyList<NotebookTable>> Results(string sectionName,
        params NotebookTable[] tables) => new(StringComparer.OrdinalIgnoreCase) { [sectionName] = tables };

    #endregion
}
