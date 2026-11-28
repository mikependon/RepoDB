namespace RepoDb.Science.Serialization;

/// <summary>
/// A log entry as a notebook file holds it.
/// </summary>
internal sealed class NotebookLogFileModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public NotebookLogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    public string? Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when it was written.
    /// </summary>
    public DateTimeOffset Time { get; set; }

    #endregion
}
