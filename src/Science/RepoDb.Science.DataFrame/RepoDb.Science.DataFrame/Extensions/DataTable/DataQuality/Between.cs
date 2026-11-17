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
    /// Tests whether present comparable values fall between two bounds.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="column">The target column.</param>
    /// <param name="lower">The lower bound with a compatible CLR type.</param>
    /// <param name="upper">The upper bound with a compatible CLR type.</param>
    /// <param name="inclusive">Whether to include both endpoints.</param>
    /// <returns>The operation result.</returns>
    public static bool[] Between(this DataTable table,
        string column,
        object lower,
        object upper,
        bool inclusive = true)
    {
        var source = InspectionHelper.Column(table,
            column);
        ArgumentNullException.ThrowIfNull(lower);
        ArgumentNullException.ThrowIfNull(upper);
        if (Comparer<object>.Default.Compare(lower,
            upper) > 0)
        {
            throw new ArgumentException("The lower bound exceeds the upper bound.",
                nameof(lower));
        }
        return InspectionHelper.Rows(table)
            .Select(row => QualityHelper.Between(row[source],
                lower,
                upper,
                inclusive))
            .ToArray();
    }

    #endregion
}
