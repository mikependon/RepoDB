namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests opening and saving a notebook.
/// </summary>
internal static class NotebookDocumentFileTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        RunSaveTests();
        RunChangeTests();
        RunOpenTests();
        await RunAsyncTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs saving scenarios.
    /// </summary>
    private static void RunSaveTests()
    {
        SaveWritesTheFile();
        SaveSetsTheFileAndName();
        SaveRemovesTheChanges();
        SaveAnnouncesTheFile();
        SaveUsesTheCurrentFile();
        SaveWithoutAFileFails();
        SaveRejectsABlankName();
    }

    /// <summary>
    /// Runs change tracking scenarios.
    /// </summary>
    private static void RunChangeTests()
    {
        EditAfterSaveIsAChange();
        ReorderingIsAChange();
        RestoringTheOrderIsNoChange();
        RemovingIsAChange();
    }

    /// <summary>
    /// Runs opening scenarios.
    /// </summary>
    private static void RunOpenTests()
    {
        OpenReadsTheSections();
        OpenHasNoChanges();
        OpenSetsTheFile();
        OpenRejectsABlankName();
        OpenFailsOnAMissingFile();
        OpenFailsOnAnotherFile();
    }

    /// <summary>
    /// Runs asynchronous scenarios.
    /// </summary>
    private static async Task RunAsyncTests()
    {
        await SaveAsyncWritesTheFile();
        await SaveAsyncUsesTheCurrentFile();
        await SaveAsyncWithoutAFileFails();
        await EditDuringSaveStaysAChange();
        await OpenAsyncReadsTheSections();
        await OpenAsyncFailsOnAnotherFile();
        await OpenAsyncHonorsCancellation();
        await SaveAsyncHonorsCancellation();
    }

    /// <summary>
    /// Saves a new notebook in a folder.
    /// </summary>
    /// <param name="folder">The folder.</param>
    /// <returns>The notebook.</returns>
    private static NotebookDocument Saved(TestFolder folder)
    {
        var document = NotebookDocument.Create("Sales");
        document.Save(folder.FileOf("Sales.rdbnb"));
        return document;
    }

    /// <summary>
    /// Tests SaveWritesTheFile.
    /// </summary>
    private static void SaveWritesTheFile()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        Saved(folder);

        // Assert
        Check(File.Exists(folder.FileOf("Sales.rdbnb")));
    }

    /// <summary>
    /// Tests SaveSetsTheFileAndName.
    /// </summary>
    private static void SaveSetsTheFileAndName()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        var document = Saved(folder);

        // Assert
        Check(document.FileName == folder.FileOf("Sales.rdbnb") && document.Name == "Sales");
    }

    /// <summary>
    /// Tests SaveRemovesTheChanges.
    /// </summary>
    private static void SaveRemovesTheChanges()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        var document = Saved(folder);

        // Assert
        Check(!document.HasChanges && document.Sections
            .All(section => !section.HasChanges));
    }

    /// <summary>
    /// Tests SaveAnnouncesTheFile.
    /// </summary>
    private static void SaveAnnouncesTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var document = new NotebookDocument();
        var recorder = new PropertyRecorder(document);

        // Act
        document.Save(folder.FileOf("a.rdbnb"));

        // Assert
        Check(string.Join(',',
            recorder.Names) == "FileName,Name,HasChanges");
    }

    /// <summary>
    /// Tests SaveUsesTheCurrentFile.
    /// </summary>
    private static void SaveUsesTheCurrentFile()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);
        ((NotebookHeaderSection)document.Sections[0]).Title = "Changed";

        // Act
        document.Save();

        // Assert
        Check(File.ReadAllText(document.FileName!)
            .Contains("Changed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests SaveWithoutAFileFails.
    /// </summary>
    private static void SaveWithoutAFileFails()
    {
        // Setup
        var document = new NotebookDocument();

        // Act
        Action operation = document.Save;

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Tests SaveRejectsABlankName.
    /// </summary>
    private static void SaveRejectsABlankName()
    {
        // Setup
        var document = new NotebookDocument();

        // Act
        Action operation = () => document.Save(" ");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests EditAfterSaveIsAChange.
    /// </summary>
    private static void EditAfterSaveIsAChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);

        // Act
        ((NotebookHeaderSection)document.Sections[0]).Title = "Changed";

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests ReorderingIsAChange.
    /// </summary>
    private static void ReorderingIsAChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);

        // Act
        document.Sections.Move(0,
            1);

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests RestoringTheOrderIsNoChange.
    /// </summary>
    private static void RestoringTheOrderIsNoChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);
        document.Sections.Move(0,
            1);

        // Act
        document.Sections.Move(1,
            0);

        // Assert
        Check(!document.HasChanges);
    }

    /// <summary>
    /// Tests RemovingIsAChange.
    /// </summary>
    private static void RemovingIsAChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);

        // Act
        document.Sections.RemoveAt(0);

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests OpenReadsTheSections.
    /// </summary>
    private static void OpenReadsTheSections()
    {
        // Setup
        using var folder = new TestFolder();
        var saved = Saved(folder);

        // Act
        var document = NotebookDocument.Open(saved.FileName!);

        // Assert
        Check(document.Sections.Count == 4 && document.Sections[0].Preview == "Sales");
    }

    /// <summary>
    /// Tests OpenHasNoChanges.
    /// </summary>
    private static void OpenHasNoChanges()
    {
        // Setup
        using var folder = new TestFolder();
        var saved = Saved(folder);

        // Act
        var document = NotebookDocument.Open(saved.FileName!);

        // Assert
        Check(!document.HasChanges);
    }

    /// <summary>
    /// Tests OpenSetsTheFile.
    /// </summary>
    private static void OpenSetsTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var saved = Saved(folder);

        // Act
        var document = NotebookDocument.Open(saved.FileName!);

        // Assert
        Check(document.FileName == saved.FileName && document.Name == "Sales");
    }

    /// <summary>
    /// Tests OpenRejectsABlankName.
    /// </summary>
    private static void OpenRejectsABlankName()
    {
        // Act
        Action operation = () => NotebookDocument.Open("");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests OpenFailsOnAMissingFile.
    /// </summary>
    private static void OpenFailsOnAMissingFile()
    {
        // Setup
        using var folder = new TestFolder();

        // Act
        Action operation = () => NotebookDocument.Open(folder.FileOf("none.rdbnb"));

        // Assert
        Throws<FileNotFoundException>(operation);
    }

    /// <summary>
    /// Tests OpenFailsOnAnotherFile.
    /// </summary>
    private static void OpenFailsOnAnotherFile()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "text");

        // Act
        Action operation = () => NotebookDocument.Open(path);

        // Assert
        Throws<InvalidDataException>(operation);
    }

    /// <summary>
    /// Tests SaveAsyncWritesTheFile.
    /// </summary>
    private static async Task SaveAsyncWritesTheFile()
    {
        // Setup
        using var folder = new TestFolder();
        var document = NotebookDocument.Create();

        // Act
        await document.SaveAsync(folder.FileOf("a.rdbnb"));

        // Assert
        Check(File.Exists(document.FileName) && !document.HasChanges);
    }

    /// <summary>
    /// Tests SaveAsyncUsesTheCurrentFile.
    /// </summary>
    private static async Task SaveAsyncUsesTheCurrentFile()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);
        ((NotebookHeaderSection)document.Sections[0]).Title = "Changed";

        // Act
        await document.SaveAsync();

        // Assert
        Check(File.ReadAllText(document.FileName!)
            .Contains("Changed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests SaveAsyncWithoutAFileFails.
    /// </summary>
    private static async Task SaveAsyncWithoutAFileFails()
    {
        // Setup
        var document = new NotebookDocument();

        // Act/Assert
        await ThrowsAsync<InvalidOperationException>(() => document.SaveAsync());
    }

    /// <summary>
    /// Tests EditDuringSaveStaysAChange.
    /// </summary>
    private static async Task EditDuringSaveStaysAChange()
    {
        // Setup
        using var folder = new TestFolder();
        var document = Saved(folder);

        // Act
        var save = document.SaveAsync();
        ((NotebookHeaderSection)document.Sections[0]).Title = "Late";
        await save;

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests OpenAsyncReadsTheSections.
    /// </summary>
    private static async Task OpenAsyncReadsTheSections()
    {
        // Setup
        using var folder = new TestFolder();
        var saved = Saved(folder);

        // Act
        var document = await NotebookDocument.OpenAsync(saved.FileName!);

        // Assert
        Check(document.Sections.Count == 4 && !document.HasChanges && document.Name == "Sales");
    }

    /// <summary>
    /// Tests OpenAsyncFailsOnAnotherFile.
    /// </summary>
    private static async Task OpenAsyncFailsOnAnotherFile()
    {
        // Setup
        using var folder = new TestFolder();
        var path = folder.Write("a.rdbnb",
            "[]");

        // Act/Assert
        await ThrowsAsync<InvalidDataException>(() => NotebookDocument.OpenAsync(path));
    }

    /// <summary>
    /// Tests OpenAsyncHonorsCancellation.
    /// </summary>
    private static async Task OpenAsyncHonorsCancellation()
    {
        // Setup
        using var folder = new TestFolder();
        var saved = Saved(folder);

        // Act/Assert
        await ThrowsAsync<OperationCanceledException>(() => NotebookDocument.OpenAsync(saved.FileName!,
            new CancellationToken(true)));
    }

    /// <summary>
    /// Tests SaveAsyncHonorsCancellation.
    /// </summary>
    private static async Task SaveAsyncHonorsCancellation()
    {
        // Setup
        using var folder = new TestFolder();
        var document = NotebookDocument.Create();

        // Act
        await ThrowsAsync<OperationCanceledException>(() => document.SaveAsync(folder.FileOf("a.rdbnb"),
            new CancellationToken(true)));

        // Assert
        Check(document.HasChanges && document.FileName is null);
    }

    #endregion
}
