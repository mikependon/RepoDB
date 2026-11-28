namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the C# executor with scripts that fail, are refused or are cancelled.
/// </summary>
internal static class NotebookCSharpExecutorFailureTests
{
    #region Private Variables

    /// <summary>
    /// A script whose table holds a value that fails to write its text.
    /// </summary>
    private const string BadTable = """
        class Bad { public override string ToString() => throw new InvalidOperationException("no text"); }
        var table = new DataTable("T");
        table.Columns.Add("Value", typeof(object));
        table.Rows.Add(new Bad());

        """;

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunRefusalsAsync();
        await RunCompileErrorsAsync();
        await RunErrorsAsync();
        await RunCancellationsAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of scripts that do not run.
    /// </summary>
    private static async Task RunRefusalsAsync()
    {
        await RejectsANullRequest();
        await IsUnavailableWithoutDynamicCode();
        await RunsNothingWithoutDynamicCode();
        await RefusesInvalidImports();
        await LogsInvalidImports();
        await UsesTheTextsOfTheHostForInvalidImports();
        await RunsNothingWithInvalidImports();
    }

    /// <summary>
    /// Runs the scenarios of scripts that do not compile.
    /// </summary>
    private static async Task RunCompileErrorsAsync()
    {
        await FailsOnACompileError();
        await LogsEveryCompileError();
        await NumbersCompileErrorsByTheLinesOfTheScript();
        await TellsAnErrorOfTheImports();
        await UsesTheTextsOfTheHostForCompileErrors();
    }

    /// <summary>
    /// Runs the scenarios of scripts that throw.
    /// </summary>
    private static async Task RunErrorsAsync()
    {
        await FailsOnAnExceptionWithItsType();
        await LogsTheException();
        await UsesTheTextsOfTheHostForExceptions();
        await FailsOnACancellationTheScriptRaisedItself();
        await FailsWhenAReturnedCollectionThrows();
        await SucceedsWhenAValueOfAReturnedTableFailsToWrite();
        await FailsWhenAScriptWithSuchATableThrows();
        await FailsWhenTheMessageOfAnExceptionFails();
        await FailsWhenTheTextsOfTheHostFail();
        await PassesOnAnOutOfMemoryOfTheTexts();
        await FailsOnACompilationErrorWithoutDiagnostics();
        await KeepsTheOutputOfAFailedScript();
        await KeepsTheTablesOfAFailedScript();
        await KeepsTheChartOfAFailedScript();
        await GivesTheConsoleBackAfterAFailure();
    }

    /// <summary>
    /// Runs the scenarios of cancelled scripts.
    /// </summary>
    private static async Task RunCancellationsAsync()
    {
        await ThrowsForACanceledToken();
        await ThrowsTheErrorRaisedAfterTheCancellation();
        await GivesTheConsoleBackAfterACancellation();
        await RunsAgainAfterACancellation();
    }

    /// <summary>
    /// Runs a script with a new executor.
    /// </summary>
    /// <param name="code">The script.</param>
    /// <param name="imports">The imports.</param>
    /// <returns>The outcome.</returns>
    private static Task<NotebookScriptResult> Run(string code,
        string imports = "") => ScriptRuns.Run(new NotebookCSharpExecutor(),
            code,
            imports);

    /// <summary>
    /// Runs a script with an executor that has the texts of a host.
    /// </summary>
    /// <param name="code">The script.</param>
    /// <param name="imports">The imports.</param>
    /// <returns>The outcome.</returns>
    private static Task<NotebookScriptResult> RunMarked(string code,
        string imports = "") => ScriptRuns.Run(new NotebookCSharpExecutor(messages: new MarkedMessages()),
            code,
            imports);

    /// <summary>
    /// Runs a script that cancels its own token and then fails.
    /// </summary>
    /// <param name="executor">The executor.</param>
    /// <returns>The run.</returns>
    private static async Task RunCancelling(NotebookCSharpExecutor executor)
    {
        using var source = new CancellationTokenSource();
        executor.Context.Set("Source",
            source);
        await ScriptRuns.Run(executor,
            """Notebook.Context.Get<System.Threading.CancellationTokenSource>("Source").Cancel(); throw new InvalidOperationException("late");""",
            cancellationToken: source.Token);
    }

    /// <summary>
    /// Tests RejectsANullRequest.
    /// </summary>
    private static async Task RejectsANullRequest()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act/Assert
        await ThrowsAsync<ArgumentNullException>(() => executor.ExecuteAsync(null!));
    }

    /// <summary>
    /// Tests IsUnavailableWithoutDynamicCode.
    /// </summary>
    private static async Task IsUnavailableWithoutDynamicCode()
    {
        // Act
        var result = await ScriptRuns.Run(new NotebookCSharpExecutor(false),
            "return 1;");

        // Assert
        Check(result.IsUnavailable && !result.Succeeded && result.Error is null);
    }

    /// <summary>
    /// Tests RunsNothingWithoutDynamicCode.
    /// </summary>
    private static async Task RunsNothingWithoutDynamicCode()
    {
        // Setup
        var executor = new NotebookCSharpExecutor(false);

        // Act
        await ScriptRuns.Run(executor,
            """Notebook.Context.Set("Ran", true);""");

        // Assert
        Check(!executor.Context.Has("Ran"));
    }

    /// <summary>
    /// Tests RefusesInvalidImports.
    /// </summary>
    private static async Task RefusesInvalidImports()
    {
        // Setup
        const string imports = "var x = 1;";

        // Act
        var result = await Run("return 1;",
            imports);

        // Assert
        Check(!result.Succeeded && result.Error == "The imports have problems: " + NotebookImportValidator.Check(imports).Errors[0].Text);
    }

    /// <summary>
    /// Tests LogsInvalidImports.
    /// </summary>
    private static async Task LogsInvalidImports()
    {
        // Act
        var result = await Run("return 1;",
            "var x = 1;");

        // Assert
        Check(result.Logs
            .Single().Message == result.Error);
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHostForInvalidImports.
    /// </summary>
    private static async Task UsesTheTextsOfTheHostForInvalidImports()
    {
        // Act
        var result = await RunMarked("return 1;",
            "var x = 1;");

        // Assert
        Check(result.Error == "bad imports");
    }

    /// <summary>
    /// Tests RunsNothingWithInvalidImports.
    /// </summary>
    private static async Task RunsNothingWithInvalidImports()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act
        await ScriptRuns.Run(executor,
            "return 1;",
            """Notebook.Context.Set("Ran", true);""");

        // Assert
        Check(!executor.Context.Has("Ran"));
    }

    /// <summary>
    /// Tests FailsOnACompileError.
    /// </summary>
    private static async Task FailsOnACompileError()
    {
        // Act
        var result = await Run("var broken = ;");

        // Assert
        Check(!result.Succeeded && result.Error!.StartsWith("Line 1: CS1525: ",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests LogsEveryCompileError.
    /// </summary>
    private static async Task LogsEveryCompileError()
    {
        // Act
        var result = await Run("Missing();\nAbsent();");

        // Assert
        Check(result.Logs.Count == 2 && result.Logs
            .All(entry => entry.Level == NotebookLogLevel.Error));
    }

    /// <summary>
    /// Tests NumbersCompileErrorsByTheLinesOfTheScript.
    /// </summary>
    private static async Task NumbersCompileErrorsByTheLinesOfTheScript()
    {
        // Act
        var result = await Run("var broken = ;",
            "using System.Text;\nusing System.Globalization;");

        // Assert
        Check(result.Error!.StartsWith("Line 1: ",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TellsAnErrorOfTheImports.
    /// </summary>
    private static async Task TellsAnErrorOfTheImports()
    {
        // Act
        var result = await Run("return 1;",
            "using Missing.Space;");

        // Assert
        Check(result.Error!.StartsWith("Imports, line 1: CS0246: ",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHostForCompileErrors.
    /// </summary>
    private static async Task UsesTheTextsOfTheHostForCompileErrors()
    {
        // Act
        var result = await RunMarked("var broken = ;");

        // Assert
        Check(result.Error == "code 1 CS1525");
    }

    /// <summary>
    /// Tests FailsOnAnExceptionWithItsType.
    /// </summary>
    private static async Task FailsOnAnExceptionWithItsType()
    {
        // Act
        var result = await Run("""throw new InvalidOperationException("boom");""");

        // Assert
        Check(!result.Succeeded && result.Error == "InvalidOperationException: boom");
    }

    /// <summary>
    /// Tests LogsTheException.
    /// </summary>
    private static async Task LogsTheException()
    {
        // Act
        var result = await Run("""throw new InvalidOperationException("boom");""");

        // Assert
        Check(result.Logs
            .Single().Message == result.Error);
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHostForExceptions.
    /// </summary>
    private static async Task UsesTheTextsOfTheHostForExceptions()
    {
        // Act
        var result = await RunMarked("""throw new InvalidOperationException("boom");""");

        // Assert
        Check(result.Error == "threw boom");
    }

    /// <summary>
    /// Tests FailsOnACancellationTheScriptRaisedItself.
    /// </summary>
    private static async Task FailsOnACancellationTheScriptRaisedItself()
    {
        // Act
        var result = await Run("""throw new OperationCanceledException("timed out");""");

        // Assert
        Check(!result.Succeeded && result.Error == "OperationCanceledException: timed out");
    }

    /// <summary>
    /// Tests FailsWhenAReturnedCollectionThrows.
    /// </summary>
    private static async Task FailsWhenAReturnedCollectionThrows()
    {
        // Act
        var result = await Run("""return new[] { 0 }.Select(value => 1 / value);""");

        // Assert
        Check(!result.Succeeded && result.Error!.StartsWith("DivideByZeroException: ",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests KeepsTheOutputOfAFailedScript.
    /// </summary>
    private static async Task KeepsTheOutputOfAFailedScript()
    {
        // Act
        var result = await Run("""Console.Write("before"); throw new Exception();""");

        // Assert
        Check(result.Output == "before");
    }

    /// <summary>
    /// Tests KeepsTheTablesOfAFailedScript.
    /// </summary>
    private static async Task KeepsTheTablesOfAFailedScript()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Print(table); throw new Exception();");

        // Assert
        Check(result.Tables.Count == 1);
    }

    /// <summary>
    /// Tests KeepsTheChartOfAFailedScript.
    /// </summary>
    private static async Task KeepsTheChartOfAFailedScript()
    {
        // Act
        var result = await Run(ScriptRuns.TwoRowTable + "Notebook.Chart.FromDataTable(table); throw new Exception();");

        // Assert
        Check(result.Chart is not null);
    }

    /// <summary>
    /// Tests GivesTheConsoleBackAfterAFailure.
    /// </summary>
    private static async Task GivesTheConsoleBackAfterAFailure()
    {
        // Setup
        var before = Console.Out;

        // Act
        await Run("throw new Exception();");

        // Assert
        Check(ReferenceEquals(Console.Out,
            before));
    }

    /// <summary>
    /// Tests ThrowsForACanceledToken.
    /// </summary>
    private static async Task ThrowsForACanceledToken()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act/Assert
        await ThrowsAsync<OperationCanceledException>(() => ScriptRuns.Run(executor,
            "return 1;",
            cancellationToken: new CancellationToken(true)));
    }

    /// <summary>
    /// Tests ThrowsTheErrorRaisedAfterTheCancellation.
    /// </summary>
    private static async Task ThrowsTheErrorRaisedAfterTheCancellation()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act
        var error = await ThrowsAsync<InvalidOperationException>(() => RunCancelling(executor));

        // Assert
        Check(error.Message == "late");
    }

    /// <summary>
    /// Tests GivesTheConsoleBackAfterACancellation.
    /// </summary>
    private static async Task GivesTheConsoleBackAfterACancellation()
    {
        // Setup
        var before = Console.Out;

        // Act
        await ThrowsAsync<InvalidOperationException>(() => RunCancelling(new NotebookCSharpExecutor()));

        // Assert
        Check(ReferenceEquals(Console.Out,
            before));
    }

    /// <summary>
    /// Tests RunsAgainAfterACancellation.
    /// </summary>
    private static async Task RunsAgainAfterACancellation()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();
        await ThrowsAsync<InvalidOperationException>(() => RunCancelling(executor));

        // Act
        var result = await ScriptRuns.Run(executor,
            "return 1;");

        // Assert
        Check(result.Succeeded);
    }

    /// <summary>
    /// Tests SucceedsWhenAValueOfAReturnedTableFailsToWrite.
    /// </summary>
    private static async Task SucceedsWhenAValueOfAReturnedTableFailsToWrite()
    {
        // Act
        var result = await Run(BadTable + "return table;");

        // Assert
        Check(result.Succeeded && result.Tables
            .Single().Rows[0][0] == "<ToString failed: no text>");
    }

    /// <summary>
    /// Tests FailsWhenAScriptWithSuchATableThrows.
    /// </summary>
    private static async Task FailsWhenAScriptWithSuchATableThrows()
    {
        // Act
        var result = await Run(BadTable + "Print(table); Notebook.Chart.FromDataTable(table); throw new Exception(\"late\");");

        // Assert
        Check(result.Error == "Exception: late" && result.Chart is not null);
    }

    /// <summary>
    /// Tests FailsWhenTheMessageOfAnExceptionFails.
    /// </summary>
    private static async Task FailsWhenTheMessageOfAnExceptionFails()
    {
        // Act
        var result = await Run("""
            class Bad : Exception { public override string Message => throw new InvalidOperationException(); }
            throw new Bad();
            """);

        // Assert
        Check(!result.Succeeded && result.Error == "Bad: <Message failed>");
    }

    /// <summary>
    /// Tests FailsWhenTheTextsOfTheHostFail.
    /// </summary>
    private static async Task FailsWhenTheTextsOfTheHostFail()
    {
        // Setup
        var executor = new NotebookCSharpExecutor(messages: new ThrowingMessages(new InvalidOperationException()));

        // Act
        var result = await ScriptRuns.Run(executor,
            """throw new ArgumentException("boom");""");

        // Assert
        Check(result.Error == "ArgumentException: boom");
    }

    /// <summary>
    /// Tests PassesOnAnOutOfMemoryOfTheTexts.
    /// </summary>
    private static async Task PassesOnAnOutOfMemoryOfTheTexts()
    {
        // Setup
        var executor = new NotebookCSharpExecutor(messages: new ThrowingMessages(new OutOfMemoryException()));

        // Act/Assert
        await ThrowsAsync<OutOfMemoryException>(() => ScriptRuns.Run(executor,
            "throw new Exception();"));
    }

    /// <summary>
    /// Tests FailsOnACompilationErrorWithoutDiagnostics.
    /// </summary>
    private static async Task FailsOnACompilationErrorWithoutDiagnostics()
    {
        // Setup
        var executor = new NotebookCSharpExecutor(allowDatabaseAccess: () => true);

        // Act
        var result = await ScriptRuns.Run(executor,
            """throw new Microsoft.CodeAnalysis.Scripting.CompilationErrorException("odd", System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic>.Empty);""");

        // Assert
        Check(result.Error == "CompilationErrorException: odd");
    }

    #endregion
}
