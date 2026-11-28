namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the chart of a script.
/// </summary>
internal static class NotebookScriptChartTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ConstructorRejectsNullContext();
        TableStartsEmpty();
        FromDataTableKeepsTheTable();
        FromDataTableReturnsTheTable();
        TheLastTableWins();
        FromDataTableRejectsNull();
        FromSqlResultChartsTheFirstTable();
        FromSqlResultReturnsEveryTable();
        FromSqlResultWithoutTablesKeepsTheTable();
        FromSqlResultOfAnUnknownSectionFails();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a chart over a section that has the given tables.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>The chart.</returns>
    private static NotebookScriptChart Chart(params NotebookTable[] tables)
    {
        var context = new NotebookScriptContext(NotebookScriptingMessages.Default);
        context.Load(SampleTables.Results("S",
            tables));
        return new NotebookScriptChart(context);
    }

    /// <summary>
    /// Tests ConstructorRejectsNullContext.
    /// </summary>
    private static void ConstructorRejectsNullContext()
    {
        // Act
        Action operation = () => _ = new NotebookScriptChart(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests TableStartsEmpty.
    /// </summary>
    private static void TableStartsEmpty()
    {
        // Act
        var chart = Chart();

        // Assert
        Check(chart.Table is null);
    }

    /// <summary>
    /// Tests FromDataTableKeepsTheTable.
    /// </summary>
    private static void FromDataTableKeepsTheTable()
    {
        // Setup
        var chart = Chart();
        var table = SampleTables.Numbers();

        // Act
        chart.FromDataTable(table);

        // Assert
        Check(ReferenceEquals(chart.Table,
            table));
    }

    /// <summary>
    /// Tests FromDataTableReturnsTheTable.
    /// </summary>
    private static void FromDataTableReturnsTheTable()
    {
        // Setup
        var table = SampleTables.Numbers();

        // Act
        var returned = Chart()
            .FromDataTable(table);

        // Assert
        Check(ReferenceEquals(returned,
            table));
    }

    /// <summary>
    /// Tests TheLastTableWins.
    /// </summary>
    private static void TheLastTableWins()
    {
        // Setup
        var chart = Chart();
        chart.FromDataTable(SampleTables.Numbers());
        var last = SampleTables.Numbers();

        // Act
        chart.FromDataTable(last);

        // Assert
        Check(ReferenceEquals(chart.Table,
            last));
    }

    /// <summary>
    /// Tests FromDataTableRejectsNull.
    /// </summary>
    private static void FromDataTableRejectsNull()
    {
        // Setup
        var chart = Chart();

        // Act/Assert
        Throws<ArgumentNullException>(() => chart.FromDataTable(null!));
    }

    /// <summary>
    /// Tests FromSqlResultChartsTheFirstTable.
    /// </summary>
    private static void FromSqlResultChartsTheFirstTable()
    {
        // Setup
        var chart = Chart(SampleTables.Customers("First"),
            SampleTables.Customers("Second"));

        // Act
        chart.FromSqlResult("S");

        // Assert
        Check(chart.Table!.TableName == "First");
    }

    /// <summary>
    /// Tests FromSqlResultReturnsEveryTable.
    /// </summary>
    private static void FromSqlResultReturnsEveryTable()
    {
        // Setup
        var chart = Chart(SampleTables.Customers(),
            SampleTables.Customers());

        // Act
        var tables = chart.FromSqlResult("S");

        // Assert
        Check(tables.Length == 2);
    }

    /// <summary>
    /// Tests FromSqlResultWithoutTablesKeepsTheTable.
    /// </summary>
    private static void FromSqlResultWithoutTablesKeepsTheTable()
    {
        // Setup
        var chart = Chart();
        var table = chart.FromDataTable(SampleTables.Numbers());

        // Act
        chart.FromSqlResult("S");

        // Assert
        Check(ReferenceEquals(chart.Table,
            table));
    }

    /// <summary>
    /// Tests FromSqlResultOfAnUnknownSectionFails.
    /// </summary>
    private static void FromSqlResultOfAnUnknownSectionFails()
    {
        // Setup
        var chart = Chart();

        // Act/Assert
        Throws<InvalidOperationException>(() => chart.FromSqlResult("Missing"));
    }

    #endregion
}
