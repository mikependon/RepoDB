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
    /// Projects columns having one of the exact declared types.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="types">The included CLR types.</param>
    /// <returns>The operation result.</returns>
    public static DataTable SelectDTypes(this DataTable table,
        params Type[] types)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(types);
        foreach (var type in types)
        {
            ArgumentNullException.ThrowIfNull(type,
                nameof(types));
        }
        var selected = table.Columns
            .Cast<DataColumn>()
            .Where(column => types.Contains(column.DataType));
        return TableHelper.Project(table,
            selected,
            InspectionHelper.Rows(table));
    }

    #endregion
}
