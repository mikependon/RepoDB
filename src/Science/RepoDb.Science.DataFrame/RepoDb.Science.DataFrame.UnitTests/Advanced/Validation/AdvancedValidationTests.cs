using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies validation additions and failure behavior.
/// </summary>
internal static class AdvancedValidationTests
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
        RunBatch6();
        RunBatch7();
        RunBatch8();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        IsNullRejectsNullSource();
        NotNullRejectsNullSource();
        InterpolateRejectsNullSource();
        SelectDTypesRejectsNullSource();
        TopNRejectsNullSource();
        BottomNRejectsNullSource();
        ClipRejectsNullSource();
        RoundRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        AbsRejectsNullSource();
        ExplodeRejectsNullSource();
        CountRejectsNullSource();
        QuantileRejectsNullSource();
        MedianRejectsNullSource();
        ModeRejectsNullSource();
        VarianceRejectsNullSource();
        StdRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        CumSumRejectsNullSource();
        CumProductRejectsNullSource();
        CumMinRejectsNullSource();
        CumMaxRejectsNullSource();
        ShiftRejectsNullSource();
        DiffRejectsNullSource();
        PctChangeRejectsNullSource();
        DuplicatedRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        IsInRejectsNullSource();
        BetweenRejectsNullSource();
        RollingRejectsNullSource();
        ExpandingRejectsNullSource();
        EwmRejectsNullSource();
        CorrRejectsNullSource();
        CovRejectsNullSource();
        CorrMatrixRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch5()
    {
        PivotRejectsNullSource();
        PivotTableRejectsNullSource();
        MeltRejectsNullSource();
        TransposeRejectsNullSource();
        InterpolateRejectsNonnumericColumn();
        MedianRejectsNonnumericColumn();
        QuantileRejectsNonnumericColumn();
        VarianceRejectsNonnumericColumn();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch6()
    {
        StdRejectsNonnumericColumn();
        CumSumRejectsNonnumericColumn();
        CumProductRejectsNonnumericColumn();
        CumMinRejectsNonnumericColumn();
        CumMaxRejectsNonnumericColumn();
        DiffRejectsNonnumericColumn();
        PctChangeRejectsNonnumericColumn();
        RollingRejectsNonnumericColumn();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch7()
    {
        ExpandingRejectsNonnumericColumn();
        EwmRejectsNonnumericColumn();
        AbsRejectsNonnumericColumn();
        RoundRejectsNonnumericColumn();
        ClipRejectsNonnumericColumn();
        IsNullRejectsMissingColumn();
        NotNullRejectsMissingColumn();
        InterpolateRejectsMissingColumn();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch8()
    {
        MedianRejectsMissingColumn();
        ModeRejectsMissingColumn();
        CountRejectsMissingColumn();
        TopNRejectsMissingColumn();
        BottomNRejectsMissingColumn();
        ShiftRejectsMissingColumn();
        ExplodeRejectsMissingColumn();
        ExplicitNullColumnIsRejected();
    }

    /// <summary>
    /// Verifies is null rejects null source.
    /// </summary>
    private static void IsNullRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .IsNull("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies not null rejects null source.
    /// </summary>
    private static void NotNullRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .NotNull("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies interpolate rejects null source.
    /// </summary>
    private static void InterpolateRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Interpolate("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies select dtypes rejects null source.
    /// </summary>
    private static void SelectDTypesRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .SelectDTypes(typeof(int));

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies top nrejects null source.
    /// </summary>
    private static void TopNRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .TopN("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies bottom nrejects null source.
    /// </summary>
    private static void BottomNRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .BottomN("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies clip rejects null source.
    /// </summary>
    private static void ClipRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Clip("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies round rejects null source.
    /// </summary>
    private static void RoundRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Round("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies abs rejects null source.
    /// </summary>
    private static void AbsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Abs("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies explode rejects null source.
    /// </summary>
    private static void ExplodeRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Explode("Key");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies count rejects null source.
    /// </summary>
    private static void CountRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Count("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies quantile rejects null source.
    /// </summary>
    private static void QuantileRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Quantile("Count",
                0.5);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies median rejects null source.
    /// </summary>
    private static void MedianRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Median("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies mode rejects null source.
    /// </summary>
    private static void ModeRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Mode("Key");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies variance rejects null source.
    /// </summary>
    private static void VarianceRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Variance("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies std rejects null source.
    /// </summary>
    private static void StdRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Std("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies cum sum rejects null source.
    /// </summary>
    private static void CumSumRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CumSum("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies cum product rejects null source.
    /// </summary>
    private static void CumProductRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CumProduct("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies cum min rejects null source.
    /// </summary>
    private static void CumMinRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CumMin("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies cum max rejects null source.
    /// </summary>
    private static void CumMaxRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CumMax("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies shift rejects null source.
    /// </summary>
    private static void ShiftRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Shift("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies diff rejects null source.
    /// </summary>
    private static void DiffRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Diff("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies pct change rejects null source.
    /// </summary>
    private static void PctChangeRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .PctChange("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies duplicated rejects null source.
    /// </summary>
    private static void DuplicatedRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Duplicated();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies is in rejects null source.
    /// </summary>
    private static void IsInRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .IsIn("Count",
                new object?[] { 1 });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies between rejects null source.
    /// </summary>
    private static void BetweenRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Between("Count",
                1,
                3);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies rolling rejects null source.
    /// </summary>
    private static void RollingRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Rolling("Count",
                2);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies expanding rejects null source.
    /// </summary>
    private static void ExpandingRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Expanding("Count");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects null source.
    /// </summary>
    private static void EwmRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Ewm("Count",
                0.5);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies corr rejects null source.
    /// </summary>
    private static void CorrRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Corr("Count",
                "Value");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies cov rejects null source.
    /// </summary>
    private static void CovRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Cov("Count",
                "Value");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies corr matrix rejects null source.
    /// </summary>
    private static void CorrMatrixRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CorrMatrix();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies pivot rejects null source.
    /// </summary>
    private static void PivotRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Pivot("Key",
                "Count",
                "Value");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies pivot table rejects null source.
    /// </summary>
    private static void PivotTableRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .PivotTable("Key",
                "Count",
                "Value");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies melt rejects null source.
    /// </summary>
    private static void MeltRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Melt(new[] { "Key" });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies transpose rejects null source.
    /// </summary>
    private static void TransposeRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Transpose();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies interpolate rejects nonnumeric column.
    /// </summary>
    private static void InterpolateRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Interpolate("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies median rejects nonnumeric column.
    /// </summary>
    private static void MedianRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Median("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies quantile rejects nonnumeric column.
    /// </summary>
    private static void QuantileRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Quantile("Key",
                0.5);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies variance rejects nonnumeric column.
    /// </summary>
    private static void VarianceRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Variance("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies std rejects nonnumeric column.
    /// </summary>
    private static void StdRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Std("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies cum sum rejects nonnumeric column.
    /// </summary>
    private static void CumSumRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CumSum("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies cum product rejects nonnumeric column.
    /// </summary>
    private static void CumProductRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CumProduct("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies cum min rejects nonnumeric column.
    /// </summary>
    private static void CumMinRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CumMin("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies cum max rejects nonnumeric column.
    /// </summary>
    private static void CumMaxRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CumMax("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies diff rejects nonnumeric column.
    /// </summary>
    private static void DiffRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Diff("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies pct change rejects nonnumeric column.
    /// </summary>
    private static void PctChangeRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .PctChange("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies rolling rejects nonnumeric column.
    /// </summary>
    private static void RollingRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Rolling("Key",
                1);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies expanding rejects nonnumeric column.
    /// </summary>
    private static void ExpandingRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Expanding("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies ewm rejects nonnumeric column.
    /// </summary>
    private static void EwmRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Ewm("Key",
                0.5);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies abs rejects nonnumeric column.
    /// </summary>
    private static void AbsRejectsNonnumericColumn()
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
    /// Verifies round rejects nonnumeric column.
    /// </summary>
    private static void RoundRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Round("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies clip rejects nonnumeric column.
    /// </summary>
    private static void ClipRejectsNonnumericColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Clip("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies is null rejects missing column.
    /// </summary>
    private static void IsNullRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .IsNull("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies not null rejects missing column.
    /// </summary>
    private static void NotNullRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .NotNull("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies interpolate rejects missing column.
    /// </summary>
    private static void InterpolateRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Interpolate("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies median rejects missing column.
    /// </summary>
    private static void MedianRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Median("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies mode rejects missing column.
    /// </summary>
    private static void ModeRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Mode("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies count rejects missing column.
    /// </summary>
    private static void CountRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Count("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies top nrejects missing column.
    /// </summary>
    private static void TopNRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .TopN("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies bottom nrejects missing column.
    /// </summary>
    private static void BottomNRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .BottomN("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies shift rejects missing column.
    /// </summary>
    private static void ShiftRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Shift("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies explode rejects missing column.
    /// </summary>
    private static void ExplodeRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Explode("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies explicit null column is rejected.
    /// </summary>
    private static void ExplicitNullColumnIsRejected()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .IsNull(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
