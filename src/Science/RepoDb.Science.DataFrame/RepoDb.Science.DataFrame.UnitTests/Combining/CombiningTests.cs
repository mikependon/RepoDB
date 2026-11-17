using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies combining operations and failure behavior.
/// </summary>
internal static class CombiningTests
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
        InnerJoinHandlesManyToManyAndMissingKeys();
        LeftJoinHandlesManyToManyAndMissingKeys();
        RightJoinHandlesManyToManyAndMissingKeys();
        FullJoinHandlesManyToManyAndMissingKeys();
        MergeExplicitKeysUsesJoinBehavior();
        JoinCompositeKeysRequiresEveryComponent();
        MergeSupportsCompositeKeys();
        JoinSupportsDistinctKeyNames();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        OuterJoinAllowsMissingOutputValues();
        JoinWithEmptyRightPreservesLeftRows();
        JoinWithEmptyLeftPreservesRightRows();
        JoinSkipsDeletedRows();
        ConcatCopiesMultipleTables();
        ConcatWithNoOthersMakesIndependentCopy();
        ConcatSkipsDeletedRows();
        JoinRejectsInvalidMode();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        JoinRejectsMissingKey();
        JoinRejectsEmptyKeys();
        JoinRejectsMismatchedKeyCounts();
        JoinRejectsNullRight();
        JoinRejectsEmptySuffix();
        JoinRejectsNullKeys();
        ConcatRejectsNullArray();
        ConcatRejectsNullTable();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        JoinRejectsMismatchedKeyTypes();
        JoinRejectsSuffixCollisions();
        ConcatRejectsDifferentNames();
        ConcatRejectsDifferentTypes();
    }

    /// <summary>
    /// Verifies InnerJoinHandlesManyToManyAndMissingKeys.
    /// </summary>
    private static void InnerJoinHandlesManyToManyAndMissingKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Inner);

        // Assert
        Check(result.Rows.Count == 4 && result.Columns[1].ColumnName is "Id_right");
        Check(left.Rows.Count == 4 && right.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies LeftJoinHandlesManyToManyAndMissingKeys.
    /// </summary>
    private static void LeftJoinHandlesManyToManyAndMissingKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Left);

        // Assert
        Check(result.Rows.Count == 6 && result.Columns[1].ColumnName is "Id_right");
        Check(left.Rows.Count == 4 && right.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies RightJoinHandlesManyToManyAndMissingKeys.
    /// </summary>
    private static void RightJoinHandlesManyToManyAndMissingKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Right);

        // Assert
        Check(result.Rows.Count == 6 && result.Columns[1].ColumnName is "Id_right");
        Check(left.Rows.Count == 4 && right.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies FullJoinHandlesManyToManyAndMissingKeys.
    /// </summary>
    private static void FullJoinHandlesManyToManyAndMissingKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Full);

        // Assert
        Check(result.Rows.Count == 8 && result.Columns[1].ColumnName is "Id_right");
        Check(left.Rows.Count == 4 && right.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies MergeExplicitKeysUsesJoinBehavior.
    /// </summary>
    private static void MergeExplicitKeysUsesJoinBehavior()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Merge(right,
                leftOn: "Id",
                rightOn: "Id",
                how: JoinType.Left);

        // Assert
        Check(result.Rows.Count == 6);
    }

    /// <summary>
    /// Verifies JoinCompositeKeysRequiresEveryComponent.
    /// </summary>
    private static void JoinCompositeKeysRequiresEveryComponent()
    {
        // Setup
        var table = TestTable.Create();
        var right = table
            .SelectColumns("Key",
                "Count");

        // Act
        var result = table
            .Join(right,
                new[] { "Key", "Count" },
                new[] { "Key", "Count" });

        // Assert
        Check(result.Rows.Count == 6);
    }

    /// <summary>
    /// Verifies MergeSupportsCompositeKeys.
    /// </summary>
    private static void MergeSupportsCompositeKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        var result = left
            .Merge(right,
                new[] { "Id" },
                new[] { "Id" },
                JoinType.Full);

        // Assert
        Check(result.Rows.Count == 8);
    }

    /// <summary>
    /// Verifies JoinSupportsDistinctKeyNames.
    /// </summary>
    private static void JoinSupportsDistinctKeyNames()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = TestTable.Keys(1);
        right.Columns[0].ColumnName = "Other";

        // Act
        var result = left
            .Join(right,
                "Id",
                "Other");

        // Assert
        Check(result.Rows.Count == 1 && result.Columns[1].ColumnName is "Other");
    }

    /// <summary>
    /// Verifies OuterJoinAllowsMissingOutputValues.
    /// </summary>
    private static void OuterJoinAllowsMissingOutputValues()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = TestTable.Keys(2);
        left.Columns[0].AllowDBNull = false;
        right.Columns[0].AllowDBNull = false;

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Full);

        // Assert
        Check(result.Rows[0].IsNull("Id_right") && result.Rows[1].IsNull("Id"));
    }

    /// <summary>
    /// Verifies JoinWithEmptyRightPreservesLeftRows.
    /// </summary>
    private static void JoinWithEmptyRightPreservesLeftRows()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = TestTable.Keys();

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Left);

        // Assert
        Check(result.Rows.Count == 1 && result.Rows[0].IsNull("Id_right"));
    }

    /// <summary>
    /// Verifies JoinWithEmptyLeftPreservesRightRows.
    /// </summary>
    private static void JoinWithEmptyLeftPreservesRightRows()
    {
        // Setup
        var left = TestTable.Keys();
        var right = TestTable.Keys(1);

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id",
                JoinType.Right);

        // Assert
        Check(result.Rows.Count == 1 && result.Rows[0].IsNull("Id"));
    }

    /// <summary>
    /// Verifies JoinSkipsDeletedRows.
    /// </summary>
    private static void JoinSkipsDeletedRows()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);
        left.AcceptChanges();
        left.Rows[1].Delete();

        // Act
        var result = left
            .Join(right,
                "Id",
                "Id");

        // Assert
        Check(result.Rows.Count == 2);
    }

    /// <summary>
    /// Verifies ConcatCopiesMultipleTables.
    /// </summary>
    private static void ConcatCopiesMultipleTables()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Concat(table,
                table);

        // Assert
        Check(result.Rows.Count == 12 && table.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies ConcatWithNoOthersMakesIndependentCopy.
    /// </summary>
    private static void ConcatWithNoOthersMakesIndependentCopy()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Concat();
        result.Rows[0]["Value"] = 99m;

        // Assert
        Check((decimal)table.Rows[0]["Value"] == 10m);
    }

    /// <summary>
    /// Verifies ConcatSkipsDeletedRows.
    /// </summary>
    private static void ConcatSkipsDeletedRows()
    {
        // Setup
        var table = TestTable.Create();
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        var result = table
            .Concat(table);

        // Assert
        Check(result.Rows.Count == 6);
    }

    /// <summary>
    /// Verifies JoinRejectsInvalidMode.
    /// </summary>
    private static void JoinRejectsInvalidMode()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                "Id",
                "Id",
                (JoinType)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsMissingKey.
    /// </summary>
    private static void JoinRejectsMissingKey()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                "Missing",
                "Id");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsEmptyKeys.
    /// </summary>
    private static void JoinRejectsEmptyKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                Array.Empty<string>(),
                Array.Empty<string>());

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsMismatchedKeyCounts.
    /// </summary>
    private static void JoinRejectsMismatchedKeyCounts()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                new[] { "Id" },
                Array.Empty<string>());

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsNullRight.
    /// </summary>
    private static void JoinRejectsNullRight()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(null!,
                "Id",
                "Id");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsEmptySuffix.
    /// </summary>
    private static void JoinRejectsEmptySuffix()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                "Id",
                "Id",
                rightSuffix: "");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsNullKeys.
    /// </summary>
    private static void JoinRejectsNullKeys()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Join(right,
                (string[])null!,
                new[] { "Id" });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ConcatRejectsNullArray.
    /// </summary>
    private static void ConcatRejectsNullArray()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Concat(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ConcatRejectsNullTable.
    /// </summary>
    private static void ConcatRejectsNullTable()
    {
        // Setup
        var left = TestTable.Keys(1,
            2,
            2,
            DBNull.Value);
        var right = TestTable.Keys(2,
            2,
            3,
            DBNull.Value);

        // Act
        Action operation = () => left
            .Concat(new DataTable[] { null! });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsMismatchedKeyTypes.
    /// </summary>
    private static void JoinRejectsMismatchedKeyTypes()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = left
            .CastColumn<string>("Id");

        // Act
        Action operation = () => left
            .Join(right,
                "Id",
                "Id");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsSuffixCollisions.
    /// </summary>
    private static void JoinRejectsSuffixCollisions()
    {
        // Setup
        var left = TestTable.Keys(1);
        left.Columns.Add("Id_right",
            typeof(int));
        var right = TestTable.Keys(1);

        // Act
        Action operation = () => left
            .Join(right,
                "Id",
                "Id");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies ConcatRejectsDifferentNames.
    /// </summary>
    private static void ConcatRejectsDifferentNames()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = TestTable.Keys(1);
        right.Columns[0].ColumnName = "Other";

        // Act
        Action operation = () => left
            .Concat(right);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies ConcatRejectsDifferentTypes.
    /// </summary>
    private static void ConcatRejectsDifferentTypes()
    {
        // Setup
        var left = TestTable.Keys(1);
        var right = left
            .CastColumn<long>("Id");

        // Act
        Action operation = () => left
            .Concat(right);

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
