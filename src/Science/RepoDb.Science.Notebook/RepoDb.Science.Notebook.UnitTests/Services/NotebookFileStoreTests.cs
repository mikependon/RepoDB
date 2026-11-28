using System.Text.Json;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests reading and writing notebook files.
/// </summary>
internal static class NotebookFileStoreTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        RunSynchronousTests();
        await RunAsynchronousTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs synchronous scenarios.
    /// </summary>
    private static void RunSynchronousTests()
    {
        WriteCreatesTheFolder();
        WriteLeavesNoTemporaryFile();
        WriteReplacesTheFile();
        FailedWriteKeepsTheFile();
        ReadReturnsTheContent();
        ReadRejectsText();
        ReadRejectsNull();
    }

    /// <summary>
    /// Runs asynchronous scenarios.
    /// </summary>
    private static async Task RunAsynchronousTests()
    {
        await WriteAsyncCreatesTheFolder();
        await WriteAsyncLeavesNoTemporaryFile();
        await FailedWriteAsyncKeepsTheFile();
        await ReadAsyncReturnsTheContent();
        await ReadAsyncRejectsText();
        await ReadAsyncRejectsNull();
    }

    /// <summary>
    /// Creates content that cannot be written.
    /// </summary>
    /// <returns>The content.</returns>
    private static NotebookFileModel Unwritable() => new()
    {
        HostData = new Dictionary<string, JsonElement>(StringComparer.Ordinal) { ["Broken"] = default }
    };

    /// <summary>
    /// Tests WriteCreatesTheFolder.
    /// </summary>
    private static void WriteCreatesTheFolder()
    {
        // Setup
        using var folder = new TestFolder();
        var path = Path.Combine(folder.Root,
            "nested",
            "a.rdbnb");

        // Act
        NotebookFileStore.Write(path,
            new NotebookFileModel());

        // Assert
        Check(File.Exists(path));
    }

    /// <summary>
    /// Tests WriteLeavesNoTemporaryFile.
    /// </summary>
    private static void WriteLeavesNoTemporaryFile()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        NotebookFileStore.Write(folder.FileOf("a.rdbnb"),
            new NotebookFileModel());

        // Assert
        Check(Directory.GetFiles(folder.Root).Length == 1);
    }

    /// <summary>
    /// Tests WriteReplacesTheFile.
    /// </summary>
    private static void WriteReplacesTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "old");

        // Act
        NotebookFileStore.Write(path,
            new NotebookFileModel());

        // Assert
        Check(File.ReadAllText(path)
            .Contains("\"Sections\"",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests FailedWriteKeepsTheFile.
    /// </summary>
    private static void FailedWriteKeepsTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "old");

        // Act
        Throws<InvalidOperationException>(() => NotebookFileStore.Write(path,
            Unwritable()));

        // Assert
        Check(File.ReadAllText(path) == "old" && Directory.GetFiles(folder.Root).Length == 1);
    }

    /// <summary>
    /// Tests ReadReturnsTheContent.
    /// </summary>
    private static void ReadReturnsTheContent()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "{\"Sections\":[{}]}");

        // Act
        var model = NotebookFileStore.Read(path);

        // Assert
        Check(model.Sections!.Count == 1);
    }

    /// <summary>
    /// Tests ReadRejectsText.
    /// </summary>
    private static void ReadRejectsText()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "text");

        // Act
        var error = Throws<InvalidDataException>(() => NotebookFileStore.Read(path));

        // Assert
        Check(error.Message.StartsWith("'a.rdbnb' is not a notebook.",
            StringComparison.Ordinal) && error.InnerException is JsonException);
    }

    /// <summary>
    /// Tests ReadRejectsNull.
    /// </summary>
    private static void ReadRejectsNull()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "null");

        // Act
        var error = Throws<InvalidDataException>(() => NotebookFileStore.Read(path));

        // Assert
        Check(error.Message == "'a.rdbnb' is not a notebook.");
    }

    /// <summary>
    /// Tests WriteAsyncCreatesTheFolder.
    /// </summary>
    private static async Task WriteAsyncCreatesTheFolder()
    {
        // Setup
        using var folder = new TestFolder();
        var path = Path.Combine(folder.Root,
            "nested",
            "a.rdbnb");

        // Act
        await NotebookFileStore.WriteAsync(path,
            new NotebookFileModel(),
            CancellationToken.None);

        // Assert
        Check(File.Exists(path));
    }

    /// <summary>
    /// Tests WriteAsyncLeavesNoTemporaryFile.
    /// </summary>
    private static async Task WriteAsyncLeavesNoTemporaryFile()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        await NotebookFileStore.WriteAsync(folder.FileOf("a.rdbnb"),
            new NotebookFileModel(),
            CancellationToken.None);

        // Assert
        Check(Directory.GetFiles(folder.Root).Length == 1);
    }

    /// <summary>
    /// Tests FailedWriteAsyncKeepsTheFile.
    /// </summary>
    private static async Task FailedWriteAsyncKeepsTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "old");

        // Act
        await ThrowsAsync<InvalidOperationException>(() => NotebookFileStore.WriteAsync(path,
            Unwritable(),
            CancellationToken.None));

        // Assert
        Check(File.ReadAllText(path) == "old" && Directory.GetFiles(folder.Root).Length == 1);
    }

    /// <summary>
    /// Tests ReadAsyncReturnsTheContent.
    /// </summary>
    private static async Task ReadAsyncReturnsTheContent()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "{\"Sections\":[{}]}");

        // Act
        var model = await NotebookFileStore.ReadAsync(path,
            CancellationToken.None);

        // Assert
        Check(model.Sections!.Count == 1);
    }

    /// <summary>
    /// Tests ReadAsyncRejectsText.
    /// </summary>
    private static async Task ReadAsyncRejectsText()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "text");

        // Act/Assert
        await ThrowsAsync<InvalidDataException>(() => NotebookFileStore.ReadAsync(path,
            CancellationToken.None));
    }

    /// <summary>
    /// Tests ReadAsyncRejectsNull.
    /// </summary>
    private static async Task ReadAsyncRejectsNull()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "null");

        // Act/Assert
        await ThrowsAsync<InvalidDataException>(() => NotebookFileStore.ReadAsync(path,
            CancellationToken.None));
    }

    #endregion
}
