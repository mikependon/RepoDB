using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Tests shared inspection contracts not exposed by table storage.
/// </summary>
internal static class InspectionHelperTests
{
    #region Public Methods

    /// <summary>
    /// Runs shared helper scenarios.
    /// </summary>
    public static void Run()
    {
        CSharpNullIsMissing();
        CSharpNullHasNoPayload();
        NumericPrimitivesAreRecognized();
        NonnumericTypesAreExcluded();
        NullTableCopyIsRejected();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks missing-value handling before DataTable normalizes nulls.
    /// </summary>
    private static void CSharpNullIsMissing()
    {
        // Setup
        object? value = null;

        // Act
        var result = InspectionHelper.IsMissing(value);

        // Assert
        Check(result);
    }

    /// <summary>
    /// Checks payload estimation before DataTable normalizes nulls.
    /// </summary>
    private static void CSharpNullHasNoPayload()
    {
        // Setup
        object? value = null;

        // Act
        var result = InspectionHelper.ValueSize(value);

        // Assert
        Check(result == 0);
    }

    /// <summary>
    /// Checks recognition of all supported numeric primitives.
    /// </summary>
    private static void NumericPrimitivesAreRecognized()
    {
        // Setup
        var types = new[] { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort),
            typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float),
            typeof(double), typeof(decimal) };

        // Act
        var result = types
            .All(InspectionHelper.IsNumeric);

        // Assert
        Check(result);
    }

    /// <summary>
    /// Checks exclusion of types without numeric statistics support.
    /// </summary>
    private static void NonnumericTypesAreExcluded()
    {
        // Setup
        var types = new[] { typeof(bool), typeof(char), typeof(string), typeof(object),
            typeof(DateTime), typeof(Guid), typeof(DayOfWeek), typeof(int?) };

        // Act
        var result = types
            .Any(InspectionHelper.IsNumeric);

        // Assert
        Check(!result);
    }

    /// <summary>
    /// Checks direct null input validation for table copying.
    /// </summary>
    private static void NullTableCopyIsRejected()
    {
        // Setup
        DataTable table = null!;
        var rows = Array.Empty<DataRow>();

        // Act
        Action operation = () => InspectionHelper.CopyRows(table,
            rows);

        // Assert
        try
        {
            operation();
        }
        catch (ArgumentNullException exception)
        {
            Check(exception.ParamName is "table");
            return;
        }
        throw new InvalidOperationException("Expected null table rejection.");
    }

    /// <summary>
    /// Checks a condition.
    /// </summary>
    /// <param name="condition">The expected condition.</param>
    private static void Check(bool condition)
    {
        if (!condition)
        {
            throw new InvalidOperationException("Inspection helper assertion failed.");
        }
    }

    #endregion
}
