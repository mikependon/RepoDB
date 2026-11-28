using System.Dynamic;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests what a script sees as Notebook.
/// </summary>
internal static class NotebookScriptHostTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static async Task RunAsync()
    {
        RunMembers();
        await RunPrintAsync();
        RunToDataTable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of the members.
    /// </summary>
    private static void RunMembers()
    {
        ConstructorRejectsNullContext();
        ConstructorRejectsNullMessages();
        ExposesTheContext();
        ChartReadsTheContext();
    }

    /// <summary>
    /// Runs the scenarios of printing.
    /// </summary>
    private static async Task RunPrintAsync()
    {
        await PrintWritesTheTypeAndTheValue();
        await PrintWritesNull();
        await PrintKeepsATable();
        await PrintKeepsTheTablesOfADataSet();
        await PrintOfAValueKeepsNoTable();
    }

    /// <summary>
    /// Runs the scenarios of the data tables.
    /// </summary>
    private static void RunToDataTable()
    {
        ReadsAReader();
        RejectsANullReader();
        RejectsAClosedReader();
        ReadsExpandoObjects();
        ReadsDictionaries();
        ReadsObjects();
    }

    /// <summary>
    /// Creates a host.
    /// </summary>
    /// <param name="messages">The texts, or null for the default ones.</param>
    /// <returns>The host.</returns>
    private static NotebookScriptHost Host(NotebookScriptingMessages? messages = null) => new(new NotebookScriptContext(NotebookScriptingMessages.Default),
        messages ?? NotebookScriptingMessages.Default);

    /// <summary>
    /// Prints a value and gets what was written.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="value">The value.</param>
    /// <returns>The output.</returns>
    private static async Task<string> Printed(NotebookScriptHost host,
        object? value)
    {
        using var capture = await NotebookConsoleCapture.StartAsync();
        host.Print(value);
        return capture.Output.TrimEnd();
    }

    /// <summary>
    /// Tests ConstructorRejectsNullContext.
    /// </summary>
    private static void ConstructorRejectsNullContext()
    {
        // Act
        Action operation = () => _ = new NotebookScriptHost(null!,
            NotebookScriptingMessages.Default);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ConstructorRejectsNullMessages.
    /// </summary>
    private static void ConstructorRejectsNullMessages()
    {
        // Act
        Action operation = () => _ = new NotebookScriptHost(new NotebookScriptContext(NotebookScriptingMessages.Default),
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ExposesTheContext.
    /// </summary>
    private static void ExposesTheContext()
    {
        // Setup
        var context = new NotebookScriptContext(NotebookScriptingMessages.Default);

        // Act
        var host = new NotebookScriptHost(context,
            NotebookScriptingMessages.Default);

        // Assert
        Check(ReferenceEquals(host.Context,
            context));
    }

    /// <summary>
    /// Tests ChartReadsTheContext.
    /// </summary>
    private static void ChartReadsTheContext()
    {
        // Setup
        var host = Host();
        host.Context.Load(SampleTables.Results("S",
            SampleTables.Customers()));

        // Act
        var tables = host.Chart.FromSqlResult("S");

        // Assert
        Check(tables.Length == 1);
    }

    /// <summary>
    /// Tests PrintWritesTheTypeAndTheValue.
    /// </summary>
    private static async Task PrintWritesTheTypeAndTheValue()
    {
        // Act
        var output = await Printed(Host(),
            19);

        // Assert
        Check(output == "Int32: 19");
    }

    /// <summary>
    /// Tests PrintWritesNull.
    /// </summary>
    private static async Task PrintWritesNull()
    {
        // Act
        var output = await Printed(Host(),
            null);

        // Assert
        Check(output == "null");
    }

    /// <summary>
    /// Tests PrintKeepsATable.
    /// </summary>
    private static async Task PrintKeepsATable()
    {
        // Setup
        var host = Host();
        var table = SampleTables.Numbers();

        // Act
        await Printed(host,
            table);

        // Assert
        Check(ReferenceEquals(host.Tables.Items
            .Single(),
            table));
    }

    /// <summary>
    /// Tests PrintKeepsTheTablesOfADataSet.
    /// </summary>
    private static async Task PrintKeepsTheTablesOfADataSet()
    {
        // Setup
        var host = Host();
        var set = new DataSet();
        set.Tables.Add(SampleTables.Numbers());

        // Act
        await Printed(host,
            set);

        // Assert
        Check(host.Tables.Items.Count == 1);
    }

    /// <summary>
    /// Tests PrintOfAValueKeepsNoTable.
    /// </summary>
    private static async Task PrintOfAValueKeepsNoTable()
    {
        // Setup
        var host = Host();

        // Act
        await Printed(host,
            "text");

        // Assert
        Check(host.Tables.Items.Count == 0);
    }

    /// <summary>
    /// Tests ReadsAReader.
    /// </summary>
    private static void ReadsAReader()
    {
        // Setup
        using var reader = SampleTables.Numbers()
            .CreateDataReader();

        // Act
        var table = Host()
            .ToDataTable(reader);

        // Assert
        Check(table.Rows.Count == 3);
    }

    /// <summary>
    /// Tests RejectsANullReader.
    /// </summary>
    private static void RejectsANullReader()
    {
        // Setup
        var host = Host();

        // Act/Assert
        Throws<ArgumentNullException>(() => host.ToDataTable((IDataReader)null!));
    }

    /// <summary>
    /// Tests RejectsAClosedReader.
    /// </summary>
    private static void RejectsAClosedReader()
    {
        // Setup
        var host = Host(new MarkedMessages());
        var reader = SampleTables.Numbers()
            .CreateDataReader();
        reader.Close();

        // Act
        var error = Throws<InvalidOperationException>(() => host.ToDataTable(reader));

        // Assert
        Check(error.Message == "closed");
    }

    /// <summary>
    /// Tests ReadsExpandoObjects.
    /// </summary>
    private static void ReadsExpandoObjects()
    {
        // Setup
        var row = new ExpandoObject();
        row.TryAdd("Name",
            "Alice");

        // Act
        var table = Host()
            .ToDataTable(new List<ExpandoObject> { row });

        // Assert
        Check(table.Rows[0]["Name"] is "Alice");
    }

    /// <summary>
    /// Tests ReadsDictionaries.
    /// </summary>
    private static void ReadsDictionaries()
    {
        // Setup
        var rows = new List<Dictionary<string, object?>> { new(StringComparer.Ordinal) { ["Id"] = 1 } };

        // Act
        var table = Host()
            .ToDataTable(rows);

        // Assert
        Check(table.Rows[0]["Id"] is 1);
    }

    /// <summary>
    /// Tests ReadsObjects.
    /// </summary>
    private static void ReadsObjects()
    {
        // Setup
        var rows = new List<object?> { new RowObject { Id = 7 } };

        // Act
        var table = Host()
            .ToDataTable(rows);

        // Assert
        Check(table.Rows[0]["Id"] is 7);
    }

    #endregion
}
