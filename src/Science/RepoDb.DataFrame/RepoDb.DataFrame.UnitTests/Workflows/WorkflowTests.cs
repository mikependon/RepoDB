using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies complete analytical workflows and schema contracts.
/// </summary>
internal static class WorkflowTests
{
    #region Public Methods

    /// <summary>
    /// Runs analytical workflow scenarios.
    /// </summary>
    public static void Run()
    {
        RunCopyScenarios();
        RunMutationScenarios();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a workflow batch.
    /// </summary>
    private static void RunCopyScenarios()
    {
        NotebookPipelineLeavesSourceUnchanged();
        InPlaceCleaningChainRetainsIdentity();
        ConvertedColumnPreservesDisplayMetadata();
        ConcatBroadensNullableOutput();
        InPlaceAssignmentHonorsReadOnlyColumn();
        InPlaceAssignmentHonorsRequiredColumn();
        InPlaceFilterPreservesOtherRowVersions();
        ApplyCallbackFailureDoesNotCommitEarlierAssignments();
    }

    /// <summary>
    /// Runs a workflow batch.
    /// </summary>
    private static void RunMutationScenarios()
    {
        EmptyInPlaceApplyRetainsIdentity();
        GroupedMissingValuesHaveMissingSum();
        SumAvoidsBinaryFloatingPointForDecimals();
        MatrixUsesOnlyCurrentRows();
        JoinSupportsCustomRightSuffix();
        ProjectionMaterializesAnalyticalSchema();
        EmptyGroupingComputesNoCallbacks();
    }

    /// <summary>
    /// Verifies NotebookPipelineLeavesSourceUnchanged.
    /// </summary>
    private static void NotebookPipelineLeavesSourceUnchanged()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls("Value")
            .Filter(row => row.Field<decimal>("Value") > 0)
            .SelectColumns("Key",
                "Value")
                .SortValues("Value",
                    false)
                .Head(2);

        // Assert
        Check(result.Rows.Count == 2 && (decimal)result.Rows[0]["Value"] == 30m);
        Check(table.Rows.Count == 4 && table.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies InPlaceCleaningChainRetainsIdentity.
    /// </summary>
    private static void InPlaceCleaningChainRetainsIdentity()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .FillNulls("Value",
                0m,
                true)
                .Filter(row => row.Field<decimal>("Value") > 0,
                    true)
                .DropDuplicates(new[] { "Key", "Value" },
                    true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows.Count == 2);
    }

    /// <summary>
    /// Verifies ConvertedColumnPreservesDisplayMetadata.
    /// </summary>
    private static void ConvertedColumnPreservesDisplayMetadata()
    {
        // Setup
        var table = TestTable.Create();
        table.Columns["Value"]!.Caption = "Revenue";
        table.Columns["Value"]!.ExtendedProperties["unit"] = "DKK";

        // Act
        var result = table
            .CastColumn<double>("Value");

        // Assert
        Check(result.Columns["Value"]!.Caption is "Revenue");
        Check(result.Columns["Value"]!.ExtendedProperties["unit"] is "DKK");
    }

    /// <summary>
    /// Verifies ConcatBroadensNullableOutput.
    /// </summary>
    private static void ConcatBroadensNullableOutput()
    {
        // Setup
        var left = TestTable.Keys(1);
        left.Columns[0].AllowDBNull = false;
        var right = TestTable.Keys(DBNull.Value);

        // Act
        var result = left
            .Concat(right);

        // Assert
        Check(result.Columns[0].AllowDBNull && result.Rows[1].IsNull(0));
        Check(!left.Columns[0].AllowDBNull);
    }

    /// <summary>
    /// Verifies InPlaceAssignmentHonorsReadOnlyColumn.
    /// </summary>
    private static void InPlaceAssignmentHonorsReadOnlyColumn()
    {
        // Setup
        var table = TestTable.Create();
        table.Columns["Count"]!.ReadOnly = true;

        // Act
        Action operation = () => table
            .Assign("Count",
                _ => 9,
                true);

        // Assert
        Throws<ReadOnlyException>(operation);
        Check((int)table.Rows[0]["Count"] == 1);
    }

    /// <summary>
    /// Verifies InPlaceAssignmentHonorsRequiredColumn.
    /// </summary>
    private static void InPlaceAssignmentHonorsRequiredColumn()
    {
        // Setup
        var table = TestTable.Create();
        table.Columns["Count"]!.AllowDBNull = false;

        // Act
        Action operation = () => table
            .Assign<int?>("Count",
                _ => null,
                true);

        // Assert
        Throws<NoNullAllowedException>(operation);
        Check((int)table.Rows[0]["Count"] == 1);
    }

    /// <summary>
    /// Verifies InPlaceFilterPreservesOtherRowVersions.
    /// </summary>
    private static void InPlaceFilterPreservesOtherRowVersions()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[0]["Count"] = 10;

        // Act
        var result = table
            .Filter(row => row.Field<int>("Count") == 10,
                true);

        // Assert
        Check(result.Rows[0].RowState == DataRowState.Modified);
        Check((int)result.Rows[0]["Count", DataRowVersion.Original] == 1);
    }

    /// <summary>
    /// Verifies ApplyCallbackFailureDoesNotCommitEarlierAssignments.
    /// </summary>
    private static void ApplyCallbackFailureDoesNotCommitEarlierAssignments()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Apply(new[] { new ColumnAssignment("Count",
                typeof(int),
                _ => 99), new ColumnAssignment("Broken",
                    typeof(int),
                    _ => throw new InvalidOperationException()) },
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check((int)table.Rows[0]["Count"] == 1 && !table.Columns.Contains("Broken"));
    }

    /// <summary>
    /// Verifies EmptyInPlaceApplyRetainsIdentity.
    /// </summary>
    private static void EmptyInPlaceApplyRetainsIdentity()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Apply(Array.Empty<ColumnAssignment>(),
                true);

        // Assert
        Check(ReferenceEquals(table,
            result));
    }

    /// <summary>
    /// Verifies GroupedMissingValuesHaveMissingSum.
    /// </summary>
    private static void GroupedMissingValuesHaveMissingSum()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Value"] = DBNull.Value;

        // Act
        var result = table
            .GroupBy("Key")
            .Aggregate(new AggregateDefinition("Value",
                AggregateFunction.Sum));

        // Assert
        Check(result.Rows[0].IsNull("Value") && (decimal)result.Rows[1]["Value"] == 60m);
    }

    /// <summary>
    /// Verifies SumAvoidsBinaryFloatingPointForDecimals.
    /// </summary>
    private static void SumAvoidsBinaryFloatingPointForDecimals()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(decimal));
        table.Rows.Add(0.1m);
        table.Rows.Add(0.2m);

        // Act
        var result = table
            .Sum("Value");

        // Assert
        Check(result == 0.3m);
    }

    /// <summary>
    /// Verifies MatrixUsesOnlyCurrentRows.
    /// </summary>
    private static void MatrixUsesOnlyCurrentRows()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[1].Delete();

        // Act
        var result = table
            .ToFeatureMatrix();

        // Assert
        Check(result.GetLength(0) == 3 && result[0, 0] == 10);
    }

    /// <summary>
    /// Verifies JoinSupportsCustomRightSuffix.
    /// </summary>
    private static void JoinSupportsCustomRightSuffix()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = TestTable.Keys(1);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                rightSuffix: "_order");

        // Assert
        Check(result.Columns[1].ColumnName is "Id_order");
    }

    /// <summary>
    /// Verifies ProjectionMaterializesAnalyticalSchema.
    /// </summary>
    private static void ProjectionMaterializesAnalyticalSchema()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows.RemoveAt(3);
        table.PrimaryKey = new[] { table.Columns["Count"]! };

        // Act
        var result = table
            .SelectColumns("Count");

        // Assert
        Check(result.PrimaryKey.Length == 0 && result.Columns["Count"]!.DataType == typeof(int));
    }

    /// <summary>
    /// Verifies EmptyGroupingComputesNoCallbacks.
    /// </summary>
    private static void EmptyGroupingComputesNoCallbacks()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();
        var calls = 0;

        // Act
        var result = table
            .Assign("Computed",
                _ => ++calls);

        // Assert
        Check(calls == 0 && result.Columns.Contains("Computed"));
    }

    #endregion
}
