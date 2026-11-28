namespace RepoDb.Science.Serialization;

/// <summary>
/// A column as a notebook file holds it.
/// </summary>
internal sealed class NotebookColumnFileModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the column name.
    /// </summary>
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value type name.
    /// </summary>
    public string? TypeName { get; set; } = "string";

    #endregion
}
