namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the run lifecycle shared by runnable sections.
/// </summary>
internal static class NotebookExecutableSectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        NewSectionIsPending();
        EditorHeightIsNormalized();
        RestoreRunSetsDurationAndLog();
        await ExecuteReturnsTheFinalState();
        await ExecuteRejectsNullContext();
        await ExecuteIsRunningDuringTheRun();
        await ExecuteRaisesTheEvents();
        await ExecuteRecordsTheDuration();
        await ExecuteReplacesTheLog();
        await ExecuteRejectsAChangeWhileRunning();
        await ExecuteTurnsAnErrorIntoAFailure();
        await ExecuteFailsWhenAHandlerThrows();
        await ExecuteThrowsWhenCancelled();
        await ExecuteRecordsACancellation();
        await ExecuteFailsOnAForeignCancellation();
        await ClearOutputResetsTheSection();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a SQL section with a script.
    /// </summary>
    /// <returns>The section.</returns>
    private static NotebookCodeSection Section() => new() { Script = "SELECT 1" };

    /// <summary>
    /// Creates a context whose SQL executor runs a function.
    /// </summary>
    /// <param name="run">The function.</param>
    /// <returns>The context.</returns>
    private static NotebookExecutionContext ContextOf(Func<NotebookSqlRequest, IReadOnlyList<NotebookTable>>? run = null) => new()
    {
        SqlExecutor = new FakeSqlExecutor(run ?? (_ => []))
    };

    /// <summary>
    /// Runs a section with a cancelled token and ignores the cancellation.
    /// </summary>
    /// <param name="section">The section.</param>
    private static Task RunCancelled(NotebookCodeSection section) => ThrowsAsync<OperationCanceledException>(() => section.ExecuteAsync(ContextOf(),
        new CancellationToken(true)));

    /// <summary>
    /// Tests NewSectionIsPending.
    /// </summary>
    private static void NewSectionIsPending()
    {
        // Act
        var section = Section();

        // Assert
        Check(section.State == NotebookRunState.Pending && !section.IsRunning);
        Check(section.StatusMessage.Length == 0 && section.Elapsed == TimeSpan.Zero);
    }

    /// <summary>
    /// Tests EditorHeightIsNormalized.
    /// </summary>
    private static void EditorHeightIsNormalized()
    {
        // Act
        var section = new NotebookCodeSection { EditorHeight = 9000 };

        // Assert
        Check(section.EditorHeight == NotebookValues.MaxHeight);
    }

    /// <summary>
    /// Tests RestoreRunSetsDurationAndLog.
    /// </summary>
    private static void RestoreRunSetsDurationAndLog()
    {
        // Setup
        var section = Section();

        // Act
        section.RestoreRun(TimeSpan.FromSeconds(2),
            [new NotebookLogEntry(NotebookLogLevel.Debug,
                "x",
                DateTimeOffset.UnixEpoch)]);

        // Assert
        Check(section.Elapsed.TotalSeconds == 2 && section.Log.Entries.Count == 1);
    }

    /// <summary>
    /// Tests ExecuteReturnsTheFinalState.
    /// </summary>
    private static async Task ExecuteReturnsTheFinalState()
    {
        // Setup
        var section = Section();

        // Act
        var state = await section.ExecuteAsync(ContextOf());

        // Assert
        Check(state == NotebookRunState.Succeeded && section.State == state);
    }

    /// <summary>
    /// Tests ExecuteRejectsNullContext.
    /// </summary>
    private static async Task ExecuteRejectsNullContext()
    {
        // Setup
        var section = Section();

        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => section.ExecuteAsync(null!));
    }

    /// <summary>
    /// Tests ExecuteIsRunningDuringTheRun.
    /// </summary>
    private static async Task ExecuteIsRunningDuringTheRun()
    {
        // Setup
        var section = Section();
        var wasRunning = false;
        var context = ContextOf(_ =>
        {
            wasRunning = section.IsRunning;
            return [];
        });

        // Act
        await section.ExecuteAsync(context);

        // Assert
        Check(wasRunning && !section.IsRunning);
    }

    /// <summary>
    /// Tests ExecuteRaisesTheEvents.
    /// </summary>
    private static async Task ExecuteRaisesTheEvents()
    {
        // Setup
        var section = Section();
        var events = new List<string>();
        section.Executing += (_,
            _) => events.Add($"Executing {section.State}");
        section.Executed += (_,
            _) => events.Add($"Executed {section.State}");

        // Act
        await section.ExecuteAsync(ContextOf());

        // Assert
        Check(string.Join(", ",
            events) == "Executing Running, Executed Succeeded");
    }

    /// <summary>
    /// Tests ExecuteRecordsTheDuration.
    /// </summary>
    private static async Task ExecuteRecordsTheDuration()
    {
        // Setup
        var section = Section();
        var context = ContextOf(_ =>
        {
            Thread.Sleep(20);
            return [];
        });

        // Act
        await section.ExecuteAsync(context);

        // Assert
        Check(section.Elapsed >= TimeSpan.FromMilliseconds(10));
    }

    /// <summary>
    /// Tests ExecuteReplacesTheLog.
    /// </summary>
    private static async Task ExecuteReplacesTheLog()
    {
        // Setup
        var section = Section();
        section.Log.Add(NotebookLogLevel.Debug,
            "Old");

        // Act
        await section.ExecuteAsync(ContextOf());

        // Assert
        Check(section.Log.Entries
            .All(entry => entry.Message != "Old"));
    }

    /// <summary>
    /// Tests ExecuteRejectsAChangeWhileRunning.
    /// </summary>
    private static async Task ExecuteRejectsAChangeWhileRunning()
    {
        // Setup
        var section = Section();
        InvalidOperationException? error = null;
        var context = ContextOf(_ =>
        {
            error = Throws<InvalidOperationException>(section.ClearOutput);
            return [];
        });

        // Act
        await section.ExecuteAsync(context);

        // Assert
        Check(error?.Message == "The section is running.");
    }

    /// <summary>
    /// Tests ExecuteTurnsAnErrorIntoAFailure.
    /// </summary>
    private static async Task ExecuteTurnsAnErrorIntoAFailure()
    {
        // Setup
        var section = Section();
        var context = ContextOf(_ => throw new TimeoutException("Too slow"));

        // Act
        await section.ExecuteAsync(context);

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "Too slow");
        Check(section.Log.Entries[^1] is { Level: NotebookLogLevel.Error, Message: "Too slow" });
    }

    /// <summary>
    /// Tests ExecuteFailsWhenAHandlerThrows.
    /// </summary>
    private static async Task ExecuteFailsWhenAHandlerThrows()
    {
        // Setup
        var section = Section();
        section.Executing += (_,
            _) => throw new InvalidOperationException("Handler");

        // Act
        var state = await section.ExecuteAsync(ContextOf());

        // Assert
        Check(state == NotebookRunState.Failed && !section.IsRunning);
    }

    /// <summary>
    /// Tests ExecuteThrowsWhenCancelled.
    /// </summary>
    private static async Task ExecuteThrowsWhenCancelled()
    {
        // Setup
        var section = Section();

        // Act/Assert
        await RunCancelled(section);
    }

    /// <summary>
    /// Tests ExecuteRecordsACancellation.
    /// </summary>
    private static async Task ExecuteRecordsACancellation()
    {
        // Setup
        var section = Section();

        // Act
        await RunCancelled(section);

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "Stopped by the user.");
        Check(section.Log.Entries
            .Single().Message == "Stopped by the user.");
    }

    /// <summary>
    /// Tests ExecuteFailsOnAForeignCancellation.
    /// </summary>
    private static async Task ExecuteFailsOnAForeignCancellation()
    {
        // Setup
        var section = Section();
        var context = ContextOf(_ => throw new OperationCanceledException("Timed out"));

        // Act
        await section.ExecuteAsync(context);

        // Assert
        Check(section.State == NotebookRunState.Failed && section.StatusMessage == "Timed out");
    }

    /// <summary>
    /// Tests ClearOutputResetsTheSection.
    /// </summary>
    private static async Task ClearOutputResetsTheSection()
    {
        // Setup
        var section = Section();
        await section.ExecuteAsync(ContextOf(_ => [SampleTables.Categories()]));

        // Act
        section.ClearOutput();

        // Assert
        Check(section.State == NotebookRunState.Pending && section.Log.Entries.Count == 0);
        Check(section.Elapsed == TimeSpan.Zero && section.StatusMessage.Length == 0);
    }

    #endregion
}
