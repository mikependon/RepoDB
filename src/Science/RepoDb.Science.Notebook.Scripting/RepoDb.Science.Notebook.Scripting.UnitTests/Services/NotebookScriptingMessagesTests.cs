namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the default texts of the problems of scripts.
/// </summary>
internal static class NotebookScriptingMessagesTests
{
    #region Private Variables

    private static readonly NotebookScriptingMessages Messages = new();

    #endregion

    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunGuardTexts();
        RunErrorTexts();
        RunContextTexts();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the texts of the guard.
    /// </summary>
    private static void RunGuardTexts()
    {
        DefaultIsShared();
        WritesTheRefusalOfDatabaseAccess();
        WritesTheDatabaseMethod();
        WritesTheDatabaseType();
        WritesTheDirective();
        WritesDynamic();
        WritesTheType();
        WritesTheTypeLookup();
        WritesTheLineOfAUse();
    }

    /// <summary>
    /// Runs the scenarios of the texts of the errors.
    /// </summary>
    private static void RunErrorTexts()
    {
        WritesACodeError();
        WritesAnImportsError();
        WritesInvalidImports();
        WritesARuntimeError();
        RuntimeErrorRejectsNull();
    }

    /// <summary>
    /// Runs the scenarios of the texts of the context.
    /// </summary>
    private static void RunContextTexts()
    {
        WritesAClosedReader();
        WritesAMissingSqlResult();
        WritesAMissingValue();
        WritesAWrongType();
        WritesAWrongTypeOfNull();
        WrongTypeRejectsNull();
    }

    /// <summary>
    /// Tests DefaultIsShared.
    /// </summary>
    private static void DefaultIsShared()
    {
        // Act
        var messages = NotebookScriptingMessages.Default;

        // Assert
        Check(ReferenceEquals(messages,
            NotebookScriptingMessages.Default));
    }

    /// <summary>
    /// Tests WritesTheRefusalOfDatabaseAccess.
    /// </summary>
    private static void WritesTheRefusalOfDatabaseAccess()
    {
        // Act
        var text = Messages.DatabaseAccessNotAllowed();

        // Assert
        Check(text == "C# scripts cannot access the database directly while database access is turned off.");
    }

    /// <summary>
    /// Tests WritesTheDatabaseMethod.
    /// </summary>
    private static void WritesTheDatabaseMethod()
    {
        // Act
        var text = Messages.ForbiddenDatabaseMethod("Query");

        // Assert
        Check(text == "Query works on a database connection");
    }

    /// <summary>
    /// Tests WritesTheDatabaseType.
    /// </summary>
    private static void WritesTheDatabaseType()
    {
        // Act
        var text = Messages.ForbiddenDatabaseType("IDbCommand");

        // Assert
        Check(text == "IDbCommand is a database connection or command");
    }

    /// <summary>
    /// Tests WritesTheDirective.
    /// </summary>
    private static void WritesTheDirective()
    {
        // Act
        var text = Messages.ForbiddenDirective();

        // Assert
        Check(text == "#r and #load load code that cannot be checked");
    }

    /// <summary>
    /// Tests WritesDynamic.
    /// </summary>
    private static void WritesDynamic()
    {
        // Act
        var text = Messages.ForbiddenDynamic();

        // Assert
        Check(text == "dynamic binds at run time, so it cannot be checked");
    }

    /// <summary>
    /// Tests WritesTheType.
    /// </summary>
    private static void WritesTheType()
    {
        // Act
        var text = Messages.ForbiddenType("System.AppDomain");

        // Assert
        Check(text == "System.AppDomain is not allowed");
    }

    /// <summary>
    /// Tests WritesTheTypeLookup.
    /// </summary>
    private static void WritesTheTypeLookup()
    {
        // Act
        var text = Messages.ForbiddenTypeLookup();

        // Assert
        Check(text == "Type.GetType finds a type by its name");
    }

    /// <summary>
    /// Tests WritesTheLineOfAUse.
    /// </summary>
    private static void WritesTheLineOfAUse()
    {
        // Act
        var text = Messages.ForbiddenUse("reason",
            3);

        // Assert
        Check(text == "reason (line 3)");
    }

    /// <summary>
    /// Tests WritesACodeError.
    /// </summary>
    private static void WritesACodeError()
    {
        // Act
        var text = Messages.CodeError(3,
            "CS1",
            "bad");

        // Assert
        Check(text == "Line 3: CS1: bad");
    }

    /// <summary>
    /// Tests WritesAnImportsError.
    /// </summary>
    private static void WritesAnImportsError()
    {
        // Act
        var text = Messages.ImportsError(1,
            "CS1",
            "bad");

        // Assert
        Check(text == "Imports, line 1: CS1: bad");
    }

    /// <summary>
    /// Tests WritesInvalidImports.
    /// </summary>
    private static void WritesInvalidImports()
    {
        // Act
        var text = Messages.InvalidImports("Line 1: no");

        // Assert
        Check(text == "The imports have problems: Line 1: no");
    }

    /// <summary>
    /// Tests WritesARuntimeError.
    /// </summary>
    private static void WritesARuntimeError()
    {
        // Act
        var text = Messages.RuntimeError(new InvalidOperationException("boom"));

        // Assert
        Check(text == "InvalidOperationException: boom");
    }

    /// <summary>
    /// Tests RuntimeErrorRejectsNull.
    /// </summary>
    private static void RuntimeErrorRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => Messages.RuntimeError(null!));
    }

    /// <summary>
    /// Tests WritesAClosedReader.
    /// </summary>
    private static void WritesAClosedReader()
    {
        // Act
        var text = Messages.DataReaderClosed();

        // Assert
        Check(text == "The data reader is closed.");
    }

    /// <summary>
    /// Tests WritesAMissingSqlResult.
    /// </summary>
    private static void WritesAMissingSqlResult()
    {
        // Act
        var text = Messages.SqlResultMissing("S");

        // Assert
        Check(text == "The SQL section 'S' has no result. Run it first, and check its name.");
    }

    /// <summary>
    /// Tests WritesAMissingValue.
    /// </summary>
    private static void WritesAMissingValue()
    {
        // Act
        var text = Messages.ValueMissing("V");

        // Assert
        Check(text == "The notebook context has no value named 'V'. Set it in a C# section that runs before this one.");
    }

    /// <summary>
    /// Tests WritesAWrongType.
    /// </summary>
    private static void WritesAWrongType()
    {
        // Act
        var text = Messages.ValueTypeMismatch("V",
            typeof(string),
            typeof(int));

        // Assert
        Check(text == "The value 'V' is a String, not a Int32.");
    }

    /// <summary>
    /// Tests WritesAWrongTypeOfNull.
    /// </summary>
    private static void WritesAWrongTypeOfNull()
    {
        // Act
        var text = Messages.ValueTypeMismatch("V",
            null,
            typeof(int));

        // Assert
        Check(text == "The value 'V' is a null, not a Int32.");
    }

    /// <summary>
    /// Tests WrongTypeRejectsNull.
    /// </summary>
    private static void WrongTypeRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => Messages.ValueTypeMismatch("V",
            null,
            null!));
    }

    #endregion
}
