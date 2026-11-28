namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the default texts.
/// </summary>
internal static class NotebookMessagesTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunGeneralTests();
        RunSqlTests();
        RunCSharpTests();
        RunChartTests();
        RunImportTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the general and run texts.
    /// </summary>
    private static void RunGeneralTests()
    {
        DefaultIsShared();
        NewInstanceIsSeparate();
        DescribesANewNotebook();
        ReportsAStoppedSection();
        ReportsAStoppedRun();
        ReportsAFailedRun();
        NamesTheImportsStep();
        NamesTheNumberedSteps();
    }

    /// <summary>
    /// Runs the SQL texts.
    /// </summary>
    private static void RunSqlTests()
    {
        ReportsAMissingSqlExecutor();
        ReportsAMissingSqlScript();
        ReportsTheStartOfSql();
        ReportsTheEndOfSql();
        ReportsLimitedRows();
        KeepsTheSqlError();
        CountsRows();
        CountsAffectedRows();
        ExplainsRefusedWrites();
    }

    /// <summary>
    /// Runs the C# texts.
    /// </summary>
    private static void RunCSharpTests()
    {
        ReportsAMissingCSharpExecutor();
        ReportsAnUnavailableCSharpExecutor();
        ReportsASuccessfulScript();
        ReportsAFailedScript();
        WritesTheReturnedValue();
    }

    /// <summary>
    /// Runs the chart texts.
    /// </summary>
    private static void RunChartTests()
    {
        ReportsAMissingSqlChartTable();
        ReportsAMissingCSharpChartTable();
        ReportsADrawnChart();
        ExplainsALabelColumn();
        ListsSkippedColumns();
        ExplainsATableWithoutRows();
        ExplainsATableWithTooFewColumns();
        ExplainsATableWithoutTimes();
        ExplainsAColumnThatIsNotNumeric();
        ExplainsATableWithoutNumbers();
    }

    /// <summary>
    /// Runs the import texts.
    /// </summary>
    private static void RunImportTests()
    {
        ExplainsCodeInTheImports();
        ExplainsAnEmptyUsing();
        ExplainsAUsingStatement();
        ExplainsAnInvalidName();
        ExplainsAnInvalidAlias();
        ExplainsAMissingSemicolon();
        ExplainsADuplicate();
        WritesAnIssueWithItsLine();
        SummarizesNoImports();
        SummarizesValidImports();
        SummarizesProblems();
    }

    /// <summary>
    /// Tests DefaultIsShared.
    /// </summary>
    private static void DefaultIsShared() => Check(ReferenceEquals(NotebookMessages.Default,
        NotebookMessages.Default));

    /// <summary>
    /// Tests NewInstanceIsSeparate.
    /// </summary>
    private static void NewInstanceIsSeparate() => Check(!ReferenceEquals(new NotebookMessages(),
        NotebookMessages.Default));

    /// <summary>
    /// Tests DescribesANewNotebook.
    /// </summary>
    private static void DescribesANewNotebook() => Check(NotebookMessages.Default.NewNotebookDescription() == NotebookDocument.DefaultDescription);

    /// <summary>
    /// Tests ReportsAStoppedSection.
    /// </summary>
    private static void ReportsAStoppedSection() => Check(NotebookMessages.Default.SectionStopped(new NotebookCodeSection()) == "Stopped by the user.");

    /// <summary>
    /// Tests ReportsAStoppedRun.
    /// </summary>
    private static void ReportsAStoppedRun() => Check(NotebookMessages.Default.RunStopped() == "Stopped by the user.");

    /// <summary>
    /// Tests ReportsAFailedRun.
    /// </summary>
    private static void ReportsAFailedRun() => Check(NotebookMessages.Default.RunFailed("Orders") == "Stopped: Orders failed.");

    /// <summary>
    /// Tests NamesTheImportsStep.
    /// </summary>
    private static void NamesTheImportsStep() => Check(NotebookMessages.Default.ImportsStepName() == "Imports");

    /// <summary>
    /// Tests NamesTheNumberedSteps.
    /// </summary>
    private static void NamesTheNumberedSteps()
    {
        // Assert
        Check(NotebookMessages.Default.ChartStepName(2) == "CHART 2");
        Check(NotebookMessages.Default.CSharpStepName(3) == "C# 3");
    }

    /// <summary>
    /// Tests ReportsAMissingSqlExecutor.
    /// </summary>
    private static void ReportsAMissingSqlExecutor() => Check(NotebookMessages.Default.SqlExecutorMissing() == "No SQL executor is configured.");

    /// <summary>
    /// Tests ReportsAMissingSqlScript.
    /// </summary>
    private static void ReportsAMissingSqlScript() => Check(NotebookMessages.Default.SqlScriptMissing() == "Type the SQL script to run.");

    /// <summary>
    /// Tests ReportsTheStartOfSql.
    /// </summary>
    private static void ReportsTheStartOfSql() => Check(NotebookMessages.Default.SqlStarted(new NotebookCodeSection()) == "Running the script.");

    /// <summary>
    /// Tests ReportsTheEndOfSql.
    /// </summary>
    private static void ReportsTheEndOfSql() => Check(NotebookMessages.Default.SqlCompleted(new NotebookCodeSection(),
        "ok") == "Done: ok");

    /// <summary>
    /// Tests ReportsLimitedRows.
    /// </summary>
    private static void ReportsLimitedRows() => Check(NotebookMessages.Default.SqlRowsLimited(new NotebookCodeSection(),
        5) == "The rows were limited to the row limit.");

    /// <summary>
    /// Tests KeepsTheSqlError.
    /// </summary>
    private static void KeepsTheSqlError() => Check(NotebookMessages.Default.SqlFailed(new NotebookCodeSection(),
        "bad") == "bad");

    /// <summary>
    /// Tests CountsRows.
    /// </summary>
    private static void CountsRows() => Check(NotebookMessages.Default.RowCount(2) == "2 row(s)");

    /// <summary>
    /// Tests CountsAffectedRows.
    /// </summary>
    private static void CountsAffectedRows() => Check(NotebookMessages.Default.RowsAffected(2) == "2 row(s) affected");

    /// <summary>
    /// Tests ExplainsRefusedWrites.
    /// </summary>
    private static void ExplainsRefusedWrites() => Check(NotebookMessages.Default.WritesNotAllowed() == "Only SELECT statements can run because writes are not allowed.");

    /// <summary>
    /// Tests ReportsAMissingCSharpExecutor.
    /// </summary>
    private static void ReportsAMissingCSharpExecutor() => Check(NotebookMessages.Default.CSharpExecutorMissing() == "C# scripts cannot be run yet.");

    /// <summary>
    /// Tests ReportsAnUnavailableCSharpExecutor.
    /// </summary>
    private static void ReportsAnUnavailableCSharpExecutor() => Check(NotebookMessages.Default.CSharpUnavailable() == "The C# executor is not available.");

    /// <summary>
    /// Tests ReportsASuccessfulScript.
    /// </summary>
    private static void ReportsASuccessfulScript() => Check(NotebookMessages.Default.CSharpSucceeded() == "The script ran well.");

    /// <summary>
    /// Tests ReportsAFailedScript.
    /// </summary>
    private static void ReportsAFailedScript() => Check(NotebookMessages.Default.CSharpFailed() == "The script failed.");

    /// <summary>
    /// Tests WritesTheReturnedValue.
    /// </summary>
    private static void WritesTheReturnedValue() => Check(NotebookMessages.Default.ReturnedValue("1") == "Returned: 1");

    /// <summary>
    /// Tests ReportsAMissingSqlChartTable.
    /// </summary>
    private static void ReportsAMissingSqlChartTable() => Check(NotebookMessages.Default.SqlChartTableMissing() == "The script returned no result set to chart.");

    /// <summary>
    /// Tests ReportsAMissingCSharpChartTable.
    /// </summary>
    private static void ReportsAMissingCSharpChartTable() => Check(NotebookMessages.Default.CSharpChartTableMissing()
        .StartsWith("The script did not give a table",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ReportsADrawnChart.
    /// </summary>
    private static void ReportsADrawnChart() => Check(NotebookMessages.Default.Charted(3,
        1) == "Charted 3 rows as 1 series.");

    /// <summary>
    /// Tests ExplainsALabelColumn.
    /// </summary>
    private static void ExplainsALabelColumn() => Check(NotebookMessages.Default.ChartLabelColumn("Name")
        .StartsWith("The first column 'Name' is not a timestamp",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ListsSkippedColumns.
    /// </summary>
    private static void ListsSkippedColumns() => Check(NotebookMessages.Default.ChartColumnsSkipped(["a", "b"]) == "Skipped the columns that are not numeric: a, b.");

    /// <summary>
    /// Tests ExplainsATableWithoutRows.
    /// </summary>
    private static void ExplainsATableWithoutRows() => Check(NotebookMessages.Default.ChartHasNoRows() == "The result has no rows to chart.");

    /// <summary>
    /// Tests ExplainsATableWithTooFewColumns.
    /// </summary>
    private static void ExplainsATableWithTooFewColumns() => Check(NotebookMessages.Default.ChartHasTooFewColumns()
        .StartsWith("A chart needs a first column",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsATableWithoutTimes.
    /// </summary>
    private static void ExplainsATableWithoutTimes() => Check(NotebookMessages.Default.ChartHasNoTimestamp() == "No row has a valid timestamp in its first column.");

    /// <summary>
    /// Tests ExplainsAColumnThatIsNotNumeric.
    /// </summary>
    private static void ExplainsAColumnThatIsNotNumeric() => Check(NotebookMessages.Default.ChartColumnNotNumeric("Note",
        "string")
        .StartsWith("The column 'Note' is not numeric (string)",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsATableWithoutNumbers.
    /// </summary>
    private static void ExplainsATableWithoutNumbers() => Check(NotebookMessages.Default.ChartHasNoNumericColumn() == "The result has no numeric column after the first one.");

    /// <summary>
    /// Tests ExplainsCodeInTheImports.
    /// </summary>
    private static void ExplainsCodeInTheImports() => Check(NotebookMessages.Default.ImportCodeNotAllowed()
        .StartsWith("Only 'using' directives are allowed",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsAnEmptyUsing.
    /// </summary>
    private static void ExplainsAnEmptyUsing() => Check(NotebookMessages.Default.ImportTargetMissing()
        .StartsWith("Write what to import",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsAUsingStatement.
    /// </summary>
    private static void ExplainsAUsingStatement() => Check(NotebookMessages.Default.ImportStatementNotAllowed()
        .Contains("not 'using' statements",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsAnInvalidName.
    /// </summary>
    private static void ExplainsAnInvalidName() => Check(NotebookMessages.Default.ImportNameInvalid("9") == "'9' is not a valid namespace or type name.");

    /// <summary>
    /// Tests ExplainsAnInvalidAlias.
    /// </summary>
    private static void ExplainsAnInvalidAlias() => Check(NotebookMessages.Default.ImportAliasInvalid("J =")
        .StartsWith("'J =' is not a valid alias",
        StringComparison.Ordinal));

    /// <summary>
    /// Tests ExplainsAMissingSemicolon.
    /// </summary>
    private static void ExplainsAMissingSemicolon() => Check(NotebookMessages.Default.ImportSemicolonMissing("using A") == "Missing ';' at the end of 'using A'.");

    /// <summary>
    /// Tests ExplainsADuplicate.
    /// </summary>
    private static void ExplainsADuplicate() => Check(NotebookMessages.Default.ImportDuplicate("using A;") == "'using A;' is already imported.");

    /// <summary>
    /// Tests WritesAnIssueWithItsLine.
    /// </summary>
    private static void WritesAnIssueWithItsLine() => Check(NotebookMessages.Default.ImportIssue(2,
        "bad") == "Line 2: bad");

    /// <summary>
    /// Tests SummarizesNoImports.
    /// </summary>
    private static void SummarizesNoImports() => Check(NotebookMessages.Default.NoImports() == "No imports yet.");

    /// <summary>
    /// Tests SummarizesValidImports.
    /// </summary>
    private static void SummarizesValidImports()
    {
        // Assert
        Check(NotebookMessages.Default.ImportsValid(1) == "1 import is valid.");
        Check(NotebookMessages.Default.ImportsValid(2) == "2 imports are valid.");
    }

    /// <summary>
    /// Tests SummarizesProblems.
    /// </summary>
    private static void SummarizesProblems()
    {
        // Assert
        Check(NotebookMessages.Default.ImportProblems(1) == "1 problem found.");
        Check(NotebookMessages.Default.ImportProblems(2) == "2 problems found.");
    }

    #endregion
}
