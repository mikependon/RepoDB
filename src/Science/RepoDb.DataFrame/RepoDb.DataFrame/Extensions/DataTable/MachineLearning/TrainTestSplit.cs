using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Provides analytical DataTable operations.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Splits shuffled current rows into disjoint nonempty tables.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="testSize">The test fraction between zero and one.</param>
    /// <param name="seed">The optional random seed.</param>
    /// <returns>The operation result.</returns>
    public static (DataTable Train,
        DataTable Test) TrainTestSplit(this DataTable table,
            double testSize = 0.2,
            int? seed = null)
    {
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        if (!double.IsFinite(testSize) || testSize <= 0 || testSize >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(testSize));
        }
        if (rows.Length < 2)
        {
            throw new ArgumentException("Splitting requires at least two current rows.",
                nameof(table));
        }
        var testCount = Math.Clamp((int)Math.Ceiling(rows.Length * testSize),
            1,
            rows.Length - 1);
        InspectionHelper.Shuffle(rows,
            rows.Length,
            seed.HasValue ? new Random(seed.Value) : Random.Shared);
        return (TableHelper.Project(table,
            table.Columns
                .Cast<DataColumn>(),
            rows
                .Skip(testCount)),
            TableHelper.Project(table,
                table.Columns
                    .Cast<DataColumn>(),
                rows
                    .Take(testCount)));
    }

    #endregion
}
