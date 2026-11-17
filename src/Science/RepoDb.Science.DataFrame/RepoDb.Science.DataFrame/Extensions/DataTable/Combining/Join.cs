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
    /// Joins current rows using SQL-style missing key behavior.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="right">The right table.</param>
    /// <param name="leftOn">The left key.</param>
    /// <param name="rightOn">The right key.</param>
    /// <param name="how">The join mode.</param>
    /// <param name="rightSuffix">The suffix for duplicate right names.</param>
    /// <returns>The operation result.</returns>
    public static DataTable Join(this DataTable table,
        DataTable right,
        string leftOn,
        string rightOn,
        JoinType how = JoinType.Inner,
        string rightSuffix = "_right")
    {
        return CombiningHelper.Join(table,
            right,
            new[] { leftOn },
            new[] { rightOn },
            how,
            rightSuffix);
    }

    /// <summary>
    /// Joins tables using composite keys.
    /// </summary>
    /// <param name="table">The left table.</param>
    /// <param name="right">The right table.</param>
    /// <param name="leftOn">The ordered left keys.</param>
    /// <param name="rightOn">The ordered right keys.</param>
    /// <param name="how">The join mode.</param>
    /// <param name="rightSuffix">The suffix for duplicate right names.</param>
    /// <returns>The joined table.</returns>
    public static DataTable Join(this DataTable table,
        DataTable right,
        string[] leftOn,
        string[] rightOn,
        JoinType how = JoinType.Inner,
        string rightSuffix = "_right")
    {
        return CombiningHelper.Join(table,
            right,
            leftOn,
            rightOn,
            how,
            rightSuffix);
    }

    #endregion
}
