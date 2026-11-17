using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies selection operations and failure behavior.
/// </summary>
internal static class SelectionTests
{
    #region Public Methods

    /// <summary>
    /// Runs the operation scenarios.
    /// </summary>
    public static void Run()
    {
        RunBatch1();
        RunBatch2();
        RunBatch3();
        RunBatch4();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        SelectColumnsPreservesRequestedOrder();
        SelectColumnsAllowsEmptyProjection();
        ProjectionMaterializesComputedColumns();
        ProjectionPreservesMetadata();
        DropColumnsCopiesByDefault();
        DropColumnsSupportsCopyOverload();
        DropColumnsSupportsInPlace();
        DropColumnsRejectsConstrainedInPlace();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        FilterCopiesMatchingRows();
        FilterRemovesRowsInPlace();
        FilterPropagatesCallbackWithoutMutation();
        FilterAcceptsEmptyResults();
        RenameColumnsSupportsSwappingNames();
        RenameColumnsSupportsInPlace();
        RenameRejectsConflictingOutputNames();
        RenameRejectsEmptyOutput();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        SortValuesIsStableAndNullsLast();
        SortValuesDescendingRetainsNullPosition();
        SortValuesSupportsNullsFirst();
        SortValuesSupportsMultipleDirections();
        SortValuesUsesOrdinalStrings();
        SortValuesComparesMissingTies();
        SortValuesRejectsDirectionMismatch();
        SortValuesRejectsEmptyKeys();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        SelectRejectsMissingColumn();
        SelectRejectsDuplicateColumns();
        SelectRejectsNullColumns();
        DropRejectsMissingColumns();
        RenameRejectsNullMapping();
        RenameRejectsMissingColumn();
        FilterRejectsNullPredicate();
        SortRejectsMissingColumn();
    }

    /// <summary>
    /// Verifies SelectColumnsPreservesRequestedOrder.
    /// </summary>
    private static void SelectColumnsPreservesRequestedOrder()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SelectColumns("Count",
                "Key");

        // Assert
        Check(result.Columns.Count == 2 && result.Columns[0].ColumnName is "Count");
        Check(result.Rows.Count == 4 && table.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies SelectColumnsAllowsEmptyProjection.
    /// </summary>
    private static void SelectColumnsAllowsEmptyProjection()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SelectColumns();

        // Assert
        Check(result.Columns.Count == 0 && result.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies ProjectionMaterializesComputedColumns.
    /// </summary>
    [UnconditionalSuppressMessage("Trimming",
        "IL2026",
        Justification = "The constant expression uses built-in decimal arithmetic without reflected members.")]
    private static void ProjectionMaterializesComputedColumns()
    {
        // Setup
        var table = TestTable.Create();
        table.Columns.Add("Double",
            typeof(decimal),
            "Value * 2");

        // Act
        var result = table
            .SelectColumns("Double");

        // Assert
        Check(result.Columns[0].Expression.Length == 0 && (decimal)result.Rows[0][0] == 20m);
    }

    /// <summary>
    /// Verifies ProjectionPreservesMetadata.
    /// </summary>
    private static void ProjectionPreservesMetadata()
    {
        // Setup
        var table = TestTable.Create();
        table.Columns[0].Caption = "Group";
        table.Columns[0].ExtendedProperties["tag"] = "dimension";

        // Act
        var result = table
            .SelectColumns("Key");

        // Assert
        Check(result.Columns[0].Caption is "Group");
        Check(result.Columns[0].ExtendedProperties["tag"] is "dimension");
    }

    /// <summary>
    /// Verifies DropColumnsCopiesByDefault.
    /// </summary>
    private static void DropColumnsCopiesByDefault()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropColumns("Value");

        // Assert
        Check(result.Columns.Count == 2 && table.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies DropColumnsSupportsCopyOverload.
    /// </summary>
    private static void DropColumnsSupportsCopyOverload()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropColumns(new[] { "Value" },
                false);

        // Assert
        Check(result.Columns.Count == 2 && !ReferenceEquals(table,
            result));
    }

    /// <summary>
    /// Verifies DropColumnsSupportsInPlace.
    /// </summary>
    private static void DropColumnsSupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropColumns(new[] { "Value" },
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && table.Columns.Count == 2);
    }

    /// <summary>
    /// Verifies DropColumnsRejectsConstrainedInPlace.
    /// </summary>
    private static void DropColumnsRejectsConstrainedInPlace()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows.RemoveAt(3);
        table.PrimaryKey = new[] { table.Columns["Count"]! };

        // Act
        Action operation = () => table
            .DropColumns(new[] { "Count" },
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Verifies FilterCopiesMatchingRows.
    /// </summary>
    private static void FilterCopiesMatchingRows()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Filter(row => row.Field<int>("Count") > 1);

        // Assert
        Check(result.Rows.Count == 3 && table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies FilterRemovesRowsInPlace.
    /// </summary>
    private static void FilterRemovesRowsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Filter(row => row.Field<int>("Count") > 2,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows.Count == 2);
    }

    /// <summary>
    /// Verifies FilterPropagatesCallbackWithoutMutation.
    /// </summary>
    private static void FilterPropagatesCallbackWithoutMutation()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Filter(_ => throw new InvalidOperationException(),
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check(table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies FilterAcceptsEmptyResults.
    /// </summary>
    private static void FilterAcceptsEmptyResults()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Filter(_ => false);

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 3);
    }

    /// <summary>
    /// Verifies RenameColumnsSupportsSwappingNames.
    /// </summary>
    private static void RenameColumnsSupportsSwappingNames()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal) { ["Key"] = "Value", ["Value"] = "Key" });

        // Assert
        Check(result.Columns[0].ColumnName is "Value" && result.Columns[1].ColumnName is "Key");
        Check(table.Columns[0].ColumnName is "Key");
    }

    /// <summary>
    /// Verifies RenameColumnsSupportsInPlace.
    /// </summary>
    private static void RenameColumnsSupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal) { ["Key"] = "Group" },
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && table.Columns[0].ColumnName is "Group");
    }

    /// <summary>
    /// Verifies RenameRejectsConflictingOutputNames.
    /// </summary>
    private static void RenameRejectsConflictingOutputNames()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal) { ["Key"] = "Value" },
                true);

        // Assert
        Throws<ArgumentException>(operation);
        Check(table.Columns[0].ColumnName is "Key");
    }

    /// <summary>
    /// Verifies RenameRejectsEmptyOutput.
    /// </summary>
    private static void RenameRejectsEmptyOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal) { ["Key"] = " " });

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SortValuesIsStableAndNullsLast.
    /// </summary>
    private static void SortValuesIsStableAndNullsLast()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SortValues("Value");

        // Assert
        Check((int)result.Rows[1]["Count"] == 3 && result.Rows[3].IsNull("Value"));
        Check((int)table.Rows[1]["Count"] == 2);
    }

    /// <summary>
    /// Verifies SortValuesDescendingRetainsNullPosition.
    /// </summary>
    private static void SortValuesDescendingRetainsNullPosition()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SortValues("Value",
                false);

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 30m && result.Rows[3].IsNull("Value"));
    }

    /// <summary>
    /// Verifies SortValuesSupportsNullsFirst.
    /// </summary>
    private static void SortValuesSupportsNullsFirst()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SortValues("Value",
                true,
                false);

        // Assert
        Check(result.Rows[0].IsNull("Value"));
    }

    /// <summary>
    /// Verifies SortValuesSupportsMultipleDirections.
    /// </summary>
    private static void SortValuesSupportsMultipleDirections()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .SortValues(new[] { "Key", "Count" },
                new[] { false, true });

        // Assert
        Check(result.Rows[0]["Key"] is "B" && (int)result.Rows[3]["Count"] == 2);
    }

    /// <summary>
    /// Verifies SortValuesUsesOrdinalStrings.
    /// </summary>
    private static void SortValuesUsesOrdinalStrings()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = "a";

        // Act
        var result = table
            .SortValues("Key");

        // Assert
        Check(result.Rows[0]["Key"] is "A" && result.Rows[3]["Key"] is "a");
    }

    /// <summary>
    /// Verifies SortValuesComparesMissingTies.
    /// </summary>
    private static void SortValuesComparesMissingTies()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[2]["Value"] = DBNull.Value;

        // Act
        var result = table
            .SortValues("Value",
                false,
                false);

        // Assert
        Check(result.Rows[0].IsNull("Value") && result.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies SortValuesRejectsDirectionMismatch.
    /// </summary>
    private static void SortValuesRejectsDirectionMismatch()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SortValues(new[] { "Value" },
                Array.Empty<bool>());

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SortValuesRejectsEmptyKeys.
    /// </summary>
    private static void SortValuesRejectsEmptyKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SortValues(Array.Empty<string>());

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SelectRejectsMissingColumn.
    /// </summary>
    private static void SelectRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SelectColumns("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SelectRejectsDuplicateColumns.
    /// </summary>
    private static void SelectRejectsDuplicateColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SelectColumns("Value",
                "value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SelectRejectsNullColumns.
    /// </summary>
    private static void SelectRejectsNullColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SelectColumns(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies DropRejectsMissingColumns.
    /// </summary>
    private static void DropRejectsMissingColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .DropColumns("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies RenameRejectsNullMapping.
    /// </summary>
    private static void RenameRejectsNullMapping()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .RenameColumns(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies RenameRejectsMissingColumn.
    /// </summary>
    private static void RenameRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal) { ["Missing"] = "X" });

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FilterRejectsNullPredicate.
    /// </summary>
    private static void FilterRejectsNullPredicate()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Filter(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies SortRejectsMissingColumn.
    /// </summary>
    private static void SortRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SortValues("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
