namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the execution context, requests and script outcome.
/// </summary>
internal static class NotebookRequestModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ContextHasDefaultLimits();
        ContextHasNoExecutorsByDefault();
        ContextKeepsItsLimits();
        ContextKeepsItsServices();
        ContextRejectsZeroRowLimit();
        ContextRejectsZeroOutputLimit();
        SqlRequestKeepsItsValues();
        SqlRequestHasDefaults();
        SqlRequestRejectsBlankScript();
        SqlRequestRejectsZeroRowLimit();
        ScriptRequestKeepsItsTexts();
        ScriptRequestKeepsItsData();
        ScriptRequestRejectsNullCode();
        ScriptRequestRejectsNullImports();
        ScriptRequestRejectsNullResults();
        ScriptRequestRejectsZeroRowLimit();
        ScriptResultIsAnEmptyFailureByDefault();
        ScriptResultHasEmptyCollectionsByDefault();
        ScriptResultKeepsItsState();
        ScriptResultKeepsItsData();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates empty SQL results.
    /// </summary>
    /// <returns>The results.</returns>
    private static Dictionary<string, IReadOnlyList<NotebookTable>> NoResults() => new(StringComparer.Ordinal);

    /// <summary>
    /// Tests ContextHasDefaultLimits.
    /// </summary>
    private static void ContextHasDefaultLimits()
    {
        // Act
        var context = new NotebookExecutionContext();

        // Assert
        Check(context.RowLimit == 1000 && context.OutputLimit == 20);
    }

    /// <summary>
    /// Tests ContextHasNoExecutorsByDefault.
    /// </summary>
    private static void ContextHasNoExecutorsByDefault()
    {
        // Act
        var context = new NotebookExecutionContext();

        // Assert
        Check(context.SqlExecutor is null && context.CSharpExecutor is null && context.Guard is null);
    }

    /// <summary>
    /// Tests ContextKeepsItsLimits.
    /// </summary>
    private static void ContextKeepsItsLimits()
    {
        // Act
        var context = new NotebookExecutionContext { RowLimit = 5, OutputLimit = 6 };

        // Assert
        Check(context.RowLimit == 5 && context.OutputLimit == 6);
    }

    /// <summary>
    /// Tests ContextKeepsItsServices.
    /// </summary>
    private static void ContextKeepsItsServices()
    {
        // Setup
        var sql = new FakeSqlExecutor();
        var csharp = new FakeCSharpExecutor(new NotebookScriptResult());
        var guard = new FakeGuard(null);

        // Act
        var context = new NotebookExecutionContext { SqlExecutor = sql, CSharpExecutor = csharp, Guard = guard };

        // Assert
        Check(context.SqlExecutor == sql && context.CSharpExecutor == csharp && context.Guard == guard);
    }

    /// <summary>
    /// Tests ContextRejectsZeroRowLimit.
    /// </summary>
    private static void ContextRejectsZeroRowLimit()
    {
        // Act
        Action operation = () => _ = new NotebookExecutionContext { RowLimit = 0 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ContextRejectsZeroOutputLimit.
    /// </summary>
    private static void ContextRejectsZeroOutputLimit()
    {
        // Act
        Action operation = () => _ = new NotebookExecutionContext { OutputLimit = 0 };

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests SqlRequestKeepsItsValues.
    /// </summary>
    private static void SqlRequestKeepsItsValues()
    {
        // Setup
        var id = Guid.NewGuid();

        // Act
        var request = new NotebookSqlRequest("SELECT 1",
            id,
            7);

        // Assert
        Check(request.Script == "SELECT 1" && request.ConnectionId == id && request.RowLimit == 7);
    }

    /// <summary>
    /// Tests SqlRequestHasDefaults.
    /// </summary>
    private static void SqlRequestHasDefaults()
    {
        // Act
        var request = new NotebookSqlRequest("SELECT 1");

        // Assert
        Check(request.ConnectionId is null && request.RowLimit == NotebookExecutionContext.DefaultRowLimit);
    }

    /// <summary>
    /// Tests SqlRequestRejectsBlankScript.
    /// </summary>
    private static void SqlRequestRejectsBlankScript()
    {
        // Act
        Action operation = () => _ = new NotebookSqlRequest(" ");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests SqlRequestRejectsZeroRowLimit.
    /// </summary>
    private static void SqlRequestRejectsZeroRowLimit()
    {
        // Act
        Action operation = () => _ = new NotebookSqlRequest("SELECT 1",
            null,
            0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ScriptRequestKeepsItsTexts.
    /// </summary>
    private static void ScriptRequestKeepsItsTexts()
    {
        // Act
        var request = new NotebookScriptRequest("var x = 1;",
            "using System;",
            NoResults());

        // Assert
        Check(request.Code == "var x = 1;" && request.Imports == "using System;");
    }

    /// <summary>
    /// Tests ScriptRequestKeepsItsData.
    /// </summary>
    private static void ScriptRequestKeepsItsData()
    {
        // Setup
        var results = NoResults();

        // Act
        var request = new NotebookScriptRequest("x",
            "",
            results,
            3);

        // Assert
        Check(ReferenceEquals(request.SqlResults,
            results) && request.RowLimit == 3);
    }

    /// <summary>
    /// Tests ScriptRequestRejectsNullCode.
    /// </summary>
    private static void ScriptRequestRejectsNullCode()
    {
        // Act
        Action operation = () => _ = new NotebookScriptRequest(null!,
            "",
            NoResults());

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ScriptRequestRejectsNullImports.
    /// </summary>
    private static void ScriptRequestRejectsNullImports()
    {
        // Act
        Action operation = () => _ = new NotebookScriptRequest("x",
            null!,
            NoResults());

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ScriptRequestRejectsNullResults.
    /// </summary>
    private static void ScriptRequestRejectsNullResults()
    {
        // Act
        Action operation = () => _ = new NotebookScriptRequest("x",
            "",
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ScriptRequestRejectsZeroRowLimit.
    /// </summary>
    private static void ScriptRequestRejectsZeroRowLimit()
    {
        // Act
        Action operation = () => _ = new NotebookScriptRequest("x",
            "",
            NoResults(),
            0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Tests ScriptResultIsAnEmptyFailureByDefault.
    /// </summary>
    private static void ScriptResultIsAnEmptyFailureByDefault()
    {
        // Act
        var result = new NotebookScriptResult();

        // Assert
        Check(!result.Succeeded && !result.IsUnavailable && result.Output.Length == 0);
        Check(result.Error is null && result.ReturnValue is null && result.Chart is null);
    }

    /// <summary>
    /// Tests ScriptResultHasEmptyCollectionsByDefault.
    /// </summary>
    private static void ScriptResultHasEmptyCollectionsByDefault()
    {
        // Act
        var result = new NotebookScriptResult();

        // Assert
        Check(result.Logs.Count == 0 && result.Tables.Count == 0);
    }

    /// <summary>
    /// Tests ScriptResultKeepsItsState.
    /// </summary>
    private static void ScriptResultKeepsItsState()
    {
        // Act
        var result = new NotebookScriptResult { Succeeded = true, IsUnavailable = true, Error = "e", Output = "o", ReturnValue = "r" };

        // Assert
        Check(result.Succeeded && result.IsUnavailable);
        Check(result.Error == "e" && result.Output == "o" && result.ReturnValue == "r");
    }

    /// <summary>
    /// Tests ScriptResultKeepsItsData.
    /// </summary>
    private static void ScriptResultKeepsItsData()
    {
        // Setup
        var table = SampleTables.Timed();

        // Act
        var result = new NotebookScriptResult { Chart = table, Tables = [table], Logs = [] };

        // Assert
        Check(result.Chart == table && result.Tables
            .Single() == table);
    }

    #endregion
}
