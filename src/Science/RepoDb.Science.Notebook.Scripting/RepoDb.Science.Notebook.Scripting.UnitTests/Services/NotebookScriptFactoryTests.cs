namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the factory of the scripts.
/// </summary>
internal static class NotebookScriptFactoryTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunSource();
        RunOptions();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the source.
    /// </summary>
    private static void RunSource()
    {
        PrefixOfBlankImportsIsEmpty();
        PrefixEndsTheImportsWithOneLineBreak();
        PrefixRejectsNull();
        CountsNoLineForBlankImports();
        CountsTheLinesOfTheImports();
        CreatePutsTheImportsBeforeTheCode();
        CreateUsesTheNotebookGlobals();
        CreateRejectsNullCode();
        CreateCompilesTheNotebook();
    }

    /// <summary>
    /// Runs the scenarios of the options.
    /// </summary>
    private static void RunOptions()
    {
        ImportsTheCommonNamespaces();
        OptionsImportTheDefaultNamespaces();
        OptionsAreBuiltOnce();
        OptionsLeaveOutWhatTheHostRefuses();
        OptionsAskForDocumentation();
        TheNotebookCompilesWithoutTheNotebookPackage();
    }

    /// <summary>
    /// Creates a factory that does not reference the notebook package.
    /// </summary>
    /// <returns>The factory.</returns>
    private static NotebookScriptFactory WithoutNotebookPackage() => new(isReferenced: path => !path.EndsWith("RepoDb.Science.Notebook.dll",
        StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Tests PrefixOfBlankImportsIsEmpty.
    /// </summary>
    private static void PrefixOfBlankImportsIsEmpty()
    {
        // Act
        var prefix = NotebookScriptFactory.PrefixOf(" \r\n ");

        // Assert
        Check(prefix.Length == 0);
    }

    /// <summary>
    /// Tests PrefixEndsTheImportsWithOneLineBreak.
    /// </summary>
    private static void PrefixEndsTheImportsWithOneLineBreak()
    {
        // Act
        var prefix = NotebookScriptFactory.PrefixOf("using System.Text;\r\n\r\n");

        // Assert
        Check(prefix == "using System.Text;\n");
    }

    /// <summary>
    /// Tests PrefixRejectsNull.
    /// </summary>
    private static void PrefixRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => NotebookScriptFactory.PrefixOf(null!));
    }

    /// <summary>
    /// Tests CountsNoLineForBlankImports.
    /// </summary>
    private static void CountsNoLineForBlankImports()
    {
        // Act
        var lines = NotebookScriptFactory.ImportLinesOf(string.Empty);

        // Assert
        Check(lines == 0);
    }

    /// <summary>
    /// Tests CountsTheLinesOfTheImports.
    /// </summary>
    private static void CountsTheLinesOfTheImports()
    {
        // Act
        var lines = NotebookScriptFactory.ImportLinesOf("using System.Text;\r\nusing System.Globalization;");

        // Assert
        Check(lines == 2);
    }

    /// <summary>
    /// Tests CreatePutsTheImportsBeforeTheCode.
    /// </summary>
    private static void CreatePutsTheImportsBeforeTheCode()
    {
        // Act
        var script = Compilations.Factory.Create("using System.Text;",
            "return 1;");

        // Assert
        Check(script.Code == "using System.Text;\nreturn 1;");
    }

    /// <summary>
    /// Tests CreateUsesTheNotebookGlobals.
    /// </summary>
    private static void CreateUsesTheNotebookGlobals()
    {
        // Act
        var script = Compilations.Factory.Create(string.Empty,
            "return 1;");

        // Assert
        Check(script.GlobalsType == typeof(NotebookScriptGlobals));
    }

    /// <summary>
    /// Tests CreateRejectsNullCode.
    /// </summary>
    private static void CreateRejectsNullCode()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => Compilations.Factory.Create(string.Empty,
            null!));
    }

    /// <summary>
    /// Tests CreateCompilesTheNotebook.
    /// </summary>
    private static void CreateCompilesTheNotebook()
    {
        // Act
        var diagnostics = Compilations.Factory.Create(string.Empty,
            "Print(Notebook.Context.Names.Count);")
            .Compile();

        // Assert
        Check(diagnostics.Length == 0);
    }

    /// <summary>
    /// Tests ImportsTheCommonNamespaces.
    /// </summary>
    private static void ImportsTheCommonNamespaces()
    {
        // Act
        var imports = string.Join(',',
            NotebookScriptFactory.DefaultImports);

        // Assert
        Check(imports == "System,System.Collections.Generic,System.Data,System.IO,System.Linq,System.Threading.Tasks");
    }

    /// <summary>
    /// Tests OptionsImportTheDefaultNamespaces.
    /// </summary>
    private static void OptionsImportTheDefaultNamespaces()
    {
        // Act
        var imports = Compilations.Factory.Options.Imports;

        // Assert
        Check(imports
            .SequenceEqual(NotebookScriptFactory.DefaultImports));
    }

    /// <summary>
    /// Tests OptionsAreBuiltOnce.
    /// </summary>
    private static void OptionsAreBuiltOnce()
    {
        // Act
        var options = Compilations.Factory.Options;

        // Assert
        Check(ReferenceEquals(options,
            Compilations.Factory.Options));
    }

    /// <summary>
    /// Tests OptionsLeaveOutWhatTheHostRefuses.
    /// </summary>
    private static void OptionsLeaveOutWhatTheHostRefuses()
    {
        // Act
        var references = WithoutNotebookPackage().Options.MetadataReferences;

        // Assert
        Check(!references
            .Any(reference => reference.Display!.EndsWith("RepoDb.Science.Notebook.dll",
                StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Tests OptionsAskForDocumentation.
    /// </summary>
    private static void OptionsAskForDocumentation()
    {
        // Setup
        var asked = 0;
        var factory = new NotebookScriptFactory(_ =>
        {
            asked++;
            return null;
        });

        // Act
        var options = factory.Options;

        // Assert
        Check(asked > 0 && asked == options.MetadataReferences.Length);
    }

    /// <summary>
    /// Tests TheNotebookCompilesWithoutTheNotebookPackage.
    /// </summary>
    private static void TheNotebookCompilesWithoutTheNotebookPackage()
    {
        // Act
        var diagnostics = WithoutNotebookPackage()
            .Create(string.Empty,
                "Print(Notebook.Context.Names.Count);")
            .Compile();

        // Assert
        Check(diagnostics.Length == 0);
    }

    #endregion
}
