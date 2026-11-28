namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the capture of the console.
/// </summary>
internal static class NotebookConsoleCaptureTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await CapturesTheOutput();
        await CapturesTheErrorOutput();
        await RestoresTheOutput();
        await RestoresTheErrorOutput();
        await MakesTheNextCaptureWait();
        await LetsTheNextCaptureStartAfterDispose();
        await CanceledStartThrows();
        await CanceledStartLeavesTheConsole();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests CapturesTheOutput.
    /// </summary>
    private static async Task CapturesTheOutput()
    {
        // Setup
        using var capture = await NotebookConsoleCapture.StartAsync();

        // Act
        Console.Write("out");

        // Assert
        Check(capture.Output == "out");
    }

    /// <summary>
    /// Tests CapturesTheErrorOutput.
    /// </summary>
    private static async Task CapturesTheErrorOutput()
    {
        // Setup
        using var capture = await NotebookConsoleCapture.StartAsync();

        // Act
        await Console.Error.WriteAsync("error");

        // Assert
        Check(capture.Output == "error");
    }

    /// <summary>
    /// Tests RestoresTheOutput.
    /// </summary>
    private static async Task RestoresTheOutput()
    {
        // Setup
        var before = Console.Out;

        // Act
        (await NotebookConsoleCapture.StartAsync())
            .Dispose();

        // Assert
        Check(ReferenceEquals(Console.Out,
            before));
    }

    /// <summary>
    /// Tests RestoresTheErrorOutput.
    /// </summary>
    private static async Task RestoresTheErrorOutput()
    {
        // Setup
        var before = Console.Error;

        // Act
        (await NotebookConsoleCapture.StartAsync())
            .Dispose();

        // Assert
        Check(ReferenceEquals(Console.Error,
            before));
    }

    /// <summary>
    /// Tests MakesTheNextCaptureWait.
    /// </summary>
    private static async Task MakesTheNextCaptureWait()
    {
        // Setup
        var first = await NotebookConsoleCapture.StartAsync();

        // Act
        var second = NotebookConsoleCapture.StartAsync();
        var waited = !second.IsCompleted;
        first.Dispose();
        (await second)
            .Dispose();

        // Assert
        Check(waited);
    }

    /// <summary>
    /// Tests LetsTheNextCaptureStartAfterDispose.
    /// </summary>
    private static async Task LetsTheNextCaptureStartAfterDispose()
    {
        // Setup
        (await NotebookConsoleCapture.StartAsync())
            .Dispose();

        // Act
        var next = NotebookConsoleCapture.StartAsync();
        var started = next.IsCompletedSuccessfully;
        (await next)
            .Dispose();

        // Assert
        Check(started);
    }

    /// <summary>
    /// Tests CanceledStartThrows.
    /// </summary>
    private static async Task CanceledStartThrows()
    {
        // Setup
        var token = new CancellationToken(true);

        // Act/Assert
        await ThrowsAsync<OperationCanceledException>(() => NotebookConsoleCapture.StartAsync(token));
    }

    /// <summary>
    /// Tests CanceledStartLeavesTheConsole.
    /// </summary>
    private static async Task CanceledStartLeavesTheConsole()
    {
        // Setup
        var before = Console.Out;

        // Act
        await ThrowsAsync<OperationCanceledException>(() => NotebookConsoleCapture.StartAsync(new CancellationToken(true)));

        // Assert
        Check(ReferenceEquals(Console.Out,
            before));
    }

    #endregion
}
