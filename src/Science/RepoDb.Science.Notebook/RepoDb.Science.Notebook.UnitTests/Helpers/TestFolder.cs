namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// A temporary folder that is deleted when disposed.
/// </summary>
internal sealed class TestFolder : IDisposable
{
    #region Constructors

    /// <summary>
    /// Creates the folder.
    /// </summary>
    public TestFolder()
    {
        Root = Directory.CreateTempSubdirectory("repodb-notebook-").FullName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the folder path.
    /// </summary>
    public string Root { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Deletes the folder.
    /// </summary>
    public void Dispose() => Directory.Delete(Root,
        true);

    /// <summary>
    /// Gets the path of a file of the folder.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <returns>The path.</returns>
    public string FileOf(string name) => Path.Combine(Root,
        name);

    /// <summary>
    /// Writes a file in the folder.
    /// </summary>
    /// <param name="name">The file name.</param>
    /// <param name="content">The content.</param>
    /// <returns>The path.</returns>
    public string Write(string name,
        string content)
    {
        var path = FileOf(name);
        File.WriteAllText(path,
            content);
        return path;
    }

    #endregion
}
