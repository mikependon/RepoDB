namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the SQL scanner and the read-only check.
/// </summary>
internal static class NotebookSqlCheckTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunScannerTests();
        RunAllowedTests();
        RunWriteTests();
        RunAmbiguityTests();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs scanner scenarios.
    /// </summary>
    private static void RunScannerTests()
    {
        ScanSplitsStatements();
        ScanUppercasesWords();
        ScanReturnsNullWhenAmbiguous();
    }

    /// <summary>
    /// Runs allowed scenarios.
    /// </summary>
    private static void RunAllowedTests()
    {
        AllowsASelect();
        AllowsACommonTableExpression();
        AllowsAParenthesizedSelect();
        AllowsManySelects();
        AllowsWritingWordsInTexts();
        AllowsDoubledQuotes();
        AllowsQuotedIdentifiers();
        AllowsBracketedIdentifiers();
        AllowsLineComments();
        AllowsAnEmptyLineCommentAtTheEnd();
        AllowsBlockComments();
        AllowsWindowsLineEndings();
        AllowsTheLetterQAsAName();
        AllowsAStatementWordWhenQuoted();
    }

    /// <summary>
    /// Runs writing scenarios.
    /// </summary>
    private static void RunWriteTests()
    {
        RejectsEmptyText();
        RejectsOnlyComments();
        RejectsADelete();
        RejectsASelectInto();
        RejectsAWriteAfterASemicolon();
        RejectsAWriteWithoutASemicolon();
        RejectsAWriteInACommonTableExpression();
        RejectsAStatementThatStartsWithText();
        RejectsAColumnNamedLikeAStatement();
    }

    /// <summary>
    /// Runs scenarios of text that databases read differently.
    /// </summary>
    private static void RunAmbiguityTests()
    {
        RejectsAnOpenText();
        RejectsABackslashInAText();
        RejectsABackslashOutsideATexts();
        RejectsATightLineComment();
        RejectsAnOpenBlockComment();
        RejectsAnExecutableComment();
        RejectsAMariaDbExecutableComment();
        RejectsANestedComment();
        RejectsAnOpenBracket();
        RejectsAQuoteInABracket();
        RejectsALineCommentInABracket();
        RejectsABlockCommentInABracket();
        RejectsADoubledBracket();
        RejectsAHashSign();
        RejectsADollarSign();
        RejectsALoneCarriageReturn();
        RejectsAlternativeQuoting();
        RejectsNationalAlternativeQuoting();
    }

    /// <summary>
    /// Checks a script.
    /// </summary>
    /// <param name="sql">The script.</param>
    /// <returns>Whether it is read only.</returns>
    private static bool ReadOnly(string sql) => NotebookSqlReadOnlyCheck.IsReadOnly(sql);

    /// <summary>
    /// Tests ScanSplitsStatements.
    /// </summary>
    private static void ScanSplitsStatements()
    {
        // Act
        var statements = NotebookSqlScanner.Scan("SELECT 1;;SELECT 2;");

        // Assert
        Check(statements is { Count: 2 });
    }

    /// <summary>
    /// Tests ScanUppercasesWords.
    /// </summary>
    private static void ScanUppercasesWords()
    {
        // Act
        var statements = NotebookSqlScanner.Scan("select a_1, @b");

        // Assert
        Check(string.Join(' ',
            statements!
                .Single()) == "SELECT A_1 , @B");
    }

    /// <summary>
    /// Tests ScanReturnsNullWhenAmbiguous.
    /// </summary>
    private static void ScanReturnsNullWhenAmbiguous()
    {
        // Act
        var statements = NotebookSqlScanner.Scan("SELECT '");

        // Assert
        Check(statements is null);
    }

    /// <summary>
    /// Tests AllowsASelect.
    /// </summary>
    private static void AllowsASelect() => Check(ReadOnly("select * from t where a = 1"));

    /// <summary>
    /// Tests AllowsACommonTableExpression.
    /// </summary>
    private static void AllowsACommonTableExpression() => Check(ReadOnly("WITH x AS (SELECT 1) SELECT * FROM x"));

    /// <summary>
    /// Tests AllowsAParenthesizedSelect.
    /// </summary>
    private static void AllowsAParenthesizedSelect() => Check(ReadOnly("((SELECT 1))"));

    /// <summary>
    /// Tests AllowsManySelects.
    /// </summary>
    private static void AllowsManySelects() => Check(ReadOnly("SELECT 1; SELECT 2"));

    /// <summary>
    /// Tests AllowsWritingWordsInTexts.
    /// </summary>
    private static void AllowsWritingWordsInTexts() => Check(ReadOnly("SELECT 'DELETE; DROP'"));

    /// <summary>
    /// Tests AllowsDoubledQuotes.
    /// </summary>
    private static void AllowsDoubledQuotes() => Check(ReadOnly("SELECT 'it''s'"));

    /// <summary>
    /// Tests AllowsQuotedIdentifiers.
    /// </summary>
    private static void AllowsQuotedIdentifiers() => Check(ReadOnly("SELECT \"Update\", `Delete` FROM t"));

    /// <summary>
    /// Tests AllowsBracketedIdentifiers.
    /// </summary>
    private static void AllowsBracketedIdentifiers() => Check(ReadOnly("SELECT [Update] FROM [dbo].[t]"));

    /// <summary>
    /// Tests AllowsLineComments.
    /// </summary>
    private static void AllowsLineComments() => Check(ReadOnly("-- delete ' [\nSELECT 1 -- drop"));

    /// <summary>
    /// Tests AllowsAnEmptyLineCommentAtTheEnd.
    /// </summary>
    private static void AllowsAnEmptyLineCommentAtTheEnd() => Check(ReadOnly("SELECT 1 --"));

    /// <summary>
    /// Tests AllowsBlockComments.
    /// </summary>
    private static void AllowsBlockComments() => Check(ReadOnly("/* delete ' */ SELECT /*+ hint */ 1"));

    /// <summary>
    /// Tests AllowsWindowsLineEndings.
    /// </summary>
    private static void AllowsWindowsLineEndings() => Check(ReadOnly("SELECT 1\r\nFROM t"));

    /// <summary>
    /// Tests AllowsTheLetterQAsAName.
    /// </summary>
    private static void AllowsTheLetterQAsAName() => Check(ReadOnly("SELECT q, nq FROM t"));

    /// <summary>
    /// Tests AllowsAStatementWordWhenQuoted.
    /// </summary>
    private static void AllowsAStatementWordWhenQuoted() => Check(ReadOnly("SELECT [Copy], \"Load\", `Move` FROM t"));

    /// <summary>
    /// Tests RejectsAColumnNamedLikeAStatement.
    /// </summary>
    private static void RejectsAColumnNamedLikeAStatement()
    {
        // Assert
        Check(!ReadOnly("SELECT Copy FROM t") && !ReadOnly("SELECT Load, Move FROM t"));
    }

    /// <summary>
    /// Tests RejectsEmptyText.
    /// </summary>
    private static void RejectsEmptyText() => Check(!ReadOnly("  "));

    /// <summary>
    /// Tests RejectsOnlyComments.
    /// </summary>
    private static void RejectsOnlyComments() => Check(!ReadOnly("/* nothing */"));

    /// <summary>
    /// Tests RejectsADelete.
    /// </summary>
    private static void RejectsADelete() => Check(!ReadOnly("DELETE FROM t"));

    /// <summary>
    /// Tests RejectsASelectInto.
    /// </summary>
    private static void RejectsASelectInto() => Check(!ReadOnly("SELECT * INTO copy FROM t"));

    /// <summary>
    /// Tests RejectsAWriteAfterASemicolon.
    /// </summary>
    private static void RejectsAWriteAfterASemicolon() => Check(!ReadOnly("SELECT 1; DROP TABLE t"));

    /// <summary>
    /// Tests RejectsAWriteWithoutASemicolon.
    /// </summary>
    private static void RejectsAWriteWithoutASemicolon() => Check(!ReadOnly("SELECT 1 TRUNCATE TABLE t"));

    /// <summary>
    /// Tests RejectsAWriteInACommonTableExpression.
    /// </summary>
    private static void RejectsAWriteInACommonTableExpression() => Check(!ReadOnly("WITH x AS (SELECT 1) UPDATE t SET a = 1"));

    /// <summary>
    /// Tests RejectsAStatementThatStartsWithText.
    /// </summary>
    private static void RejectsAStatementThatStartsWithText() => Check(!ReadOnly("'x' SELECT 1"));

    /// <summary>
    /// Tests RejectsAnOpenText.
    /// </summary>
    private static void RejectsAnOpenText() => Check(!ReadOnly("SELECT 'a"));

    /// <summary>
    /// Tests RejectsABackslashInAText.
    /// </summary>
    private static void RejectsABackslashInAText() => Check(!ReadOnly("SELECT '\\'' ; DELETE FROM t; -- '"));

    /// <summary>
    /// Tests RejectsABackslashOutsideATexts.
    /// </summary>
    private static void RejectsABackslashOutsideATexts() => Check(!ReadOnly("SELECT 1 \\ 2"));

    /// <summary>
    /// Tests RejectsATightLineComment.
    /// </summary>
    private static void RejectsATightLineComment() => Check(!ReadOnly("SELECT 1 --1; DELETE FROM t"));

    /// <summary>
    /// Tests RejectsAnOpenBlockComment.
    /// </summary>
    private static void RejectsAnOpenBlockComment() => Check(!ReadOnly("SELECT 1 /* open"));

    /// <summary>
    /// Tests RejectsAnExecutableComment.
    /// </summary>
    private static void RejectsAnExecutableComment() => Check(!ReadOnly("SELECT 1 /*! ; DELETE FROM t */"));

    /// <summary>
    /// Tests RejectsAMariaDbExecutableComment.
    /// </summary>
    private static void RejectsAMariaDbExecutableComment() => Check(!ReadOnly("SELECT 1 /*M! ; DELETE FROM t */"));

    /// <summary>
    /// Tests RejectsANestedComment.
    /// </summary>
    private static void RejectsANestedComment() => Check(!ReadOnly("SELECT 1 /* /* */ ; DELETE FROM t"));

    /// <summary>
    /// Tests RejectsAnOpenBracket.
    /// </summary>
    private static void RejectsAnOpenBracket() => Check(!ReadOnly("SELECT [a"));

    /// <summary>
    /// Tests RejectsAQuoteInABracket.
    /// </summary>
    private static void RejectsAQuoteInABracket() => Check(!ReadOnly("SELECT [a'] ; DELETE FROM t; SELECT [b']"));

    /// <summary>
    /// Tests RejectsALineCommentInABracket.
    /// </summary>
    private static void RejectsALineCommentInABracket() => Check(!ReadOnly("SELECT [a--b]"));

    /// <summary>
    /// Tests RejectsABlockCommentInABracket.
    /// </summary>
    private static void RejectsABlockCommentInABracket() => Check(!ReadOnly("SELECT [a/*b]"));

    /// <summary>
    /// Tests RejectsADoubledBracket.
    /// </summary>
    private static void RejectsADoubledBracket() => Check(!ReadOnly("SELECT [a]]'] ; DELETE FROM t; SELECT [']"));

    /// <summary>
    /// Tests RejectsAHashSign.
    /// </summary>
    private static void RejectsAHashSign() => Check(!ReadOnly("SELECT 1 # '\n; DELETE FROM t; SELECT '"));

    /// <summary>
    /// Tests RejectsADollarSign.
    /// </summary>
    private static void RejectsADollarSign() => Check(!ReadOnly("SELECT $$'$$; DELETE FROM t; SELECT $$'$$"));

    /// <summary>
    /// Tests RejectsALoneCarriageReturn.
    /// </summary>
    private static void RejectsALoneCarriageReturn() => Check(!ReadOnly("SELECT 1\rFROM t"));

    /// <summary>
    /// Tests RejectsAlternativeQuoting.
    /// </summary>
    private static void RejectsAlternativeQuoting() => Check(!ReadOnly("SELECT q'[x]' FROM dual"));

    /// <summary>
    /// Tests RejectsNationalAlternativeQuoting.
    /// </summary>
    private static void RejectsNationalAlternativeQuoting() => Check(!ReadOnly("SELECT nq'[x]' FROM dual"));

    #endregion
}
