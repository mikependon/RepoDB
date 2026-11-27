namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests what a cancelled run keeps.
/// </summary>
internal static class NotebookCancellationTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await CancelledSqlKeepsTheEarlierTables();
        await CancelledSqlKeepsTheEarlierStatus();
        await CancelledBeforeStartingKeepsTheTables();
        await StatusCanBeReplacedAfterACancellation();
        await CancelledCSharpKeepsNoTable();
        await CancelledChartKeepsNoTable();
        await DriverErrorAfterACancellationIsACancellation();
        await DriverErrorIsKeptAsTheCause();
        await CancelledOutcomeIsNotApplied();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a context that returns a table.
    /// </summary>
    /// <returns>The context.</returns>
    private static NotebookExecutionContext Returning() => new()
    {
        SqlExecutor = new FakeSqlExecutor(SampleTables.Categories()),
        CSharpExecutor = new FakeCSharpExecutor(new NotebookScriptResult { Succeeded = true, Tables = [SampleTables.Categories()] })
    };

    /// <summary>
    /// Runs a section once with a table and then cancels a second run.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The section.</returns>
    private static async Task<T> RanThenCancelled<T>(T section)
        where T : NotebookScriptSection
    {
        await section.ExecuteAsync(Returning());
        await ThrowsAsync<OperationCanceledException>(() => section.ExecuteAsync(Returning(),
            new CancellationToken(true)));
        return section;
    }

    /// <summary>
    /// Runs a section whose executor cancels the run and then throws.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The exception.</returns>
    private static async Task<OperationCanceledException> CancelledByDriverError(NotebookCodeSection section)
    {
        using var source = new CancellationTokenSource();
        var context = new NotebookExecutionContext
        {
            SqlExecutor = new FakeSqlExecutor(_ =>
            {
                source.Cancel();
                throw new TimeoutException("driver");
            })
        };
        return await ThrowsAsync<OperationCanceledException>(() => section.ExecuteAsync(context,
            source.Token));
    }

    /// <summary>
    /// Tests CancelledSqlKeepsTheEarlierTables.
    /// </summary>
    private static async Task CancelledSqlKeepsTheEarlierTables()
    {
        // Act
        var section = await RanThenCancelled(new NotebookCodeSection { Script = "SELECT 1" });

        // Assert
        Check(section.Results.Count == 1 && section.State == NotebookRunState.Failed);
    }

    /// <summary>
    /// Tests CancelledSqlKeepsTheEarlierStatus.
    /// </summary>
    private static async Task CancelledSqlKeepsTheEarlierStatus()
    {
        // Act
        var section = await RanThenCancelled(new NotebookCodeSection { Script = "SELECT 1" });

        // Assert
        Check(section.ResultStatus == "2 row(s)" && section.StatusMessage == "Stopped by the user.");
    }

    /// <summary>
    /// Tests CancelledBeforeStartingKeepsTheTables.
    /// </summary>
    private static async Task CancelledBeforeStartingKeepsTheTables()
    {
        // Act
        var section = await RanThenCancelled(new NotebookCodeSection { Script = "SELECT 1" });

        // Assert
        Check(section.Log.Entries
            .Single().Message == "Stopped by the user.");
    }

    /// <summary>
    /// Tests StatusCanBeReplacedAfterACancellation.
    /// </summary>
    private static async Task StatusCanBeReplacedAfterACancellation()
    {
        // Setup
        var section = await RanThenCancelled(new NotebookCodeSection { Script = "SELECT 1" });

        // Act
        section.SetResultStatus("Stopped after 5 ms");

        // Assert
        Check(section.ResultStatus == "Stopped after 5 ms" && section.Results.Count == 1);
    }

    /// <summary>
    /// Tests CancelledCSharpKeepsNoTable.
    /// </summary>
    private static async Task CancelledCSharpKeepsNoTable()
    {
        // Act
        var section = await RanThenCancelled(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp });

        // Assert
        Check(section.Results.Count == 0 && section.ResultStatus.Length == 0);
    }

    /// <summary>
    /// Tests CancelledChartKeepsNoTable.
    /// </summary>
    private static async Task CancelledChartKeepsNoTable()
    {
        // Act
        var section = await RanThenCancelled(new NotebookChartSection { SqlScript = "SELECT 1" });

        // Assert
        Check(section.Table is null && section.Data is null);
    }

    /// <summary>
    /// Tests DriverErrorAfterACancellationIsACancellation.
    /// </summary>
    private static async Task DriverErrorAfterACancellationIsACancellation()
    {
        // Setup
        var section = new NotebookCodeSection { Script = "SELECT 1" };

        // Act
        await CancelledByDriverError(section);

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "Stopped by the user.");
    }

    /// <summary>
    /// Tests DriverErrorIsKeptAsTheCause.
    /// </summary>
    private static async Task DriverErrorIsKeptAsTheCause()
    {
        // Act
        var error = await CancelledByDriverError(new NotebookCodeSection { Script = "SELECT 1" });

        // Assert
        Check(error.InnerException is TimeoutException && error.Message == "driver");
    }

    /// <summary>
    /// Tests CancelledOutcomeIsNotApplied.
    /// </summary>
    private static async Task CancelledOutcomeIsNotApplied()
    {
        // Setup
        var section = new NotebookCodeSection { Script = "SELECT 1" };

        // Act
        await ThrowsAsync<OperationCanceledException>(() => section.ExecuteAsync(Returning(),
            new CancellationToken(true)));

        // Assert
        Check(section.Results.Count == 0);
    }

    #endregion
}
