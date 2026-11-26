namespace RepoDb.Science;

/// <summary>
/// The form of a using directive.
/// </summary>
public enum NotebookImportKind
{
    /// <summary>
    /// A namespace import.
    /// </summary>
    Namespace,

    /// <summary>
    /// A static import.
    /// </summary>
    Static,

    /// <summary>
    /// An alias import.
    /// </summary>
    Alias
}
