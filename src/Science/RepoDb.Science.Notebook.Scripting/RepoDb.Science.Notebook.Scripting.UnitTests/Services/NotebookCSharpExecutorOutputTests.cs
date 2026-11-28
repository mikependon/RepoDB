namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the output, tables and chart of the scripts the executor runs.
/// </summary>
internal static class NotebookCSharpExecutorOutputTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunConsoleAsync();
        await RunPrintAsync();
        await RunTablesAsync();
        await RunChartAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the console.
    /// </summary>
    private static async Task RunConsoleAsync()
    {
        await CapturesTheConsoleOutput();
        await CapturesTheErrorOutput();
        await GivesTheConsoleBack();
        await KeepsTheOutputOfScriptsThatRunTogetherApart();
    }

    /// <summary>
    /// Runs the scenarios of printing.
    /// </summary>
    private static async Task RunPrintAsync()
    {
        await PrintsWithTheGlobalPrint();
        await PrintsWithTheNotebook();
        await PrintsANestedGenericType();
    }

    /// <summary>
    /// Runs the scenarios of the tables.
    /// </summary>
    private static async Task RunTablesAsync()
    {
        await ShowsAPrintedTable();
        await ShowsAReturnedTable();
        await ShowsAReturnedCollectionOfTables();
        await ShowsATableOnce();
        await TitlesAnUnnamedTable();
        await ShowsNoTableThatIsOnlyStored();
        await LimitsTheShownTables();
        await BuildsATableFromAReader();
        await BuildsATableFromObjects();
        await BuildsATableFromDictionaries();
    }

    /// <summary>
    /// Runs the scenarios of the chart.
    /// </summary>
    private static async Task RunChartAsync()
    {
        await ReturnsTheChart();
        await ReturnsNoChartWithoutOne();
        await LimitsTheChart();
    }

    /// <summary>
    /// Runs a script with a new executor.
    /// </summary>
    /// <param name="code">The script.</param>
    /// <param name="rowLimit">The row limit.</param>
    /// <returns>The outcome.</returns>
    private static Task<NotebookScriptResult> Run(string code,
        int rowLimit = 10) => ScriptRuns.Run(new NotebookCSharpExecutor(),
            code,
            rowLimit: rowLimit);

    /// <summary>
    /// Creates an executor that holds a name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The executor.</returns>
    private static NotebookCSharpExecutor Named(string name)
    {
        var executor = new NotebookCSharpExecutor();
        executor.Context.Set("Name",
            name);
        return executor;
    }

    /// <summary>
    /// Tests CapturesTheConsoleOutput.
    /// </summary>
    private static async Task CapturesTheConsoleOutput()
    {
        // Act
        var result = await Run("""Console.Write("hello");""");

        // Assert
        Check(result.Output == "hello");
    }

    /// <summary>
    /// Tests CapturesTheErrorOutput.
    /// </summary>
    private static async Task CapturesTheErrorOutput()
    {
        // Act
        var result = await Run("""Console.Error.Write("warning");""");

        // Assert
        Check(result.Output == "warning");
    }

    /// <summary>
    /// Tests GivesTheConsoleBack.
    /// </summary>
    private static async Task GivesTheConsoleBack()
    {
        // Setup
        var before = Console.Out;

        // Act
        await Run("Console.Write(1);");

        // Assert
        Check(ReferenceEquals(Console.Out,
            before));
    }

    /// <summary>
    /// Tests KeepsTheOutputOfScriptsThatRunTogetherApart.
    /// </summary>
    private static async Task KeepsTheOutputOfScriptsThatRunTogetherApart()
    {
        // Setup
        const string code = """await Task.Delay(20); Console.Write(Notebook.Context.Get("Name"));""";

        // Act
        var results = await Task.WhenAll(ScriptRuns.Run(Named("first"),
                code),
            ScriptRuns.Run(Named("second"),
                code));

        // Assert
        Check(results[0].Output == "first" && results[1].Output == "second");
    }

    /// <summary>
    /// Tests PrintsWithTheGlobalPrint.
    /// </summary>
    private static async Task PrintsWithTheGlobalPrint()
    {
        // Act
        var result = await Run("Print(new[] { 1, 2 });");

        // Assert
        Check(result.Output.TrimEnd() == "Int32[]: [1, 2]");
    }

    /// <summary>
    /// Tests PrintsWithTheNotebook.
    /// </summary>
    private static async Task PrintsWithTheNotebook()
    {
        // Act
        var result = await Run("Notebook.Print(42);");

        // Assert
        Check(result.Output.TrimEnd() == "Int32: 42");
    }

    /// <summary>
    /// Tests PrintsANestedGenericType.
    /// </summary>
    private static async Task PrintsANestedGenericType()
    {
        // Act
        var result = await Run("""Print(new Dictionary<string, int> { ["a"] = 1 }.Keys);""");

        // Assert
        Check(result.Output.TrimEnd() == "System.Collections.Generic.KeyCollection: [a]");
    }

    /// <summary>
    /// Tests ShowsAPrintedTable.
    /// </summary>
    private static async Task ShowsAPrintedTable()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Print(table);");

        // Assert
        Check(result.Tables
            .Single().Title == "T");
    }

    /// <summary>
    /// Tests ShowsAReturnedTable.
    /// </summary>
    private static async Task ShowsAReturnedTable()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "return table;");

        // Assert
        Check(result.Tables.Count == 1 && result.ReturnValue == "DataTable 'T': 1 columns, 2 rows");
    }

    /// <summary>
    /// Tests ShowsAReturnedCollectionOfTables.
    /// </summary>
    private static async Task ShowsAReturnedCollectionOfTables()
    {
        // Act
        var result = await Run("""return new List<DataTable> { new DataTable("A"), new DataTable("B") };""");

        // Assert
        Check(result.Tables.Count == 2);
    }

    /// <summary>
    /// Tests ShowsATableOnce.
    /// </summary>
    private static async Task ShowsATableOnce()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Print(table); return table;");

        // Assert
        Check(result.Tables.Count == 1);
    }

    /// <summary>
    /// Tests TitlesAnUnnamedTable.
    /// </summary>
    private static async Task TitlesAnUnnamedTable()
    {
        // Act
        var result = await Run("return new DataTable();");

        // Assert
        Check(result.Tables
            .Single().Title == "Table");
    }

    /// <summary>
    /// Tests ShowsNoTableThatIsOnlyStored.
    /// </summary>
    private static async Task ShowsNoTableThatIsOnlyStored()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + """Notebook.Context.Set("T", table);""");

        // Assert
        Check(result.Tables.Count == 0);
    }

    /// <summary>
    /// Tests LimitsTheShownTables.
    /// </summary>
    private static async Task LimitsTheShownTables()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Print(table);",
            1);

        // Assert
        Check(result.Tables[0].Rows.Count == 1 && result.Tables[0].IsTruncated);
    }

    /// <summary>
    /// Tests BuildsATableFromAReader.
    /// </summary>
    private static async Task BuildsATableFromAReader()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "return Notebook.ToDataTable(table.CreateDataReader());");

        // Assert
        Check(result.Tables
            .Single().Rows.Count == 2);
    }

    /// <summary>
    /// Tests BuildsATableFromObjects.
    /// </summary>
    private static async Task BuildsATableFromObjects()
    {
        // Act
        var result = await Run("return Notebook.ToDataTable(new List<object> { new { Id = 1 } });");

        // Assert
        Check(result.Tables
            .Single().Columns[0].Name == "Id");
    }

    /// <summary>
    /// Tests BuildsATableFromDictionaries.
    /// </summary>
    private static async Task BuildsATableFromDictionaries()
    {
        // Act
        var result = await Run("""return Notebook.ToDataTable(new List<Dictionary<string, object>> { new() { ["Id"] = 1 } });""");

        // Assert
        Check(result.Tables
            .Single().Rows[0][0] == "1");
    }

    /// <summary>
    /// Tests ReturnsTheChart.
    /// </summary>
    private static async Task ReturnsTheChart()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Notebook.Chart.FromDataTable(table);");

        // Assert
        Check(result.Chart!.Rows.Count == 2);
    }

    /// <summary>
    /// Tests ReturnsNoChartWithoutOne.
    /// </summary>
    private static async Task ReturnsNoChartWithoutOne()
    {
        // Act
        var result = await Run("var total = 1;");

        // Assert
        Check(result.Chart is null);
    }

    /// <summary>
    /// Tests LimitsTheChart.
    /// </summary>
    private static async Task LimitsTheChart()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Notebook.Chart.FromDataTable(table);",
            1);

        // Assert
        Check(result.Chart!.Rows.Count == 1 && result.Chart.IsTruncated);
    }

    #endregion
}
