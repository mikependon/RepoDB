using System.Data;
using RepoDb.Science.UnitTests.Helpers;

namespace RepoDb.Science.UnitTests;

/// <summary>
/// Verifies fitted one-hot encoding.
/// </summary>
internal static class OneHotEncodeTests
{
    #region Public Methods

    /// <summary>
    /// Runs the encoder tests.
    /// </summary>
    public static void Run()
    {
        EncodesStringColumnsAsNumericIndicators();
        PreservesFittedOrderAcrossBatches();
        EncodesNullAndUnseenCategoriesAsZeros();
        FitsMultipleColumnsInSelectionOrder();
        ExcludesDeletedTrainingRows();
        RejectsIndicatorNameCollisions();
        RejectsNonStringCategories();
        RejectsEmptySelection();
        RejectsChangedColumnTypes();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a categorical column and a retained numeric column.
    /// </summary>
    /// <returns>The empty source table.</returns>
    private static DataTable CreateTable()
    {
        var table = new DataTable();
        table.Columns
            .Add("Country",
                typeof(string));
        table.Columns
            .Add("Revenue",
                typeof(int));
        return table;
    }

    /// <summary>
    /// Checks that indicators are usable as numeric features.
    /// </summary>
    private static void EncodesStringColumnsAsNumericIndicators()
    {
        var table = CreateTable();
        table.Rows
            .Add("SE",
                5);
        table.Rows
            .Add("DK",
                8);
        var matrix = table
            .OneHotEncode("Country")
            .ToFeatureMatrix();
        AssertHelper.Check(matrix[0, 0] == 5 && matrix[0, 1] == 0 && matrix[0, 2] == 1);
        AssertHelper.Check(matrix[1, 0] == 8 && matrix[1, 1] == 1 && matrix[1, 2] == 0);
        AssertHelper.Check(table.Columns.Count == 2 && string.Equals((string)table.Rows[0]["Country"],
            "SE",
            StringComparison.Ordinal));
    }

    /// <summary>
    /// Checks that batch schema order and absent levels do not change the output schema.
    /// </summary>
    private static void PreservesFittedOrderAcrossBatches()
    {
        var train = CreateTable();
        train.Columns
            .Add("Units",
                typeof(int));
        train.Rows
            .Add("DK",
                5,
                2);
        train.Rows
            .Add("SE",
                8,
                4);
        var encoder = DataTableOneHotEncoder.Fit(train,
            "Country");
        var test = CreateTable();
        test.Columns
            .Add("Units",
                typeof(int))
            .SetOrdinal(0);
        test.Columns
            .Add("Extra",
                typeof(int));
        test.Rows
            .Add(3,
                "DK",
                10);
        var output = encoder.Transform(test);
        AssertHelper.Check(output.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .SequenceEqual(new[] { "Revenue", "Units", "Country_DK", "Country_SE" },
                StringComparer.Ordinal));
        AssertHelper.Check((int)output.Rows[0]["Country_SE"] == 0);
    }

    /// <summary>
    /// Checks that deleted rows contribute neither categories nor output rows.
    /// </summary>
    private static void ExcludesDeletedTrainingRows()
    {
        var table = CreateTable();
        var deleted = table.Rows
            .Add("SE",
                5);
        table.Rows
            .Add("DK",
                8);
        table
            .AcceptChanges();
        deleted
            .Delete();
        var output = table
            .OneHotEncode("Country");
        AssertHelper.Check(output.Rows.Count == 1);
        AssertHelper.Check(output.Columns.Count == 2 && (int)output.Rows[0]["Country_DK"] == 1);
    }

    /// <summary>
    /// Checks the indicator values for null and unseen categories.
    /// </summary>
    private static void EncodesNullAndUnseenCategoriesAsZeros()
    {
        var train = CreateTable();
        train.Rows
            .Add("DK",
                5);
        var encoder = DataTableOneHotEncoder.Fit(train,
            "Country");
        var test = CreateTable();
        test.Rows
            .Add(DBNull.Value,
                7);
        test.Rows
            .Add("NO",
                8);
        var output = encoder.Transform(test);
        AssertHelper.Check((int)output.Rows[0]["Country_DK"] == 0);
        AssertHelper.Check((int)output.Rows[1]["Country_DK"] == 0);
    }

    /// <summary>
    /// Checks that each categorical column gets its own indicators.
    /// </summary>
    private static void FitsMultipleColumnsInSelectionOrder()
    {
        var table = CreateTable();
        table.Columns
            .Add("Channel",
                typeof(string));
        table.Rows
            .Add("DK",
                5,
                "Web");
        var output = table
            .OneHotEncode("Channel",
                "Country");
        AssertHelper.Check(output.Columns
            .Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .SequenceEqual(new[] { "Revenue", "Channel_Web", "Country_DK" },
                StringComparer.Ordinal));
        AssertHelper.Check((int)output.Rows[0]["Channel_Web"] == 1 && (int)output.Rows[0]["Country_DK"] == 1);
    }

    /// <summary>
    /// Checks that indicators cannot overwrite retained columns.
    /// </summary>
    private static void RejectsIndicatorNameCollisions()
    {
        var table = CreateTable();
        table.Columns
            .Add("Country_DK",
                typeof(int));
        table.Rows
            .Add("DK",
                5,
                7);
        AssertHelper.Throws<ArgumentException>(() => table
            .OneHotEncode("Country"));
    }

    /// <summary>
    /// Checks that selected columns must contain strings.
    /// </summary>
    private static void RejectsNonStringCategories()
    {
        var table = CreateTable();
        AssertHelper.Throws<ArgumentException>(() => table
            .OneHotEncode("Revenue"));
    }

    /// <summary>
    /// Checks that fitting requires at least one column.
    /// </summary>
    private static void RejectsEmptySelection()
    {
        var table = CreateTable();
        AssertHelper.Throws<ArgumentException>(() => table
            .OneHotEncode());
    }

    /// <summary>
    /// Checks that the fitted feature types are enforced.
    /// </summary>
    private static void RejectsChangedColumnTypes()
    {
        var train = CreateTable();
        var encoder = DataTableOneHotEncoder.Fit(train,
            "Country");
        var test = CreateTable();
        test.Columns["Revenue"]!.DataType = typeof(double);
        AssertHelper.Throws<ArgumentException>(() => encoder
            .Transform(test));
    }

    #endregion
}
