using System.Diagnostics;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The base of the sections that can run and keep a log of their last run.
/// </summary>
public abstract class NotebookExecutableSection : NotebookSection
{
    #region Private Variables

    private double _editorHeight;
    private TimeSpan _elapsed;
    private NotebookRunState _state;
    private string _statusMessage = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a runnable section.
    /// </summary>
    private protected NotebookExecutableSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the editor height, where zero means automatic.
    /// </summary>
    public double EditorHeight
    {
        get => _editorHeight;
        set => SetProperty(ref _editorHeight,
            NotebookValues.Height(value));
    }

    /// <summary>
    /// Gets how long the last run took.
    /// </summary>
    public TimeSpan Elapsed
    {
        get => _elapsed;
        private set => SetProperty(ref _elapsed,
            value);
    }

    /// <summary>
    /// Gets whether the section runs now.
    /// </summary>
    public bool IsRunning => State == NotebookRunState.Running;

    /// <summary>
    /// Gets the log of the last run.
    /// </summary>
    public NotebookLog Log { get; } = new();

    /// <summary>
    /// Gets the state of the section.
    /// </summary>
    public NotebookRunState State
    {
        get => _state;
        private set
        {
            if (SetProperty(ref _state,
                value))
            {
                OnPropertyChanged(nameof(IsRunning));
            }
        }
    }

    /// <summary>
    /// Gets why the last run failed or was skipped, or its summary.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetProperty(ref _statusMessage,
            value);
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a run ended, successfully or not.
    /// </summary>
    public event EventHandler? Executed;

    /// <summary>
    /// Occurs when a run starts.
    /// </summary>
    public event EventHandler? Executing;

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes what the last run produced.
    /// </summary>
    public void ClearOutput()
    {
        ThrowIfRunning();
        ResetOutput();
        Elapsed = TimeSpan.Zero;
        StatusMessage = string.Empty;
        State = NotebookRunState.Pending;
    }

    /// <summary>
    /// Runs the section, replacing the log and results of the previous run.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The final state.</returns>
    public async Task<NotebookRunState> ExecuteAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        var restore = SnapshotOutput();
        ClearOutput();
        State = NotebookRunState.Running;
        var stopwatch = Stopwatch.StartNew();
        var result = NotebookSectionResult.Failed(string.Empty);
        try
        {
            result = await TryRunAsync(context,
                cancellationToken)
                .ConfigureAwait(true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            result = Stop(restore);
            throw;
        }
        finally
        {
            Finish(result,
                stopwatch.Elapsed);
        }
        return result.State;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Restores the saved duration and log.
    /// </summary>
    /// <param name="elapsed">The duration.</param>
    /// <param name="entries">The log entries.</param>
    internal void RestoreRun(TimeSpan elapsed,
        IEnumerable<NotebookLogEntry> entries)
    {
        Elapsed = elapsed;
        Log.Load(entries);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Logs an error and fails the run.
    /// </summary>
    /// <param name="message">Why it failed.</param>
    /// <returns>The result.</returns>
    private protected NotebookSectionResult Fail(string message)
    {
        Write(NotebookLogLevel.Error,
            message);
        return NotebookSectionResult.Failed(message);
    }

    /// <summary>
    /// Fails a run whose executor may have logged the error already.
    /// </summary>
    /// <param name="line">The line written when no error is logged yet.</param>
    /// <param name="status">Why it failed.</param>
    /// <returns>The result.</returns>
    private protected NotebookSectionResult FailExecution(string line,
        string status)
    {
        if (!Log.Entries
            .Any(entry => entry.Level is NotebookLogLevel.Error or NotebookLogLevel.Critical))
        {
            Write(NotebookLogLevel.Error,
                line);
        }
        return NotebookSectionResult.Failed(status);
    }

    /// <summary>
    /// Removes the output of a run.
    /// </summary>
    private protected virtual void ResetOutput() => Log.Clear();

    /// <summary>
    /// Runs what the section holds.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private protected abstract Task<NotebookSectionResult> RunAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken);

    /// <summary>
    /// Logs a warning and skips the run.
    /// </summary>
    /// <param name="message">Why it was skipped.</param>
    /// <returns>The result.</returns>
    private protected NotebookSectionResult Skip(string message)
    {
        Write(NotebookLogLevel.Warning,
            message);
        return NotebookSectionResult.Skipped(message);
    }

    /// <summary>
    /// Remembers the output that a cancelled run brings back.
    /// </summary>
    /// <returns>The action that brings it back.</returns>
    private protected virtual Action SnapshotOutput() => static () => { };

    /// <summary>
    /// Adds a log line unless it is empty.
    /// </summary>
    /// <param name="level">The level.</param>
    /// <param name="message">The message.</param>
    private protected void Write(NotebookLogLevel level,
        string message)
    {
        if (message.Length > 0)
        {
            Log.Add(level,
                message);
        }
    }

    /// <summary>
    /// Records how a run ended.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="elapsed">The duration.</param>
    private void Finish(NotebookSectionResult result,
        TimeSpan elapsed)
    {
        Elapsed = elapsed;
        StatusMessage = result.Message;
        State = result.State;
        Executed?.Invoke(this,
            EventArgs.Empty);
    }

    /// <summary>
    /// Records a cancelled run and brings back the kept output.
    /// </summary>
    /// <param name="restore">Brings back the kept output.</param>
    /// <returns>The result.</returns>
    private NotebookSectionResult Stop(Action restore)
    {
        restore();
        return Fail(Messages.SectionStopped(this));
    }

    /// <summary>
    /// Rejects a change while the section runs.
    /// </summary>
    private void ThrowIfRunning()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("The section is running.");
        }
    }

    /// <summary>
    /// Runs the section. Errors fail the run, and any error after a cancellation counts as the cancellation.
    /// </summary>
    /// <param name="context">What the run needs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result.</returns>
    private async Task<NotebookSectionResult> TryRunAsync(NotebookExecutionContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            Executing?.Invoke(this,
                EventArgs.Empty);
            cancellationToken.ThrowIfCancellationRequested();
            return await RunAsync(context,
                cancellationToken)
                .ConfigureAwait(true);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return Fail(exception.Message);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new OperationCanceledException(exception.Message,
                exception,
                cancellationToken);
        }
    }

    #endregion
}
