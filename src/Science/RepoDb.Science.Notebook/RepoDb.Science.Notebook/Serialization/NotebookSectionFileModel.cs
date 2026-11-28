namespace RepoDb.Science.Serialization;

/// <summary>
/// A section as a notebook file holds it: one shape for every kind.
/// </summary>
internal sealed class NotebookSectionFileModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the chart bucket name.
    /// </summary>
    public string? ChartBucket { get; set; } = "1min";

    /// <summary>
    /// Gets or sets the C# script of a chart.
    /// </summary>
    public string? ChartCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the chart colors.
    /// </summary>
    public Dictionary<string, string>? ChartColors { get; set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets or sets the chart source.
    /// </summary>
    public NotebookCodeLanguage ChartSource { get; set; }

    /// <summary>
    /// Gets or sets the chart type name.
    /// </summary>
    public string? ChartType { get; set; } = "Lines (Stacked)";

    /// <summary>
    /// Gets or sets the connection identifier.
    /// </summary>
    public Guid? ConnectionId { get; set; }

    /// <summary>
    /// Gets or sets the header description.
    /// </summary>
    public string? Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the editor height.
    /// </summary>
    public double EditorHeight { get; set; }

    /// <summary>
    /// Gets or sets the duration of the last run.
    /// </summary>
    public double ElapsedSeconds { get; set; }

    /// <summary>
    /// Gets or sets whether the section is closed.
    /// </summary>
    public bool IsCollapsed { get; set; }

    /// <summary>
    /// Gets or sets the section kind.
    /// </summary>
    public NotebookFileSectionKind Language { get; set; }

    /// <summary>
    /// Gets or sets the log of the last run.
    /// </summary>
    public List<NotebookLogFileModel?>? Logs { get; set; } = [];

    /// <summary>
    /// Gets or sets the section name.
    /// </summary>
    public string? Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the script output.
    /// </summary>
    public string? Output { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tables of the last run.
    /// </summary>
    public List<NotebookTableFileModel?>? Results { get; set; } = [];

    /// <summary>
    /// Gets or sets the result summary.
    /// </summary>
    public string? ResultStatus { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the script.
    /// </summary>
    public string? Script { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the table height.
    /// </summary>
    public double TableHeight { get; set; }

    /// <summary>
    /// Gets or sets the header title.
    /// </summary>
    public string? Title { get; set; } = string.Empty;

    #endregion
}
