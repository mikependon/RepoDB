using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies advancedselection additions and failure behavior.
/// </summary>
internal static class AdvancedSelectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the operation scenarios.
    /// </summary>
    public static void Run()
    {
        RunBatch1();
        RunBatch2();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        SelectDTypesUsesExactTypes();
        SelectDTypesCanSelectNothing();
        SelectDTypesIgnoresRepeatedTypeRequests();
        TopNReturnsPresentOrderedRows();
        TopNHandlesOversizedCount();
        TopNHandlesZeroCount();
        TopNRejectsNegativeCount();
        BottomNReturnsPresentOrderedRows();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        BottomNHandlesOversizedCount();
        BottomNHandlesZeroCount();
        BottomNRejectsNegativeCount();
    }

    /// <summary>
    /// Verifies select dtypes uses exact types.
    /// </summary>
    private static void SelectDTypesUsesExactTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SelectDTypes(typeof(decimal),
                typeof(int));

        // Assert
        Check(result.Columns.Count == 2 && result.Columns[0].ColumnName is "Value");
        Check(table.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies select dtypes can select nothing.
    /// </summary>
    private static void SelectDTypesCanSelectNothing()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SelectDTypes();

        // Assert
        Check(result.Columns.Count == 0 && result.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies select dtypes ignores repeated type requests.
    /// </summary>
    private static void SelectDTypesIgnoresRepeatedTypeRequests()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SelectDTypes(typeof(int),
                typeof(int));

        // Assert
        Check(result.Columns.Count == 1);
    }

    /// <summary>
    /// Verifies top nreturns present ordered rows.
    /// </summary>
    private static void TopNReturnsPresentOrderedRows()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .TopN("Value",
                2);

        // Assert
        Check(result.Rows.Count == 2 && (decimal)result.Rows[0]["Value"] == 30m && (decimal)result.Rows[1]["Value"] == 30m);
        Check(table.Rows[1]
            .IsNull("Value"));
    }

    /// <summary>
    /// Verifies top nhandles oversized count.
    /// </summary>
    private static void TopNHandlesOversizedCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .TopN("Value",
                99);

        // Assert
        Check(result.Rows.Count == 3);
    }

    /// <summary>
    /// Verifies top nhandles zero count.
    /// </summary>
    private static void TopNHandlesZeroCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .TopN("Count",
                0);

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies top nrejects negative count.
    /// </summary>
    private static void TopNRejectsNegativeCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TopN("Value",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies bottom nreturns present ordered rows.
    /// </summary>
    private static void BottomNReturnsPresentOrderedRows()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .BottomN("Value",
                2);

        // Assert
        Check(result.Rows.Count == 2 && (decimal)result.Rows[0]["Value"] == 10m && (decimal)result.Rows[1]["Value"] == 30m);
        Check(table.Rows[1]
            .IsNull("Value"));
    }

    /// <summary>
    /// Verifies bottom nhandles oversized count.
    /// </summary>
    private static void BottomNHandlesOversizedCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .BottomN("Value",
                99);

        // Assert
        Check(result.Rows.Count == 3);
    }

    /// <summary>
    /// Verifies bottom nhandles zero count.
    /// </summary>
    private static void BottomNHandlesZeroCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .BottomN("Count",
                0);

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies bottom nrejects negative count.
    /// </summary>
    private static void BottomNRejectsNegativeCount()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .BottomN("Value",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    #endregion
}
