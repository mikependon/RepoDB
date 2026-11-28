namespace RepoDb.Science;

/// <summary>
/// An accepted using directive.
/// </summary>
public sealed class NotebookImportEntry
{
    #region Constructors

    /// <summary>
    /// Creates an import entry.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <param name="kind">The directive form.</param>
    /// <param name="target">What is imported.</param>
    public NotebookImportEntry(int line,
        NotebookImportKind kind,
        string target)
    {
        ArgumentNullException.ThrowIfNull(target);
        Line = line;
        Kind = kind;
        Target = target;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the directive form.
    /// </summary>
    public NotebookImportKind Kind { get; }

    /// <summary>
    /// Gets the line number.
    /// </summary>
    public int Line { get; }

    /// <summary>
    /// Gets what is imported.
    /// </summary>
    public string Target { get; }

    /// <summary>
    /// Gets the directive as C# writes it.
    /// </summary>
    public string Text => Kind == NotebookImportKind.Static ? $"using static {Target};" : $"using {Target};";

    #endregion
}
