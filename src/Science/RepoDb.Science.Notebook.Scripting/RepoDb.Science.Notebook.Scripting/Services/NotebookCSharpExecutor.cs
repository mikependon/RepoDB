using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Runs the C# scripts of a notebook with Roslyn. A script can do much of what the process can: it is not sandboxed.
/// </summary>
[RequiresDynamicCode(Requirement)]
[RequiresUnreferencedCode(Requirement)]
public sealed class NotebookCSharpExecutor : INotebookCSharpExecutor
{
    #region Private Variables

    private const string Requirement = "C# scripts are compiled and loaded while the application runs.";
    private readonly Func<bool> _allowDatabaseAccess;
    private readonly bool _isDynamicCodeSupported;
    private readonly NotebookScriptingMessages _messages;
    private readonly NotebookMessages _notebookMessages;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an executor.
    /// </summary>
    /// <param name="allowDatabaseAccess">Asked before each run whether the script may access the database. Null means never.</param>
    /// <param name="messages">The texts of the problems of scripts, or null for the default ones.</param>
    /// <param name="notebookMessages">The texts of the problems of the imports, or null for the default ones.</param>
    /// <param name="scripts">Creates the scripts, or null for a factory over every assembly of the application.</param>
    public NotebookCSharpExecutor(Func<bool>? allowDatabaseAccess = null,
        NotebookScriptingMessages? messages = null,
        NotebookMessages? notebookMessages = null,
        NotebookScriptFactory? scripts = null)
        : this(RuntimeFeature.IsDynamicCodeSupported,
            allowDatabaseAccess,
            messages,
            notebookMessages,
            scripts)
    {
    }

    /// <summary>
    /// Creates an executor for a runtime.
    /// </summary>
    /// <param name="isDynamicCodeSupported">Whether the runtime can run code compiled while it runs.</param>
    /// <param name="allowDatabaseAccess">Whether a script may access the database.</param>
    /// <param name="messages">The texts of the problems of scripts.</param>
    /// <param name="notebookMessages">The texts of the problems of the imports.</param>
    /// <param name="scripts">Creates the scripts.</param>
    internal NotebookCSharpExecutor(bool isDynamicCodeSupported,
        Func<bool>? allowDatabaseAccess = null,
        NotebookScriptingMessages? messages = null,
        NotebookMessages? notebookMessages = null,
        NotebookScriptFactory? scripts = null)
    {
        _allowDatabaseAccess = allowDatabaseAccess ?? (static () => false);
        _isDynamicCodeSupported = isDynamicCodeSupported;
        _messages = messages ?? NotebookScriptingMessages.Default;
        _notebookMessages = notebookMessages ?? NotebookMessages.Default;
        Context = new NotebookScriptContext(_messages);
        Scripts = scripts ?? new NotebookScriptFactory();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the values the scripts of this executor share. Use one executor for each notebook.
    /// </summary>
    public NotebookScriptContext Context { get; }

    /// <summary>
    /// Gets the factory of the scripts this executor runs.
    /// </summary>
    public NotebookScriptFactory Scripts { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs a C# script. Only one script runs at a time in a process, because the console is redirected.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <param name="cancellationToken">Stops the wait and the compilation. A script that loops does not observe it.</param>
    /// <returns>The outcome of the script.</returns>
    public async Task<NotebookScriptResult> ExecuteAsync(NotebookScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Refusal(request) ?? await CaptureAsync(request,
            _allowDatabaseAccess(),
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Removes every shared value and SQL result, for example when another notebook is opened. Do not call it while
    /// a script runs.
    /// </summary>
    public void Reset() => Context.Clear();

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the outcome of a script that must not run.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <returns>The outcome, or null when the script may run.</returns>
    private NotebookScriptResult? Refusal(NotebookScriptRequest request)
    {
        if (!_isDynamicCodeSupported)
        {
            return new NotebookScriptResult { IsUnavailable = true };
        }
        var imports = NotebookImportValidator.Check(request.Imports,
            _notebookMessages);
        return imports.IsValid ? null : Invalid(_messages.InvalidImports(imports.Errors[0].Text));
    }

    /// <summary>
    /// Builds the outcome of imports that are not allowed.
    /// </summary>
    /// <param name="message">Why they are not allowed.</param>
    /// <returns>The outcome.</returns>
    private static NotebookScriptResult Invalid(string message) => new()
    {
        Error = message,
        Logs = [NotebookScriptDiagnostics.Error(message)]
    };

    /// <summary>
    /// Runs the script while its console output is captured.
    /// </summary>
    /// <param name="request">The script to run.</param>
    /// <param name="allowDatabaseAccess">Whether the script may access the database.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the script.</returns>
    private async Task<NotebookScriptResult> CaptureAsync(NotebookScriptRequest request,
        bool allowDatabaseAccess,
        CancellationToken cancellationToken)
    {
        using var console = await NotebookConsoleCapture.StartAsync(cancellationToken)
            .ConfigureAwait(false);
        Context.Load(request.SqlResults);
        return await RunAsync(new NotebookScriptRun(request,
                new NotebookScriptHost(Context,
                    _messages),
                console),
            allowDatabaseAccess,
            cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Runs the script off the calling thread and turns its errors into a failed outcome, unless it was cancelled.
    /// </summary>
    /// <param name="run">The run.</param>
    /// <param name="allowDatabaseAccess">Whether the script may access the database.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the script.</returns>
    private async Task<NotebookScriptResult> RunAsync(NotebookScriptRun run,
        bool allowDatabaseAccess,
        CancellationToken cancellationToken)
    {
        try
        {
            return await Task.Run(() => EvaluateAsync(run,
                    allowDatabaseAccess,
                    cancellationToken),
                cancellationToken)
                .ConfigureAwait(false);
        }
        catch (CompilationErrorException exception) when (HasErrors(exception))
        {
            return run.Failure(NotebookScriptDiagnostics.ErrorsOf(exception.Diagnostics,
                run.ImportLines,
                _messages));
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return run.Failure([NotebookScriptDiagnostics.Error(RuntimeErrorOf(exception))]);
        }
    }

    /// <summary>
    /// Tells whether a compilation exception says what is wrong, which one a script throws itself may not.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>Whether it holds an error.</returns>
    private static bool HasErrors(CompilationErrorException exception) => exception.Diagnostics
        .Any(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

    /// <summary>
    /// Describes an exception of a script. An exception that cannot be described by the texts is described plainly.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The text.</returns>
    private string RuntimeErrorOf(Exception exception)
    {
        try
        {
            return _messages.RuntimeError(exception);
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            return $"{exception.GetType().Name}: {NotebookScriptPrinter.MessageOf(exception)}";
        }
    }

    /// <summary>
    /// Compiles the script, checks what it uses and runs it.
    /// </summary>
    /// <param name="run">The run.</param>
    /// <param name="allowDatabaseAccess">Whether the script may access the database.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The outcome of the script.</returns>
    private async Task<NotebookScriptResult> EvaluateAsync(NotebookScriptRun run,
        bool allowDatabaseAccess,
        CancellationToken cancellationToken)
    {
        var script = Scripts.Create(run.Request.Imports,
            run.Request.Code);
        var problems = allowDatabaseAccess
            ? []
            : ProblemsOf(script,
                run.ImportLines,
                cancellationToken);
        if (problems.Count > 0)
        {
            return run.Failure(NotAllowed(problems));
        }
        var state = await script.RunAsync(new NotebookScriptGlobals(run.Host),
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        return run.Success(state.ReturnValue);
    }

    /// <summary>
    /// Compiles a script that has no database access and gets what it uses that it may not.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <param name="importLines">The number of lines the imports take before the code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>One text for each use.</returns>
    private IReadOnlyList<string> ProblemsOf(Script<object> script,
        int importLines,
        CancellationToken cancellationToken) => new NotebookScriptGuard(_messages)
        .Check(script.GetCompilation(),
            importLines,
            cancellationToken);

    /// <summary>
    /// Builds the log of a script that uses what it may not.
    /// </summary>
    /// <param name="problems">One text for each use.</param>
    /// <returns>The log entries.</returns>
    private List<NotebookLogEntry> NotAllowed(IReadOnlyList<string> problems) =>
    [
        NotebookScriptDiagnostics.Error(_messages.DatabaseAccessNotAllowed()),
        .. problems
            .Select(NotebookScriptDiagnostics.Warning)
    ];

    #endregion
}
