namespace RepoDb.Science.Helpers;

/// <summary>
/// Checks that imports hold only using directives.
/// </summary>
internal static class NotebookImportValidator
{
    #region Public Methods

    /// <summary>
    /// Checks the text of the imports.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The outcome.</returns>
    public static NotebookImportCheck Check(string text)
    {
        var entries = new List<NotebookImportEntry>();
        var errors = new List<NotebookImportIssue>();
        var inBlock = false;
        var lines = text.Split(["\r\n", "\n", "\r"],
            StringSplitOptions.None);
        for (var index = 0; index < lines.Length; index++)
        {
            var code = NotebookImportComments.Strip(lines[index],
                ref inBlock);
            CheckLine(index + 1,
                code,
                entries,
                errors);
        }
        return new NotebookImportCheck(entries,
            errors,
            WarningsOf(entries));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks the directives of a line.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="code">The code of the line.</param>
    /// <param name="entries">Receives the allowed directives.</param>
    /// <param name="errors">Receives the problems.</param>
    private static void CheckLine(int line,
        string code,
        List<NotebookImportEntry> entries,
        List<NotebookImportIssue> errors)
    {
        var statements = code.Split(';');
        for (var index = 0; index < statements.Length; index++)
        {
            CheckStatement(line,
                statements[index].Trim(),
                index < statements.Length - 1,
                entries,
                errors);
        }
    }

    /// <summary>
    /// Checks one directive.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="statement">The directive without its semicolon.</param>
    /// <param name="hasSemicolon">Whether a semicolon follows it.</param>
    /// <param name="entries">Receives the allowed directives.</param>
    /// <param name="errors">Receives the problems.</param>
    private static void CheckStatement(int line,
        string statement,
        bool hasSemicolon,
        List<NotebookImportEntry> entries,
        List<NotebookImportIssue> errors)
    {
        if (statement.Length == 0)
        {
            return;
        }
        if (!NotebookImportParser.TryParse(line,
            statement,
            out var entry,
            out var error))
        {
            errors.Add(new NotebookImportIssue(line,
                error));
        }
        else if (!hasSemicolon)
        {
            errors.Add(new NotebookImportIssue(line,
                $"Missing ';' at the end of '{statement}'."));
        }
        else
        {
            entries.Add(entry!);
        }
    }

    /// <summary>
    /// Warns about directives written more than once.
    /// </summary>
    /// <param name="entries">The allowed directives.</param>
    /// <returns>The warnings.</returns>
    private static List<NotebookImportIssue> WarningsOf(IReadOnlyList<NotebookImportEntry> entries)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        return entries
            .Where(entry => !seen.Add(entry.Text))
            .Select(entry => new NotebookImportIssue(entry.Line,
                $"'{entry.Text}' is already imported."))
            .ToList();
    }

    #endregion
}
