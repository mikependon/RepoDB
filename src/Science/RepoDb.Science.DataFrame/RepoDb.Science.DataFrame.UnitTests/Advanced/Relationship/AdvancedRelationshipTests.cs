using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies relationship additions and failure behavior.
/// </summary>
internal static class AdvancedRelationshipTests
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
        CorrelationAndCovarianceUsePairedValues();
        CorrelationCanBeNegative();
        CorrelationDropsIncompletePairs();
        CorrelationHandlesBothConstantSides();
        CorrelationHandlesEmptyInput();
        CorrelationMatrixDefaultsToNumericColumns();
        CorrelationMatrixSupportsRequestedOrder();
        CorrelationMatrixPreservesUndefinedCells();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        CorrelationMatrixSupportsNoNumericColumns();
        CovarianceRejectsNegativeDegrees();
        CorrelationRejectsMissingColumn();
        CorrelationMatrixRejectsMissingColumn();
        CorrelationMatrixRejectsNullSelection();
        CorrelationMatrixRejectsDuplicateSelection();
        CorrelationMatrixRejectsTextSelection();
        CorrelationMatrixRejectsReservedLabel();
    }

    /// <summary>
    /// Verifies correlation and covariance use paired values.
    /// </summary>
    private static void CorrelationAndCovarianceUsePairedValues()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var corr = table
            .Corr("Id",
                "Other");
        var sample = table
            .Cov("Id",
                "Other");
        var population = table
            .Cov("Id",
                "Other",
                0);

        // Assert
        Check(corr.HasValue && Math.Abs(corr.Value - 1) < 1e-12 && sample == 2);
        Check(population.HasValue && Math.Abs(population.Value - 4.0 / 3) < 1e-12);
    }

    /// <summary>
    /// Verifies correlation can be negative.
    /// </summary>
    private static void CorrelationCanBeNegative()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var result = table
            .Assign("Other",
                row => -row.Field<int>("Other"))
            .Corr("Id",
                "Other");

        // Assert
        Check(result.HasValue && Math.Abs(result.Value + 1) < 1e-12);
    }

    /// <summary>
    /// Verifies correlation drops incomplete pairs.
    /// </summary>
    private static void CorrelationDropsIncompletePairs()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);
        table.Rows[0]["Id"] = DBNull.Value;
        table.Rows[1]["Other"] = DBNull.Value;

        // Act
        var corr = table
            .Corr("Id",
                "Other");
        var cov = table
            .Cov("Id",
                "Other",
                0);

        // Assert
        Check(corr is null && cov == 0);
    }

    /// <summary>
    /// Verifies correlation handles both constant sides.
    /// </summary>
    private static void CorrelationHandlesBothConstantSides()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var left = table
            .Assign("Id",
                _ => 1)
            .Corr("Id",
                "Other");
        var right = table
            .Assign("Other",
                _ => 1)
            .Corr("Id",
                "Other");

        // Assert
        Check(left is null && right is null);
    }

    /// <summary>
    /// Verifies correlation handles empty input.
    /// </summary>
    private static void CorrelationHandlesEmptyInput()
    {
        // Setup
        var table = TestTable.Create()
            .Clone();

        // Act
        var corr = table
            .Corr("Count",
                "Value");
        var covariance = table
            .Cov("Count",
                "Value",
                0);

        // Assert
        Check(corr is null && covariance is null);
    }

    /// <summary>
    /// Verifies correlation matrix defaults to numeric columns.
    /// </summary>
    private static void CorrelationMatrixDefaultsToNumericColumns()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var result = table
            .CorrMatrix();

        // Assert
        Check(result.Rows.Count == 2 && result.Columns.Count == 3 && result.Rows[0]["Column"] is "Id");
        Check(Math.Abs((double)result.Rows[0]["Other"] - 1) < 1e-12);
    }

    /// <summary>
    /// Verifies correlation matrix supports requested order.
    /// </summary>
    private static void CorrelationMatrixSupportsRequestedOrder()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var result = table
            .CorrMatrix("Other",
                "Id");

        // Assert
        Check(result.Rows[0]["Column"] is "Other" && result.Columns[1].ColumnName is "Other");
    }

    /// <summary>
    /// Verifies correlation matrix preserves undefined cells.
    /// </summary>
    private static void CorrelationMatrixPreservesUndefinedCells()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        var result = table
            .Assign("Id",
                _ => 1)
            .CorrMatrix();

        // Assert
        Check(result.Rows[0]
            .IsNull("Other") && result.Rows[1]
            .IsNull("Id"));
    }

    /// <summary>
    /// Verifies correlation matrix supports no numeric columns.
    /// </summary>
    private static void CorrelationMatrixSupportsNoNumericColumns()
    {
        // Setup
        var table = TestTable.Create()
            .SelectColumns("Key");

        // Act
        var result = table
            .CorrMatrix();

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 1);
    }

    /// <summary>
    /// Verifies covariance rejects negative degrees.
    /// </summary>
    private static void CovarianceRejectsNegativeDegrees()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        Action operation = () => table
            .Cov("Id",
                "Other",
                -1);

        // Assert
        Throws<ArgumentOutOfRangeException>(operation);
    }

    /// <summary>
    /// Verifies correlation rejects missing column.
    /// </summary>
    private static void CorrelationRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        Action operation = () => table
            .Corr("Id",
                "Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies correlation matrix rejects missing column.
    /// </summary>
    private static void CorrelationMatrixRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        Action operation = () => table
            .CorrMatrix("Missing");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies correlation matrix rejects null selection.
    /// </summary>
    private static void CorrelationMatrixRejectsNullSelection()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        Action operation = () => table
            .CorrMatrix(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies correlation matrix rejects duplicate selection.
    /// </summary>
    private static void CorrelationMatrixRejectsDuplicateSelection()
    {
        // Setup
        var table = TestTable.Keys(1,
            2,
            3);
        table = table
            .Assign("Other",
                row => row.Field<int>("Id") * 2);

        // Act
        Action operation = () => table
            .CorrMatrix("Id",
                "Id");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies correlation matrix rejects text selection.
    /// </summary>
    private static void CorrelationMatrixRejectsTextSelection()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .CorrMatrix("Key");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies correlation matrix rejects reserved label.
    /// </summary>
    private static void CorrelationMatrixRejectsReservedLabel()
    {
        // Setup
        var table = TestTable.Keys(1,
            2);
        table.Columns[0].ColumnName = "Column";

        // Act
        Action operation = () => table
            .CorrMatrix();

        // Assert
        Throws<DuplicateNameException>(operation);
    }

    #endregion
}
