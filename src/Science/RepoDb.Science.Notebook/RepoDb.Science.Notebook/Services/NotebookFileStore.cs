using System.Text.Json;
using RepoDb.Science.Serialization;

namespace RepoDb.Science.Services;

/// <summary>
/// Reads and writes notebook files. A write replaces the file only after the new content is complete.
/// </summary>
internal static class NotebookFileStore
{
    #region Public Methods

    /// <summary>
    /// Reads a notebook file.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <returns>The content.</returns>
    public static NotebookFileModel Read(string path)
    {
        using var stream = File.OpenRead(path);
        try
        {
            return Required(JsonSerializer.Deserialize(stream,
                    NotebookFileJsonContext.Default.NotebookFileModel),
                path);
        }
        catch (JsonException exception)
        {
            throw Invalid(path,
                exception);
        }
    }

    /// <summary>
    /// Reads a notebook file.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The content.</returns>
    public static async Task<NotebookFileModel> ReadAsync(string path,
        CancellationToken cancellationToken)
    {
        var stream = File.OpenRead(path);
        await using var scope = stream.ConfigureAwait(false);
        try
        {
            var model = await JsonSerializer.DeserializeAsync(stream,
                NotebookFileJsonContext.Default.NotebookFileModel,
                cancellationToken)
                .ConfigureAwait(false);
            return Required(model,
                path);
        }
        catch (JsonException exception)
        {
            throw Invalid(path,
                exception);
        }
    }

    /// <summary>
    /// Writes a notebook file, creating its folder when needed.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <param name="model">The content.</param>
    public static void Write(string path,
        NotebookFileModel model)
    {
        var temporary = TemporaryFor(path);
        try
        {
            using (var stream = File.Create(temporary))
            {
                JsonSerializer.Serialize(stream,
                    model,
                    NotebookFileJsonContext.Default.NotebookFileModel);
            }
            File.Move(temporary,
                path,
                true);
        }
        finally
        {
            File.Delete(temporary);
        }
    }

    /// <summary>
    /// Writes a notebook file, creating its folder when needed.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <param name="model">The content.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task of the write.</returns>
    public static async Task WriteAsync(string path,
        NotebookFileModel model,
        CancellationToken cancellationToken)
    {
        var temporary = TemporaryFor(path);
        try
        {
            var stream = File.Create(temporary);
            await using (stream.ConfigureAwait(false))
            {
                await JsonSerializer.SerializeAsync(stream,
                    model,
                    NotebookFileJsonContext.Default.NotebookFileModel,
                    cancellationToken)
                    .ConfigureAwait(false);
            }
            File.Move(temporary,
                path,
                true);
        }
        finally
        {
            File.Delete(temporary);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Describes a file that is not a notebook.
    /// </summary>
    /// <param name="path">The file.</param>
    /// <param name="inner">The cause.</param>
    /// <returns>The error.</returns>
    private static InvalidDataException Invalid(string path,
        Exception? inner) => new($"'{Path.GetFileName(path)}' is not a notebook.{(inner is null ? string.Empty : $" {inner.Message}")}",
            inner);

    /// <summary>
    /// Requires a file to have content.
    /// </summary>
    /// <param name="model">The content.</param>
    /// <param name="path">The file.</param>
    /// <returns>The content.</returns>
    private static NotebookFileModel Required(NotebookFileModel? model,
        string path) => model ?? throw Invalid(path,
            null);

    /// <summary>
    /// Creates the folder of a file and names a temporary file next to it.
    /// </summary>
    /// <param name="path">The full path of the file.</param>
    /// <returns>The temporary file.</returns>
    private static string TemporaryFor(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        return $"{path}.{Guid.NewGuid():N}.tmp";
    }

    #endregion
}
