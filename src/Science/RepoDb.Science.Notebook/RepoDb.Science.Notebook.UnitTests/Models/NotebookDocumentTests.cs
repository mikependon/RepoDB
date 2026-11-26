namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the state of a notebook document.
/// </summary>
internal static class NotebookDocumentTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        NewDocumentIsEmpty();
        NewDocumentHasNoFile();
        CreateMakesFourSections();
        CreateWritesTheHeader();
        CreateUsesTheDefaultImports();
        CreateAcceptsImports();
        CreateNamesTheSqlSection();
        CreatedDocumentHasChanges();
        DocumentWithoutListenersCanChange();
        await ExecuteRunsTheSections();
        await ClearOutputClearsEverySection();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests NewDocumentIsEmpty.
    /// </summary>
    private static void NewDocumentIsEmpty()
    {
        // Act
        var document = new NotebookDocument();

        // Assert
        Check(document.Sections.Count == 0 && document.HostData.Count == 0 && !document.HasChanges);
    }

    /// <summary>
    /// Tests NewDocumentHasNoFile.
    /// </summary>
    private static void NewDocumentHasNoFile()
    {
        // Act
        var document = new NotebookDocument();

        // Assert
        Check(document.FileName is null && document.Name.Length == 0 && document.LogLimit == NotebookLog.DefaultLimit);
    }

    /// <summary>
    /// Tests CreateMakesFourSections.
    /// </summary>
    private static void CreateMakesFourSections()
    {
        // Act
        var document = NotebookDocument.Create();

        // Assert
        Check(string.Join(',',
            document.Sections
                .Select(section => section.Type)) == "Header,Imports,Code,Code");
        Check(((NotebookCodeSection)document.Sections[3]).Language == NotebookCodeLanguage.CSharp);
    }

    /// <summary>
    /// Tests CreateWritesTheHeader.
    /// </summary>
    private static void CreateWritesTheHeader()
    {
        // Act
        var header = (NotebookHeaderSection)NotebookDocument.Create("Sales").Sections[0];

        // Assert
        Check(header.Title == "Sales" && header.Description == NotebookDocument.DefaultDescription);
    }

    /// <summary>
    /// Tests CreateUsesTheDefaultImports.
    /// </summary>
    private static void CreateUsesTheDefaultImports()
    {
        // Act
        var document = NotebookDocument.Create();

        // Assert
        Check(document.Sections.ImportsScript == NotebookDocument.DefaultImports);
    }

    /// <summary>
    /// Tests CreateAcceptsImports.
    /// </summary>
    private static void CreateAcceptsImports()
    {
        // Act
        var document = NotebookDocument.Create(imports: "using A;");

        // Assert
        Check(document.Sections.ImportsScript == "using A;");
    }

    /// <summary>
    /// Tests CreateNamesTheSqlSection.
    /// </summary>
    private static void CreateNamesTheSqlSection()
    {
        // Act
        var section = (NotebookCodeSection)NotebookDocument.Create().Sections[2];

        // Assert
        Check(section.Name == "SQLSection1");
    }

    /// <summary>
    /// Tests CreatedDocumentHasChanges.
    /// </summary>
    private static void CreatedDocumentHasChanges()
    {
        // Act
        var document = NotebookDocument.Create();

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests DocumentWithoutListenersCanChange.
    /// </summary>
    private static void DocumentWithoutListenersCanChange()
    {
        // Setup
        var document = new NotebookDocument();

        // Act
        document.Sections.Add(new NotebookHeaderSection());

        // Assert
        Check(document.HasChanges);
    }

    /// <summary>
    /// Tests ExecuteRunsTheSections.
    /// </summary>
    private static async Task ExecuteRunsTheSections()
    {
        // Setup
        var document = NotebookDocument.Create();
        ((NotebookCodeSection)document.Sections[2]).Script = "SELECT 1";

        // Act
        var result = await document.ExecuteAsync(new NotebookExecutionContext { SqlExecutor = new FakeSqlExecutor() });

        // Assert
        Check(result.Succeeded && result.Items.Count == 3);
    }

    /// <summary>
    /// Tests ClearOutputClearsEverySection.
    /// </summary>
    private static async Task ClearOutputClearsEverySection()
    {
        // Setup
        var document = NotebookDocument.Create();
        await document.ExecuteAsync(new NotebookExecutionContext());

        // Act
        document.ClearOutput();

        // Assert
        Check(document.Sections
            .OfType<NotebookExecutableSection>()
            .All(section => section.State == NotebookRunState.Pending));
    }

    #endregion
}
