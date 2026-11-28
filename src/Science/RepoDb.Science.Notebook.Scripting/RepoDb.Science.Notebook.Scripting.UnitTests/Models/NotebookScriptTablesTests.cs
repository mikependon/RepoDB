namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the tables a script shows.
/// </summary>
internal static class NotebookScriptTablesTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunSingleValues();
        RunCollections();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of single values.
    /// </summary>
    private static void RunSingleValues()
    {
        StartsEmpty();
        KeepsATable();
        KeepsATableOnce();
        KeepsTheTablesInOrder();
        IgnoresNull();
        IgnoresAString();
        IgnoresAnotherValue();
    }

    /// <summary>
    /// Runs the scenarios of data sets and collections.
    /// </summary>
    private static void RunCollections()
    {
        KeepsTheTablesOfADataSet();
        KeepsTheTablesOfACollection();
        IgnoresTheOtherItemsOfACollection();
        LooksAtNoMoreThanTheLimit();
    }

    /// <summary>
    /// Adds values and counts the tables.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>The number of tables.</returns>
    private static int CountOf(params object?[] values)
    {
        var tables = new NotebookScriptTables();
        foreach (var value in values)
        {
            tables.AddFrom(value);
        }
        return tables.Items.Count;
    }

    /// <summary>
    /// Tests StartsEmpty.
    /// </summary>
    private static void StartsEmpty()
    {
        // Act
        var tables = new NotebookScriptTables();

        // Assert
        Check(tables.Items.Count == 0);
    }

    /// <summary>
    /// Tests KeepsATable.
    /// </summary>
    private static void KeepsATable()
    {
        // Setup
        var tables = new NotebookScriptTables();
        var table = SampleTables.Numbers();

        // Act
        tables.AddFrom(table);

        // Assert
        Check(ReferenceEquals(tables.Items
            .Single(),
            table));
    }

    /// <summary>
    /// Tests KeepsATableOnce.
    /// </summary>
    private static void KeepsATableOnce()
    {
        // Setup
        var table = SampleTables.Numbers();

        // Act
        var count = CountOf(table,
            table);

        // Assert
        Check(count == 1);
    }

    /// <summary>
    /// Tests KeepsTheTablesInOrder.
    /// </summary>
    private static void KeepsTheTablesInOrder()
    {
        // Setup
        var tables = new NotebookScriptTables();
        tables.AddFrom(new DataTable("First"));

        // Act
        tables.AddFrom(new DataTable("Second"));

        // Assert
        Check(tables.Items[0].TableName == "First" && tables.Items[1].TableName == "Second");
    }

    /// <summary>
    /// Tests IgnoresNull.
    /// </summary>
    private static void IgnoresNull()
    {
        // Act
        var count = CountOf([null]);

        // Assert
        Check(count == 0);
    }

    /// <summary>
    /// Tests IgnoresAString.
    /// </summary>
    private static void IgnoresAString()
    {
        // Act
        var count = CountOf("text");

        // Assert
        Check(count == 0);
    }

    /// <summary>
    /// Tests IgnoresAnotherValue.
    /// </summary>
    private static void IgnoresAnotherValue()
    {
        // Act
        var count = CountOf(1);

        // Assert
        Check(count == 0);
    }

    /// <summary>
    /// Tests KeepsTheTablesOfADataSet.
    /// </summary>
    private static void KeepsTheTablesOfADataSet()
    {
        // Setup
        var set = new DataSet();
        set.Tables.Add(new DataTable("A"));
        set.Tables.Add(new DataTable("B"));

        // Act
        var count = CountOf(set);

        // Assert
        Check(count == 2);
    }

    /// <summary>
    /// Tests KeepsTheTablesOfACollection.
    /// </summary>
    private static void KeepsTheTablesOfACollection()
    {
        // Act
        var count = CountOf(new List<DataTable> { new("A"), new("B") });

        // Assert
        Check(count == 2);
    }

    /// <summary>
    /// Tests IgnoresTheOtherItemsOfACollection.
    /// </summary>
    private static void IgnoresTheOtherItemsOfACollection()
    {
        // Act
        var count = CountOf(new object?[] { 1, null, new DataTable("A") });

        // Assert
        Check(count == 1);
    }

    /// <summary>
    /// Tests LooksAtNoMoreThanTheLimit.
    /// </summary>
    private static void LooksAtNoMoreThanTheLimit()
    {
        // Setup
        var endless = Enumerable.Range(0,
            int.MaxValue)
            .Select(_ => new DataTable());

        // Act
        var count = CountOf(endless);

        // Assert
        Check(count == NotebookScriptTables.ItemLimit);
    }

    #endregion
}
