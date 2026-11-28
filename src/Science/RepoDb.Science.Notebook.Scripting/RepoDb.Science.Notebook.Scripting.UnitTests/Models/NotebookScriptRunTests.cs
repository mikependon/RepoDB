namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests one run of a script.
/// </summary>
internal static class NotebookScriptRunTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunArgumentsAsync();
        await RunSuccessAsync();
        await RunFailureAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the arguments and members.
    /// </summary>
    private static async Task RunArgumentsAsync()
    {
        await RejectsANullRequest();
        await RejectsANullHost();
        RejectsANullConsole();
        await ExposesTheRequestAndTheHost();
        await CountsTheImportLines();
    }

    /// <summary>
    /// Runs the scenarios of success.
    /// </summary>
    private static async Task RunSuccessAsync()
    {
        await SuccessHasNoErrorAndNoLog();
        await SuccessFormatsTheReturnedValue();
        await SuccessShowsAReturnedTable();
        await SuccessKeepsTheOutput();
        await SuccessHasTheChartCutAtTheRowLimit();
        await SuccessWithoutAChartHasNone();
    }

    /// <summary>
    /// Runs the scenarios of failure.
    /// </summary>
    private static async Task RunFailureAsync()
    {
        await FailureUsesTheFirstLogAsTheError();
        await FailureKeepsTheShownTables();
        await FailureRejectsNull();
    }

    /// <summary>
    /// Creates the request of the runs.
    /// </summary>
    /// <returns>The request, whose row limit is one.</returns>
    private static NotebookScriptRequest Request() => new("return 1;",
        "using System.Text;",
        new Dictionary<string, IReadOnlyList<NotebookTable>>(StringComparer.Ordinal),
        1);

    /// <summary>
    /// Creates a host.
    /// </summary>
    /// <returns>The host.</returns>
    private static NotebookScriptHost Host() => new(new NotebookScriptContext(NotebookScriptingMessages.Default),
        NotebookScriptingMessages.Default);

    /// <summary>
    /// Builds the outcome of a run.
    /// </summary>
    /// <param name="outcome">Builds the outcome.</param>
    /// <returns>The outcome.</returns>
    private static async Task<NotebookScriptResult> Outcome(Func<NotebookScriptRun, NotebookScriptResult> outcome)
    {
        using var console = await NotebookConsoleCapture.StartAsync();
        return outcome(new NotebookScriptRun(Request(),
            Host(),
            console));
    }

    /// <summary>
    /// Tests RejectsANullRequest.
    /// </summary>
    private static async Task RejectsANullRequest()
    {
        // Setup
        using var console = await NotebookConsoleCapture.StartAsync();

        // Act/Assert
        Throws<ArgumentNullException>(() => _ = new NotebookScriptRun(null!,
            Host(),
            console));
    }

    /// <summary>
    /// Tests RejectsANullHost.
    /// </summary>
    private static async Task RejectsANullHost()
    {
        // Setup
        using var console = await NotebookConsoleCapture.StartAsync();

        // Act/Assert
        Throws<ArgumentNullException>(() => _ = new NotebookScriptRun(Request(),
            null!,
            console));
    }

    /// <summary>
    /// Tests RejectsANullConsole.
    /// </summary>
    private static void RejectsANullConsole()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => _ = new NotebookScriptRun(Request(),
            Host(),
            null!));
    }

    /// <summary>
    /// Tests ExposesTheRequestAndTheHost.
    /// </summary>
    private static async Task ExposesTheRequestAndTheHost()
    {
        // Setup
        using var console = await NotebookConsoleCapture.StartAsync();
        var request = Request();
        var host = Host();

        // Act
        var run = new NotebookScriptRun(request,
            host,
            console);

        // Assert
        Check(ReferenceEquals(run.Request,
            request) && ReferenceEquals(run.Host,
            host));
    }

    /// <summary>
    /// Tests CountsTheImportLines.
    /// </summary>
    private static async Task CountsTheImportLines()
    {
        // Setup
        using var console = await NotebookConsoleCapture.StartAsync();

        // Act
        var run = new NotebookScriptRun(Request(),
            Host(),
            console);

        // Assert
        Check(run.ImportLines == 1);
    }

    /// <summary>
    /// Tests SuccessHasNoErrorAndNoLog.
    /// </summary>
    private static async Task SuccessHasNoErrorAndNoLog()
    {
        // Act
        var result = await Outcome(run => run.Success(null));

        // Assert
        Check(result.Succeeded && result.Error is null && result.Logs.Count == 0);
    }

    /// <summary>
    /// Tests SuccessFormatsTheReturnedValue.
    /// </summary>
    private static async Task SuccessFormatsTheReturnedValue()
    {
        // Act
        var result = await Outcome(run => run.Success(42));

        // Assert
        Check(result.ReturnValue == "42");
    }

    /// <summary>
    /// Tests SuccessShowsAReturnedTable.
    /// </summary>
    private static async Task SuccessShowsAReturnedTable()
    {
        // Act
        var result = await Outcome(run => run.Success(SampleTables.Numbers()));

        // Assert
        Check(result.Tables
            .Single().Title == "Numbers");
    }

    /// <summary>
    /// Tests SuccessKeepsTheOutput.
    /// </summary>
    private static async Task SuccessKeepsTheOutput()
    {
        // Act
        var result = await Outcome(run =>
        {
            Console.Write("out");
            return run.Success(null);
        });

        // Assert
        Check(result.Output == "out");
    }

    /// <summary>
    /// Tests SuccessHasTheChartCutAtTheRowLimit.
    /// </summary>
    private static async Task SuccessHasTheChartCutAtTheRowLimit()
    {
        // Act
        var result = await Outcome(run =>
        {
            run.Host.Chart.FromDataTable(SampleTables.Numbers());
            return run.Success(null);
        });

        // Assert
        Check(result.Chart!.Rows.Count == 1 && result.Chart.IsTruncated);
    }

    /// <summary>
    /// Tests SuccessWithoutAChartHasNone.
    /// </summary>
    private static async Task SuccessWithoutAChartHasNone()
    {
        // Act
        var result = await Outcome(run => run.Success(null));

        // Assert
        Check(result.Chart is null);
    }

    /// <summary>
    /// Tests FailureUsesTheFirstLogAsTheError.
    /// </summary>
    private static async Task FailureUsesTheFirstLogAsTheError()
    {
        // Act
        var result = await Outcome(run => run.Failure([NotebookScriptDiagnostics.Error("first"), NotebookScriptDiagnostics.Warning("second")]));

        // Assert
        Check(!result.Succeeded && result.Error == "first" && result.Logs.Count == 2);
    }

    /// <summary>
    /// Tests FailureKeepsTheShownTables.
    /// </summary>
    private static async Task FailureKeepsTheShownTables()
    {
        // Act
        var result = await Outcome(run =>
        {
            run.Host.Tables.AddFrom(SampleTables.Numbers());
            return run.Failure([NotebookScriptDiagnostics.Error("failed")]);
        });

        // Assert
        Check(result.Tables.Count == 1);
    }

    /// <summary>
    /// Tests FailureRejectsNull.
    /// </summary>
    private static async Task FailureRejectsNull()
    {
        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => Outcome(run => run.Failure(null!)));
    }

    #endregion
}
