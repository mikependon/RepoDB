using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Redirects the console of the process into a text, one capture at a time.
/// </summary>
internal sealed class NotebookConsoleCapture : IDisposable
{
    #region Private Variables

    private static readonly SemaphoreSlim Gate = new(1,
        1);
    private readonly TextWriter _previousError;
    private readonly TextWriter _previousOutput;
    private readonly StringWriter _writer = new(CultureInfo.InvariantCulture);

    #endregion

    #region Constructors

    /// <summary>
    /// Redirects the console. The caller holds the gate.
    /// </summary>
    private NotebookConsoleCapture()
    {
        _previousError = Console.Error;
        _previousOutput = Console.Out;
        var shared = TextWriter.Synchronized(_writer);
        Console.SetError(shared);
        Console.SetOut(shared);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets what was written to the console since the capture started.
    /// </summary>
    public string Output => _writer.ToString();

    #endregion

    #region Public Methods

    /// <summary>
    /// Waits for any other capture to end and then starts one.
    /// </summary>
    /// <param name="cancellationToken">Stops the wait.</param>
    /// <returns>The capture, which gives the console back when disposed.</returns>
    public static async Task<NotebookConsoleCapture> StartAsync(CancellationToken cancellationToken = default)
    {
        await Gate.WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        return new NotebookConsoleCapture();
    }

    /// <summary>
    /// Gives the console back and lets the next capture start.
    /// </summary>
    public void Dispose()
    {
        Console.SetError(_previousError);
        Console.SetOut(_previousOutput);
        _writer.Dispose();
        Gate.Release();
    }

    #endregion
}
