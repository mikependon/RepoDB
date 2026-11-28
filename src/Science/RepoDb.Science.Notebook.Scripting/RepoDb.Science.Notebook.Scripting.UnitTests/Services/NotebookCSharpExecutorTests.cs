namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the C# executor with scripts that succeed.
/// </summary>
internal static class NotebookCSharpExecutorTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        await RunOutcomesAsync();
        await RunLanguageAsync();
        await RunContextAsync();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the outcome.
    /// </summary>
    private static async Task RunOutcomesAsync()
    {
        ContextIsKeptByTheExecutor();
        KeepsTheFactoryOfTheHost();
        CreatesAFactoryWithoutOne();
        await SucceedsWithoutAnError();
        await ReturnsTheReturnedValue();
        await ReturnsTheValueWithTheInvariantCulture();
        await ReturnsNoValueForAStatement();
    }

    /// <summary>
    /// Runs the scenarios of what a script may use.
    /// </summary>
    private static async Task RunLanguageAsync()
    {
        await AppliesTheImports();
        await ImportsTheCommonNamespaces();
        await RunsAwaitedCode();
        await RunsWithTheAssembliesOfTheFactory();
    }

    /// <summary>
    /// Runs the scenarios of the context.
    /// </summary>
    private static async Task RunContextAsync()
    {
        await ReadsTheSqlResults();
        await KeepsValuesBetweenScripts();
        await ReadsATypedValue();
        await TellsAndRemovesValues();
        await DoesNotShareValuesBetweenExecutors();
        await ResetRemovesTheValues();
        await ResetRemovesTheSqlResults();
        await DoesNotLetAScriptClearTheContext();
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
    /// Tests ContextIsKeptByTheExecutor.
    /// </summary>
    private static void ContextIsKeptByTheExecutor()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();

        // Act
        var context = executor.Context;

        // Assert
        Check(ReferenceEquals(context,
            executor.Context));
    }

    /// <summary>
    /// Tests KeepsTheFactoryOfTheHost.
    /// </summary>
    private static void KeepsTheFactoryOfTheHost()
    {
        // Act
        var executor = new NotebookCSharpExecutor(scripts: Compilations.Factory);

        // Assert
        Check(ReferenceEquals(executor.Scripts,
            Compilations.Factory));
    }

    /// <summary>
    /// Tests CreatesAFactoryWithoutOne.
    /// </summary>
    private static void CreatesAFactoryWithoutOne()
    {
        // Act
        var executor = new NotebookCSharpExecutor();

        // Assert
        Check(executor.Scripts is not null);
    }

    /// <summary>
    /// Tests SucceedsWithoutAnError.
    /// </summary>
    private static async Task SucceedsWithoutAnError()
    {
        // Act
        var result = await Run("var total = 1;");

        // Assert
        Check(result.Succeeded && result.Error is null && !result.IsUnavailable);
    }

    /// <summary>
    /// Tests ReturnsTheReturnedValue.
    /// </summary>
    private static async Task ReturnsTheReturnedValue()
    {
        // Act
        var result = await Run("return 6 * 7;");

        // Assert
        Check(result.ReturnValue == "42");
    }

    /// <summary>
    /// Tests ReturnsTheValueWithTheInvariantCulture.
    /// </summary>
    private static async Task ReturnsTheValueWithTheInvariantCulture()
    {
        // Setup
        using var culture = new CultureScope();

        // Act
        var result = await Run("return 1.5;");

        // Assert
        Check(result.ReturnValue == "1.5");
    }

    /// <summary>
    /// Tests ReturnsNoValueForAStatement.
    /// </summary>
    private static async Task ReturnsNoValueForAStatement()
    {
        // Act
        var result = await Run("var total = 1;");

        // Assert
        Check(result.ReturnValue is null);
    }

    /// <summary>
    /// Tests AppliesTheImports.
    /// </summary>
    private static async Task AppliesTheImports()
    {
        // Act
        var result = await Run("""return new StringBuilder("built").ToString();""",
            "using System.Text;");

        // Assert
        Check(result.ReturnValue == "built");
    }

    /// <summary>
    /// Tests ImportsTheCommonNamespaces.
    /// </summary>
    private static async Task ImportsTheCommonNamespaces()
    {
        // Setup
        const string code = """var list = new List<int> { 1 }; var table = new DataTable(); var name = Path.GetFileName("a"); await Task.Yield(); return list.Sum();""";

        // Act
        var result = await Run(code);

        // Assert
        Check(result.ReturnValue == "1");
    }

    /// <summary>
    /// Tests RunsAwaitedCode.
    /// </summary>
    private static async Task RunsAwaitedCode()
    {
        // Act
        var result = await Run("return await Task.FromResult(5);");

        // Assert
        Check(result.ReturnValue == "5");
    }

    /// <summary>
    /// Tests RunsWithTheAssembliesOfTheFactory.
    /// </summary>
    private static async Task RunsWithTheAssembliesOfTheFactory()
    {
        // Setup
        var factory = new NotebookScriptFactory(isReferenced: path => !path.EndsWith("RepoDb.Science.Notebook.dll",
            StringComparison.OrdinalIgnoreCase));

        // Act
        var result = await ScriptRuns.Run(new NotebookCSharpExecutor(scripts: factory),
            "return typeof(RepoDb.Science.NotebookDocument).Name;");

        // Assert
        Check(!result.Succeeded);
    }

    /// <summary>
    /// Tests ReadsTheSqlResults.
    /// </summary>
    private static async Task ReadsTheSqlResults()
    {
        // Setup
        var request = new NotebookScriptRequest("""return Notebook.Context.FromSqlResult("Section1")[0].Rows[0]["Orders"];""",
            string.Empty,
            SampleTables.Results("Section1",
                SampleTables.Customers()));

        // Act
        var result = await new NotebookCSharpExecutor()
            .ExecuteAsync(request);

        // Assert
        Check(result.ReturnValue == "12");
    }

    /// <summary>
    /// Tests KeepsValuesBetweenScripts.
    /// </summary>
    private static async Task KeepsValuesBetweenScripts()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();
        await ScriptRuns.Run(executor,
            """Notebook.Context.Set("Total", 19);""");

        // Act
        var result = await ScriptRuns.Run(executor,
            """return Notebook.Context.Get("Total");""");

        // Assert
        Check(result.ReturnValue == "19");
    }

    /// <summary>
    /// Tests ReadsATypedValue.
    /// </summary>
    private static async Task ReadsATypedValue()
    {
        // Act
        var result = await Run("""Notebook.Context.Set("Total", 19); return Notebook.Context.Get<int>("Total") + 1;""");

        // Assert
        Check(result.ReturnValue == "20");
    }

    /// <summary>
    /// Tests TellsAndRemovesValues.
    /// </summary>
    private static async Task TellsAndRemovesValues()
    {
        // Act
        var result = await Run("""Notebook.Context.Set("A", 1); var had = Notebook.Context.Has("A"); var names = Notebook.Context.Names.Count; Notebook.Context.Remove("A"); return $"{had} {names} {Notebook.Context.Has("A")}";""");

        // Assert
        Check(result.ReturnValue == "True 1 False");
    }

    /// <summary>
    /// Tests DoesNotShareValuesBetweenExecutors.
    /// </summary>
    private static async Task DoesNotShareValuesBetweenExecutors()
    {
        // Setup
        await Run("""Notebook.Context.Set("Total", 19);""");

        // Act
        var result = await Run("""return Notebook.Context.Get("Total");""");

        // Assert
        Check(!result.Succeeded);
    }

    /// <summary>
    /// Tests ResetRemovesTheValues.
    /// </summary>
    private static async Task ResetRemovesTheValues()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();
        await ScriptRuns.Run(executor,
            """Notebook.Context.Set("Total", 1);""");

        // Act
        executor.Reset();

        // Assert
        Check(executor.Context.Names.Count == 0);
    }

    /// <summary>
    /// Tests ResetRemovesTheSqlResults.
    /// </summary>
    private static async Task ResetRemovesTheSqlResults()
    {
        // Setup
        var executor = new NotebookCSharpExecutor();
        await executor.ExecuteAsync(new NotebookScriptRequest("return 1;",
            string.Empty,
            SampleTables.Results("Section1",
                SampleTables.Customers()),
            10));

        // Act
        executor.Reset();

        // Assert
        Throws<InvalidOperationException>(() => executor.Context.FromSqlResult("Section1"));
    }

    /// <summary>
    /// Tests DoesNotLetAScriptClearTheContext.
    /// </summary>
    private static async Task DoesNotLetAScriptClearTheContext()
    {
        // Act
        var result = await Run("Notebook.Context.Clear();");

        // Assert
        Check(!result.Succeeded && result.Error!.Contains("Clear",
            StringComparison.Ordinal));
    }

    #endregion
}
