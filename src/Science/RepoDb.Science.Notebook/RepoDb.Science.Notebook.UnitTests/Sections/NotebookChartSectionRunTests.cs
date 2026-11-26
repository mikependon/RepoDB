namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the runs of the chart section.
/// </summary>
internal static class NotebookChartSectionRunTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await SqlRunDrawsTheFirstTable();
        await SqlRunSucceedsWithTheRowCount();
        await SqlRunFailsWithoutATable();
        await UnusableTableFails();
        await TimedRunIsLogged();
        await CategoryRunExplainsTheLabels();
        await SkippedColumnsAreWarned();
        await NonNumericColumnsCanFailTheRun();
        await CSharpRunDrawsTheChartTable();
        await CSharpRunKeepsOnlyTheChartTable();
        await CSharpRunFailsWithoutAChartTable();
        await FailedRunDrawsNothing();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a chart against SQL tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>The chart.</returns>
    private static async Task<NotebookChartSection> Ran(params NotebookTable[] tables)
    {
        var section = new NotebookChartSection { SqlScript = "SELECT 1" };
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(tables) });
        return section;
    }

    /// <summary>
    /// Runs a chart against a C# outcome.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <returns>The chart.</returns>
    private static async Task<NotebookChartSection> RanCSharp(NotebookScriptResult result)
    {
        var section = new NotebookChartSection { Source = NotebookCodeLanguage.CSharp };
        await section.ExecuteAsync(new NotebookExecutionContext { CSharpExecutor = new FakeCSharpExecutor(result), RowLimit = 2 });
        return section;
    }

    /// <summary>
    /// Tests SqlRunDrawsTheFirstTable.
    /// </summary>
    private static async Task SqlRunDrawsTheFirstTable()
    {
        // Setup
        var first = SampleTables.Timed();

        // Act
        var section = await Ran(first,
            SampleTables.Categories());

        // Assert
        Check(section.Table == first && section.Results.Count == 1 && section.Data is not null);
    }

    /// <summary>
    /// Tests SqlRunSucceedsWithTheRowCount.
    /// </summary>
    private static async Task SqlRunSucceedsWithTheRowCount()
    {
        // Act
        var section = await Ran(SampleTables.Timed());

        // Assert
        Check(section.State == NotebookRunState.Succeeded && section.ResultStatus == "3 row(s)");
    }

    /// <summary>
    /// Tests SqlRunFailsWithoutATable.
    /// </summary>
    private static async Task SqlRunFailsWithoutATable()
    {
        // Act
        var section = await Ran();

        // Assert
        Check(section.StatusMessage == "The script returned no result set to chart.");
    }

    /// <summary>
    /// Tests UnusableTableFails.
    /// </summary>
    private static async Task UnusableTableFails()
    {
        // Act
        var section = await Ran(SampleTables.Create(["a:int", "b:int"]));

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "The result has no rows to chart.");
    }

    /// <summary>
    /// Tests TimedRunIsLogged.
    /// </summary>
    private static async Task TimedRunIsLogged()
    {
        // Act
        var section = await Ran(SampleTables.Timed());

        // Assert
        Check(section.Log.Entries[^1].Message == "Charted 3 rows as 1 series.");
    }

    /// <summary>
    /// Tests CategoryRunExplainsTheLabels.
    /// </summary>
    private static async Task CategoryRunExplainsTheLabels()
    {
        // Act
        var section = await Ran(SampleTables.Categories());

        // Assert
        Check(section.Log.Entries[^1].Message.Contains("is not a timestamp",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests SkippedColumnsAreWarned.
    /// </summary>
    private static async Task SkippedColumnsAreWarned()
    {
        // Act
        var section = await Ran(SampleTables.Mixed());

        // Assert
        Check(section.Log.Entries[^1] is { Level: NotebookLogLevel.Warning, Message: "Skipped the columns that are not numeric: Note." });
    }

    /// <summary>
    /// Tests NonNumericColumnsCanFailTheRun.
    /// </summary>
    private static async Task NonNumericColumnsCanFailTheRun()
    {
        // Setup
        var section = new NotebookChartSection { SqlScript = "SELECT 1", FailOnNonNumeric = true };

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(SampleTables.Mixed()) });

        // Assert
        Check(section.State == NotebookRunState.Failed && section.Table is null);
    }

    /// <summary>
    /// Tests CSharpRunDrawsTheChartTable.
    /// </summary>
    private static async Task CSharpRunDrawsTheChartTable()
    {
        // Act
        var section = await RanCSharp(new NotebookScriptResult { Succeeded = true, Chart = SampleTables.Timed() });

        // Assert
        Check(section.State == NotebookRunState.Succeeded && section.Table is { Rows.Count: 2, IsTruncated: true });
    }

    /// <summary>
    /// Tests CSharpRunKeepsOnlyTheChartTable.
    /// </summary>
    private static async Task CSharpRunKeepsOnlyTheChartTable()
    {
        // Setup
        var result = new NotebookScriptResult
        {
            Succeeded = true,
            Chart = SampleTables.Categories(),
            Tables = [SampleTables.Timed(), SampleTables.Mixed()]
        };

        // Act
        var section = await RanCSharp(result);

        // Assert
        Check(section.Results.Count == 1 && section.Table!.Columns[0].Name == "Country");
    }

    /// <summary>
    /// Tests CSharpRunFailsWithoutAChartTable.
    /// </summary>
    private static async Task CSharpRunFailsWithoutAChartTable()
    {
        // Act
        var section = await RanCSharp(new NotebookScriptResult { Succeeded = true, Tables = [SampleTables.Timed()] });

        // Assert
        Check(section.State == NotebookRunState.Failed && section.Results.Count == 0 && section.ResultStatus.Length == 0);
    }

    /// <summary>
    /// Tests FailedRunDrawsNothing.
    /// </summary>
    private static async Task FailedRunDrawsNothing()
    {
        // Setup
        var section = await Ran(SampleTables.Timed());

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(section.Data is null && section.Table is null);
    }

    #endregion
}
