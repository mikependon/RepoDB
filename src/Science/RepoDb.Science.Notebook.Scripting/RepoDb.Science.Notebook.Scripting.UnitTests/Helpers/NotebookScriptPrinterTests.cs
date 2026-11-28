namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests how printed values are written.
/// </summary>
internal static class NotebookScriptPrinterTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunValues();
        RunTexts();
        RunTables();
        RunCollections();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of single values.
    /// </summary>
    private static void RunValues()
    {
        WritesNull();
        WritesTheTypeAndTheValue();
        WritesAStringAsAValue();
    }

    /// <summary>
    /// Runs the scenarios of the text of a value.
    /// </summary>
    private static void RunTexts()
    {
        UsesTheInvariantCulture();
        TextOfNullIsAWord();
        TextOfAValueWithoutTextIsEmpty();
        TextOfTellsAMessageThatFails();
        MessageOfPassesOnAnOutOfMemory();
        TellsWhyATextCannotBeWritten();
        LetsOutOfMemoryThrough();
    }

    /// <summary>
    /// Runs the scenarios of tables.
    /// </summary>
    private static void RunTables()
    {
        WritesTheShapeOfADataTable();
        WritesTheShapeOfADataSet();
    }

    /// <summary>
    /// Runs the scenarios of collections.
    /// </summary>
    private static void RunCollections()
    {
        WritesTheItemsOfACollection();
        WritesAnEmptyCollection();
        WritesNoMoreThanTheLimit();
        AddsNoDotsAtTheLimit();
        WritesTheTablesOfACollection();
        WritesNullItems();
        StopsReadingAfterTheLimit();
    }

    /// <summary>
    /// Tests WritesNull.
    /// </summary>
    private static void WritesNull()
    {
        // Act
        var text = NotebookScriptPrinter.Format(null);

        // Assert
        Check(text == "null");
    }

    /// <summary>
    /// Tests WritesTheTypeAndTheValue.
    /// </summary>
    private static void WritesTheTypeAndTheValue()
    {
        // Act
        var text = NotebookScriptPrinter.Format(42);

        // Assert
        Check(text == "Int32: 42");
    }

    /// <summary>
    /// Tests WritesAStringAsAValue.
    /// </summary>
    private static void WritesAStringAsAValue()
    {
        // Act
        var text = NotebookScriptPrinter.Format("Hello");

        // Assert
        Check(text == "String: Hello");
    }

    /// <summary>
    /// Tests UsesTheInvariantCulture.
    /// </summary>
    private static void UsesTheInvariantCulture()
    {
        // Setup
        using var culture = new CultureScope();

        // Act
        var text = NotebookScriptPrinter.Format(1.5);

        // Assert
        Check(text == "Double: 1.5");
    }

    /// <summary>
    /// Tests TextOfNullIsAWord.
    /// </summary>
    private static void TextOfNullIsAWord()
    {
        // Act
        var text = NotebookScriptPrinter.TextOf(null);

        // Assert
        Check(text == "null");
    }

    /// <summary>
    /// Tests TextOfAValueWithoutTextIsEmpty.
    /// </summary>
    private static void TextOfAValueWithoutTextIsEmpty()
    {
        // Act
        var text = NotebookScriptPrinter.TextOf(new EmptyText());

        // Assert
        Check(text.Length == 0);
    }

    /// <summary>
    /// Tests TellsWhyATextCannotBeWritten.
    /// </summary>
    private static void TellsWhyATextCannotBeWritten()
    {
        // Act
        var text = NotebookScriptPrinter.TextOf(new FailingTexts(new InvalidOperationException("no text")));

        // Assert
        Check(text == "<ToString failed: no text>");
    }

    /// <summary>
    /// Tests LetsOutOfMemoryThrough.
    /// </summary>
    private static void LetsOutOfMemoryThrough()
    {
        // Setup
        var value = new FailingTexts(new OutOfMemoryException());

        // Act/Assert
        Throws<OutOfMemoryException>(() => NotebookScriptPrinter.TextOf(value));
    }

    /// <summary>
    /// Tests WritesTheShapeOfADataTable.
    /// </summary>
    private static void WritesTheShapeOfADataTable()
    {
        // Act
        var text = NotebookScriptPrinter.Format(SampleTables.Numbers());

        // Assert
        Check(text == "System.Data.DataTable: 'Numbers', 1 columns, 3 rows");
    }

    /// <summary>
    /// Tests WritesTheShapeOfADataSet.
    /// </summary>
    private static void WritesTheShapeOfADataSet()
    {
        // Setup
        var set = new DataSet("Set");
        set.Tables.Add(SampleTables.Numbers());

        // Act
        var text = NotebookScriptPrinter.Format(set);

        // Assert
        Check(text == "System.Data.DataSet: 'Set', 1 tables");
    }

    /// <summary>
    /// Tests WritesTheItemsOfACollection.
    /// </summary>
    private static void WritesTheItemsOfACollection()
    {
        // Act
        var text = NotebookScriptPrinter.Format(new List<int> { 1, 2, 3 });

        // Assert
        Check(text == "System.Collections.Generic.List<Int32>: [1, 2, 3]");
    }

    /// <summary>
    /// Tests WritesAnEmptyCollection.
    /// </summary>
    private static void WritesAnEmptyCollection()
    {
        // Act
        var text = NotebookScriptPrinter.Format(Array.Empty<int>());

        // Assert
        Check(text == "Int32[]: []");
    }

    /// <summary>
    /// Tests WritesNoMoreThanTheLimit.
    /// </summary>
    private static void WritesNoMoreThanTheLimit()
    {
        // Act
        var text = NotebookScriptPrinter.Format(Enumerable.Range(0,
            11)
            .ToArray());

        // Assert
        Check(text == "Int32[]: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, ...]");
    }

    /// <summary>
    /// Tests AddsNoDotsAtTheLimit.
    /// </summary>
    private static void AddsNoDotsAtTheLimit()
    {
        // Act
        var text = NotebookScriptPrinter.Format(Enumerable.Range(0,
            10)
            .ToArray());

        // Assert
        Check(text == "Int32[]: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]");
    }

    /// <summary>
    /// Tests WritesTheTablesOfACollection.
    /// </summary>
    private static void WritesTheTablesOfACollection()
    {
        // Act
        var text = NotebookScriptPrinter.Format(new[] { SampleTables.Numbers() });

        // Assert
        Check(text == "System.Data.DataTable[]: ['Numbers' (3 rows)]");
    }

    /// <summary>
    /// Tests WritesNullItems.
    /// </summary>
    private static void WritesNullItems()
    {
        // Act
        var text = NotebookScriptPrinter.Format(new string?[] { null, null });

        // Assert
        Check(text == "String[]: [null, null]");
    }

    /// <summary>
    /// Tests StopsReadingAfterTheLimit.
    /// </summary>
    private static void StopsReadingAfterTheLimit()
    {
        // Setup
        var pulled = 0;
        var endless = Enumerable.Range(0,
            int.MaxValue)
            .Select(value =>
            {
                pulled++;
                return value;
            });

        // Act
        NotebookScriptPrinter.Format(endless);

        // Assert
        Check(pulled == NotebookScriptPrinter.ItemLimit + 1);
    }

    /// <summary>
    /// Tests TextOfTellsAMessageThatFails.
    /// </summary>
    private static void TextOfTellsAMessageThatFails()
    {
        // Act
        var text = NotebookScriptPrinter.TextOf(new FailingTexts(new FailingMessage(new InvalidOperationException())));

        // Assert
        Check(text == "<ToString failed: <Message failed>>");
    }

    /// <summary>
    /// Tests MessageOfPassesOnAnOutOfMemory.
    /// </summary>
    private static void MessageOfPassesOnAnOutOfMemory()
    {
        // Act
        Action operation = () => NotebookScriptPrinter.MessageOf(new FailingMessage(new OutOfMemoryException()));

        // Assert
        Throws<OutOfMemoryException>(operation);
    }

    #endregion
}
