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
    /// Rounds present numeric values while preserving their CLR type.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The numeric column.</param>
    /// <param name="decimals">The decimal places from zero through fifteen.</param>
    /// <param name="mode">The midpoint rounding rule.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Round(this DataTable table,
        string column,
        int decimals = 0,
        MidpointRounding mode = MidpointRounding.ToEven,
        bool inPlace = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(decimals);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(decimals,
            15);
        if (!Enum.IsDefined(mode))
        {
            throw new ArgumentOutOfRangeException(nameof(mode));
        }
        return NumericTransformationHelper.Apply(table,
            column,
            value => decimal.Round(value,
                decimals,
                mode),
            value => Math.Round(value,
                decimals,
                mode),
            inPlace);
    }

    #endregion
}
