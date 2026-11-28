using System.Dynamic;
using Microsoft.Data.Sqlite;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Tests the data tables built from what a script has.
/// </summary>
internal static class NotebookDataTablesTests
{
    #region Public Methods

    /// <summary>
    /// Runs the scenarios.
    /// </summary>
    public static void Run()
    {
        RunReader();
        RunReaderNames();
        RunRows();
        RunObjects();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs the scenarios of data readers.
    /// </summary>
    private static void RunReader()
    {
        FromReaderRejectsNull();
        FromReaderGivesTheTypedColumns();
        FromReaderGivesTheValues();
        FromReaderKeepsNulls();
        FromReaderLeavesTheReaderOpen();
        FromReaderKeepsTheColumnsWithoutRows();
        FromReaderWithoutAResultIsEmpty();
        FromReaderReadsOnlyTheFirstResult();
    }

    /// <summary>
    /// Runs the scenarios of the column names of a reader.
    /// </summary>
    private static void RunReaderNames()
    {
        FromReaderNumbersARepeatedName();
        FromReaderNamesAColumnWithoutAName();
    }

    /// <summary>
    /// Runs the scenarios of rows by key.
    /// </summary>
    private static void RunRows()
    {
        FromRowsRejectsNull();
        FromRowsMakesAColumnForEveryKey();
        FromRowsTypesAColumnByItsValues();
        FromRowsUsesObjectForMixedTypes();
        FromRowsUsesObjectForNullsOnly();
        FromRowsFillsAMissingKeyWithNull();
        FromRowsFillsANullValueWithNull();
        FromRowsLeavesOutNullRows();
        FromRowsWithoutRowsIsEmpty();
    }

    /// <summary>
    /// Runs the scenarios of objects.
    /// </summary>
    private static void RunObjects()
    {
        FromObjectsRejectsNull();
        FromObjectsReadsTheReadableProperties();
        FromObjectsReadsTheValues();
        FromObjectsReadsExpandoObjectsByKey();
        FromObjectsLeavesOutNulls();
    }

    /// <summary>
    /// Reads a query into a table.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <returns>The table.</returns>
    private static DataTable Read(string query)
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = query;
        using var reader = command.ExecuteReader();
        return NotebookDataTables.FromReader(reader);
    }

    /// <summary>
    /// Gets the column names of a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The names.</returns>
    private static string NamesOf(DataTable table) => string.Join(',',
        table.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName));

    /// <summary>
    /// Builds a table from two rows.
    /// </summary>
    /// <returns>The table.</returns>
    private static DataTable TwoRows() => NotebookDataTables.FromRows(
    [
        new Dictionary<string, object?>(StringComparer.Ordinal) { ["Id"] = 1, ["Mixed"] = 1, ["Empty"] = null },
        new Dictionary<string, object?>(StringComparer.Ordinal) { ["Mixed"] = "two", ["Extra"] = null }
    ]);

    /// <summary>
    /// Tests FromReaderRejectsNull.
    /// </summary>
    private static void FromReaderRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => NotebookDataTables.FromReader(null!));
    }

    /// <summary>
    /// Tests FromReaderGivesTheTypedColumns.
    /// </summary>
    private static void FromReaderGivesTheTypedColumns()
    {
        // Act
        var table = Read("SELECT 1 AS Id, 'a' AS Name");

        // Assert
        Check(table.Columns["Id"]!.DataType == typeof(long) && table.Columns["Name"]!.DataType == typeof(string));
    }

    /// <summary>
    /// Tests FromReaderGivesTheValues.
    /// </summary>
    private static void FromReaderGivesTheValues()
    {
        // Act
        var table = Read("SELECT 1 AS Id UNION ALL SELECT 2");

        // Assert
        Check(table.Rows.Count == 2 && table.Rows[1][0] is 2L);
    }

    /// <summary>
    /// Tests FromReaderKeepsNulls.
    /// </summary>
    private static void FromReaderKeepsNulls()
    {
        // Act
        var table = Read("SELECT NULL AS Id");

        // Assert
        Check(table.Rows[0][0] is DBNull);
    }

    /// <summary>
    /// Tests FromReaderLeavesTheReaderOpen.
    /// </summary>
    private static void FromReaderLeavesTheReaderOpen()
    {
        // Setup
        using var reader = SampleTables.Numbers()
            .CreateDataReader();

        // Act
        NotebookDataTables.FromReader(reader);

        // Assert
        Check(!reader.IsClosed);
    }

    /// <summary>
    /// Tests FromReaderKeepsTheColumnsWithoutRows.
    /// </summary>
    private static void FromReaderKeepsTheColumnsWithoutRows()
    {
        // Act
        var table = Read("SELECT 1 AS Id WHERE 0");

        // Assert
        Check(table.Columns.Count == 1 && table.Rows.Count == 0);
    }

    /// <summary>
    /// Tests FromReaderWithoutAResultIsEmpty.
    /// </summary>
    private static void FromReaderWithoutAResultIsEmpty()
    {
        // Act
        var table = Read("CREATE TABLE t (a)");

        // Assert
        Check(table.Columns.Count == 0 && table.Rows.Count == 0);
    }

    /// <summary>
    /// Tests FromReaderReadsOnlyTheFirstResult.
    /// </summary>
    private static void FromReaderReadsOnlyTheFirstResult()
    {
        // Act
        var table = Read("SELECT 1 AS A; SELECT 2 AS B");

        // Assert
        Check(NamesOf(table) == "A" && table.Rows.Count == 1);
    }

    /// <summary>
    /// Tests FromReaderNumbersARepeatedName.
    /// </summary>
    private static void FromReaderNumbersARepeatedName()
    {
        // Act
        var table = Read("SELECT 1 AS A, 2 AS A, 3 AS A");

        // Assert
        Check(NamesOf(table) == "A,A_2,A_3");
    }

    /// <summary>
    /// Tests FromReaderNamesAColumnWithoutAName.
    /// </summary>
    private static void FromReaderNamesAColumnWithoutAName()
    {
        // Act
        var table = Read("SELECT 1 AS \" \", 2 AS B");

        // Assert
        Check(NamesOf(table) == "Column1,B");
    }

    /// <summary>
    /// Tests FromRowsRejectsNull.
    /// </summary>
    private static void FromRowsRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => NotebookDataTables.FromRows(null!));
    }

    /// <summary>
    /// Tests FromRowsMakesAColumnForEveryKey.
    /// </summary>
    private static void FromRowsMakesAColumnForEveryKey()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(NamesOf(table) == "Id,Mixed,Empty,Extra");
    }

    /// <summary>
    /// Tests FromRowsTypesAColumnByItsValues.
    /// </summary>
    private static void FromRowsTypesAColumnByItsValues()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(table.Columns["Id"]!.DataType == typeof(int));
    }

    /// <summary>
    /// Tests FromRowsUsesObjectForMixedTypes.
    /// </summary>
    private static void FromRowsUsesObjectForMixedTypes()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(table.Columns["Mixed"]!.DataType == typeof(object));
    }

    /// <summary>
    /// Tests FromRowsUsesObjectForNullsOnly.
    /// </summary>
    private static void FromRowsUsesObjectForNullsOnly()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(table.Columns["Empty"]!.DataType == typeof(object));
    }

    /// <summary>
    /// Tests FromRowsFillsAMissingKeyWithNull.
    /// </summary>
    private static void FromRowsFillsAMissingKeyWithNull()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(table.Rows[1]["Id"] is DBNull);
    }

    /// <summary>
    /// Tests FromRowsFillsANullValueWithNull.
    /// </summary>
    private static void FromRowsFillsANullValueWithNull()
    {
        // Act
        var table = TwoRows();

        // Assert
        Check(table.Rows[0]["Empty"] is DBNull);
    }

    /// <summary>
    /// Tests FromRowsLeavesOutNullRows.
    /// </summary>
    private static void FromRowsLeavesOutNullRows()
    {
        // Act
        var table = NotebookDataTables.FromRows([null]);

        // Assert
        Check(table.Rows.Count == 0);
    }

    /// <summary>
    /// Tests FromRowsWithoutRowsIsEmpty.
    /// </summary>
    private static void FromRowsWithoutRowsIsEmpty()
    {
        // Act
        var table = NotebookDataTables.FromRows([]);

        // Assert
        Check(table.Columns.Count == 0);
    }

    /// <summary>
    /// Tests FromObjectsRejectsNull.
    /// </summary>
    private static void FromObjectsRejectsNull()
    {
        // Act/Assert
        Throws<ArgumentNullException>(() => NotebookDataTables.FromObjects(null!));
    }

    /// <summary>
    /// Tests FromObjectsReadsTheReadableProperties.
    /// </summary>
    private static void FromObjectsReadsTheReadableProperties()
    {
        // Act
        var table = NotebookDataTables.FromObjects([new RowObject { Id = 7, Secret = "s" }]);

        // Assert
        Check(NamesOf(table) == "Id");
    }

    /// <summary>
    /// Tests FromObjectsReadsTheValues.
    /// </summary>
    private static void FromObjectsReadsTheValues()
    {
        // Act
        var table = NotebookDataTables.FromObjects([new RowObject { Id = 7 }]);

        // Assert
        Check(table.Rows[0]["Id"] is 7);
    }

    /// <summary>
    /// Tests FromObjectsReadsExpandoObjectsByKey.
    /// </summary>
    private static void FromObjectsReadsExpandoObjectsByKey()
    {
        // Setup
        var row = new ExpandoObject();
        row.TryAdd("Name",
            "Alice");

        // Act
        var table = NotebookDataTables.FromObjects([row]);

        // Assert
        Check(NamesOf(table) == "Name" && table.Rows[0][0] is "Alice");
    }

    /// <summary>
    /// Tests FromObjectsLeavesOutNulls.
    /// </summary>
    private static void FromObjectsLeavesOutNulls()
    {
        // Act
        var table = NotebookDataTables.FromObjects([null]);

        // Assert
        Check(table.Rows.Count == 0);
    }

    #endregion
}
