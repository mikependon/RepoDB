using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// What a C# script sees as Notebook. It lives for one run.
/// </summary>
public sealed class NotebookScriptHost
{
    #region Private Variables

    private readonly NotebookScriptingMessages _messages;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the host of a run.
    /// </summary>
    /// <param name="context">The values the scripts share.</param>
    /// <param name="messages">The texts of the problems.</param>
    internal NotebookScriptHost(NotebookScriptContext context,
        NotebookScriptingMessages messages)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(messages);
        _messages = messages;
        Context = context;
        Chart = new NotebookScriptChart(context);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the chart of the script.
    /// </summary>
    public NotebookScriptChart Chart { get; }

    /// <summary>
    /// Gets the values the scripts share and the results of the SQL sections.
    /// </summary>
    public NotebookScriptContext Context { get; }

    /// <summary>
    /// Gets the tables the script shows.
    /// </summary>
    internal NotebookScriptTables Tables { get; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes the type and the value to the output. Data tables, alone or in a data set or a collection, are also shown.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Print(object? value)
    {
        Console.WriteLine(NotebookScriptPrinter.Format(value));
        Tables.AddFrom(value);
    }

    /// <summary>
    /// Reads the rows of the first result of a data reader. The reader stays open.
    /// </summary>
    /// <param name="reader">The data reader.</param>
    /// <returns>The data table.</returns>
    public DataTable ToDataTable(IDataReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        return reader.IsClosed
            ? throw new InvalidOperationException(_messages.DataReaderClosed())
            : NotebookDataTables.FromReader(reader);
    }

    /// <summary>
    /// Builds a table from expando objects, whose keys are the columns.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>The data table.</returns>
    public DataTable ToDataTable(IList<ExpandoObject> rows) => NotebookDataTables.FromRows(rows);

    /// <summary>
    /// Builds a table from dictionaries, whose keys are the columns.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>The data table.</returns>
    public DataTable ToDataTable(IList<Dictionary<string, object?>> rows) => NotebookDataTables.FromRows(rows);

    /// <summary>
    /// Builds a table from objects: a dictionary is a row by its keys, and another object by its public properties.
    /// </summary>
    /// <param name="rows">The rows.</param>
    /// <returns>The data table.</returns>
    [RequiresUnreferencedCode(NotebookDataTables.PropertiesRequirement)]
    public DataTable ToDataTable(IList<object?> rows) => NotebookDataTables.FromObjects(rows);

    #endregion
}
