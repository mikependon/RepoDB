using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Inspects tables without modifying their contents.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Gets declared column types in column order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <returns>A read-only name to type mapping.</returns>
    public static IReadOnlyDictionary<string, Type> DTypes(this DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        return new System.Collections.ObjectModel.ReadOnlyDictionary<string, Type>(
            table.Columns
                .Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName,
                    column => column.DataType,
                    StringComparer.Ordinal));
    }

    #endregion
}
