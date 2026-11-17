using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Copies random rows without replacement.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="count">The requested row count.</param>
    /// <param name="seed">An optional reproducible random seed.</param>
    /// <returns>An independent table in sample order.</returns>
    public static DataTable Sample(this DataTable table,
        int count = 5,
        int? seed = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count,
            rows.Length);
        var random = seed.HasValue ? new Random(seed.Value) : Random.Shared;
        InspectionHelper.Shuffle(rows,
            count,
            random);
        return InspectionHelper.CopyRows(table,
            rows
                .Take(count));
    }

    #endregion
}
