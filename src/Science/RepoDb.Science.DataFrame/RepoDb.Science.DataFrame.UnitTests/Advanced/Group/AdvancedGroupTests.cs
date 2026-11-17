using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies group additions and failure behavior.
/// </summary>
internal static class AdvancedGroupTests
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
        TransformBroadcastsSummariesInSourceOrder();
        TransformCanReplaceColumnByDefault();
        TransformSupportsInPlaceOutput();
        TransformSupportsMissingKeysAndEmptyGroups();
        HavingFiltersAggregateResults();
        HavingRejectsNullPredicate();
        TransformRejectsUnknownFunction();
        TransformRejectsMissingColumn();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        TransformRejectsBlankOutput();
    }

    /// <summary>
    /// Verifies transform broadcasts summaries in source order.
    /// </summary>
    private static void TransformBroadcastsSummariesInSourceOrder()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Sum,
                "Total");

        // Assert
        Check((decimal)result.Rows[0]["Total"] == 10m && (decimal)result.Rows[1]["Total"] == 10m && (decimal)result.Rows[2]["Total"] == 60m);
        Check(!table.Columns.Contains("Total"));
    }

    /// <summary>
    /// Verifies transform can replace column by default.
    /// </summary>
    private static void TransformCanReplaceColumnByDefault()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Mean);

        // Assert
        Check(result.Columns["Value"]!.DataType == typeof(double) && (double)result.Rows[1]["Value"] == 10);
    }

    /// <summary>
    /// Verifies transform supports in place output.
    /// </summary>
    private static void TransformSupportsInPlaceOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Count,
                "Present",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[1]["Present"] == 1);
    }

    /// <summary>
    /// Verifies transform supports missing keys and empty groups.
    /// </summary>
    private static void TransformSupportsMissingKeysAndEmptyGroups()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = DBNull.Value;
        table.Rows[1]["Key"] = DBNull.Value;

        // Act
        var result = table
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Sum);
        var empty = table
            .Clone()
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Sum);

        // Assert
        Check((decimal)result.Rows[1]["Value"] == 10m && empty.Rows.Count == 0);
    }

    /// <summary>
    /// Verifies having filters aggregate results.
    /// </summary>
    private static void HavingFiltersAggregateResults()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .GroupBy("Key")
            .Having(row => row.Field<decimal>("Total") > 20m,
                new AggregateDefinition("Value",
                    AggregateFunction.Sum,
                    "Total"));

        // Assert
        Check(result.Rows.Count == 1 && result.Rows[0]["Key"] is "B" && (decimal)result.Rows[0]["Total"] == 60m);
    }

    /// <summary>
    /// Verifies having rejects null predicate.
    /// </summary>
    private static void HavingRejectsNullPredicate()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key")
            .Having(null!,
                new AggregateDefinition("Value",
                    AggregateFunction.Sum));

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies transform rejects unknown function.
    /// </summary>
    private static void TransformRejectsUnknownFunction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key")
            .Transform("Value",
                (AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies transform rejects missing column.
    /// </summary>
    private static void TransformRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key")
            .Transform("Missing",
                AggregateFunction.Sum);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies transform rejects blank output.
    /// </summary>
    private static void TransformRejectsBlankOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .GroupBy("Key")
            .Transform("Value",
                AggregateFunction.Sum,
                "");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
