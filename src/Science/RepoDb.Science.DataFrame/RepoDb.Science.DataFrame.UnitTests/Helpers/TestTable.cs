using System.Data;

namespace RepoDb.Science.UnitTests.Helpers;

/// <summary>
/// Creates analytical test fixtures.
/// </summary>
internal static class TestTable
{
    #region Public Methods

    /// <summary>
    /// Creates mixed current values with duplicates and missing data.
    /// </summary>
    /// <returns>The fixture.</returns>
    public static DataTable Create()
    {
        var table = new DataTable("Data");
        table.Columns.Add("Key",
            typeof(string));
        table.Columns.Add("Value",
            typeof(decimal));
        table.Columns.Add("Count",
            typeof(int));
        table.Rows.Add("A",
            10m,
            1);
        table.Rows.Add("A",
            DBNull.Value,
            2);
        table.Rows.Add("B",
            30m,
            3);
        table.Rows.Add("B",
            30m,
            3);
        return table;
    }

    /// <summary>
    /// Creates integer join keys.
    /// </summary>
    /// <param name="values">The key values.</param>
    /// <returns>The key table.</returns>
    public static DataTable Keys(params object[] values)
    {
        var table = new DataTable();
        table.Columns.Add("Id",
            typeof(int));
        foreach (var value in values)
        {
            table.Rows.Add(value);
        }
        return table;
    }

    #endregion
}
