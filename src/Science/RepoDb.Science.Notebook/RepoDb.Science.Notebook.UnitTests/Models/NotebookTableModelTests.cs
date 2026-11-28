namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the column, table and log entry models.
/// </summary>
internal static class NotebookTableModelTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        ColumnKeepsItsValues();
        ColumnDefaultsToString();
        ColumnRejectsNullName();
        ColumnRejectsBlankTypeName();
        TableKeepsItsData();
        TableKeepsItsFlags();
        TableDefaultsToCompleteAndUntitled();
        TableRejectsNullColumns();
        TableRejectsNullRows();
        TableRejectsNullTitle();
        LogEntryKeepsItsValues();
        LogEntryRejectsNullMessage();
        LogEventArgsKeepTheEntry();
        LogEventArgsRejectNull();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tests ColumnKeepsItsValues.
    /// </summary>
    private static void ColumnKeepsItsValues()
    {
        // Act
        var column = new NotebookColumn("Id",
            "int");

        // Assert
        Check(column.Name == "Id" && column.TypeName == "int");
    }

    /// <summary>
    /// Tests ColumnDefaultsToString.
    /// </summary>
    private static void ColumnDefaultsToString()
    {
        // Act
        var column = new NotebookColumn("Name");

        // Assert
        Check(column.TypeName == "string");
    }

    /// <summary>
    /// Tests ColumnRejectsNullName.
    /// </summary>
    private static void ColumnRejectsNullName()
    {
        // Act
        Action operation = () => _ = new NotebookColumn(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests ColumnRejectsBlankTypeName.
    /// </summary>
    private static void ColumnRejectsBlankTypeName()
    {
        // Act
        Action operation = () => _ = new NotebookColumn("Id",
            " ");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Tests TableKeepsItsData.
    /// </summary>
    private static void TableKeepsItsData()
    {
        // Setup
        NotebookColumn[] columns = [new("Id")];
        string?[][] rows = [["1"]];

        // Act
        var table = new NotebookTable(columns,
            rows);

        // Assert
        Check(ReferenceEquals(table.Columns,
            columns) && ReferenceEquals(table.Rows,
                rows));
    }

    /// <summary>
    /// Tests TableKeepsItsFlags.
    /// </summary>
    private static void TableKeepsItsFlags()
    {
        // Act
        var table = new NotebookTable([],
            [],
            true,
            "Totals");

        // Assert
        Check(table.IsTruncated && table.Title == "Totals");
    }

    /// <summary>
    /// Tests TableDefaultsToCompleteAndUntitled.
    /// </summary>
    private static void TableDefaultsToCompleteAndUntitled()
    {
        // Act
        var table = new NotebookTable([],
            []);

        // Assert
        Check(!table.IsTruncated && table.Title.Length == 0);
    }

    /// <summary>
    /// Tests TableRejectsNullColumns.
    /// </summary>
    private static void TableRejectsNullColumns()
    {
        // Act
        Action operation = () => _ = new NotebookTable(null!,
            []);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests TableRejectsNullRows.
    /// </summary>
    private static void TableRejectsNullRows()
    {
        // Act
        Action operation = () => _ = new NotebookTable([],
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests TableRejectsNullTitle.
    /// </summary>
    private static void TableRejectsNullTitle()
    {
        // Act
        Action operation = () => _ = new NotebookTable([],
            [],
            false,
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests LogEntryKeepsItsValues.
    /// </summary>
    private static void LogEntryKeepsItsValues()
    {
        // Setup
        var time = DateTimeOffset.UnixEpoch;

        // Act
        var entry = new NotebookLogEntry(NotebookLogLevel.Warning,
            "Careful",
            time);

        // Assert
        Check(entry.Level == NotebookLogLevel.Warning && entry.Message == "Careful" && entry.Time == time);
    }

    /// <summary>
    /// Tests LogEntryRejectsNullMessage.
    /// </summary>
    private static void LogEntryRejectsNullMessage()
    {
        // Act
        Action operation = () => _ = new NotebookLogEntry(NotebookLogLevel.Error,
            null!,
            DateTimeOffset.UnixEpoch);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Tests LogEventArgsKeepTheEntry.
    /// </summary>
    private static void LogEventArgsKeepTheEntry()
    {
        // Setup
        var entry = new NotebookLogEntry(NotebookLogLevel.Debug,
            "x",
            DateTimeOffset.UnixEpoch);

        // Act
        var args = new NotebookLogEventArgs(entry);

        // Assert
        Check(ReferenceEquals(args.Entry,
            entry));
    }

    /// <summary>
    /// Tests LogEventArgsRejectNull.
    /// </summary>
    private static void LogEventArgsRejectNull()
    {
        // Act
        Action operation = () => _ = new NotebookLogEventArgs(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
