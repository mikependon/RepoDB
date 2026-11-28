namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests how returned values are written.
/// </summary>
internal static class NotebookScriptValueFormatterTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        WritesNothingForNull();
        WritesTheShapeOfADataTable();
        UsesTheInvariantCulture();
        WritesAStringAsItIs();
        UsesToStringForOtherValues();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests WritesNothingForNull.
    /// </summary>
    private static void WritesNothingForNull()
    {
        // Act
        var text = NotebookScriptValueFormatter.Format(null);

        // Assert
        Check(text is null);
    }

    /// <summary>
    /// Tests WritesTheShapeOfADataTable.
    /// </summary>
    private static void WritesTheShapeOfADataTable()
    {
        // Act
        var text = NotebookScriptValueFormatter.Format(SampleTables.Numbers());

        // Assert
        Check(text == "DataTable 'Numbers': 1 columns, 3 rows");
    }

    /// <summary>
    /// Tests UsesTheInvariantCulture.
    /// </summary>
    private static void UsesTheInvariantCulture()
    {
        // Setup
        using var culture = new CultureScope();

        // Act
        var text = NotebookScriptValueFormatter.Format(1.5);

        // Assert
        Check(text == "1.5");
    }

    /// <summary>
    /// Tests WritesAStringAsItIs.
    /// </summary>
    private static void WritesAStringAsItIs()
    {
        // Act
        var text = NotebookScriptValueFormatter.Format("Alice");

        // Assert
        Check(text == "Alice");
    }

    /// <summary>
    /// Tests UsesToStringForOtherValues.
    /// </summary>
    private static void UsesToStringForOtherValues()
    {
        // Act
        var text = NotebookScriptValueFormatter.Format(true);

        // Assert
        Check(text == "True");
    }

    #endregion
}
