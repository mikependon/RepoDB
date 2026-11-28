namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the names of types.
/// </summary>
internal static class NotebookScriptTypeNamesTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RejectsNull();
        ShortensATypeOfSystem();
        KeepsTheNamespaceOfAnotherType();
        AddsBracketsToAnArray();
        WritesTheArgumentOfAGenericType();
        WritesEveryArgumentOfAGenericType();
        WritesTheArgumentsOfAnArrayOfGenericTypes();
        WritesANestedTypeOfAGenericType();
        WritesATypeWithoutANamespace();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests RejectsNull.
    /// </summary>
    private static void RejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => NotebookScriptTypeNames.Of(null!));
    }

    /// <summary>
    /// Tests ShortensATypeOfSystem.
    /// </summary>
    private static void ShortensATypeOfSystem()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(int));

        // Assert
        Check(name == "Int32");
    }

    /// <summary>
    /// Tests KeepsTheNamespaceOfAnotherType.
    /// </summary>
    private static void KeepsTheNamespaceOfAnotherType()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(DataTable));

        // Assert
        Check(name == "System.Data.DataTable");
    }

    /// <summary>
    /// Tests AddsBracketsToAnArray.
    /// </summary>
    private static void AddsBracketsToAnArray()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(int[]));

        // Assert
        Check(name == "Int32[]");
    }

    /// <summary>
    /// Tests WritesTheArgumentOfAGenericType.
    /// </summary>
    private static void WritesTheArgumentOfAGenericType()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(List<int>));

        // Assert
        Check(name == "System.Collections.Generic.List<Int32>");
    }

    /// <summary>
    /// Tests WritesEveryArgumentOfAGenericType.
    /// </summary>
    private static void WritesEveryArgumentOfAGenericType()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(Dictionary<string, int>));

        // Assert
        Check(name == "System.Collections.Generic.Dictionary<String, Int32>");
    }

    /// <summary>
    /// Tests WritesTheArgumentsOfAnArrayOfGenericTypes.
    /// </summary>
    private static void WritesTheArgumentsOfAnArrayOfGenericTypes()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(List<string>[]));

        // Assert
        Check(name == "System.Collections.Generic.List<String>[]");
    }

    /// <summary>
    /// Tests WritesANestedTypeOfAGenericType.
    /// </summary>
    private static void WritesANestedTypeOfAGenericType()
    {
        // Act
        var name = NotebookScriptTypeNames.Of(typeof(Dictionary<string, int>.KeyCollection));

        // Assert
        Check(name == "System.Collections.Generic.KeyCollection");
    }

    /// <summary>
    /// Tests WritesATypeWithoutANamespace.
    /// </summary>
    private static void WritesATypeWithoutANamespace()
    {
        // Setup
        var value = new { Id = 1 };

        // Act
        var name = NotebookScriptTypeNames.Of(value.GetType());

        // Assert
        Check(name.EndsWith("<Int32>",
            StringComparison.Ordinal) && !name.Contains('.',
            StringComparison.Ordinal));
    }

    #endregion
}
