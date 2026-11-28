using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A notebook with a SQL section named Section1, over an in-memory database.
/// </summary>
internal sealed class SalesNotebook
{
    #region Public Variables

    /// <summary>
    /// A script that uses the context, the printing and the chart.
    /// </summary>
    public const string Snippet = """
        var dataTable = Notebook.Context.FromSqlResult("Section1")[0];

        Notebook.Context.Set("UpdatedCustomers", dataTable);

        Notebook.Print(dataTable);

        Notebook.Chart.FromDataTable(dataTable);
        """;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates the notebook.
    /// </summary>
    /// <param name="allowDatabaseAccess">Whether the C# scripts may access the database.</param>
    public SalesNotebook(bool allowDatabaseAccess = false)
    {
        var query = new NotebookCodeSection { Script = "SELECT 'Alice' AS Name, 12 AS Orders UNION ALL SELECT 'Bob', 7" };
        Document.Sections.Add(query);
        query.TryRename("Section1");
        Context = new NotebookExecutionContext
        {
            SqlExecutor = new NotebookSqlExecutor((_,
                _) => ValueTask.FromResult<DbConnection>(new SqliteConnection("Data Source=:memory:"))),
            CSharpExecutor = new NotebookCSharpExecutor(() => allowDatabaseAccess)
        };
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets what the sections run with.
    /// </summary>
    public NotebookExecutionContext Context { get; }

    /// <summary>
    /// Gets the notebook.
    /// </summary>
    public NotebookDocument Document { get; } = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a chart section that reads from C#.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>The section.</returns>
    public NotebookChartSection AddChart(string script)
    {
        var section = new NotebookChartSection
        {
            Source = NotebookCodeLanguage.CSharp,
            CSharpScript = script
        };
        Document.Sections.Add(section);
        return section;
    }

    /// <summary>
    /// Adds a C# code section.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>The section.</returns>
    public NotebookCodeSection AddCode(string script)
    {
        var section = new NotebookCodeSection
        {
            Language = NotebookCodeLanguage.CSharp,
            Script = script
        };
        Document.Sections.Add(section);
        return section;
    }

    /// <summary>
    /// Runs the whole notebook.
    /// </summary>
    /// <returns>The outcome.</returns>
    public Task<NotebookRunResult> RunAsync() => Document.ExecuteAsync(Context);

    #endregion
}
