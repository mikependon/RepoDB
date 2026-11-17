using System.Data;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies backward propagation of missing column values.
/// </summary>
internal static class BackwardFillTests
{
    #region Public Methods

    /// <summary>
    /// Runs directional fill scenarios.
    /// </summary>
    public static void Run()
    {
        RunValueScenarios();
        RunTableScenarios();
        RunValidationScenarios();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs a batch of fill scenarios.
    /// </summary>
    private static void RunValueScenarios()
    {
        FillsConsecutiveGapsInCurrentOrder();
        CopiesByDefault();
        MutatesSameInstanceWhenRequested();
        PreservesUnresolvedEdgeGaps();
        HandlesEmptyTable();
        PreservesAllMissingValues();
        PreservesPresentValues();
        HandlesSingleMissingValue();
    }

    /// <summary>
    /// Runs a batch of fill scenarios.
    /// </summary>
    private static void RunTableScenarios()
    {
        SkipsDeletedSeeds();
        PreservesDeletedRowsInPlace();
        HandlesDeletedOnlyTable();
        UsesModifiedCurrentValues();
        PreservesColumnMetadataAndOtherColumns();
        TreatsEmptyStringsAsPresent();
        TreatsNaNAsPresent();
        RespectsReadOnlyColumnsInPlace();
    }

    /// <summary>
    /// Runs a batch of fill scenarios.
    /// </summary>
    private static void RunValidationScenarios()
    {
        CopyMaterializesReadOnlyColumns();
        RespectsUniqueConstraintsInPlace();
        CopyAllowsRepeatedFilledValues();
        RejectsNullSource();
        RejectsNullColumn();
        RejectsEmptyColumn();
        RejectsWhitespaceColumn();
        RejectsUnknownColumn();
    }

    /// <summary>
    /// Fills Consecutive Gaps In Current Order.
    /// </summary>
    private static void FillsConsecutiveGapsInCurrentOrder()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10,
            DBNull.Value,
            DBNull.Value,
            20,
            DBNull.Value);

        // Act
        var result = table
            .BackwardFill("Value");
        var values = result.Rows
            .Cast<DataRow>()
            .Select(row => row[0]);

        // Assert
        Check(values
            .SequenceEqual(new object[] { 10, 10, 20, 20, 20, DBNull.Value }));
    }

    /// <summary>
    /// Copies By Default.
    /// </summary>
    private static void CopiesByDefault()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(!ReferenceEquals(table,
            result));
        Check(table.Rows[0].IsNull(0) && (int)result.Rows[0][0] == 10);
    }

    /// <summary>
    /// Mutates Same Instance When Requested.
    /// </summary>
    private static void MutatesSameInstanceWhenRequested()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);

        // Act
        var result = table
            .BackwardFill("Value",
                inPlace: true);

        // Assert
        Check(ReferenceEquals(table,
            result) && (int)table.Rows[0][0] == 10);
    }

    /// <summary>
    /// Preserves Unresolved Edge Gaps.
    /// </summary>
    private static void PreservesUnresolvedEdgeGaps()
    {
        // Setup
        var table = CreateTable(10,
            DBNull.Value,
            DBNull.Value);

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows[1].IsNull(0) && result.Rows[2].IsNull(0));
    }

    /// <summary>
    /// Handles Empty Table.
    /// </summary>
    private static void HandlesEmptyTable()
    {
        // Setup
        var table = CreateTable();

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows.Count == 0 && result.Columns["Value"]!.DataType == typeof(int));
    }

    /// <summary>
    /// Preserves All Missing Values.
    /// </summary>
    private static void PreservesAllMissingValues()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            DBNull.Value);

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows[0].IsNull(0) && result.Rows[1].IsNull(0));
    }

    /// <summary>
    /// Preserves Present Values.
    /// </summary>
    private static void PreservesPresentValues()
    {
        // Setup
        var table = CreateTable(10,
            20,
            30);

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check((int)result.Rows[0][0] == 10 && (int)result.Rows[1][0] == 20 && (int)result.Rows[2][0] == 30);
    }

    /// <summary>
    /// Handles Single Missing Value.
    /// </summary>
    private static void HandlesSingleMissingValue()
    {
        // Setup
        var table = CreateTable(DBNull.Value);

        // Act
        var result = table
            .BackwardFill("Value",
                true);

        // Assert
        Check(ReferenceEquals(table,
            result) && result.Rows[0].IsNull(0));
    }

    /// <summary>
    /// Skips Deleted Seeds.
    /// </summary>
    private static void SkipsDeletedSeeds()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            99,
            10);
        table.AcceptChanges();
        table.Rows[1].Delete();

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows.Count == 2 && (int)result.Rows[0][0] == 10);
        Check(table.Rows[1].RowState == DataRowState.Deleted);
    }

    /// <summary>
    /// Preserves Deleted Rows In Place.
    /// </summary>
    private static void PreservesDeletedRowsInPlace()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            99,
            10);
        table.AcceptChanges();
        table.Rows[1].Delete();

        // Act
        var result = table
            .BackwardFill("Value",
                true);

        // Assert
        Check(result.Rows.Count == 3 && result.Rows[1].RowState == DataRowState.Deleted);
        Check((int)result.Rows[0][0] == 10);
    }

    /// <summary>
    /// Handles Deleted Only Table.
    /// </summary>
    private static void HandlesDeletedOnlyTable()
    {
        // Setup
        var table = CreateTable(10);
        table.AcceptChanges();
        table.Rows[0].Delete();

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows.Count == 0 && result.Columns.Count == 1);
    }

    /// <summary>
    /// Uses Modified Current Values.
    /// </summary>
    private static void UsesModifiedCurrentValues()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.AcceptChanges();
        table.Rows[1][0] = 20;

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check((int)result.Rows[0][0] == 20);
        Check((int)table.Rows[1][0, DataRowVersion.Original] == 10);
    }

    /// <summary>
    /// Preserves Column Metadata And Other Columns.
    /// </summary>
    private static void PreservesColumnMetadataAndOtherColumns()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.Columns[0].Caption = "Reading";
        table.Columns[0].ExtendedProperties["unit"] = "C";
        table.Columns.Add("Other",
            typeof(string));
        table.Rows[0]["Other"] = "unchanged";

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Columns[0].Caption is "Reading" && result.Columns[0].ExtendedProperties["unit"] is "C");
        Check(result.Rows[0]["Other"] is "unchanged" && result.Rows[1].IsNull("Other"));
    }

    /// <summary>
    /// Treats Empty Strings As Present.
    /// </summary>
    private static void TreatsEmptyStringsAsPresent()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(string));
        foreach (var value in new object[] { DBNull.Value, "", DBNull.Value, "B" })
        {
            table.Rows.Add(value);
        }

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(result.Rows[0][0] is "" && result.Rows[1][0] is "");
    }

    /// <summary>
    /// Treats Na NAs Present.
    /// </summary>
    private static void TreatsNaNAsPresent()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(double));
        table.Rows.Add(DBNull.Value);
        table.Rows.Add(double.NaN);

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check(double.IsNaN((double)result.Rows[0][0]) && double.IsNaN((double)result.Rows[1][0]));
    }

    /// <summary>
    /// Respects Read Only Columns In Place.
    /// </summary>
    private static void RespectsReadOnlyColumnsInPlace()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.Columns[0].ReadOnly = true;

        // Act
        Action operation = () => table
            .BackwardFill("Value",
                true);

        // Assert
        Throws<ReadOnlyException>(operation);
        Check(table.Rows[0].IsNull(0));
    }

    /// <summary>
    /// Copy Materializes Read Only Columns.
    /// </summary>
    private static void CopyMaterializesReadOnlyColumns()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.Columns[0].ReadOnly = true;

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check((int)result.Rows[0][0] == 10 && table.Rows[0].IsNull(0));
    }

    /// <summary>
    /// Respects Unique Constraints In Place.
    /// </summary>
    private static void RespectsUniqueConstraintsInPlace()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.Columns[0].Unique = true;

        // Act
        Action operation = () => table
            .BackwardFill("Value",
                true);

        // Assert
        Throws<ConstraintException>(operation);
        Check(table.Rows[0].IsNull(0));
    }

    /// <summary>
    /// Copy Allows Repeated Filled Values.
    /// </summary>
    private static void CopyAllowsRepeatedFilledValues()
    {
        // Setup
        var table = CreateTable(DBNull.Value,
            10);
        table.Columns[0].Unique = true;

        // Act
        var result = table
            .BackwardFill("Value");

        // Assert
        Check((int)result.Rows[0][0] == 10 && (int)result.Rows[1][0] == 10);
    }

    /// <summary>
    /// Rejects Null Source.
    /// </summary>
    private static void RejectsNullSource()
    {
        // Setup
        DataTable table = null!;

        // Act
        Action operation = () => table
            .BackwardFill("Value");

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Rejects Null Column.
    /// </summary>
    private static void RejectsNullColumn()
    {
        // Setup
        var table = CreateTable(10);

        // Act
        Action operation = () => table
            .BackwardFill(null!,
                true);

        // Assert
        Throws<ArgumentNullException>(operation);
        Check((int)table.Rows[0][0] == 10);
    }

    /// <summary>
    /// Rejects Empty Column.
    /// </summary>
    private static void RejectsEmptyColumn()
    {
        // Setup
        var table = CreateTable(10);

        // Act
        Action operation = () => table
            .BackwardFill("",
                true);

        // Assert
        Throws<ArgumentException>(operation);
        Check((int)table.Rows[0][0] == 10);
    }

    /// <summary>
    /// Rejects Whitespace Column.
    /// </summary>
    private static void RejectsWhitespaceColumn()
    {
        // Setup
        var table = CreateTable(10);

        // Act
        Action operation = () => table
            .BackwardFill(" \t ",
                true);

        // Assert
        Throws<ArgumentException>(operation);
        Check((int)table.Rows[0][0] == 10);
    }

    /// <summary>
    /// Rejects Unknown Column.
    /// </summary>
    private static void RejectsUnknownColumn()
    {
        // Setup
        var table = CreateTable(10);

        // Act
        Action operation = () => table
            .BackwardFill("Missing",
                true);

        // Assert
        Throws<ArgumentException>(operation);
        Check((int)table.Rows[0][0] == 10);
    }

    /// <summary>
    /// Creates a nullable integer column in the supplied row order.
    /// </summary>
    /// <param name="values">The current column values.</param>
    /// <returns>The source table.</returns>
    private static DataTable CreateTable(params object[] values)
    {
        var table = new DataTable();
        table.Columns.Add("Value",
            typeof(int));
        foreach (var value in values)
        {
            table.Rows.Add(value);
        }
        return table;
    }

    #endregion
}
