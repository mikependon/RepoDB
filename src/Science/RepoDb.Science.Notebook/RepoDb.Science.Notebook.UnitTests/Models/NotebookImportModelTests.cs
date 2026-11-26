namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the models of the imports check.
/// </summary>
internal static class NotebookImportModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        EntryKeepsItsValues();
        NamespaceEntryWritesAUsing();
        StaticEntryWritesAUsingStatic();
        AliasEntryWritesAUsing();
        EntryRejectsNullTarget();
        IssueKeepsItsValues();
        IssueWritesItsLine();
        IssueRejectsNullMessage();
        CheckKeepsItsLists();
        CheckIsValidWithoutErrors();
        CheckIsInvalidWithErrors();
        SummaryOfNoImports();
        SummaryOfOneImport();
        SummaryOfManyImports();
        SummaryOfOneProblem();
        SummaryOfManyProblems();
        CheckRejectsNullEntries();
        CheckRejectsNullErrors();
        CheckRejectsNullWarnings();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a check with a number of entries and errors.
    /// </summary>
    /// <param name="entries">The number of entries.</param>
    /// <param name="errors">The number of errors.</param>
    /// <returns>The check.</returns>
    private static NotebookImportCheck CheckOf(int entries,
        int errors) => new(Enumerable
            .Repeat(new NotebookImportEntry(1,
                    NotebookImportKind.Namespace,
                    "System"),
                entries)
            .ToList(),
        Enumerable
            .Repeat(new NotebookImportIssue(1,
                    "bad"),
                errors)
            .ToList(),
        []);

    /// <summary>
    /// Tests EntryKeepsItsValues.
    /// </summary>
    private static void EntryKeepsItsValues()
    {
        // Act
        var entry = new NotebookImportEntry(2,
            NotebookImportKind.Static,
            "System.Math");

        // Assert
        Check(entry.Line == 2 && entry.Kind == NotebookImportKind.Static && entry.Target == "System.Math");
    }

    /// <summary>
    /// Tests NamespaceEntryWritesAUsing.
    /// </summary>
    private static void NamespaceEntryWritesAUsing()
    {
        // Act
        var entry = new NotebookImportEntry(1,
            NotebookImportKind.Namespace,
            "System");

        // Assert
        Check(entry.Text == "using System;");
    }

    /// <summary>
    /// Tests StaticEntryWritesAUsingStatic.
    /// </summary>
    private static void StaticEntryWritesAUsingStatic()
    {
        // Act
        var entry = new NotebookImportEntry(1,
            NotebookImportKind.Static,
            "System.Math");

        // Assert
        Check(entry.Text == "using static System.Math;");
    }

    /// <summary>
    /// Tests AliasEntryWritesAUsing.
    /// </summary>
    private static void AliasEntryWritesAUsing()
    {
        // Act
        var entry = new NotebookImportEntry(1,
            NotebookImportKind.Alias,
            "J = System.Text.Json");

        // Assert
        Check(entry.Text == "using J = System.Text.Json;");
    }

    /// <summary>
    /// Tests EntryRejectsNullTarget.
    /// </summary>
    private static void EntryRejectsNullTarget()
    {
        // Act
        Action operation = () => _ = new NotebookImportEntry(1,
            NotebookImportKind.Namespace,
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests IssueKeepsItsValues.
    /// </summary>
    private static void IssueKeepsItsValues()
    {
        // Act
        var issue = new NotebookImportIssue(3,
            "bad");

        // Assert
        Check(issue.Line == 3 && issue.Message == "bad");
    }

    /// <summary>
    /// Tests IssueWritesItsLine.
    /// </summary>
    private static void IssueWritesItsLine()
    {
        // Act
        var issue = new NotebookImportIssue(3,
            "bad");

        // Assert
        Check(issue.Text == "Line 3: bad");
    }

    /// <summary>
    /// Tests IssueRejectsNullMessage.
    /// </summary>
    private static void IssueRejectsNullMessage()
    {
        // Act
        Action operation = () => _ = new NotebookImportIssue(1,
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CheckKeepsItsLists.
    /// </summary>
    private static void CheckKeepsItsLists()
    {
        // Setup
        NotebookImportEntry[] entries = [];
        NotebookImportIssue[] errors = [];
        NotebookImportIssue[] warnings = [new(1,
            "w")];

        // Act
        var check = new NotebookImportCheck(entries,
            errors,
            warnings);

        // Assert
        Check(ReferenceEquals(check.Entries,
            entries) && ReferenceEquals(check.Errors,
                errors) && ReferenceEquals(check.Warnings,
                    warnings));
    }

    /// <summary>
    /// Tests CheckIsValidWithoutErrors.
    /// </summary>
    private static void CheckIsValidWithoutErrors()
    {
        // Act
        var check = CheckOf(1,
            0);

        // Assert
        Check(check.IsValid);
    }

    /// <summary>
    /// Tests CheckIsInvalidWithErrors.
    /// </summary>
    private static void CheckIsInvalidWithErrors()
    {
        // Act
        var check = CheckOf(0,
            1);

        // Assert
        Check(!check.IsValid);
    }

    /// <summary>
    /// Tests SummaryOfNoImports.
    /// </summary>
    private static void SummaryOfNoImports()
    {
        // Act
        var summary = CheckOf(0,
            0).Summary;

        // Assert
        Check(summary == "No imports yet.");
    }

    /// <summary>
    /// Tests SummaryOfOneImport.
    /// </summary>
    private static void SummaryOfOneImport()
    {
        // Act
        var summary = CheckOf(1,
            0).Summary;

        // Assert
        Check(summary == "1 import is valid.");
    }

    /// <summary>
    /// Tests SummaryOfManyImports.
    /// </summary>
    private static void SummaryOfManyImports()
    {
        // Act
        var summary = CheckOf(2,
            0).Summary;

        // Assert
        Check(summary == "2 imports are valid.");
    }

    /// <summary>
    /// Tests SummaryOfOneProblem.
    /// </summary>
    private static void SummaryOfOneProblem()
    {
        // Act
        var summary = CheckOf(0,
            1).Summary;

        // Assert
        Check(summary == "1 problem found.");
    }

    /// <summary>
    /// Tests SummaryOfManyProblems.
    /// </summary>
    private static void SummaryOfManyProblems()
    {
        // Act
        var summary = CheckOf(0,
            2).Summary;

        // Assert
        Check(summary == "2 problems found.");
    }

    /// <summary>
    /// Tests CheckRejectsNullEntries.
    /// </summary>
    private static void CheckRejectsNullEntries()
    {
        // Act
        Action operation = () => _ = new NotebookImportCheck(null!,
            [],
            []);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CheckRejectsNullErrors.
    /// </summary>
    private static void CheckRejectsNullErrors()
    {
        // Act
        Action operation = () => _ = new NotebookImportCheck([],
            null!,
            []);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CheckRejectsNullWarnings.
    /// </summary>
    private static void CheckRejectsNullWarnings()
    {
        // Act
        Action operation = () => _ = new NotebookImportCheck([],
            [],
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
