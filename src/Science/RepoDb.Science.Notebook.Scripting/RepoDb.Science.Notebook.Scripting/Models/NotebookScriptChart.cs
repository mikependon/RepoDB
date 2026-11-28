using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Gives a table to the chart section that runs the script. A code section ignores it.
/// </summary>
public sealed class NotebookScriptChart
{
    #region Private Variables

    private readonly NotebookScriptContext _context;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the chart of a run.
    /// </summary>
    /// <param name="context">The context that holds the SQL results.</param>
    internal NotebookScriptChart(NotebookScriptContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the table given to the chart, or null.
    /// </summary>
    internal DataTable? Table { get; private set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Charts a data table. The last table given wins.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The same table.</returns>
    public DataTable FromDataTable(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        Table = table;
        return table;
    }

    /// <summary>
    /// Charts the first result table of a SQL section.
    /// </summary>
    /// <param name="sectionName">The name of the SQL section.</param>
    /// <returns>Every result table of the section.</returns>
    public DataTable[] FromSqlResult(string sectionName)
    {
        var tables = _context.FromSqlResult(sectionName);
        Table = tables.FirstOrDefault() ?? Table;
        return tables;
    }

    #endregion
}
