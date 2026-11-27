namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the comment, parser and validator helpers of the imports.
/// </summary>
internal static class NotebookImportHelperTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunCommentTests();
        RunParserTests();
        RunValidatorTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs comment scenarios.
    /// </summary>
    private static void RunCommentTests()
    {
        StripRemovesALineComment();
        StripRemovesABlockInALine();
        StripOpensABlock();
        StripStaysInABlock();
        StripClosesABlock();
    }

    /// <summary>
    /// Runs parser scenarios.
    /// </summary>
    private static void RunParserTests()
    {
        ParsesANamespace();
        ParsesAStaticImport();
        ParsesAnAlias();
        ParsesAGlobalName();
        RejectsOtherCode();
        RejectsAWordThatStartsWithUsing();
        RejectsABareUsing();
        RejectsAUsingStatement();
        RejectsAUsingVariable();
        RejectsAnInvalidName();
        RejectsAnEmptyNamePart();
        RejectsAnInvalidAliasName();
        RejectsAnInvalidAliasTarget();
        ReturnsNoEntryForARejectedTarget();
    }

    /// <summary>
    /// Runs validator scenarios.
    /// </summary>
    private static void RunValidatorTests()
    {
        AcceptsEmptyText();
        AcceptsManyImportsInALine();
        IgnoresComments();
        ReportsAMissingSemicolon();
        ReportsTheLineOfAnError();
        WarnsAboutADuplicate();
        CountsAllLineEndings();
    }

    /// <summary>
    /// Parses a statement and returns its problem.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>The problem.</returns>
    private static string? ProblemOf(string statement)
    {
        _ = NotebookImportParser.TryParse(1,
            statement,
            NotebookMessages.Default,
            out _,
            out var error);
        return error;
    }

    /// <summary>
    /// Parses a statement and returns its entry.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns>The entry.</returns>
    private static NotebookImportEntry? EntryOf(string statement)
    {
        _ = NotebookImportParser.TryParse(1,
            statement,
            NotebookMessages.Default,
            out var entry,
            out _);
        return entry;
    }

    /// <summary>
    /// Tests StripRemovesALineComment.
    /// </summary>
    private static void StripRemovesALineComment()
    {
        // Setup
        var inBlock = false;

        // Act
        var code = NotebookImportComments.Strip("using A; // note",
            ref inBlock);

        // Assert
        Check(code == "using A; ");
    }

    /// <summary>
    /// Tests StripRemovesABlockInALine.
    /// </summary>
    private static void StripRemovesABlockInALine()
    {
        // Setup
        var inBlock = false;

        // Act
        var code = NotebookImportComments.Strip("a/* x */b",
            ref inBlock);

        // Assert
        Check(code == "ab" && !inBlock);
    }

    /// <summary>
    /// Tests StripOpensABlock.
    /// </summary>
    private static void StripOpensABlock()
    {
        // Setup
        var inBlock = false;

        // Act
        var code = NotebookImportComments.Strip("a /* x",
            ref inBlock);

        // Assert
        Check(code == "a " && inBlock);
    }

    /// <summary>
    /// Tests StripStaysInABlock.
    /// </summary>
    private static void StripStaysInABlock()
    {
        // Setup
        var inBlock = true;

        // Act
        var code = NotebookImportComments.Strip("still inside",
            ref inBlock);

        // Assert
        Check(code.Length == 0 && inBlock);
    }

    /// <summary>
    /// Tests StripClosesABlock.
    /// </summary>
    private static void StripClosesABlock()
    {
        // Setup
        var inBlock = true;

        // Act
        var code = NotebookImportComments.Strip("end */b",
            ref inBlock);

        // Assert
        Check(code == "b" && !inBlock);
    }

    /// <summary>
    /// Tests ParsesANamespace.
    /// </summary>
    private static void ParsesANamespace()
    {
        // Act
        var entry = EntryOf("using System.Linq");

        // Assert
        Check(entry is { Kind: NotebookImportKind.Namespace, Target: "System.Linq" });
    }

    /// <summary>
    /// Tests ParsesAStaticImport.
    /// </summary>
    private static void ParsesAStaticImport()
    {
        // Act
        var entry = EntryOf("using static System.Math");

        // Assert
        Check(entry is { Kind: NotebookImportKind.Static, Target: "System.Math" });
    }

    /// <summary>
    /// Tests ParsesAnAlias.
    /// </summary>
    private static void ParsesAnAlias()
    {
        // Act
        var entry = EntryOf("using J=System.Text.Json");

        // Assert
        Check(entry is { Kind: NotebookImportKind.Alias, Target: "J = System.Text.Json" });
    }

    /// <summary>
    /// Tests ParsesAGlobalName.
    /// </summary>
    private static void ParsesAGlobalName()
    {
        // Act
        var entry = EntryOf("using global::System");

        // Assert
        Check(entry is not null);
    }

    /// <summary>
    /// Tests RejectsOtherCode.
    /// </summary>
    private static void RejectsOtherCode()
    {
        // Act
        var error = ProblemOf("var x = 1");

        // Assert
        Check(error!.StartsWith("Only 'using' directives are allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAWordThatStartsWithUsing.
    /// </summary>
    private static void RejectsAWordThatStartsWithUsing()
    {
        // Act
        var error = ProblemOf("usingSystem");

        // Assert
        Check(error!.StartsWith("Only 'using' directives are allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsABareUsing.
    /// </summary>
    private static void RejectsABareUsing()
    {
        // Act
        var error = ProblemOf("using");

        // Assert
        Check(error!.StartsWith("Write what to import",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAUsingStatement.
    /// </summary>
    private static void RejectsAUsingStatement()
    {
        // Act
        var error = ProblemOf("using (var x = y)");

        // Assert
        Check(error!.Contains("not 'using' statements",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAUsingVariable.
    /// </summary>
    private static void RejectsAUsingVariable()
    {
        // Act
        var error = ProblemOf("using var x = y");

        // Assert
        Check(error!.Contains("not 'using' statements",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAnInvalidName.
    /// </summary>
    private static void RejectsAnInvalidName()
    {
        // Act
        var error = ProblemOf("using System-Linq");

        // Assert
        Check(error!.Contains("is not a valid namespace",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAnEmptyNamePart.
    /// </summary>
    private static void RejectsAnEmptyNamePart()
    {
        // Act
        var error = ProblemOf("using System..Linq");

        // Assert
        Check(error!.Contains("is not a valid namespace",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAnInvalidAliasName.
    /// </summary>
    private static void RejectsAnInvalidAliasName()
    {
        // Act
        var error = ProblemOf("using 1J = System");

        // Assert
        Check(error!.Contains("is not a valid alias",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RejectsAnInvalidAliasTarget.
    /// </summary>
    private static void RejectsAnInvalidAliasTarget()
    {
        // Act
        var error = ProblemOf("using J =");

        // Assert
        Check(error!.Contains("is not a valid alias",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests ReturnsNoEntryForARejectedTarget.
    /// </summary>
    private static void ReturnsNoEntryForARejectedTarget()
    {
        // Act
        var entry = EntryOf("using 9");

        // Assert
        Check(entry is null);
    }

    /// <summary>
    /// Tests AcceptsEmptyText.
    /// </summary>
    private static void AcceptsEmptyText()
    {
        // Act
        var check = NotebookImportValidator.Check("");

        // Assert
        Check(check.IsValid && check.Entries.Count == 0);
    }

    /// <summary>
    /// Tests AcceptsManyImportsInALine.
    /// </summary>
    private static void AcceptsManyImportsInALine()
    {
        // Act
        var check = NotebookImportValidator.Check("using A; using B;");

        // Assert
        Check(check.IsValid && check.Entries.Count == 2);
    }

    /// <summary>
    /// Tests IgnoresComments.
    /// </summary>
    private static void IgnoresComments()
    {
        // Act
        var check = NotebookImportValidator.Check("// a\n/* b\nc */\nusing A;");

        // Assert
        Check(check.IsValid && check.Entries
            .Single().Line == 4);
    }

    /// <summary>
    /// Tests ReportsAMissingSemicolon.
    /// </summary>
    private static void ReportsAMissingSemicolon()
    {
        // Act
        var check = NotebookImportValidator.Check("using A");

        // Assert
        Check(check.Errors
            .Single().Message == "Missing ';' at the end of 'using A'.");
    }

    /// <summary>
    /// Tests ReportsTheLineOfAnError.
    /// </summary>
    private static void ReportsTheLineOfAnError()
    {
        // Act
        var check = NotebookImportValidator.Check("using A;\nreturn;");

        // Assert
        Check(check.Errors
            .Single().Line == 2);
    }

    /// <summary>
    /// Tests WarnsAboutADuplicate.
    /// </summary>
    private static void WarnsAboutADuplicate()
    {
        // Act
        var check = NotebookImportValidator.Check("using A;\nusing A;");

        // Assert
        Check(check.Warnings
            .Single() is { Line: 2, Message: "'using A;' is already imported." });
    }

    /// <summary>
    /// Tests CountsAllLineEndings.
    /// </summary>
    private static void CountsAllLineEndings()
    {
        // Act
        var check = NotebookImportValidator.Check("using A;\r\nusing B;\rusing C;");

        // Assert
        Check(check.Entries[^1].Line == 3);
    }

    #endregion
}
