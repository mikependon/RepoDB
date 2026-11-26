namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the imports section.
/// </summary>
internal static class NotebookImportsSectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        SectionStartsEmpty();
        SectionIsImports();
        NullScriptIsEmpty();
        ScriptIsContent();
        ScriptGivesThePreview();
        EditingWithoutACheckKeepsItEmpty();
        RestoreChecksAScript();
        RestoreLeavesABlankScriptUnchecked();
        EditingUpdatesTheCheck();
        await ValidImportsSucceed();
        await InvalidImportsFail();
        await ProblemsAreLogged();
        await WarningsAreLogged();
        await ClearOutputRemovesTheCheck();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates and runs a section.
    /// </summary>
    /// <param name="script">The script.</param>
    /// <returns>The section.</returns>
    private static async Task<NotebookImportsSection> Ran(string script)
    {
        var section = new NotebookImportsSection { Script = script };
        await section.ExecuteAsync(new NotebookExecutionContext());
        return section;
    }

    /// <summary>
    /// Tests SectionStartsEmpty.
    /// </summary>
    private static void SectionStartsEmpty()
    {
        // Act
        var section = new NotebookImportsSection();

        // Assert
        Check(section.Script.Length == 0 && section.Check is null);
    }

    /// <summary>
    /// Tests SectionIsImports.
    /// </summary>
    private static void SectionIsImports()
    {
        // Act
        var section = new NotebookImportsSection();

        // Assert
        Check(section.Type == NotebookSectionType.Imports);
    }

    /// <summary>
    /// Tests NullScriptIsEmpty.
    /// </summary>
    private static void NullScriptIsEmpty()
    {
        // Act
        var section = new NotebookImportsSection { Script = null! };

        // Assert
        Check(section.Script.Length == 0);
    }

    /// <summary>
    /// Tests ScriptIsContent.
    /// </summary>
    private static void ScriptIsContent()
    {
        // Setup
        var section = new NotebookImportsSection();

        // Act
        section.Script = "using A;";

        // Assert
        Check(section.HasChanges);
    }

    /// <summary>
    /// Tests ScriptGivesThePreview.
    /// </summary>
    private static void ScriptGivesThePreview()
    {
        // Act
        var section = new NotebookImportsSection { Script = "using A;\nusing B;" };

        // Assert
        Check(section.Preview == "using A;");
    }

    /// <summary>
    /// Tests EditingWithoutACheckKeepsItEmpty.
    /// </summary>
    private static void EditingWithoutACheckKeepsItEmpty()
    {
        // Act
        var section = new NotebookImportsSection { Script = "bad" };

        // Assert
        Check(section.Check is null);
    }

    /// <summary>
    /// Tests RestoreChecksAScript.
    /// </summary>
    private static void RestoreChecksAScript()
    {
        // Setup
        var section = new NotebookImportsSection { Script = "using A;" };

        // Act
        section.RestoreCheck();

        // Assert
        Check(section.Check is { IsValid: true });
    }

    /// <summary>
    /// Tests RestoreLeavesABlankScriptUnchecked.
    /// </summary>
    private static void RestoreLeavesABlankScriptUnchecked()
    {
        // Setup
        var section = new NotebookImportsSection { Script = " " };

        // Act
        section.RestoreCheck();

        // Assert
        Check(section.Check is null);
    }

    /// <summary>
    /// Tests EditingUpdatesTheCheck.
    /// </summary>
    private static void EditingUpdatesTheCheck()
    {
        // Setup
        var section = new NotebookImportsSection { Script = "using A;" };
        section.RestoreCheck();

        // Act
        section.Script = "bad";

        // Assert
        Check(section.Check is { IsValid: false });
    }

    /// <summary>
    /// Tests ValidImportsSucceed.
    /// </summary>
    private static async Task ValidImportsSucceed()
    {
        // Act
        var section = await Ran("using A;");

        // Assert
        Check(section.State == NotebookRunState.Succeeded && section.StatusMessage == "1 import is valid.");
        Check(section.Log.Entries
            .Single().Level == NotebookLogLevel.Information);
    }

    /// <summary>
    /// Tests InvalidImportsFail.
    /// </summary>
    private static async Task InvalidImportsFail()
    {
        // Act
        var section = await Ran("using A");

        // Assert
        Check(section.State == NotebookRunState.Failed);
        Check(section.StatusMessage == "Line 1: Missing ';' at the end of 'using A'.");
    }

    /// <summary>
    /// Tests ProblemsAreLogged.
    /// </summary>
    private static async Task ProblemsAreLogged()
    {
        // Act
        var section = await Ran("using A");

        // Assert
        Check(section.Log.Entries
            .Count(entry => entry.Level == NotebookLogLevel.Error) == 2);
    }

    /// <summary>
    /// Tests WarningsAreLogged.
    /// </summary>
    private static async Task WarningsAreLogged()
    {
        // Act
        var section = await Ran("using A; using A;");

        // Assert
        Check(section.Log.Entries[0].Level == NotebookLogLevel.Warning);
    }

    /// <summary>
    /// Tests ClearOutputRemovesTheCheck.
    /// </summary>
    private static async Task ClearOutputRemovesTheCheck()
    {
        // Setup
        var section = await Ran("using A;");

        // Act
        section.ClearOutput();

        // Assert
        Check(section.Check is null);
    }

    #endregion
}
