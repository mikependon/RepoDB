namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests how a section applies the outcome of a SQL executor.
/// </summary>
internal static class NotebookSqlOutcomeTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunStatusTests();
        await RunFailureTests();
        await RunLogTests();
        RunSetStatusTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the status scenarios.
    /// </summary>
    private static async Task RunStatusTests()
    {
        await HostStatusIsUsed();
        await AffectedRowsGiveTheStatus();
        await TablesWinOverAffectedRows();
        await StatusCountsTheFirstTable();
        await EmptyOutcomeCountsNoRows();
    }

    /// <summary>
    /// Runs the failure scenarios.
    /// </summary>
    private static async Task RunFailureTests()
    {
        await ReturnedErrorFailsTheRun();
        await ReturnedErrorIsLogged();
        await ReturnedErrorShowsNoTable();
        await ErrorLoggedByTheExecutorIsNotRepeated();
        await CriticalLoggedByTheExecutorIsNotRepeated();
        await ThrownErrorUsesTheSqlMessage();
    }

    /// <summary>
    /// Runs the log scenarios.
    /// </summary>
    private static async Task RunLogTests()
    {
        await ExecutorReceivesTheSectionLog();
        await ExecutorReceivesTheNotebookMessages();
        await ExecutorLinesSitBetweenStartAndEnd();
        await EmptyMessageRemovesTheLine();
    }

    /// <summary>
    /// Runs the host status scenarios.
    /// </summary>
    private static void RunSetStatusTests()
    {
        SetResultStatusReplacesTheStatus();
        SetResultStatusIsNotAContentChange();
        SetResultStatusRejectsNull();
    }

    /// <summary>
    /// Runs a SQL section against an outcome.
    /// </summary>
    /// <param name="result">The outcome.</param>
    /// <returns>The section.</returns>
    private static Task<NotebookCodeSection> Ran(NotebookSqlResult result) => RanWith(_ => result);

    /// <summary>
    /// Runs a SQL section against a function.
    /// </summary>
    /// <param name="run">The function.</param>
    /// <returns>The section.</returns>
    private static async Task<NotebookCodeSection> RanWith(Func<NotebookSqlRequest, NotebookSqlResult> run)
    {
        var section = new NotebookCodeSection { Script = "UPDATE t SET a = 1" };
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = FakeSqlExecutor.Answering(run) });
        return section;
    }

    /// <summary>
    /// Writes the messages of a log in one line.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The messages.</returns>
    private static string LinesOf(NotebookExecutableSection section) => string.Join('|',
        section.Log.Entries
            .Select(entry => entry.Message));

    /// <summary>
    /// Tests HostStatusIsUsed.
    /// </summary>
    private static async Task HostStatusIsUsed()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { RowsAffected = 2, Status = "2 row(s) affected in 5 ms" });

        // Assert
        Check(section.ResultStatus == "2 row(s) affected in 5 ms" && section.StatusMessage == section.ResultStatus);
        Check(section.Log.Entries[^1].Message == "Done: 2 row(s) affected in 5 ms");
    }

    /// <summary>
    /// Tests AffectedRowsGiveTheStatus.
    /// </summary>
    private static async Task AffectedRowsGiveTheStatus()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { RowsAffected = 3 });

        // Assert
        Check(section.ResultStatus == "3 row(s) affected");
    }

    /// <summary>
    /// Tests TablesWinOverAffectedRows.
    /// </summary>
    private static async Task TablesWinOverAffectedRows()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { RowsAffected = 3, Tables = [SampleTables.Categories()] });

        // Assert
        Check(section.ResultStatus == "2 row(s)");
    }

    /// <summary>
    /// Tests StatusCountsTheFirstTable.
    /// </summary>
    private static async Task StatusCountsTheFirstTable()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { Tables = [SampleTables.Categories(), SampleTables.Timed()] });

        // Assert
        Check(section.ResultStatus == "2 row(s)" && section.Results.Count == 2);
    }

    /// <summary>
    /// Tests EmptyOutcomeCountsNoRows.
    /// </summary>
    private static async Task EmptyOutcomeCountsNoRows()
    {
        // Act
        var section = await Ran(new NotebookSqlResult());

        // Assert
        Check(section.ResultStatus == "0 row(s)");
    }

    /// <summary>
    /// Tests ReturnedErrorFailsTheRun.
    /// </summary>
    private static async Task ReturnedErrorFailsTheRun()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { Error = "denied" });

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "denied");
    }

    /// <summary>
    /// Tests ReturnedErrorIsLogged.
    /// </summary>
    private static async Task ReturnedErrorIsLogged()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { Error = "denied" });

        // Assert
        Check(section.Log.Entries[^1] is { Level: NotebookLogLevel.Error, Message: "denied" });
    }

    /// <summary>
    /// Tests ReturnedErrorShowsNoTable.
    /// </summary>
    private static async Task ReturnedErrorShowsNoTable()
    {
        // Act
        var section = await Ran(new NotebookSqlResult { Error = "denied", Tables = [SampleTables.Categories()] });

        // Assert
        Check(section.Results.Count == 0 && section.ResultStatus.Length == 0);
    }

    /// <summary>
    /// Tests ErrorLoggedByTheExecutorIsNotRepeated.
    /// </summary>
    private static async Task ErrorLoggedByTheExecutorIsNotRepeated()
    {
        // Act
        var section = await RanWith(request =>
        {
            request.Log.Add(NotebookLogLevel.Error,
                "Failed on 'Main': denied");
            return new NotebookSqlResult { Error = "denied" };
        });

        // Assert
        Check(LinesOf(section) == "Running the script.|Failed on 'Main': denied" && section.StatusMessage == "denied");
    }

    /// <summary>
    /// Tests CriticalLoggedByTheExecutorIsNotRepeated.
    /// </summary>
    private static async Task CriticalLoggedByTheExecutorIsNotRepeated()
    {
        // Act
        var section = await RanWith(request =>
        {
            request.Log.Add(NotebookLogLevel.Critical,
                "down");
            return new NotebookSqlResult { Error = "denied" };
        });

        // Assert
        Check(section.Log.Entries.Count == 2);
    }

    /// <summary>
    /// Tests ThrownErrorUsesTheSqlMessage.
    /// </summary>
    private static async Task ThrownErrorUsesTheSqlMessage()
    {
        // Setup
        var document = new NotebookDocument(new TagMessages());
        var section = new NotebookCodeSection { Script = "SELECT 1" };
        document.Sections.Add(section);

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext
        {
            SqlExecutor = new FakeSqlExecutor(_ => throw new TimeoutException("slow"))
        });

        // Assert
        Check(section.StatusMessage == "slow" && section.Log.Entries[^1].Message == "sql-failed:slow");
    }

    /// <summary>
    /// Tests ExecutorReceivesTheSectionLog.
    /// </summary>
    private static async Task ExecutorReceivesTheSectionLog()
    {
        // Setup
        NotebookLog? received = null;

        // Act
        var section = await RanWith(request =>
        {
            received = request.Log;
            return new NotebookSqlResult();
        });

        // Assert
        Check(ReferenceEquals(received,
            section.Log));
    }

    /// <summary>
    /// Tests ExecutorReceivesTheNotebookMessages.
    /// </summary>
    private static async Task ExecutorReceivesTheNotebookMessages()
    {
        // Setup
        var messages = new TagMessages();
        var document = new NotebookDocument(messages);
        var section = new NotebookCodeSection { Script = "SELECT 1" };
        document.Sections.Add(section);
        var executor = new FakeSqlExecutor();

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = executor });

        // Assert
        Check(ReferenceEquals(executor.Requests[0].Messages,
            messages));
    }

    /// <summary>
    /// Tests ExecutorLinesSitBetweenStartAndEnd.
    /// </summary>
    private static async Task ExecutorLinesSitBetweenStartAndEnd()
    {
        // Act
        var section = await RanWith(request =>
        {
            request.Log.Add(NotebookLogLevel.Debug,
                "Operation: ExecuteNonQuery");
            return new NotebookSqlResult { RowsAffected = 1 };
        });

        // Assert
        Check(LinesOf(section) == "Running the script.|Operation: ExecuteNonQuery|Done: 1 row(s) affected");
    }

    /// <summary>
    /// Tests EmptyMessageRemovesTheLine.
    /// </summary>
    private static async Task EmptyMessageRemovesTheLine()
    {
        // Setup
        var document = new NotebookDocument(new SilentMessages());
        var section = new NotebookCodeSection { Script = "SELECT 1" };
        document.Sections.Add(section);

        // Act
        await section.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor() });

        // Assert
        Check(LinesOf(section) == "Done: 0 row(s)");
    }

    /// <summary>
    /// Tests SetResultStatusReplacesTheStatus.
    /// </summary>
    private static void SetResultStatusReplacesTheStatus()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        section.SetResultStatus("second table");

        // Assert
        Check(section.ResultStatus == "second table");
    }

    /// <summary>
    /// Tests SetResultStatusIsNotAContentChange.
    /// </summary>
    private static void SetResultStatusIsNotAContentChange()
    {
        // Setup
        var section = new NotebookCodeSection();
        var recorder = new PropertyRecorder(section);

        // Act
        section.SetResultStatus("second table");

        // Assert
        Check(!section.HasChanges && recorder.Names
            .Single() == nameof(NotebookScriptSection.ResultStatus));
    }

    /// <summary>
    /// Tests SetResultStatusRejectsNull.
    /// </summary>
    private static void SetResultStatusRejectsNull()
    {
        // Setup
        var section = new NotebookCodeSection();

        // Act
        Action operation = () => section.SetResultStatus(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
