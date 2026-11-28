using System.Globalization;
using Microsoft.CodeAnalysis;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Builds the log entries of the problems of a script.
/// </summary>
internal static class NotebookScriptDiagnostics
{
    #region Public Methods

    /// <summary>
    /// Creates an error entry.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The entry.</returns>
    public static NotebookLogEntry Error(string message) => new(NotebookLogLevel.Error,
        message,
        DateTimeOffset.Now);

    /// <summary>
    /// Creates an entry for each compile error, which tells its line.
    /// </summary>
    /// <param name="diagnostics">The compiler diagnostics.</param>
    /// <param name="importLines">The number of lines the imports take before the code.</param>
    /// <param name="messages">The texts of the problems.</param>
    /// <returns>The entries.</returns>
    public static List<NotebookLogEntry> ErrorsOf(IEnumerable<Diagnostic> diagnostics,
        int importLines,
        NotebookScriptingMessages messages) => diagnostics
        .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
        .Select(diagnostic => Error(LineOf(diagnostic,
            importLines,
            messages)))
        .ToList();

    /// <summary>
    /// Creates a warning entry.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>The entry.</returns>
    public static NotebookLogEntry Warning(string message) => new(NotebookLogLevel.Warning,
        message,
        DateTimeOffset.Now);

    #endregion

    #region Private Methods

    /// <summary>
    /// Describes a diagnostic by its line in the code or in the imports.
    /// </summary>
    /// <param name="diagnostic">The diagnostic.</param>
    /// <param name="importLines">The number of lines of the imports.</param>
    /// <param name="messages">The texts of the problems.</param>
    /// <returns>The text.</returns>
    private static string LineOf(Diagnostic diagnostic,
        int importLines,
        NotebookScriptingMessages messages)
    {
        var line = diagnostic.Location
            .GetLineSpan()
            .StartLinePosition.Line + 1;
        var message = diagnostic.GetMessage(CultureInfo.InvariantCulture);
        return line > importLines
            ? messages.CodeError(line - importLines,
                diagnostic.Id,
                message)
            : messages.ImportsError(line,
                diagnostic.Id,
                message);
    }

    #endregion
}
