namespace RepoDb.Science;

/// <summary>
/// A problem found in a line of the imports.
/// </summary>
public sealed class NotebookImportIssue
{
    #region Constructors

    /// <summary>
    /// Creates an import issue.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="message">What is wrong.</param>
    public NotebookImportIssue(int line,
        string message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Line = line;
        Message = message;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the line number.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets what is wrong.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the issue as a line of text.
    /// </summary>
    public string Text => $"Line {Line}: {Message}";

    #endregion
}
