namespace RepoDb.Science;

/// <summary>
/// The kind of a notebook section.
/// </summary>
public enum NotebookSectionType
{
    /// <summary>
    /// A title and a description.
    /// </summary>
    Header,

    /// <summary>
    /// The using directives of the C# scripts.
    /// </summary>
    Imports,

    /// <summary>
    /// A SQL or a C# script.
    /// </summary>
    Code,

    /// <summary>
    /// A chart drawn from a script result.
    /// </summary>
    Chart
}
