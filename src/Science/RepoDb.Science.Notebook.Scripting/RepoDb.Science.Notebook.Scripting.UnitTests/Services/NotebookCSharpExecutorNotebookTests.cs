namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the C# executor inside the run of a notebook.
/// </summary>
internal static class NotebookCSharpExecutorNotebookTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunsTheSnippet();
        await ShowsTheOutputOfTheSnippet();
        await ShowsTheTableOfTheSnippet();
        await DrawsTheChartOfTheSnippet();
        await SharesAValueWithALaterSection();
        await ChartsASqlResult();
        await DoesNotChangeTheSqlSection();
        await RunsWithImportsOfApplicationAssemblies();
        await FailsTheSectionWithTheError();
        await SkipsTheSectionWithoutDynamicCode();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests RunsTheSnippet.
    /// </summary>
    private static async Task RunsTheSnippet()
    {
        // Setup
        var notebook = new SalesNotebook();
        notebook.AddCode(SalesNotebook.Snippet);

        // Act
        var result = await notebook.RunAsync();

        // Assert
        Check(result.Succeeded);
    }

    /// <summary>
    /// Tests ShowsTheOutputOfTheSnippet.
    /// </summary>
    private static async Task ShowsTheOutputOfTheSnippet()
    {
        // Setup
        var notebook = new SalesNotebook();
        var code = notebook.AddCode(SalesNotebook.Snippet);

        // Act
        await notebook.RunAsync();

        // Assert
        Check(code.Output.TrimEnd() == "System.Data.DataTable: '', 2 columns, 2 rows");
    }

    /// <summary>
    /// Tests ShowsTheTableOfTheSnippet.
    /// </summary>
    private static async Task ShowsTheTableOfTheSnippet()
    {
        // Setup
        var notebook = new SalesNotebook();
        var code = notebook.AddCode(SalesNotebook.Snippet);

        // Act
        await notebook.RunAsync();

        // Assert
        Check(code.Results
            .Single().Rows[0][0] == "Alice");
    }

    /// <summary>
    /// Tests DrawsTheChartOfTheSnippet.
    /// </summary>
    private static async Task DrawsTheChartOfTheSnippet()
    {
        // Setup
        var notebook = new SalesNotebook();
        var chart = notebook.AddChart(SalesNotebook.Snippet);

        // Act
        await notebook.RunAsync();

        // Assert
        Check(chart.Data!.Series[0].Points
            .SequenceEqual([12d, 7d]));
    }

    /// <summary>
    /// Tests SharesAValueWithALaterSection.
    /// </summary>
    private static async Task SharesAValueWithALaterSection()
    {
        // Setup
        var notebook = new SalesNotebook();
        notebook.AddCode(SalesNotebook.Snippet);
        var later = notebook.AddCode("""((DataTable)Notebook.Context.Get("UpdatedCustomers")).Rows.Count""");

        // Act
        await notebook.RunAsync();

        // Assert
        Check(later.Output == "Returned: 2");
    }

    /// <summary>
    /// Tests ChartsASqlResult.
    /// </summary>
    private static async Task ChartsASqlResult()
    {
        // Setup
        var notebook = new SalesNotebook();
        var chart = notebook.AddChart("""Notebook.Chart.FromSqlResult("Section1");""");

        // Act
        await notebook.RunAsync();

        // Assert
        Check(chart.Data!.Labels
            .SequenceEqual(["Alice", "Bob"]));
    }

    /// <summary>
    /// Tests DoesNotChangeTheSqlSection.
    /// </summary>
    private static async Task DoesNotChangeTheSqlSection()
    {
        // Setup
        var notebook = new SalesNotebook();
        notebook.AddCode("""Notebook.Context.FromSqlResult("Section1")[0].Rows[0]["Name"] = "Zed";""");
        var later = notebook.AddCode("""Notebook.Context.FromSqlResult("Section1")[0].Rows[0]["Name"]""");

        // Act
        await notebook.RunAsync();

        // Assert
        Check(later.Output == "Returned: Alice");
    }

    /// <summary>
    /// Tests RunsWithImportsOfApplicationAssemblies.
    /// </summary>
    private static async Task RunsWithImportsOfApplicationAssemblies()
    {
        // Setup
        var notebook = new SalesNotebook(true);
        notebook.Document.Sections.Add(new NotebookImportsSection { Script = "using Microsoft.Data.Sqlite;\nusing RepoDb.Science;" });
        notebook.AddCode("var builder = new SqliteConnectionStringBuilder(); var table = default(NotebookTable);");

        // Act
        var result = await notebook.RunAsync();

        // Assert
        Check(result.Succeeded);
    }

    /// <summary>
    /// Tests FailsTheSectionWithTheError.
    /// </summary>
    private static async Task FailsTheSectionWithTheError()
    {
        // Setup
        var notebook = new SalesNotebook();
        var code = notebook.AddCode("""Notebook.Context.FromSqlResult("Missing");""");

        // Act
        await notebook.RunAsync();

        // Assert
        Check(code.State == NotebookRunState.Failed && code.StatusMessage.Contains("'Missing' has no result",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests SkipsTheSectionWithoutDynamicCode.
    /// </summary>
    private static async Task SkipsTheSectionWithoutDynamicCode()
    {
        // Setup
        var notebook = new SalesNotebook();
        var code = notebook.AddCode("return 1;");
        var context = new NotebookExecutionContext { CSharpExecutor = new NotebookCSharpExecutor(false) };

        // Act
        await code.ExecuteAsync(context);

        // Assert
        Check(code.State == NotebookRunState.Skipped);
    }

    #endregion
}
