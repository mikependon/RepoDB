namespace RepoDb.Science.Serialization;

/// <summary>
/// A table as a notebook file holds it.
/// </summary>
internal sealed class NotebookTableFileModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public List<NotebookColumnFileModel?>? Columns { get; set; } = [];

    /// <summary>
    /// Gets or sets whether rows were left out.
    /// </summary>
    public bool IsTruncated { get; set; }

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    public List<List<string?>?>? Rows { get; set; } = [];

    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    public string? Title { get; set; } = string.Empty;

    #endregion
}
