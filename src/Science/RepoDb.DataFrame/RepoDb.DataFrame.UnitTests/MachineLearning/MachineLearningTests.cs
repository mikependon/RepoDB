using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies machinelearning operations and failure behavior.
/// </summary>
internal static class MachineLearningTests
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
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        SplitIsReproducibleAndDisjoint();
        SplitRowsPartitionSourcePositions();
        SplitUsesIndependentTables();
        UnseededSplitMaintainsNonemptySets();
        SplitRoundsSmallFractionsUp();
        SplitRejectsZeroFraction();
        SplitRejectsNegativeFraction();
        SplitRejectsOneFraction();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        SplitRejectsTooLargeFraction();
        SplitRejectsNaNFraction();
        SplitRejectsInfinityFraction();
        SplitRejectsNegativeInfinityFraction();
        SplitRejectsInsufficientRows();
        SplitRejectsDeletedOnlyTrainingSource();
        FeatureMatrixUsesNumericColumnsByDefault();
        FeatureMatrixPreservesRequestedOrder();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        FeatureMatrixSupportsEmptyNumericSelection();
        FeatureMatrixSupportsEmptyRows();
        FeatureMatrixRejectsMissingValues();
        FeatureMatrixRejectsTextFeatures();
        FeatureMatrixRejectsMissingColumn();
        FeatureMatrixRejectsDuplicateFeatures();
        FeatureMatrixRejectsNullArray();
        FeatureMatrixRejectsNonfiniteValues();
    }

    /// <summary>
    /// Verifies SplitIsReproducibleAndDisjoint.
    /// </summary>
    private static void SplitIsReproducibleAndDisjoint()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var first = table
            .TrainTestSplit(0.5,
                42);
        var second = table
            .TrainTestSplit(0.5,
                42);

        // Assert
        Check(first.Train.Rows.Count == 2 && first.Test.Rows.Count == 2);
        Check(first.Train.Rows
            .Cast<DataRow>()
            .Select(row => row["Count"])
            .SequenceEqual(second.Train.Rows
            .Cast<DataRow>()
            .Select(row => row["Count"])));
    }

    /// <summary>
    /// Verifies SplitRowsPartitionSourcePositions.
    /// </summary>
    private static void SplitRowsPartitionSourcePositions()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3,
            4,
            5,
            6);

        // Act
        var result = table
            .TrainTestSplit(0.3,
                1);
        var train = result.Train.Rows
            .Cast<DataRow>()
            .Select(row => (int)row[0])
            .ToHashSet();
        var test = result.Test.Rows
            .Cast<DataRow>()
            .Select(row => (int)row[0])
            .ToHashSet();

        // Assert
        Check(train.Count == 4 && test.Count == 2 && !train.Overlaps(test));
        Check(train
            .Union(test)
            .Order()
            .SequenceEqual(new[] { 1, 2, 3, 4, 5, 6 }));
    }

    /// <summary>
    /// Verifies SplitUsesIndependentTables.
    /// </summary>
    private static void SplitUsesIndependentTables()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .TrainTestSplit(seed: 5);
        result.Train.Rows[0]["Count"] = 99;
        result.Test.Rows[0]["Count"] = 99;

        // Assert
        Check(table.Rows
            .Cast<DataRow>()
            .All(row => (int)row["Count"] < 99));
    }

    /// <summary>
    /// Verifies UnseededSplitMaintainsNonemptySets.
    /// </summary>
    private static void UnseededSplitMaintainsNonemptySets()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .TrainTestSplit(0.99);

        // Assert
        Check(result.Train.Rows.Count == 1 && result.Test.Rows.Count == 3);
    }

    /// <summary>
    /// Verifies SplitRoundsSmallFractionsUp.
    /// </summary>
    private static void SplitRoundsSmallFractionsUp()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .TrainTestSplit(double.Epsilon,
                1);

        // Assert
        Check(result.Train.Rows.Count == 1 && result.Test.Rows.Count == 1);
    }

    /// <summary>
    /// Verifies SplitRejectsZeroFraction.
    /// </summary>
    private static void SplitRejectsZeroFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(0);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsNegativeFraction.
    /// </summary>
    private static void SplitRejectsNegativeFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(-0.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsOneFraction.
    /// </summary>
    private static void SplitRejectsOneFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsTooLargeFraction.
    /// </summary>
    private static void SplitRejectsTooLargeFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(1.1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsNaNFraction.
    /// </summary>
    private static void SplitRejectsNaNFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(double.NaN);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsInfinityFraction.
    /// </summary>
    private static void SplitRejectsInfinityFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(double.PositiveInfinity);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsNegativeInfinityFraction.
    /// </summary>
    private static void SplitRejectsNegativeInfinityFraction()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TrainTestSplit(double.NegativeInfinity);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsInsufficientRows.
    /// </summary>
    private static void SplitRejectsInsufficientRows()
    {
        // Setup
        var table = TestTable.Keys(1);

        // Act
        Action operation = () => table
            .TrainTestSplit();

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies SplitRejectsDeletedOnlyTrainingSource.
    /// </summary>
    private static void SplitRejectsDeletedOnlyTrainingSource()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        Action operation = () => table
            .TrainTestSplit();

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixUsesNumericColumnsByDefault.
    /// </summary>
    private static void FeatureMatrixUsesNumericColumnsByDefault()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls("Value")
            .ToFeatureMatrix();

        // Assert
        Check(result.GetLength(0) == 3 && result.GetLength(1) == 2);
        Check(result[0, 0] == 10 && result[0, 1] == 1);
    }

    /// <summary>
    /// Verifies FeatureMatrixPreservesRequestedOrder.
    /// </summary>
    private static void FeatureMatrixPreservesRequestedOrder()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .DropNulls("Value")
            .ToFeatureMatrix("Count",
                "Value");

        // Assert
        Check(result[0, 0] == 1 && result[0, 1] == 10);
    }

    /// <summary>
    /// Verifies FeatureMatrixSupportsEmptyNumericSelection.
    /// </summary>
    private static void FeatureMatrixSupportsEmptyNumericSelection()
    {
        // Setup
        var table = TestTable.Create()
            .SelectColumns("Key");

        // Act
        var result = table
            .ToFeatureMatrix();

        // Assert
        Check(result.GetLength(0) == 4 && result.GetLength(1) == 0);
    }

    /// <summary>
    /// Verifies FeatureMatrixSupportsEmptyRows.
    /// </summary>
    private static void FeatureMatrixSupportsEmptyRows()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var result = table
            .ToFeatureMatrix("Value");

        // Assert
        Check(result.GetLength(0) == 0 && result.GetLength(1) == 1);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsMissingValues.
    /// </summary>
    private static void FeatureMatrixRejectsMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .ToFeatureMatrix();

        // Assert
        Throws<InvalidOperationException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsTextFeatures.
    /// </summary>
    private static void FeatureMatrixRejectsTextFeatures()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .ToFeatureMatrix("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsMissingColumn.
    /// </summary>
    private static void FeatureMatrixRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .ToFeatureMatrix("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsDuplicateFeatures.
    /// </summary>
    private static void FeatureMatrixRejectsDuplicateFeatures()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .ToFeatureMatrix("Count",
                "Count");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsNullArray.
    /// </summary>
    private static void FeatureMatrixRejectsNullArray()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .ToFeatureMatrix(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies FeatureMatrixRejectsNonfiniteValues.
    /// </summary>
    private static void FeatureMatrixRejectsNonfiniteValues()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.NaN);

        // Act
        Action operation = () => table
            .ToFeatureMatrix();

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
