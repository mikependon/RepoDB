namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the check of a script that has no database access.
/// </summary>
internal static class NotebookScriptGuardTests
{
    #region Private Variables

    private const string SqlClient = "using Microsoft.Data.Sqlite;";
    private const string Uses = "RepoDb.Science.UnitTests.Helpers.DatabaseUses";

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunArguments();
        RunAllowed();
        RunDatabaseUses();
        RunReachingByName();
        RunTexts();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the arguments.
    /// </summary>
    private static void RunArguments()
    {
        ConstructorRejectsNullMessages();
        CheckRejectsNullCompilation();
        CheckStopsForACanceledToken();
    }

    /// <summary>
    /// Runs the scenarios of what is allowed.
    /// </summary>
    private static void RunAllowed()
    {
        AllowsPlainCode();
        AllowsDataTables();
        AllowsTheNotebook();
        AllowsAnUnusedDatabaseImport();
        AllowsTheTypeOfAValue();
        AllowsAMethodNamedAsTheTypeLookup();
    }

    /// <summary>
    /// Runs the scenarios of database uses.
    /// </summary>
    private static void RunDatabaseUses()
    {
        RefusesAConnection();
        RefusesARenamedConnection();
        RefusesAConnectionInterface();
        RefusesAnArrayOfConnections();
        RefusesAMethodThatTakesAConnection();
        RefusesAMethodThatReturnsAConnection();
        RefusesAPropertyThatHoldsAConnection();
        RefusesAFieldThatHoldsAConnection();
    }

    /// <summary>
    /// Runs the scenarios of the ways to reach what is not allowed.
    /// </summary>
    private static void RunReachingByName()
    {
        RefusesReflection();
        RefusesActivator();
        RefusesProcesses();
        RefusesNativeCode();
        RefusesTheTypeLookup();
        RefusesDynamic();
        RefusesAReferenceDirective();
        RefusesALoadDirective();
        RefusesTheExecutor();
        RefusesTheScriptFactory();
    }

    /// <summary>
    /// Runs the scenarios of the texts.
    /// </summary>
    private static void RunTexts()
    {
        TellsTheLineOfTheCode();
        CountsAUseInTheImportsAsTheFirstLine();
        TellsTheSameUseOnce();
        UsesTheTextsOfTheHost();
    }

    /// <summary>
    /// Checks a script.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="imports">The imports.</param>
    /// <returns>The problems in one text.</returns>
    private static string Problems(string code,
        string imports = "") => string.Join('|',
        new NotebookScriptGuard(NotebookScriptingMessages.Default)
            .Check(Compilations.Of(NotebookScriptFactory.PrefixOf(imports) + code),
                NotebookScriptFactory.ImportLinesOf(imports)));

    /// <summary>
    /// Tests ConstructorRejectsNullMessages.
    /// </summary>
    private static void ConstructorRejectsNullMessages()
    {
        // Act
        Action operation = () => _ = new NotebookScriptGuard(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests CheckRejectsNullCompilation.
    /// </summary>
    private static void CheckRejectsNullCompilation()
    {
        // Setup
        var guard = new NotebookScriptGuard(NotebookScriptingMessages.Default);

        // Act/Assert
        Throws<ArgumentNullException>(() => guard.Check(null!,
            0));
    }

    /// <summary>
    /// Tests CheckStopsForACanceledToken.
    /// </summary>
    private static void CheckStopsForACanceledToken()
    {
        // Setup
        var guard = new NotebookScriptGuard(NotebookScriptingMessages.Default);
        var compilation = Compilations.Of("var total = 1;");

        // Act/Assert
        Throws<OperationCanceledException>(() => guard.Check(compilation,
            0,
            new CancellationToken(true)));
    }

    /// <summary>
    /// Tests AllowsPlainCode.
    /// </summary>
    private static void AllowsPlainCode()
    {
        // Act
        var problems = Problems("return 1 + 1;");

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests AllowsDataTables.
    /// </summary>
    private static void AllowsDataTables()
    {
        // Act
        var problems = Problems("var table = new DataTable(\"T\"); return table.TableName;");

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests AllowsTheNotebook.
    /// </summary>
    private static void AllowsTheNotebook()
    {
        // Act
        var problems = Problems("var table = Notebook.Context.FromSqlResult(\"S\")[0]; Notebook.Chart.FromDataTable(table); Print(table);");

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests AllowsAnUnusedDatabaseImport.
    /// </summary>
    private static void AllowsAnUnusedDatabaseImport()
    {
        // Act
        var problems = Problems("return 1;",
            SqlClient);

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests AllowsTheTypeOfAValue.
    /// </summary>
    private static void AllowsTheTypeOfAValue()
    {
        // Act
        var problems = Problems("return 1.GetType().Name;");

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests AllowsAMethodNamedAsTheTypeLookup.
    /// </summary>
    private static void AllowsAMethodNamedAsTheTypeLookup()
    {
        // Act
        var problems = Problems($"return {Uses}.GetType(\"x\");");

        // Assert
        Check(problems.Length == 0);
    }

    /// <summary>
    /// Tests RefusesAConnection.
    /// </summary>
    private static void RefusesAConnection()
    {
        // Act
        var problems = Problems("var connection = new SqliteConnection(\"x\");",
            SqlClient);

        // Assert
        Check(problems.Contains("Microsoft.Data.Sqlite.SqliteConnection is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesARenamedConnection.
    /// </summary>
    private static void RefusesARenamedConnection()
    {
        // Act
        var problems = Problems("var connection = new Db(\"x\");",
            "using Db = Microsoft.Data.Sqlite.SqliteConnection;");

        // Assert
        Check(problems.Contains("SqliteConnection is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesAConnectionInterface.
    /// </summary>
    private static void RefusesAConnectionInterface()
    {
        // Act
        var problems = Problems("System.Data.IDbConnection connection = null; connection.Open();");

        // Assert
        Check(problems.Contains("IDbConnection is a database connection or command",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesAnArrayOfConnections.
    /// </summary>
    private static void RefusesAnArrayOfConnections()
    {
        // Act
        var problems = Problems($"var all = new[] {{ {Uses}.Create() }};");

        // Assert
        Check(problems.Contains("IDbConnection is a database connection or command",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesAMethodThatTakesAConnection.
    /// </summary>
    private static void RefusesAMethodThatTakesAConnection()
    {
        // Act
        var problems = Problems($"{Uses}.Use(null);");

        // Assert
        Check(problems == "Use works on a database connection (line 1)");
    }

    /// <summary>
    /// Tests RefusesAMethodThatReturnsAConnection.
    /// </summary>
    private static void RefusesAMethodThatReturnsAConnection()
    {
        // Act
        var problems = Problems($"{Uses}.Create();");

        // Assert
        Check(problems == "IDbConnection is a database connection or command (line 1)");
    }

    /// <summary>
    /// Tests RefusesAPropertyThatHoldsAConnection.
    /// </summary>
    private static void RefusesAPropertyThatHoldsAConnection()
    {
        // Act
        var problems = Problems($"return {Uses}.Property == null;");

        // Assert
        Check(problems == "IDbConnection is a database connection or command (line 1)");
    }

    /// <summary>
    /// Tests RefusesAFieldThatHoldsAConnection.
    /// </summary>
    private static void RefusesAFieldThatHoldsAConnection()
    {
        // Act
        var problems = Problems($"return {Uses}.Field == null;");

        // Assert
        Check(problems == "IDbConnection is a database connection or command (line 1)");
    }

    /// <summary>
    /// Tests RefusesReflection.
    /// </summary>
    private static void RefusesReflection()
    {
        // Act
        var problems = Problems("return typeof(string).GetMethods().Length;");

        // Assert
        Check(problems.Contains("System.Reflection.MethodInfo is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesActivator.
    /// </summary>
    private static void RefusesActivator()
    {
        // Act
        var problems = Problems("return Activator.CreateInstance(typeof(object));");

        // Assert
        Check(problems.Contains("System.Activator is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesProcesses.
    /// </summary>
    private static void RefusesProcesses()
    {
        // Act
        var problems = Problems("System.Diagnostics.Process.Start(\"x\");");

        // Assert
        Check(problems.Contains("System.Diagnostics.Process is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesNativeCode.
    /// </summary>
    private static void RefusesNativeCode()
    {
        // Act
        var problems = Problems("return System.Runtime.InteropServices.Marshal.SizeOf<int>();");

        // Assert
        Check(problems.Contains("System.Runtime.InteropServices.Marshal is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesTheTypeLookup.
    /// </summary>
    private static void RefusesTheTypeLookup()
    {
        // Act
        var problems = Problems("return Type.GetType(\"System.Object\");");

        // Assert
        Check(problems == "Type.GetType finds a type by its name (line 1)");
    }

    /// <summary>
    /// Tests RefusesDynamic.
    /// </summary>
    private static void RefusesDynamic()
    {
        // Act
        var problems = Problems("dynamic value = 1; return value;");

        // Assert
        Check(problems.Contains("dynamic binds at run time",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesAReferenceDirective.
    /// </summary>
    private static void RefusesAReferenceDirective()
    {
        // Act
        var problems = Problems("#r \"System.Data.dll\"\nreturn 1;");

        // Assert
        Check(problems == "#r and #load load code that cannot be checked (line 1)");
    }

    /// <summary>
    /// Tests RefusesALoadDirective.
    /// </summary>
    private static void RefusesALoadDirective()
    {
        // Act
        var problems = Problems("#load \"missing.csx\"\nreturn 1;");

        // Assert
        Check(problems == "#r and #load load code that cannot be checked (line 1)");
    }

    /// <summary>
    /// Tests RefusesTheExecutor.
    /// </summary>
    private static void RefusesTheExecutor()
    {
        // Act
        var problems = Problems("var executor = new RepoDb.Science.NotebookCSharpExecutor(() => true);");

        // Assert
        Check(problems.Contains("RepoDb.Science.NotebookCSharpExecutor is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests RefusesTheScriptFactory.
    /// </summary>
    private static void RefusesTheScriptFactory()
    {
        // Act
        var problems = Problems("var factory = new RepoDb.Science.NotebookScriptFactory();");

        // Assert
        Check(problems.Contains("RepoDb.Science.NotebookScriptFactory is not allowed",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Tests TellsTheLineOfTheCode.
    /// </summary>
    private static void TellsTheLineOfTheCode()
    {
        // Act
        var problems = Problems($"var first = 1;\n{Uses}.Use(null);",
            "using System.Text;\nusing System.Globalization;");

        // Assert
        Check(problems == "Use works on a database connection (line 2)");
    }

    /// <summary>
    /// Tests CountsAUseInTheImportsAsTheFirstLine.
    /// </summary>
    private static void CountsAUseInTheImportsAsTheFirstLine()
    {
        // Act
        var problems = Problems("return 1;",
            "using System.Text;\nusing static System.Activator;");

        // Assert
        Check(problems == "System.Activator is not allowed (line 1)");
    }

    /// <summary>
    /// Tests TellsTheSameUseOnce.
    /// </summary>
    private static void TellsTheSameUseOnce()
    {
        // Act
        var problems = Problems($"{Uses}.Use(null); {Uses}.Use(null);");

        // Assert
        Check(problems == "Use works on a database connection (line 1)");
    }

    /// <summary>
    /// Tests UsesTheTextsOfTheHost.
    /// </summary>
    private static void UsesTheTextsOfTheHost()
    {
        // Setup
        var guard = new NotebookScriptGuard(new MarkedMessages());

        // Act
        var problems = guard.Check(Compilations.Of("System.Activator.CreateInstance<int>();"),
            0);

        // Assert
        Check(problems[0] == "blocked System.Activator (line 1)");
    }

    #endregion
}
