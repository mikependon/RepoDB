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
    /// Bounds present numeric values while preserving their CLR type.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="lower">The optional inclusive lower bound.</param>
    /// <param name="upper">The optional inclusive upper bound.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Clip(this DataTable table,
        string column,
        decimal? lower = null,
        decimal? upper = null,
        bool inPlace = false)
    {
        if (lower > upper)
        {
            throw new ArgumentException("The lower bound exceeds the upper bound.",
                nameof(lower));
        }
        NumericTransformationHelper.ValidateClip(table,
            column,
            lower,
            upper);
        return NumericTransformationHelper.Apply(table,
            column,
            value => Math.Min(upper ?? decimal.MaxValue,
                Math.Max(lower ?? decimal.MinValue,
                    value)),
            value => Math.Min(upper.HasValue ? (double)upper.Value : double.PositiveInfinity,
                Math.Max(lower.HasValue ? (double)lower.Value : double.NegativeInfinity,
                    value)),
            inPlace);
    }

    #endregion
}
