namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the status, errors and output limit of C# outcomes.
/// </summary>
internal static class NotebookCSharpOutcomeTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await HostStatusIsUsed();
        await HostStatusWorksWithoutTables();
        await StatusCountsTheFirstTable();
        await ChartUsesTheHostStatus();
        await ChartCountsItsRows();
        await ChartLogsTheScriptSuccess();
        await ErrorInTheLogsIsNotRepeated();
        await ErrorWithoutALogLineIsAdded();
        await OutputLimitCanBeApplied();
        OutputLimitRejectsZero();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a C# section with an outcome.
    /// </summary>
    /// <typeparam name="T">The section type.</typeparam>
    /// <param name="section">The section.</param>
    /// <param name="result">The outcome.</param>
    /// <returns>The section.</returns>
    private static async Task<T> Ran<T>(T section,
        NotebookScriptResult result)
        where T : NotebookScriptSection
    {
        await section.ExecuteAsync(new NotebookExecutionContext { CSharpExecutor = new FakeCSharpExecutor(result) });
        return section;
    }

    /// <summary>
    /// Creates a C# code section.
    /// </summary>
    /// <returns>The section.</returns>
    private static NotebookCodeSection Code() => new() { Language = NotebookCodeLanguage.CSharp };

    /// <summary>
    /// Creates a C# chart.
    /// </summary>
    /// <returns>The chart.</returns>
    private static NotebookChartSection Chart() => new() { Source = NotebookCodeLanguage.CSharp };

    /// <summary>
    /// Creates a failed outcome that logged its error.
    /// </summary>
    /// <returns>The outcome.</returns>
    private static NotebookScriptResult FailedWithLog() => new()
    {
        Error = "CS0103",
        Logs =
        [
            new NotebookLogEntry(NotebookLogLevel.Error,
                "(1,1): CS0103",
                DateTimeOffset.UnixEpoch)
        ]
    };

    /// <summary>
    /// Tests HostStatusIsUsed.
    /// </summary>
    private static async Task HostStatusIsUsed()
    {
        // Act
        var section = await Ran(Code(),
            new NotebookScriptResult { Succeeded = true, Tables = [SampleTables.Timed()], Status = "mine" });

        // Assert
        Check(section.ResultStatus == "mine");
    }

    /// <summary>
    /// Tests HostStatusWorksWithoutTables.
    /// </summary>
    private static async Task HostStatusWorksWithoutTables()
    {
        // Act
        var section = await Ran(Code(),
            new NotebookScriptResult { Succeeded = true, Status = "mine" });

        // Assert
        Check(section.ResultStatus == "mine");
    }

    /// <summary>
    /// Tests StatusCountsTheFirstTable.
    /// </summary>
    private static async Task StatusCountsTheFirstTable()
    {
        // Act
        var section = await Ran(Code(),
            new NotebookScriptResult { Succeeded = true, Tables = [SampleTables.Timed(), SampleTables.Categories()] });

        // Assert
        Check(section.ResultStatus == "3 row(s)");
    }

    /// <summary>
    /// Tests ChartUsesTheHostStatus.
    /// </summary>
    private static async Task ChartUsesTheHostStatus()
    {
        // Act
        var section = await Ran(Chart(),
            new NotebookScriptResult { Succeeded = true, Chart = SampleTables.Timed(), Status = "mine" });

        // Assert
        Check(section.ResultStatus == "mine");
    }

    /// <summary>
    /// Tests ChartCountsItsRows.
    /// </summary>
    private static async Task ChartCountsItsRows()
    {
        // Act
        var section = await Ran(Chart(),
            new NotebookScriptResult { Succeeded = true, Chart = SampleTables.Categories(), Tables = [SampleTables.Timed()] });

        // Assert
        Check(section.ResultStatus == "2 row(s)");
    }

    /// <summary>
    /// Tests ChartLogsTheScriptSuccess.
    /// </summary>
    private static async Task ChartLogsTheScriptSuccess()
    {
        // Act
        var section = await Ran(Chart(),
            new NotebookScriptResult { Succeeded = true, Chart = SampleTables.Timed() });

        // Assert
        Check(section.Log.Entries[0].Message == "The script ran well.");
    }

    /// <summary>
    /// Tests ErrorInTheLogsIsNotRepeated.
    /// </summary>
    private static async Task ErrorInTheLogsIsNotRepeated()
    {
        // Act
        var section = await Ran(Code(),
            FailedWithLog());

        // Assert
        Check(section.Log.Entries
            .Single().Message == "(1,1): CS0103" && section.StatusMessage == "CS0103");
    }

    /// <summary>
    /// Tests ErrorWithoutALogLineIsAdded.
    /// </summary>
    private static async Task ErrorWithoutALogLineIsAdded()
    {
        // Act
        var section = await Ran(Code(),
            new NotebookScriptResult { Error = "CS0103" });

        // Assert
        Check(section.Log.Entries
            .Single() is { Level: NotebookLogLevel.Error, Message: "CS0103" });
    }

    /// <summary>
    /// Tests OutputLimitCanBeApplied.
    /// </summary>
    private static async Task OutputLimitCanBeApplied()
    {
        // Setup
        var section = await Ran(Code(),
            new NotebookScriptResult { Succeeded = true, Output = "a\nb\nc" });

        // Act
        section.ApplyOutputLimit(1);

        // Assert
        Check(section.Output == "... 2 earlier lines not shown" + Environment.NewLine + "c");
    }

    /// <summary>
    /// Tests OutputLimitRejectsZero.
    /// </summary>
    private static void OutputLimitRejectsZero()
    {
        // Setup
        var section = Code();

        // Act
        Action operation = () => section.ApplyOutputLimit(0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    #endregion
}
