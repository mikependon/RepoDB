namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the log entries of the problems of a script.
/// </summary>
internal static class NotebookScriptDiagnosticsTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ErrorCreatesAnError();
        WarningCreatesAWarning();
        TellsTheLineOfTheCode();
        TellsTheLineOfTheImports();
        TellsEveryError();
        LeavesOutWarnings();
        UsesTheTextsOfTheHost();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the entries of the errors of a script.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="imports">The imports.</param>
    /// <param name="messages">The texts, or null for the default ones.</param>
    /// <returns>The entries.</returns>
    private static List<NotebookLogEntry> Errors(string code,
        string imports = "",
        NotebookScriptingMessages? messages = null) => NotebookScriptDiagnostics.ErrorsOf(Compilations.Of(NotebookScriptFactory.PrefixOf(imports) + code)
            .GetDiagnostics(),
        NotebookScriptFactory.ImportLinesOf(imports),
        messages ?? NotebookScriptingMessages.Default);

    /// <summary>
    /// Tests ErrorCreatesAnError.
    /// </summary>
    private static void ErrorCreatesAnError()
    {
        // Act
        var entry = NotebookScriptDiagnostics.Error("broken");

        // Assert
        Check(entry.Level == NotebookLogLevel.Error && entry.Message == "broken");
    }

    /// <summary>
    /// Tests WarningCreatesAWarning.
    /// </summary>
    private static void WarningCreatesAWarning()
    {
        // Act
        var entry = NotebookScriptDiagnostics.Warning("careful");

        // Assert
        Check(entry.Level == NotebookLogLevel.Warning && entry.Message == "careful");
    }

    /// <summary>
    /// Tests TellsTheLineOfTheCode.
    /// </summary>
    private static void TellsTheLineOfTheCode()
    {
        // Act
        var entries = Errors("var first = 1;\nvar broken = ;",
            "using System.Text;");

        // Assert
        Check(entries
            .Single().Message.StartsWith("Line 2: CS1525: ",
                StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TellsTheLineOfTheImports.
    /// </summary>
    private static void TellsTheLineOfTheImports()
    {
        // Act
        var entries = Errors("return 1;",
            "using System.Text;\nusing Missing.Space;");

        // Assert
        Check(entries
            .Single().Message.StartsWith("Imports, line 2: CS0246: ",
                StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TellsEveryError.
    /// </summary>
    private static void TellsEveryError()
    {
        // Act
        var entries = Errors("Missing();\nAbsent();");

        // Assert
        Check(entries.Count == 2);
    }

    /// <summary>
    /// Tests LeavesOutWarnings.
    /// </summary>
    private static void LeavesOutWarnings()
    {
        // Act
        var entries = Errors("#warning careful\nreturn 1;");

        // Assert
        Check(entries.Count == 0);
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHost.
    /// </summary>
    private static void UsesTheTextsOfTheHost()
    {
        // Act
        var entries = Errors("var broken = ;",
            messages: new MarkedMessages());

        // Assert
        Check(entries
            .Single().Message == "code 1 CS1525");
    }

    #endregion
}
