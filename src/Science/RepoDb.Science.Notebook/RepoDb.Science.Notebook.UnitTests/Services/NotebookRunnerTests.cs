namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the run of a whole notebook.
/// </summary>
internal static class NotebookRunnerTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await ImportsRunFirst();
        await SectionsHaveNames();
        await ItemsCarryTheOutcome();
        await SkippedScriptDoesNotStopTheRun();
        await FailureStopsTheRun();
        await SectionsAfterAFailureAreSkipped();
        await ProgressIsReported();
        await CancelledRunSkipsEverySection();
        await CancellationDuringARunStopsIt();
        await CancellationAfterASectionStopsTheRun();
        await EmptyNotebookSucceeds();
        await NullContextIsRejected();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a notebook whose imports are not the first section.
    /// </summary>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Mixed()
    {
        var document = new NotebookDocument();
        document.Sections.Add(new NotebookHeaderSection());
        document.Sections.Add(new NotebookCodeSection { Script = "SELECT 1" });
        document.Sections.Add(new NotebookCodeSection { Language = NotebookCodeLanguage.CSharp });
        document.Sections.Add(new NotebookChartSection { SqlScript = "SELECT 2" });
        document.Sections.Add(new NotebookImportsSection());
        return document;
    }

    /// <summary>
    /// Creates a context that charts successfully.
    /// </summary>
    /// <returns>The context.</returns>
    private static NotebookExecutionContext Context() => new() { SqlExecutor = new FakeSqlExecutor(SampleTables.Timed()) };

    /// <summary>
    /// Creates a context whose SQL executor cancels the run.
    /// </summary>
    /// <param name="source">The cancellation source.</param>
    /// <param name="throws">Whether the executor also throws.</param>
    /// <returns>The context.</returns>
    private static NotebookExecutionContext Cancelling(CancellationTokenSource source,
        bool throws) => new()
    {
        SqlExecutor = new FakeSqlExecutor(_ =>
        {
            source.Cancel();
            return throws ? throw new OperationCanceledException(source.Token) : [];
        })
    };

    /// <summary>
    /// Runs a notebook while cancelling it.
    /// </summary>
    /// <param name="throws">Whether the executor also throws.</param>
    /// <returns>The result.</returns>
    private static async Task<NotebookRunResult> RunCancelling(bool throws)
    {
        using var source = new CancellationTokenSource();
        return await Mixed()
            .ExecuteAsync(Cancelling(source,
                throws),
            cancellationToken: source.Token);
    }

    /// <summary>
    /// Tests ImportsRunFirst.
    /// </summary>
    private static async Task ImportsRunFirst()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(Context());

        // Assert
        Check(result.Items[0].Section is NotebookImportsSection && result.Items.Count == 4);
    }

    /// <summary>
    /// Tests SectionsHaveNames.
    /// </summary>
    private static async Task SectionsHaveNames()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(Context());

        // Assert
        Check(string.Join('|',
            result.Items
                .Select(item => item.Name)) == "Imports|SQLSection1|C# 3|CHART 4");
    }

    /// <summary>
    /// Tests ItemsCarryTheOutcome.
    /// </summary>
    private static async Task ItemsCarryTheOutcome()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(Context());

        // Assert
        Check(result.Items[0] is { State: NotebookRunState.Succeeded, Message: "No imports yet." });
        Check(result.Items[1].Elapsed == result.Items[1].Section.Elapsed);
    }

    /// <summary>
    /// Tests SkippedScriptDoesNotStopTheRun.
    /// </summary>
    private static async Task SkippedScriptDoesNotStopTheRun()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(Context());

        // Assert
        Check(result.Succeeded && result.Items[2].State == NotebookRunState.Skipped);
        Check(result.Items[3].State == NotebookRunState.Succeeded);
    }

    /// <summary>
    /// Tests FailureStopsTheRun.
    /// </summary>
    private static async Task FailureStopsTheRun()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(!result.Succeeded && result.FailureMessage == "Stopped: SQLSection1 failed.");
    }

    /// <summary>
    /// Tests SectionsAfterAFailureAreSkipped.
    /// </summary>
    private static async Task SectionsAfterAFailureAreSkipped()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(new NotebookExecutionContext());

        // Assert
        Check(result.Items
            .Skip(2)
            .All(item => item.State == NotebookRunState.Skipped && item.Message == result.FailureMessage));
        Check(result.Items[3].Section.State == NotebookRunState.Pending);
    }

    /// <summary>
    /// Tests ProgressIsReported.
    /// </summary>
    private static async Task ProgressIsReported()
    {
        // Setup
        var progress = new ProgressRecorder();

        // Act
        await Mixed()
            .ExecuteAsync(Context(),
            progress);

        // Assert
        Check(progress.Reports.Count == 4 && progress.Reports[0] is { Completed: 1, Total: 4, Percentage: 25 });
        Check(progress.Reports[^1].Percentage == 100);
    }

    /// <summary>
    /// Tests CancelledRunSkipsEverySection.
    /// </summary>
    private static async Task CancelledRunSkipsEverySection()
    {
        // Act
        var result = await Mixed()
            .ExecuteAsync(Context(),
            cancellationToken: new CancellationToken(true));

        // Assert
        Check(result.FailureMessage == "Stopped by the user.");
        Check(result.Items
            .All(item => item.State == NotebookRunState.Skipped));
    }

    /// <summary>
    /// Tests CancellationDuringARunStopsIt.
    /// </summary>
    private static async Task CancellationDuringARunStopsIt()
    {
        // Act
        var result = await RunCancelling(true);

        // Assert
        Check(result.FailureMessage == "Stopped by the user." && result.Items[1].State == NotebookRunState.Failed);
        Check(result.Items[2].State == NotebookRunState.Skipped);
    }

    /// <summary>
    /// Tests CancellationAfterASectionStopsTheRun.
    /// </summary>
    private static async Task CancellationAfterASectionStopsTheRun()
    {
        // Act
        var result = await RunCancelling(false);

        // Assert
        Check(result.FailureMessage == "Stopped by the user." && result.Items[1].State == NotebookRunState.Succeeded);
        Check(result.Items[2].State == NotebookRunState.Skipped);
    }

    /// <summary>
    /// Tests EmptyNotebookSucceeds.
    /// </summary>
    private static async Task EmptyNotebookSucceeds()
    {
        // Act
        var result = await new NotebookDocument().ExecuteAsync(Context());

        // Assert
        Check(result.Succeeded && result.Items.Count == 0);
    }

    /// <summary>
    /// Tests NullContextIsRejected.
    /// </summary>
    private static async Task NullContextIsRejected()
    {
        // Setup
        var document = new NotebookDocument();

        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => document.ExecuteAsync(null!));
    }

    #endregion
}
