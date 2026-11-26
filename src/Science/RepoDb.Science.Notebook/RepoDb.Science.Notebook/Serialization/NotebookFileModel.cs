using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepoDb.Science.Serialization;

/// <summary>
/// A notebook as its file holds it.
/// </summary>
internal sealed class NotebookFileModel
{
    #region Properties

    /// <summary>
    /// Gets or sets the file format version. A file without one is version 1.
    /// </summary>
    public int FormatVersion { get; set; } = NotebookDocument.FormatVersion;

    /// <summary>
    /// Gets or sets the properties the package does not own.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? HostData { get; set; }

    /// <summary>
    /// Gets or sets the sections.
    /// </summary>
    public List<NotebookSectionFileModel?>? Sections { get; set; } = [];

    #endregion
}
