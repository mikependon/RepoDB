namespace RepoDb.Science;

/// <summary>
/// A title and a description. It cannot run and has no log.
/// </summary>
public sealed class NotebookHeaderSection : NotebookSection
{
    #region Private Variables

    private string _description = string.Empty;
    private string _title = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty header.
    /// </summary>
    public NotebookHeaderSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetContent(ref _description,
            value ?? string.Empty);
    }

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string Title
    {
        get => _title;
        set => SetContent(ref _title,
            value ?? string.Empty);
    }

    /// <summary>
    /// Gets the kind of section.
    /// </summary>
    public override NotebookSectionType Type => NotebookSectionType.Header;

    /// <summary>
    /// Gets the text the preview comes from.
    /// </summary>
    private protected override string PreviewSource => Title + Environment.NewLine + Description;

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the authored values.
    /// </summary>
    /// <returns>The values.</returns>
    private protected override IEnumerable<string?> GetContent() => [Title, Description];

    #endregion
}
