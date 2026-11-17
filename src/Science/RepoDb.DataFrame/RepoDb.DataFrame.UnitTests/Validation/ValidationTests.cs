using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies validation operations and failure behavior.
/// </summary>
internal static class ValidationTests
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
        SelectColumnsRejectsNullSource();
        DropColumnsRejectsNullSource();
        FilterRejectsNullSource();
        RenameColumnsRejectsNullSource();
        SortValuesRejectsNullSource();
        DropNullsRejectsNullSource();
        FillNullsRejectsNullSource();
        DropDuplicatesRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        ReplaceRejectsNullSource();
        CastColumnRejectsNullSource();
        AssignRejectsNullSource();
        ApplyRejectsNullSource();
        MapColumnRejectsNullSource();
        GroupByRejectsNullSource();
        AggregateRejectsNullSource();
        SumRejectsNullSource();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        MeanRejectsNullSource();
        MinRejectsNullSource();
        MaxRejectsNullSource();
        JoinRejectsNullSource();
        MergeRejectsNullSource();
        ConcatRejectsNullSource();
        TrainTestSplitRejectsNullSource();
        ToFeatureMatrixRejectsNullSource();
    }

    /// <summary>
    /// Verifies SelectColumnsRejectsNullSource.
    /// </summary>
    private static void SelectColumnsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .SelectColumns("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies DropColumnsRejectsNullSource.
    /// </summary>
    private static void DropColumnsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .DropColumns("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies FilterRejectsNullSource.
    /// </summary>
    private static void FilterRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Filter(_ => true);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies RenameColumnsRejectsNullSource.
    /// </summary>
    private static void RenameColumnsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .RenameColumns(new Dictionary<string, string>(StringComparer.Ordinal));

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies SortValuesRejectsNullSource.
    /// </summary>
    private static void SortValuesRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .SortValues("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies DropNullsRejectsNullSource.
    /// </summary>
    private static void DropNullsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .DropNulls();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies FillNullsRejectsNullSource.
    /// </summary>
    private static void FillNullsRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .FillNulls("X",
                0);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies DropDuplicatesRejectsNullSource.
    /// </summary>
    private static void DropDuplicatesRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .DropDuplicates();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ReplaceRejectsNullSource.
    /// </summary>
    private static void ReplaceRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Replace("X",
                1,
                2);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies CastColumnRejectsNullSource.
    /// </summary>
    private static void CastColumnRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .CastColumn<int>("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AssignRejectsNullSource.
    /// </summary>
    private static void AssignRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Assign("X",
                _ => 1);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ApplyRejectsNullSource.
    /// </summary>
    private static void ApplyRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Apply();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MapColumnRejectsNullSource.
    /// </summary>
    private static void MapColumnRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .MapColumn<int, int>("X",
                value => value);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies GroupByRejectsNullSource.
    /// </summary>
    private static void GroupByRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .GroupBy("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AggregateRejectsNullSource.
    /// </summary>
    private static void AggregateRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Aggregate(new AggregateDefinition("X",
                AggregateFunction.Sum));

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies SumRejectsNullSource.
    /// </summary>
    private static void SumRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Sum("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MeanRejectsNullSource.
    /// </summary>
    private static void MeanRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Mean("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MinRejectsNullSource.
    /// </summary>
    private static void MinRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Min("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MaxRejectsNullSource.
    /// </summary>
    private static void MaxRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Max("X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies JoinRejectsNullSource.
    /// </summary>
    private static void JoinRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Join(new DataTable(),
                "X",
                "X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MergeRejectsNullSource.
    /// </summary>
    private static void MergeRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Merge(new DataTable(),
                "X",
                "X");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ConcatRejectsNullSource.
    /// </summary>
    private static void ConcatRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .Concat();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies TrainTestSplitRejectsNullSource.
    /// </summary>
    private static void TrainTestSplitRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .TrainTestSplit();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ToFeatureMatrixRejectsNullSource.
    /// </summary>
    private static void ToFeatureMatrixRejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .ToFeatureMatrix();

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
