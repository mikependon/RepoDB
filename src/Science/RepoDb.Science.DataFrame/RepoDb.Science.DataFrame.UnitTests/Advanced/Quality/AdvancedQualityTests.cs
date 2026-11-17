using System.Data;
using RepoDb.Science.UnitTests.Helpers;
using static RepoDb.Science.UnitTests.Helpers.AssertHelper;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies quality additions and failure behavior.
/// </summary>
internal static class AdvancedQualityTests
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
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch1()
    {
        NullMasksRespectMissingValues();
        DuplicatedMarksOnlySubsequentKeys();
        DuplicateMissingKeysCompareEqually();
        MembershipNormalizesNullAndPreservesClrTypes();
        MembershipWithEmptySetReturnsFalse();
        BetweenIncludesBothEndpoints();
        BetweenCanExcludeEndpoints();
        BetweenRejectsValuesOnBothSides();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch2()
    {
        BetweenSupportsDates();
        TypesRejectNullArray();
        TypesRejectNullElement();
        NullMaskRejectsUnknownColumn();
        DuplicatesRejectDuplicateKeys();
        MembershipRejectsNullSet();
        BetweenRejectsReversedBounds();
        BetweenRejectsNullLower();
    }

    /// <summary>
    /// Runs a batch of related scenarios.
    /// </summary>
    private static void RunBatch3()
    {
        BetweenRejectsNullUpper();
        BetweenRejectsIncompatibleBounds();
    }

    /// <summary>
    /// Verifies null masks respect missing values.
    /// </summary>
    private static void NullMasksRespectMissingValues()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var missing = table
            .IsNull("Value");
        var present = table
            .NotNull("Value");

        // Assert
        Check(missing
            .SequenceEqual(new[] { false, true, false, false }));
        Check(present
            .SequenceEqual(new[] { true, false, true, true }));
    }

    /// <summary>
    /// Verifies duplicated marks only subsequent keys.
    /// </summary>
    private static void DuplicatedMarksOnlySubsequentKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Duplicated();
        var keys = table
            .Duplicated("Key");

        // Assert
        Check(result
            .SequenceEqual(new[] { false, false, false, true }));
        Check(keys
            .SequenceEqual(new[] { false, true, false, true }));
    }

    /// <summary>
    /// Verifies duplicate missing keys compare equally.
    /// </summary>
    private static void DuplicateMissingKeysCompareEqually()
    {
        // Setup
        var table = TestTable.Create();
        table.Rows[0]["Key"] = DBNull.Value;
        table.Rows[1]["Key"] = DBNull.Value;

        // Act
        var result = table
            .Duplicated("Key");

        // Assert
        Check(result
            .SequenceEqual(new[] { false, true, false, true }));
    }

    /// <summary>
    /// Verifies membership normalizes null and preserves clr types.
    /// </summary>
    private static void MembershipNormalizesNullAndPreservesClrTypes()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .IsIn("Value",
                new object?[] { null, 30m, 10 });

        // Assert
        Check(result
            .SequenceEqual(new[] { false, true, true, true }));
    }

    /// <summary>
    /// Verifies membership with empty set returns false.
    /// </summary>
    private static void MembershipWithEmptySetReturnsFalse()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .IsIn("Count",
                Array.Empty<object?>());

        // Assert
        Check(result
            .All(value => !value));
    }

    /// <summary>
    /// Verifies between includes both endpoints.
    /// </summary>
    private static void BetweenIncludesBothEndpoints()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Between("Value",
                10m,
                30m);

        // Assert
        Check(result
            .SequenceEqual(new[] { true, false, true, true }));
    }

    /// <summary>
    /// Verifies between can exclude endpoints.
    /// </summary>
    private static void BetweenCanExcludeEndpoints()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Between("Count",
                1,
                3,
                false);

        // Assert
        Check(result
            .SequenceEqual(new[] { false, true, false, false }));
    }

    /// <summary>
    /// Verifies between rejects values on both sides.
    /// </summary>
    private static void BetweenRejectsValuesOnBothSides()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        var result = table
            .Between("Count",
                2,
                2);

        // Assert
        Check(result
            .SequenceEqual(new[] { false, true, false, false }));
    }

    /// <summary>
    /// Verifies between supports dates.
    /// </summary>
    private static void BetweenSupportsDates()
    {
        // Setup
        var table = new DataTable();
        table.Columns.Add("Date",
            typeof(DateTime));
        table.Rows.Add(new DateTime(2020,
            1,
            1));

        // Act
        var result = table
            .Between("Date",
                new DateTime(2019,
                    1,
                    1),
                new DateTime(2021,
                    1,
                    1));

        // Assert
        Check(result[0]);
    }

    /// <summary>
    /// Verifies types reject null array.
    /// </summary>
    private static void TypesRejectNullArray()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SelectDTypes(null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies types reject null element.
    /// </summary>
    private static void TypesRejectNullElement()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .SelectDTypes(new Type[] { null! });

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies null mask rejects unknown column.
    /// </summary>
    private static void NullMaskRejectsUnknownColumn()
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
    /// Verifies duplicates reject duplicate keys.
    /// </summary>
    private static void DuplicatesRejectDuplicateKeys()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Duplicated("Count",
                "Count");

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies membership rejects null set.
    /// </summary>
    private static void MembershipRejectsNullSet()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .IsIn("Key",
                null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies between rejects reversed bounds.
    /// </summary>
    private static void BetweenRejectsReversedBounds()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Between("Count",
                3,
                1);

        // Assert
        Throws<ArgumentException>(operation);
    }

    /// <summary>
    /// Verifies between rejects null lower.
    /// </summary>
    private static void BetweenRejectsNullLower()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Between("Count",
                null!,
                1);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies between rejects null upper.
    /// </summary>
    private static void BetweenRejectsNullUpper()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Between("Count",
                1,
                null!);

        // Assert
        Throws<ArgumentNullException>(operation);
    }

    /// <summary>
    /// Verifies between rejects incompatible bounds.
    /// </summary>
    private static void BetweenRejectsIncompatibleBounds()
    {
        // Setup
        var table = TestTable.Create();

        // Act
        Action operation = () => table
            .Between("Count",
                "A",
                "Z");

        // Assert
        Throws<ArgumentException>(operation);
    }

    #endregion
}
