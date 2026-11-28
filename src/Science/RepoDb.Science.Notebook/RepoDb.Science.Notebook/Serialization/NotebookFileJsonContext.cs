using System.Text.Json.Serialization;

namespace RepoDb.Science.Serialization;

/// <summary>
/// The source-generated JSON metadata of notebook files.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true,
    UseStringEnumConverter = true)]
[JsonSerializable(typeof(NotebookFileModel))]
internal sealed partial class NotebookFileJsonContext : JsonSerializerContext;
