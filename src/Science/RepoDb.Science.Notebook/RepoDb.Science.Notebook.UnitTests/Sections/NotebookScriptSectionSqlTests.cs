namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests how script sections run SQL.
/// </summary>
internal static class NotebookScriptSectionSqlTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        TableHeightIsNormalized();
        ConnectionIsContent();
        RestoreSetsTheResults();
        await GuardCanBlockARun();
        await GuardReceivesTheSection();
        await RunFailsWithoutAnExecutor();
        await RunFailsWithoutAScript();
        await RequestCarriesTheScript();
        await ResultsAreKept();
        await ResultStatusCountsTheRows();
        await RunIsLogged();
        await ResultsAreLimited();
        await TruncationIsWarned();
        await FailureRemovesTheEarlierResults();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a SQL section with a script.
    /// </summary>
    /// <returns>The section.</returns>
    private static NotebookCodeSection Section() => new() { Script = "SELECT 1" };

    /// <summary>
    /// Runs a section against tables.
    /// </summary>
    /// <param name="rowLimit">The row limit.</param>
    /// <returns>The section.</returns>
    private static async Task<NotebookCodeSection> Ran(int rowLimit = 10)
    {
        var section = Section();
        await section.ExecuteAsync(new NotebookExecutionContext
        {
            SqlExecutor = new FakeSqlExecutor(SampleTables.Categories()),
            RowLimit = rowLimit
        });
        return section;
    }

    /// <summary>
    /// Tests TableHeightIsNormalized.
    /// </summary>
    private static void TableHeightIsNormalized()
    {
        // Act
        var section = new NotebookCodeSection { TableHeight = -1 };

        // Assert
        Check(section.TableHeight == 0);
    }

    /// <summary>
    /// Tests ConnectionIsContent.
    /// </summary>
    private static void ConnectionIsContent()
    {
        // Setup
        var section = Section();
        section.MarkSaved();

        // Act
        section.ConnectionId = Guid.NewGuid();

        // Assert
        Check(section.HasChanges);
    }

    /// <summary>
    /// Tests RestoreSetsTheResults.
    /// </summary>
    private static void RestoreSetsTheResults()
    {
        // Setup
        var section = Section();

        // Act
        section.RestoreResults("out",
            [SampleTables.Categories()],
            "2 row(s)");

        // Assert
        Check(section.Output == "out" && section.Results.Count == 1 && section.ResultStatus == "2 row(s)");
    }

    /// <summary>
    /// Tests GuardCanBlockARun.
    /// </summary>
    private static async Task GuardCanBlockARun()
    {
        // Setup
        var section = Section();
        var executor = new FakeSqlExecutor();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = executor, Guard = new FakeGuard("Not allowed") });

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "Not allowed");
        Check(executor.Requests.Count == 0);
    }

    /// <summary>
    /// Tests GuardReceivesTheSection.
    /// </summary>
    private static async Task GuardReceivesTheSection()
    {
        // Setup
        var section = Section();
        var guard = new FakeGuard(null);

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor(), Guard = guard });

        // Assert
        Check(guard.Checked
            .Single() == section && section.State == NotebookRunState.Succeeded);
    }

    /// <summary>
    /// Tests RunFailsWithoutAnExecutor.
    /// </summary>
    private static async Task RunFailsWithoutAnExecutor()
    {
        // Setup
        var section = Section();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(section.StatusMessage == "No SQL executor is configured.");
    }

    /// <summary>
    /// Tests RunFailsWithoutAScript.
    /// </summary>
    private static async Task RunFailsWithoutAScript()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor() });

        // Assert
        Check(section.StatusMessage == "Type the SQL script to run.");
    }

    /// <summary>
    /// Tests RequestCarriesTheScript.
    /// </summary>
    private static async Task RequestCarriesTheScript()
    {
        // Setup
        var id = Guid.NewGuid();
        var section = new NotebookCodeSection { Script = "SELECT 2", ConnectionId = id };
        var executor = new FakeSqlExecutor();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = executor, RowLimit = 5 });

        // Assert
        Check(executor.Requests
            .Single() is { Script: "SELECT 2", RowLimit: 5 } request && request.ConnectionId == id);
    }

    /// <summary>
    /// Tests ResultsAreKept.
    /// </summary>
    private static async Task ResultsAreKept()
    {
        // Act
        var section = await Ran();

        // Assert
        Check(section.Results
            .Single().Rows.Count == 2);
    }

    /// <summary>
    /// Tests ResultStatusCountsTheRows.
    /// </summary>
    private static async Task ResultStatusCountsTheRows()
    {
        // Act
        var section = await Ran();

        // Assert
        Check(section.ResultStatus == "2 row(s)" && section.StatusMessage == "2 row(s)");
    }

    /// <summary>
    /// Tests RunIsLogged.
    /// </summary>
    private static async Task RunIsLogged()
    {
        // Act
        var section = await Ran();

        // Assert
        Check(string.Join('|',
            section.Log.Entries
                .Select(entry => entry.Message)) == "Running the script.|Done: 2 row(s)");
    }

    /// <summary>
    /// Tests ResultsAreLimited.
    /// </summary>
    private static async Task ResultsAreLimited()
    {
        // Act
        var section = await Ran(1);

        // Assert
        Check(section.Results
            .Single() is { Rows.Count: 1, IsTruncated: true });
    }

    /// <summary>
    /// Tests TruncationIsWarned.
    /// </summary>
    private static async Task TruncationIsWarned()
    {
        // Act
        var section = await Ran(1);

        // Assert
        Check(section.Log.Entries[^1].Level == NotebookLogLevel.Warning);
    }

    /// <summary>
    /// Tests FailureRemovesTheEarlierResults.
    /// </summary>
    private static async Task FailureRemovesTheEarlierResults()
    {
        // Setup
        var section = await Ran();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(section.Results.Count == 0 && section.ResultStatus.Length == 0);
    }

    #endregion
}
