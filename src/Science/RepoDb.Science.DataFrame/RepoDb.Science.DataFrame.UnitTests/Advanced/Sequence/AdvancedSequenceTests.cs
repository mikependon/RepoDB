using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies sequence additions and failure behavior.
/// </summary>
internal static class AdvancedSequenceTests
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
        RunBatch5();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        InterpolateFillsOnlyBoundedGaps();
        InterpolateHandlesAdjacentAndEmptyValues();
        InterpolateHandlesAllMissingValues();
        InterpolateHandlesEmptySeries();
        InterpolateSupportsInPlaceDoubleColumn();
        InterpolateRejectsInPlaceTypeChange();
        CumSumResumesAfterMissingValues();
        CumSumStopsAfterMissingValues();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        CumProductResumesAfterMissingValues();
        CumProductStopsAfterMissingValues();
        CumMinResumesAfterMissingValues();
        CumMinStopsAfterMissingValues();
        CumMaxResumesAfterMissingValues();
        CumMaxStopsAfterMissingValues();
        CumulativeMinMaxUpdateForUnsortedValues();
        CumulativeOperationsSupportInPlace();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        CumulativeOperationsCanStopAtLeadingGap();
        CumulativeOverflowIsRejected();
        ShiftPreservesTypeAndIntroducesGaps();
        ShiftSupportsNegativePeriods();
        ShiftZeroCreatesIndependentCopy();
        ShiftSupportsInPlace();
        ShiftSupportsNonnumericColumns();
        ShiftHandlesMinimumOffset();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        ShiftHandlesMaximumOffset();
        DiffHandlesMinimumOffset();
        DiffHandlesMaximumOffset();
        PctChangeHandlesMinimumOffset();
        PctChangeHandlesMaximumOffset();
        DiffComputesSignedOffsets();
        DiffPreservesMissingComparisons();
        DiffCanCompareZeroOffsets();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch5()
    {
        PctChangeReturnsFractionalChange();
        PctChangeDoesNotDivideByZero();
        PctChangePreservesMissingComparisons();
        RowCalculationsSkipDeletedObservations();
    }

    /// <summary>
    /// Verifies interpolate fills only bounded gaps.
    /// </summary>
    private static void InterpolateFillsOnlyBoundedGaps()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value,
            0,
            DBNull.Value,
            DBNull.Value,
            9,
            DBNull.Value);

        // Act
        var result = table
            .Interpolate("Id");

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[5]
            .IsNull(0));
        Check((double)result.Rows[2][0] == 3 && (double)result.Rows[3][0] == 6);
    }

    /// <summary>
    /// Verifies interpolate handles adjacent and empty values.
    /// </summary>
    private static void InterpolateHandlesAdjacentAndEmptyValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Interpolate("Id");

        // Assert
        Check((double)result.Rows[0][0] == 1 && (double)result.Rows[1][0] == 2);
    }

    /// <summary>
    /// Verifies interpolate handles all missing values.
    /// </summary>
    private static void InterpolateHandlesAllMissingValues()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value,
            DBNull.Value);

        // Act
        var result = table
            .Interpolate("Id");

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies interpolate handles empty series.
    /// </summary>
    private static void InterpolateHandlesEmptySeries()
    {
        // Setup
        var table = TestTable.Keys();

        // Act
        var result = table
            .Interpolate("Id");

        // Assert
        Check(result.Rows.Count == 0 && result.Columns[0].DataType == typeof(double));
    }

    /// <summary>
    /// Verifies interpolate supports in place double column.
    /// </summary>
    private static void InterpolateSupportsInPlaceDoubleColumn()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            3)
            .CastColumn<double>("Id");

        // Act
        var result = table
            .Interpolate("Id",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (double)table.Rows[1][0] == 2);
    }

    /// <summary>
    /// Verifies interpolate rejects in place type change.
    /// </summary>
    private static void InterpolateRejectsInPlaceTypeChange()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            3);

        // Act
        Action operation = () => table
            .Interpolate("Id",
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check(table.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cum sum resumes after missing values.
    /// </summary>
    private static void CumSumResumesAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumSum("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && (double)result.Rows[3][0] == 6);
        Check(table.Columns[0].DataType == typeof(int));
    }

    /// <summary>
    /// Verifies cum sum stops after missing values.
    /// </summary>
    private static void CumSumStopsAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumSum("Id",
                skipNulls: false);

        // Assert
        Check((double)result.Rows[0][0] == 1 && result.Rows[2]
            .IsNull(0) && result.Rows[3]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cum product resumes after missing values.
    /// </summary>
    private static void CumProductResumesAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumProduct("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && (double)result.Rows[3][0] == 6);
        Check(table.Columns[0].DataType == typeof(int));
    }

    /// <summary>
    /// Verifies cum product stops after missing values.
    /// </summary>
    private static void CumProductStopsAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumProduct("Id",
                skipNulls: false);

        // Assert
        Check((double)result.Rows[0][0] == 1 && result.Rows[2]
            .IsNull(0) && result.Rows[3]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cum min resumes after missing values.
    /// </summary>
    private static void CumMinResumesAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumMin("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && (double)result.Rows[3][0] == 1);
        Check(table.Columns[0].DataType == typeof(int));
    }

    /// <summary>
    /// Verifies cum min stops after missing values.
    /// </summary>
    private static void CumMinStopsAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumMin("Id",
                skipNulls: false);

        // Assert
        Check((double)result.Rows[0][0] == 1 && result.Rows[2]
            .IsNull(0) && result.Rows[3]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cum max resumes after missing values.
    /// </summary>
    private static void CumMaxResumesAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumMax("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && (double)result.Rows[3][0] == 3);
        Check(table.Columns[0].DataType == typeof(int));
    }

    /// <summary>
    /// Verifies cum max stops after missing values.
    /// </summary>
    private static void CumMaxStopsAfterMissingValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            2,
            3);

        // Act
        var result = table
            .CumMax("Id",
                skipNulls: false);

        // Assert
        Check((double)result.Rows[0][0] == 1 && result.Rows[2]
            .IsNull(0) && result.Rows[3]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cumulative min max update for unsorted values.
    /// </summary>
    private static void CumulativeMinMaxUpdateForUnsortedValues()
    {
        // Setup
        var table = TestTable.Keys(3,
            1,
            2);

        // Act
        var minimum = table
            .CumMin("Id");
        var maximum = table
            .CumMax("Id");

        // Assert
        Check((double)minimum.Rows[1][0] == 1 && (double)maximum.Rows[2][0] == 3);
    }

    /// <summary>
    /// Verifies cumulative operations support in place.
    /// </summary>
    private static void CumulativeOperationsSupportInPlace()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3)
            .CastColumn<double>("Id");

        // Act
        var result = table
            .CumSum("Id",
                inPlace: true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (double)table.Rows[2][0] == 6);
    }

    /// <summary>
    /// Verifies cumulative operations can stop at leading gap.
    /// </summary>
    private static void CumulativeOperationsCanStopAtLeadingGap()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value,
            1);

        // Act
        var result = table
            .CumProduct("Id",
                false);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies cumulative overflow is rejected.
    /// </summary>
    private static void CumulativeOverflowIsRejected()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.MaxValue);
        table.Rows.Add(double.MaxValue);

        // Act
        Action operation = () => table
            .CumSum("Value");

        // Assert
        Throws<ArithmeticException>(operation);
    }

    /// <summary>
    /// Verifies shift preserves type and introduces gaps.
    /// </summary>
    private static void ShiftPreservesTypeAndIntroducesGaps()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Shift("Id");

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && (int)result.Rows[1][0] == 1);
        Check(result.Columns[0].DataType == typeof(int));
    }

    /// <summary>
    /// Verifies shift supports negative periods.
    /// </summary>
    private static void ShiftSupportsNegativePeriods()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Shift("Id",
                -1);

        // Assert
        Check((int)result.Rows[0][0] == 2 && result.Rows[2]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies shift zero creates independent copy.
    /// </summary>
    private static void ShiftZeroCreatesIndependentCopy()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Shift("Id",
                0);
        result.Rows[0][0] = 9;

        // Assert
        Check((int)table.Rows[0][0] == 1 && (int)result.Rows[1][0] == 2);
    }

    /// <summary>
    /// Verifies shift supports in place.
    /// </summary>
    private static void ShiftSupportsInPlace()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);

        // Act
        var result = table
            .Shift("Id",
                1,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && table.Rows[0]
            .IsNull(0) && (int)table.Rows[2][0] == 2);
    }

    /// <summary>
    /// Verifies shift supports nonnumeric columns.
    /// </summary>
    private static void ShiftSupportsNonnumericColumns()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Shift("Key");

        // Assert
        Check(result.Rows[0]
            .IsNull("Key") && result.Rows[1]["Key"] is "A");
    }

    /// <summary>
    /// Verifies shift handles minimum offset.
    /// </summary>
    private static void ShiftHandlesMinimumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Shift("Id",
                int.MinValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies shift handles maximum offset.
    /// </summary>
    private static void ShiftHandlesMaximumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Shift("Id",
                int.MaxValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies diff handles minimum offset.
    /// </summary>
    private static void DiffHandlesMinimumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Diff("Id",
                int.MinValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies diff handles maximum offset.
    /// </summary>
    private static void DiffHandlesMaximumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .Diff("Id",
                int.MaxValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies pct change handles minimum offset.
    /// </summary>
    private static void PctChangeHandlesMinimumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .PctChange("Id",
                int.MinValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies pct change handles maximum offset.
    /// </summary>
    private static void PctChangeHandlesMaximumOffset()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        var result = table
            .PctChange("Id",
                int.MaxValue);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies diff computes signed offsets.
    /// </summary>
    private static void DiffComputesSignedOffsets()
    {
        // Setup
        var table = TestTable.Keys(1,
            3,
            8);

        // Act
        var result = table
            .Diff("Id");
        var future = table
            .Diff("Id",
                -1);

        // Assert
        Check(result.Rows[0]
            .IsNull(0) && (double)result.Rows[2][0] == 5);
        Check((double)future.Rows[0][0] == -2 && future.Rows[2]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies diff preserves missing comparisons.
    /// </summary>
    private static void DiffPreservesMissingComparisons()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value,
            3);

        // Act
        var result = table
            .Diff("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && result.Rows[2]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies diff can compare zero offsets.
    /// </summary>
    private static void DiffCanCompareZeroOffsets()
    {
        // Setup
        var table = TestTable.Keys(1,
            DBNull.Value);

        // Act
        var result = table
            .Diff("Id",
                0);

        // Assert
        Check((double)result.Rows[0][0] == 0 && result.Rows[1]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies pct change returns fractional change.
    /// </summary>
    private static void PctChangeReturnsFractionalChange()
    {
        // Setup
        var table = TestTable.Keys(10,
            15,
            30);

        // Act
        var result = table
            .PctChange("Id");

        // Assert
        Check((double)result.Rows[1][0] == 0.5 && (double)result.Rows[2][0] == 1);
    }

    /// <summary>
    /// Verifies pct change does not divide by zero.
    /// </summary>
    private static void PctChangeDoesNotDivideByZero()
    {
        // Setup
        var table = TestTable.Keys(0,
            1,
            0);

        // Act
        var result = table
            .PctChange("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && (double)result.Rows[2][0] == -1);
    }

    /// <summary>
    /// Verifies pct change preserves missing comparisons.
    /// </summary>
    private static void PctChangePreservesMissingComparisons()
    {
        // Setup
        var table = TestTable.Keys(DBNull.Value,
            1,
            DBNull.Value);

        // Act
        var result = table
            .PctChange("Id");

        // Assert
        Check(result.Rows[1]
            .IsNull(0) && result.Rows[2]
            .IsNull(0));
    }

    /// <summary>
    /// Verifies row calculations skip deleted observations.
    /// </summary>
    private static void RowCalculationsSkipDeletedObservations()
    {
        // Setup
        var table = TestTable.Keys(1,
            100,
            3);
        table.AcceptChanges();
        table.Rows[1].Delete();

        // Act
        var result = table
            .Diff("Id");

        // Assert
        Check(result.Rows.Count == 2 && (double)result.Rows[1][0] == 2);
    }

    #endregion
}
