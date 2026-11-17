using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Compares rows using ordered sort columns.
/// </summary>
internal sealed class RowComparer : IComparer<DataRow>
{
    #region Private Variables

    private readonly bool[] _ascending;
    private readonly DataColumn[] _columns;
    private readonly bool _nullsLast;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a multi-column comparer.
    /// </summary>
    /// <param name="columns">The ordered sort columns.</param>
    /// <param name="ascending">The sort directions.</param>
    /// <param name="nullsLast">Whether missing values sort last.</param>
    public RowComparer(DataColumn[] columns,
        bool[] ascending,
        bool nullsLast)
    {
        _ascending = ascending;
        _columns = columns;
        _nullsLast = nullsLast;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Compares two rows.
    /// </summary>
    /// <param name="x">The left row.</param>
    /// <param name="y">The right row.</param>
    /// <returns>The sort order.</returns>
    public int Compare(DataRow? x,
        DataRow? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        for (var index = 0; index < _columns.Length; index++)
        {
            var result = CompareValue(x[_columns[index]],
                y[_columns[index]],
                _ascending[index]);
            if (result != 0)
            {
                return result;
            }
        }
        return 0;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Compares present values or explicit null positions.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <param name="ascending">The direction.</param>
    /// <returns>The sort order.</returns>
    private int CompareValue(object left,
        object right,
        bool ascending)
    {
        var leftNull = InspectionHelper.IsMissing(left);
        var rightNull = InspectionHelper.IsMissing(right);
        if (leftNull || rightNull)
        {
            return leftNull == rightNull ? 0 : leftNull == _nullsLast ? 1 : -1;
        }
        var comparison = left is string text && right is string other ?
            StringComparer.Ordinal.Compare(text,
                other) : Comparer<object>.Default.Compare(left,
                    right);
        return ascending ? comparison : -Math.Sign(comparison);
    }

    #endregion
}
