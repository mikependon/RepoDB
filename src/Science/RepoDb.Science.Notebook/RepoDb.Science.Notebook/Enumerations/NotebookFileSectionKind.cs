namespace RepoDb.Science;

/// <summary>
/// The section kind as a notebook file names it.
/// </summary>
internal enum NotebookFileSectionKind
{
    /// <summary>
    /// A SQL code section.
    /// </summary>
    Sql,

    /// <summary>
    /// A C# code section.
    /// </summary>
    CSharp,

    /// <summary>
    /// A header section.
    /// </summary>
    Header,

    /// <summary>
    /// An imports section.
    /// </summary>
    Imports,

    /// <summary>
    /// A chart section.
    /// </summary>
    Chart
}
