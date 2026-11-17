using System.Data;
using System.Diagnostics.CodeAnalysis;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies transformation operations and failure behavior.
/// </summary>
internal static class TransformationTests
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
        AssignAddsTypedColumn();
        AssignReplacesColumnAndType();
        AssignSupportsInPlaceNewColumn();
        AssignSupportsInPlaceExistingColumn();
        AssignSupportsNullableOutput();
        AssignRejectsTypeChangeInPlace();
        AssignCallbackFailureLeavesSourceUnchanged();
        ApplyUsesOriginalValuesSimultaneously();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        ApplySupportsInPlace();
        ApplySupportsEmptyAssignments();
        ApplyCopyOverloadLeavesSourceUnchanged();
        ApplyRejectsInPlaceTypeChangesBeforeCommit();
        MapColumnChangesType();
        MapColumnSkipsMissingValues();
        MapColumnSupportsInPlace();
        AssignRejectsNullSelector();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        AssignRejectsEmptyName();
        MapRejectsNullMapper();
        MapRejectsMissingColumn();
        MapRejectsIncompatibleInput();
        ApplyRejectsNullArray();
        ApplyRejectsNullDefinition();
        ApplyRejectsDuplicateNames();
        AssignmentRejectsMissingName();
    }

    /// <summary>
    /// Runs a batch of scenarios.
    /// </summary>
    private static void RunBatch4()
    {
        AssignmentRejectsNullType();
        AssignmentRejectsNullSelector();
    }

    /// <summary>
    /// Verifies AssignAddsTypedColumn.
    /// </summary>
    private static void AssignAddsTypedColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Assign("Double",
                row => row.Field<int>("Count") * 2);

        // Assert
        Check(result.Columns["Double"]!.DataType == typeof(int) && (int)result.Rows[2]["Double"] == 6);
        Check(!table.Columns.Contains("Double"));
    }

    /// <summary>
    /// Verifies AssignReplacesColumnAndType.
    /// </summary>
    private static void AssignReplacesColumnAndType()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Assign("Value",
                row => row.Field<int>("Count"));

        // Assert
        Check(result.Columns["Value"]!.DataType == typeof(int) && result.Columns[1].ColumnName is "Value");
    }

    /// <summary>
    /// Verifies AssignSupportsInPlaceNewColumn.
    /// </summary>
    private static void AssignSupportsInPlaceNewColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Assign("Double",
                row => row.Field<int>("Count") * 2,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0]["Double"] == 2);
    }

    /// <summary>
    /// Verifies AssignSupportsInPlaceExistingColumn.
    /// </summary>
    private static void AssignSupportsInPlaceExistingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Assign("Count",
                row => row.Field<int>("Count") + 1,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0]["Count"] == 2);
    }

    /// <summary>
    /// Verifies AssignSupportsNullableOutput.
    /// </summary>
    private static void AssignSupportsNullableOutput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Assign<int?>("Optional",
                _ => null);

        // Assert
        Check(result.Rows[0].IsNull("Optional") && result.Columns["Optional"]!.DataType == typeof(int));
    }

    /// <summary>
    /// Verifies AssignRejectsTypeChangeInPlace.
    /// </summary>
    private static void AssignRejectsTypeChangeInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Assign("Value",
                row => row.Field<int>("Count"),
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check(table.Columns["Value"]!.DataType == typeof(decimal));
    }

    /// <summary>
    /// Verifies AssignCallbackFailureLeavesSourceUnchanged.
    /// </summary>
    private static void AssignCallbackFailureLeavesSourceUnchanged()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Assign<int>("New",
                _ => throw new InvalidOperationException(),
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check(!table.Columns.Contains("New"));
    }

    /// <summary>
    /// Verifies ApplyUsesOriginalValuesSimultaneously.
    /// </summary>
    private static void ApplyUsesOriginalValuesSimultaneously()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Apply(new ColumnAssignment("Count",
                typeof(int),
                row => row.Field<int>("Count") + 10),
                new ColumnAssignment("Original",
                    typeof(int),
                    row => row.Field<int>("Count")));

        // Assert
        Check((int)result.Rows[0]["Count"] == 11 && (int)result.Rows[0]["Original"] == 1);
    }

    /// <summary>
    /// Verifies ApplySupportsInPlace.
    /// </summary>
    private static void ApplySupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Apply(new[] { new ColumnAssignment("Count",
                typeof(int),
                _ => 99), new ColumnAssignment("New",
                    typeof(string),
                    _ => null) },
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0]["Count"] == 99);
        Check(table.Rows[0].IsNull("New"));
    }

    /// <summary>
    /// Verifies ApplySupportsEmptyAssignments.
    /// </summary>
    private static void ApplySupportsEmptyAssignments()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Apply();

        // Assert
        Check(!ReferenceEquals(table,
            result) && result.Rows.Count == 4);
    }

    /// <summary>
    /// Verifies ApplyCopyOverloadLeavesSourceUnchanged.
    /// </summary>
    private static void ApplyCopyOverloadLeavesSourceUnchanged()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Apply(new[] { new ColumnAssignment("Count",
                typeof(int),
                _ => 9) },
                false);

        // Assert
        Check((int)result.Rows[0]["Count"] == 9 && (int)table.Rows[0]["Count"] == 1);
    }

    /// <summary>
    /// Verifies ApplyRejectsInPlaceTypeChangesBeforeCommit.
    /// </summary>
    private static void ApplyRejectsInPlaceTypeChangesBeforeCommit()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Apply(new[] { new ColumnAssignment("Count",
                typeof(int),
                _ => 99), new ColumnAssignment("Value",
                    typeof(string),
                    _ => "text") },
                true);

        // Assert
        Throws<InvalidOperationException>(operation);
        Check((int)table.Rows[0]["Count"] == 1);
    }

    /// <summary>
    /// Verifies MapColumnChangesType.
    /// </summary>
    private static void MapColumnChangesType()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .MapColumn<int, string>("Count",
                value => value.ToString(System.Globalization.CultureInfo.InvariantCulture));

        // Assert
        Check(result.Rows[0]["Count"] is "1" && result.Columns["Count"]!.DataType == typeof(string));
    }

    /// <summary>
    /// Verifies MapColumnSkipsMissingValues.
    /// </summary>
    private static void MapColumnSkipsMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .MapColumn<decimal, decimal>("Value",
                value => value * 2);

        // Assert
        Check((decimal)result.Rows[0]["Value"] == 20m && result.Rows[1].IsNull("Value"));
    }

    /// <summary>
    /// Verifies MapColumnSupportsInPlace.
    /// </summary>
    private static void MapColumnSupportsInPlace()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .MapColumn<int, int>("Count",
                value => value + 1,
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0]["Count"] == 2);
    }

    /// <summary>
    /// Verifies AssignRejectsNullSelector.
    /// </summary>
    private static void AssignRejectsNullSelector()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Assign<int>("New",
                null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AssignRejectsEmptyName.
    /// </summary>
    private static void AssignRejectsEmptyName()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Assign("",
                _ => 1);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies MapRejectsNullMapper.
    /// </summary>
    private static void MapRejectsNullMapper()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .MapColumn<int, int>("Count",
                null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies MapRejectsMissingColumn.
    /// </summary>
    private static void MapRejectsMissingColumn()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .MapColumn<int, int>("Missing",
                value => value);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies MapRejectsIncompatibleInput.
    /// </summary>
    private static void MapRejectsIncompatibleInput()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .MapColumn<string, int>("Count",
                value => value.Length);

        // Assert
        Throws<InvalidCastException>(operation);
    }

    /// <summary>
    /// Verifies ApplyRejectsNullArray.
    /// </summary>
    private static void ApplyRejectsNullArray()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Apply(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ApplyRejectsNullDefinition.
    /// </summary>
    private static void ApplyRejectsNullDefinition()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Apply(new ColumnAssignment[] { null! });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies ApplyRejectsDuplicateNames.
    /// </summary>
    private static void ApplyRejectsDuplicateNames()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Apply(new ColumnAssignment("X",
                typeof(int),
                _ => 1),
                new ColumnAssignment("x",
                    typeof(int),
                    _ => 2));

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies AssignmentRejectsMissingName.
    /// </summary>
    private static void AssignmentRejectsMissingName()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new ColumnAssignment("",
            typeof(int),
            _ => 1);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies AssignmentRejectsNullType.
    /// </summary>
    private static void AssignmentRejectsNullType()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new ColumnAssignment("X",
            null!,
            _ => 1);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies AssignmentRejectsNullSelector.
    /// </summary>
    private static void AssignmentRejectsNullSelector()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => new ColumnAssignment("X",
            typeof(int),
            null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    #endregion
}
