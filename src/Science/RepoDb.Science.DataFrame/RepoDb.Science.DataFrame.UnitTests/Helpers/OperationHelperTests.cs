using System.Data;
using RepoDb.Science.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Verifies shared operation contracts and key equality.
/// </summary>
internal static class OperationHelperTests
{
    #region Public Methods

    /// <summary>
    /// Runs helper boundary scenarios.
    /// </summary>
    public static void Run()
    {
        CompositeKeysCompareThroughObjectEquality();
        CompositeKeysRejectOtherObjects();
        CompositeKeysRejectNullKeys();
        CompositeKeyHashesNormalizeMissingValues();
        ScalarRejectsInvalidFunction();
        RenameAvoidsExistingTemporaryNames();
        RenameAvoidsReservedOutputNames();
        ComparerRejectsNullRows();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks boxed composite key equality.
    /// </summary>
    private static void CompositeKeysCompareThroughObjectEquality()
    {
        // Setup
        var left = new RowKey(new object?[] { 1, "A" });
        object right = new RowKey(new object?[] { 1, "A" });

        // Act
        var result = left.Equals(right);

        // Assert
        Check(result);
    }

    /// <summary>
    /// Checks rejection of unrelated boxed values.
    /// </summary>
    private static void CompositeKeysRejectOtherObjects()
    {
        // Setup
        var key = new RowKey(new object?[] { 1 });

        // Act
        var result = key.Equals(new object());

        // Assert
        Check(!result);
    }

    /// <summary>
    /// Checks typed null equality.
    /// </summary>
    private static void CompositeKeysRejectNullKeys()
    {
        // Setup
        var key = new RowKey(new object?[] { 1 });

        // Act
        var result = key.Equals((RowKey?)null);

        // Assert
        Check(!result);
    }

    /// <summary>
    /// Checks normalized null hashing.
    /// </summary>
    private static void CompositeKeyHashesNormalizeMissingValues()
    {
        // Setup
        var left = new RowKey(new object?[] { null });
        var right = new RowKey(new object?[] { DBNull.Value });

        // Act
        var equal = left.Equals(right);
        var sameHash = left.GetHashCode() == right.GetHashCode();

        // Assert
        Check(equal && sameHash && left.HasNull);
    }

    /// <summary>
    /// Checks invalid internal summary requests.
    /// </summary>
    private static void ScalarRejectsInvalidFunction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => AggregationHelper.Scalar(table,
            "Value",
            (AggregateFunction)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Checks temporary names against existing names.
    /// </summary>
    private static void RenameAvoidsExistingTemporaryNames()
    {
        // Setup
        var table = TestTable.Keys(1);
        table.Columns[0].ColumnName = "__RepoDb_0";
        var names = new Dictionary<string, string>(StringComparer.Ordinal) { ["__RepoDb_0"] = "Renamed" };

        // Act
        var result = table
            .RenameColumns(names);

        // Assert
        Check(result.Columns[0].ColumnName is "Renamed" && (int)result.Rows[0][0] == 1);
    }

    /// <summary>
    /// Checks temporary names against final names.
    /// </summary>
    private static void RenameAvoidsReservedOutputNames()
    {
        // Setup
        var table = TestTable.Keys(1);
        var names = new Dictionary<string, string>(StringComparer.Ordinal) { ["Id"] = "__RepoDb_0" };

        // Act
        var result = table
            .RenameColumns(names,
                true);

        // Assert
        Check(result.Columns[0].ColumnName is "__RepoDb_0");
    }

    /// <summary>
    /// Checks comparer null argument validation.
    /// </summary>
    private static void ComparerRejectsNullRows()
    {
        // Setup
        var table = TestTable.Keys(1);
        var comparer = new RowComparer(new[] { table.Columns[0] },
            new[] { true },
            true);

        // Act
        Action left = () => comparer.Compare(null,
            table.Rows[0]);
        Action right = () => comparer.Compare(table.Rows[0],
            null);

        // Assert
        Throws<ArgumentNullException>(left);
        Throws<ArgumentNullException>(right);
    }

    #endregion
}
