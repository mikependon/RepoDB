namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the database access of the scripts the executor runs.
/// </summary>
internal static class NotebookCSharpExecutorGuardTests
{
    #region Private Variables

    private const string UsesTheDatabase = """Notebook.Context.Set("Ran", true); System.Data.IDbConnection connection = null; return 1;""";

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunRefusalsAsync();
        await RunAllowanceAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of refused scripts.
    /// </summary>
    private static async Task RunRefusalsAsync()
    {
        await RefusesTheDatabaseByDefault();
        await RunsNothingOfARefusedScript();
        await SaysWhyFirst();
        await WarnsForEachUse();
        await ReportsTheGuardBeforeCompileErrors();
        await UsesTheTextsOfTheHost();
        await CannotEscapeWithAnotherExecutor();
    }

    /// <summary>
    /// Runs the scenarios of allowing.
    /// </summary>
    private static async Task RunAllowanceAsync()
    {
        await RunsWhenTheHostAllows();
        await AsksBeforeEveryRun();
        await DoesNotAskForARefusedScript();
    }

    /// <summary>
    /// Runs a script that has no database access.
    /// </summary>
    /// <param name="code">The script.</param>
    /// <returns>The outcome.</returns>
    private static Task<NotebookScriptResult> Run(string code) => ScriptRuns.Run(new NotebookCSharpExecutor(),
        code);

    /// <summary>
    /// Tests RefusesTheDatabaseByDefault.
    /// </summary>
    private static async Task RefusesTheDatabaseByDefault()
    {
        // Act
        var result = await Run(UsesTheDatabase);

        // Assert
        Check(!result.Succeeded && result.Error == NotebookScriptingMessages.Default.DatabaseAccessNotAllowed());
    }

    /// <summary>
    /// Tests RunsNothingOfARefusedScript.
    /// </summary>
    private static async Task RunsNothingOfARefusedScript()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act
        await ScriptRuns.Run(executor,
            UsesTheDatabase);

        // Assert
        Check(!executor.Context.Has("Ran"));
    }

    /// <summary>
    /// Tests SaysWhyFirst.
    /// </summary>
    private static async Task SaysWhyFirst()
    {
        // Act
        var result = await Run(UsesTheDatabase);

        // Assert
        Check(result.Logs[0].Level == NotebookLogLevel.Error && result.Logs[0].Message == result.Error);
    }

    /// <summary>
    /// Tests WarnsForEachUse.
    /// </summary>
    private static async Task WarnsForEachUse()
    {
        // Act
        var result = await Run(UsesTheDatabase);

        // Assert
        Check(result.Logs[1].Level == NotebookLogLevel.Warning && result.Logs[1].Message == "IDbConnection is a database connection or command (line 1)");
    }

    /// <summary>
    /// Tests ReportsTheGuardBeforeCompileErrors.
    /// </summary>
    private static async Task ReportsTheGuardBeforeCompileErrors()
    {
        // Act
        var result = await Run(UsesTheDatabase + " var broken = ;");

        // Assert
        Check(result.Error == NotebookScriptingMessages.Default.DatabaseAccessNotAllowed());
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHost.
    /// </summary>
    private static async Task UsesTheTextsOfTheHost()
    {
        // Act
        var result = await ScriptRuns.Run(new NotebookCSharpExecutor(messages: new MarkedMessages()),
            "System.Activator.CreateInstance<int>();");

        // Assert
        Check(result.Error == "no database" && result.Logs[1].Message == "blocked System.Activator (line 1)");
    }

    /// <summary>
    /// Tests CannotEscapeWithAnotherExecutor.
    /// </summary>
    private static async Task CannotEscapeWithAnotherExecutor()
    {
        // Act
        var result = await Run("var other = new RepoDb.Science.NotebookCSharpExecutor(() => true);");

        // Assert
        Check(!result.Succeeded);
    }

    /// <summary>
    /// Tests RunsWhenTheHostAllows.
    /// </summary>
    private static async Task RunsWhenTheHostAllows()
    {
        // Act
        var result = await ScriptRuns.Run(new NotebookCSharpExecutor(() => true),
            UsesTheDatabase);

        // Assert
        Check(result.Succeeded && result.ReturnValue == "1");
    }

    /// <summary>
    /// Tests AsksBeforeEveryRun.
    /// </summary>
    private static async Task AsksBeforeEveryRun()
    {
        // Setup
        var allow = false;
        var executor = new NotebookCSharpExecutor(() => allow);
        await ScriptRuns.Run(executor,
            UsesTheDatabase);

        // Act
        allow = true;
        var result = await ScriptRuns.Run(executor,
            UsesTheDatabase);

        // Assert
        Check(result.Succeeded);
    }

    /// <summary>
    /// Tests DoesNotAskForARefusedScript.
    /// </summary>
    private static async Task DoesNotAskForARefusedScript()
    {
        // Setup
        var asked = 0;
        var executor = new NotebookCSharpExecutor(() =>
        {
            asked++;
            return true;
        });

        // Act
        await ScriptRuns.Run(executor,
            "return 1;",
            "var invalid = 1;");

        // Assert
        Check(asked == 0);
    }

    #endregion
}
