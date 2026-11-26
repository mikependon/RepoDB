namespace RepoDb.Science.Helpers;

/// <summary>
/// Reads one using directive.
/// </summary>
internal static class NotebookImportParser
{
    #region Public Methods

    /// <summary>
    /// Reads a directive written without its semicolon.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="statement">The trimmed directive.</param>
    /// <param name="entry">The directive, when it is allowed.</param>
    /// <param name="error">Why it is not allowed.</param>
    /// <returns>Whether it is allowed.</returns>
    public static bool TryParse(int line,
        string statement,
        out NotebookImportEntry? entry,
        out string error)
    {
        entry = null;
        error = Problem(statement);
        if (error.Length > 0)
        {
            return false;
        }
        var parsed = Read(line,
            statement["using".Length..].Trim());
        error = TargetProblem(parsed);
        entry = error.Length == 0 ? parsed : null;
        return error.Length == 0;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Writes an alias with single spaces around its equals sign.
    /// </summary>
    /// <param name="rest">The alias.</param>
    /// <returns>The normalized alias.</returns>
    private static string Alias(string rest)
    {
        var parts = rest.Split('=',
            2,
            StringSplitOptions.TrimEntries);
        return $"{parts[0]} = {parts[1]}";
    }

    /// <summary>
    /// Checks an alias.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <returns>The problem, or empty.</returns>
    private static string AliasProblem(string alias)
    {
        var parts = alias.Split('=',
            2,
            StringSplitOptions.TrimEntries);
        return IsIdentifier(parts[0]) && IsName(parts[1])
            ? string.Empty
            : $"'{alias}' is not a valid alias (write it as: using Name = Namespace;).";
    }

    /// <summary>
    /// Checks an identifier.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>Whether it is an identifier.</returns>
    private static bool IsIdentifier(string text) => text.Length > 0
        && (char.IsLetter(text[0]) || text[0] == '_')
        && text
            .All(character => char.IsLetterOrDigit(character) || character == '_');

    /// <summary>
    /// Checks a namespace or type name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Whether it is a name.</returns>
    private static bool IsName(string name)
    {
        var rest = name.StartsWith("global::",
            StringComparison.Ordinal) ? name["global::".Length..] : name;
        return rest
            .Split('.')
            .All(IsIdentifier);
    }

    /// <summary>
    /// Checks that a statement starts with the using keyword.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>Whether it does.</returns>
    private static bool IsUsing(string statement) => statement.StartsWith("using",
            StringComparison.Ordinal)
        && (statement.Length == 5 || !(char.IsLetterOrDigit(statement[5]) || statement[5] == '_'));

    /// <summary>
    /// Checks that a statement is a using directive with content.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>The problem, or empty.</returns>
    private static string Problem(string statement)
    {
        if (!IsUsing(statement))
        {
            return "Only 'using' directives are allowed here, no other code.";
        }
        var rest = statement["using".Length..].Trim();
        if (rest.Length == 0)
        {
            return "Write what to import after 'using' (for example: using System.Linq;).";
        }
        return rest.StartsWith('(') || rest.StartsWith("var ",
            StringComparison.Ordinal)
            ? "Only 'using' directives (using System.Linq;) are allowed, not 'using' statements."
            : string.Empty;
    }

    /// <summary>
    /// Reads what follows the using keyword.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="rest">The text after the keyword.</param>
    /// <returns>The directive.</returns>
    private static NotebookImportEntry Read(int line,
        string rest)
    {
        if (rest.StartsWith("static ",
            StringComparison.Ordinal))
        {
            return new NotebookImportEntry(line,
                NotebookImportKind.Static,
                rest["static ".Length..].Trim());
        }
        return rest.Contains('=')
            ? new NotebookImportEntry(line,
                NotebookImportKind.Alias,
                Alias(rest))
            : new NotebookImportEntry(line,
                NotebookImportKind.Namespace,
                rest);
    }

    /// <summary>
    /// Checks what a directive imports.
    /// </summary>
    /// <param name="entry">The directive.</param>
    /// <returns>The problem, or empty.</returns>
    private static string TargetProblem(NotebookImportEntry entry)
    {
        if (entry.Kind == NotebookImportKind.Alias)
        {
            return AliasProblem(entry.Target);
        }
        return IsName(entry.Target) ? string.Empty : $"'{entry.Target}' is not a valid namespace or type name.";
    }

    #endregion
}
