namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the globals of a script.
/// </summary>
internal static class NotebookScriptGlobalsTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        ConstructorRejectsNullHost();
        ExposesTheHost();
        await PrintWritesAsTheNotebookDoes();
        await PrintShowsATable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a host.
    /// </summary>
    /// <returns>The host.</returns>
    private static NotebookScriptHost Host() => new(new NotebookScriptContext(NotebookScriptingMessages.Default),
        NotebookScriptingMessages.Default);

    /// <summary>
    /// Tests ConstructorRejectsNullHost.
    /// </summary>
    private static void ConstructorRejectsNullHost()
    {
        // Act
        Action operation = () => _ = new NotebookScriptGlobals(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ExposesTheHost.
    /// </summary>
    private static void ExposesTheHost()
    {
        // Setup
        var host = Host();

        // Act
        var globals = new NotebookScriptGlobals(host);

        // Assert
        Check(ReferenceEquals(globals.Notebook,
            host));
    }

    /// <summary>
    /// Tests PrintWritesAsTheNotebookDoes.
    /// </summary>
    private static async Task PrintWritesAsTheNotebookDoes()
    {
        // Setup
        var globals = new NotebookScriptGlobals(Host());
        using var capture = await NotebookConsoleCapture.StartAsync();

        // Act
        globals.Print(42);

        // Assert
        Check(capture.Output.TrimEnd() == "Int32: 42");
    }

    /// <summary>
    /// Tests PrintShowsATable.
    /// </summary>
    private static async Task PrintShowsATable()
    {
        // Setup
        var globals = new NotebookScriptGlobals(Host());
        using var capture = await NotebookConsoleCapture.StartAsync();

        // Act
        globals.Print(SampleTables.Numbers());

        // Assert
        Check(globals.Notebook.Tables.Items.Count == 1);
    }

    #endregion
}
