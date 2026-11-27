namespace RepoDb.Science;

/// <summary>
/// A problem found in a line of the imports.
/// </summary>
public sealed class NotebookImportIssue
{
    #region Private Variables

    private readonly NotebookMessages _messages;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an import issue.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="message">What is wrong.</param>
    /// <param name="messages">The wording of the line, or null for the default one.</param>
    public NotebookImportIssue(int line,
        string message,
        NotebookMessages? messages = null)
    {
        ArgumentNullException.ThrowIfNull(message);
        Line = line;
        Message = message;
        _messages = messages ?? NotebookMessages.Default;
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
    public string Text => _messages.ImportIssue(Line,
        Message);

    #endregion
}
