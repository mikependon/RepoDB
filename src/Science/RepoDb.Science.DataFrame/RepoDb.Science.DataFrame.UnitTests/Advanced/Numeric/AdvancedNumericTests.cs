using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies numeric additions and failure behavior.
/// </summary>
internal static class AdvancedNumericTests
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
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        AbsPreservesDecimalAndMissingValues();
        AbsSupportsInPlaceIntegers();
        AbsChecksIntegerOverflow();
        AbsSupportsSingle();
        AbsSupportsDouble();
        AbsPreservesUnsignedPrecision();
        ClipBoundsDecimalValues();
        ClipSupportsOnlyLowerBound();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        ClipSupportsOnlyUpperBound();
        ClipWithoutBoundsPreservesValues();
        ClipFloatingBothBounds();
        ClipFloatingLowerBounds();
        ClipFloatingUpperBounds();
        ClipFloatingNeitherBounds();
        RoundSupportsDecimalRoundingModes();
        RoundSupportsFloatingAndInPlace();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        ClipRejectsReversedBounds();
        RoundRejectsNegativePrecision();
        RoundRejectsExcessPrecision();
        RoundRejectsInvalidMode();
        NumericTransformRejectsText();
        NumericTransformRejectsNonfiniteInput();
        ClipSupportsIntegralColumns();
        ClipRejectsFractionalLowerForIntegers();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        ClipRejectsFractionalUpperForIntegers();
        ClipSupportsSinglePrecision();
    }

    /// <summary>
    /// Verifies abs preserves decimal and missing values.
    /// </summary>
    private static void AbsPreservesDecimalAndMissingValues()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Value"] = -10m;

        // Act
        var result = table
            .Abs("Value");

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 10m && result.Rows[1]
            .IsNull("Value"));
        Check((decimal)table.Rows[0]["Value"] == -10m);
    }

    /// <summary>
    /// Verifies abs supports in place integers.
    /// </summary>
    private static void AbsSupportsInPlaceIntegers()
    {
        // Setup
        var table = TestTable.Keys(-2,
            -1);

        // Act
        var result = table
            .Abs("Id",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0][0] == 2);
    }

    /// <summary>
    /// Verifies abs checks integer overflow.
    /// </summary>
    private static void AbsChecksIntegerOverflow()
    {
        // Setup
        var table = TestTable.Keys(int.MinValue);

        // Act
        Action operation = () => table
            .Abs("Id",
                true);

        // Assert
        Throws<OverflowException>(operation);
        Check((int)table.Rows[0][0] == int.MinValue);
    }

    /// <summary>
    /// Verifies abs supports single.
    /// </summary>
    private static void AbsSupportsSingle()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(float));
        table.Rows.Add(-1.5f);

        // Act
        var result = table
            .Abs("Value");

        // Assert
        Check(Convert.ToDouble(result.Rows[0][0],
            System.Globalization.CultureInfo.InvariantCulture) == 1.5);
    }

    /// <summary>
    /// Verifies abs supports double.
    /// </summary>
    private static void AbsSupportsDouble()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(-1.5);

        // Act
        var result = table
            .Abs("Value");

        // Assert
        Check(Convert.ToDouble(result.Rows[0][0],
            System.Globalization.CultureInfo.InvariantCulture) == 1.5);
    }

    /// <summary>
    /// Verifies abs preserves unsigned precision.
    /// </summary>
    private static void AbsPreservesUnsignedPrecision()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(ulong));
        table.Rows.Add(ulong.MaxValue);

        // Act
        var result = table
            .Abs("Value");

        // Assert
        Check((ulong)result.Rows[0][0] == ulong.MaxValue);
    }

    /// <summary>
    /// Verifies clip bounds decimal values.
    /// </summary>
    private static void ClipBoundsDecimalValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Clip("Value",
                15m,
                25m);

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 15m && (decimal)result.Rows[2]["Value"] == 25m);
        Check(result.Rows[1]
            .IsNull("Value"));
    }

    /// <summary>
    /// Verifies clip supports only lower bound.
    /// </summary>
    private static void ClipSupportsOnlyLowerBound()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Clip("Value",
                lower: 20m,
                inPlace: true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (decimal)table.Rows[0]["Value"] == 20m);
    }

    /// <summary>
    /// Verifies clip supports only upper bound.
    /// </summary>
    private static void ClipSupportsOnlyUpperBound()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Clip("Value",
                upper: 20m);

        // Assert
        Check((decimal)result.Rows[2]["Value"] == 20m);
    }

    /// <summary>
    /// Verifies clip without bounds preserves values.
    /// </summary>
    private static void ClipWithoutBoundsPreservesValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Clip("Value");

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 10m);
    }

    /// <summary>
    /// Verifies clip floating both bounds.
    /// </summary>
    private static void ClipFloatingBothBounds()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(-1.5);
        table.Rows.Add(3.5);

        // Act
        var result = table
            .Clip("Value",
                1m,
                2m);

        // Assert
        Check(result.Columns[0].DataType == typeof(double));
        Check((double)result.Rows[0][0] == 1 && (double)result.Rows[1][0] == 2);
    }

    /// <summary>
    /// Verifies clip floating lower bounds.
    /// </summary>
    private static void ClipFloatingLowerBounds()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(-1.5);
        table.Rows.Add(3.5);

        // Act
        var result = table
            .Clip("Value",
                lower: 1m);

        // Assert
        Check(result.Columns[0].DataType == typeof(double));
        Check((double)result.Rows[0][0] == 1 && (double)result.Rows[1][0] == 3.5);
    }

    /// <summary>
    /// Verifies clip floating upper bounds.
    /// </summary>
    private static void ClipFloatingUpperBounds()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(-1.5);
        table.Rows.Add(3.5);

        // Act
        var result = table
            .Clip("Value",
                upper: 2m);

        // Assert
        Check(result.Columns[0].DataType == typeof(double));
        Check((double)result.Rows[0][0] == -1.5 && (double)result.Rows[1][0] == 2);
    }

    /// <summary>
    /// Verifies clip floating neither bounds.
    /// </summary>
    private static void ClipFloatingNeitherBounds()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(-1.5);
        table.Rows.Add(3.5);

        // Act
        var result = table
            .Clip("Value");

        // Assert
        Check(result.Columns[0].DataType == typeof(double));
        Check((double)result.Rows[0][0] == -1.5 && (double)result.Rows[1][0] == 3.5);
    }

    /// <summary>
    /// Verifies round supports decimal rounding modes.
    /// </summary>
    private static void RoundSupportsDecimalRoundingModes()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(decimal));
        table.Rows.Add(1.25m);

        // Act
        var even = table
            .Round("Value",
                1);
        var away = table
            .Round("Value",
                1,
                MidpointRounding.AwayFromZero);

        // Assert
        Check((decimal)even.Rows[0][0] == 1.2m && (decimal)away.Rows[0][0] == 1.3m);
    }

    /// <summary>
    /// Verifies round supports floating and in place.
    /// </summary>
    private static void RoundSupportsFloatingAndInPlace()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(1.25);

        // Act
        var result = table
            .Round("Value",
                1,
                MidpointRounding.AwayFromZero,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (double)table.Rows[0][0] == 1.3);
    }

    /// <summary>
    /// Verifies clip rejects reversed bounds.
    /// </summary>
    private static void ClipRejectsReversedBounds()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Clip("Value",
                2m,
                1m);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies round rejects negative precision.
    /// </summary>
    private static void RoundRejectsNegativePrecision()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Round("Value",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies round rejects excess precision.
    /// </summary>
    private static void RoundRejectsExcessPrecision()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Round("Value",
                16);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies round rejects invalid mode.
    /// </summary>
    private static void RoundRejectsInvalidMode()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Round("Value",
                mode: (MidpointRounding)999);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies numeric transform rejects text.
    /// </summary>
    private static void NumericTransformRejectsText()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Abs("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies numeric transform rejects nonfinite input.
    /// </summary>
    private static void NumericTransformRejectsNonfiniteInput()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(double.NaN);

        // Act
        Action operation = () => table
            .Abs("Value");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies clip supports integral columns.
    /// </summary>
    private static void ClipSupportsIntegralColumns()
    {
        // Setup
        var table = TestTable.Keys(-1,
            2,
            5);

        // Act
        var result = table
            .Clip("Id",
                0m,
                3m);
        var unchanged = table
            .Clip("Id");

        // Assert
        Check((int)result.Rows[0][0] == 0 && (int)result.Rows[2][0] == 3);
        Check((int)unchanged.Rows[0][0] == -1);
    }

    /// <summary>
    /// Verifies clip rejects fractional lower for integers.
    /// </summary>
    private static void ClipRejectsFractionalLowerForIntegers()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Clip("Id",
                lower: 1.5m);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies clip rejects fractional upper for integers.
    /// </summary>
    private static void ClipRejectsFractionalUpperForIntegers()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);

        // Act
        Action operation = () => table
            .Clip("Id",
                lower: 0m,
                upper: 1.5m);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies clip supports single precision.
    /// </summary>
    private static void ClipSupportsSinglePrecision()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(float));
        table.Rows.Add(0f);

        // Act
        var result = table
            .Clip("Value",
                0.5m,
                1.5m);

        // Assert
        Check((float)result.Rows[0][0] == 0.5f);
    }

    #endregion
}
