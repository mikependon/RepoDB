namespace RepoDb.Science;

/// <summary>
/// A section as it is planned in the run of a notebook.
/// </summary>
public sealed class NotebookRunStep
{
    #region Constructors

    /// <summary>
    /// Creates a run step.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="name">The name in the run.</param>
    public NotebookRunStep(NotebookExecutableSection section,
        string name)
    {
        ArgumentNullException.ThrowIfNull(section);
        ArgumentNullException.ThrowIfNull(name);
        Section = section;
        Name = name;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name in the run.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the section.
    /// </summary>
    public NotebookExecutableSection Section { get; }

    #endregion
}
