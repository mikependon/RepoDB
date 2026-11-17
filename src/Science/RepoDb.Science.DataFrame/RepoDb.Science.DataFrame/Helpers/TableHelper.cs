using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Builds materialized analytical tables.
/// </summary>
internal static class TableHelper
{
    #region Public Methods

    /// <summary>
    /// Resolves distinct column names in requested order.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="names">The requested names.</param>
    /// <param name="allWhenEmpty">Whether an empty selection means all columns.</param>
    /// <returns>The resolved columns.</returns>
    public static DataColumn[] Columns(DataTable table,
        string[] names,
        bool allWhenEmpty = false)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(names);
        var columns = allWhenEmpty && names.Length == 0 ? table.Columns
            .Cast<DataColumn>()
            .ToArray() :
            names
                .Select(name => InspectionHelper.Column(table,
                    name))
                    .ToArray();
        if (columns
            .Distinct()
            .Count() != columns.Length)
        {
            throw new ArgumentException("Columns must be distinct.",
                nameof(names));
        }
        return columns;
    }

    /// <summary>
    /// Creates a value snapshot using selected columns and rows.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected columns.</param>
    /// <param name="rows">The selected rows.</param>
    /// <returns>The materialized table.</returns>
    public static DataTable Project(DataTable table,
        IEnumerable<DataColumn> columns,
        IEnumerable<DataRow> rows)
    {
        var selected = columns
            .ToArray();
        var result = Schema(table,
            selected);
        foreach (var row in rows)
        {
            result.Rows.Add(selected
                .Select(column => row[column])
                .ToArray());
        }
        return result;
    }

    /// <summary>
    /// Creates an analytical schema without constraints or expressions.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="columns">The selected columns.</param>
    /// <returns>The empty schema.</returns>
    public static DataTable Schema(DataTable table,
        IEnumerable<DataColumn> columns)
    {
        var result = new DataTable(table.TableName) { Locale = table.Locale, CaseSensitive = table.CaseSensitive };
        foreach (var column in columns)
        {
            CopyColumn(result,
                column,
                column.ColumnName);
        }
        return result;
    }

    /// <summary>
    /// Copies column metadata while materializing expressions.
    /// </summary>
    /// <param name="target">The target table.</param>
    /// <param name="source">The source column.</param>
    /// <param name="name">The output name.</param>
    /// <param name="type">An optional replacement type with nullable output.</param>
    /// <returns>The output column.</returns>
    public static DataColumn CopyColumn(DataTable target,
        DataColumn source,
        string name,
        Type? type = null)
    {
        var column = AddColumn(target,
            name,
            type ?? source.DataType);
        column.Caption = source.Caption;
        column.AllowDBNull = type is not null || source.AllowDBNull;
        foreach (var key in source.ExtendedProperties.Keys)
        {
            column.ExtendedProperties[key] = source.ExtendedProperties[key];
        }
        return column;
    }

    /// <summary>
    /// Adds a runtime-typed value column.
    /// </summary>
    /// <param name="table">The target table.</param>
    /// <param name="name">The column name.</param>
    /// <param name="type">The stored CLR type.</param>
    /// <returns>The added column.</returns>
    [UnconditionalSuppressMessage("Trimming",
        "IL2067",
        Justification = "Columns store existing CLR values; this library does not reflect over their members.")]
    public static DataColumn AddColumn(DataTable table,
        string name,
        Type type)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(type);
        return table.Columns.Add(name,
            Nullable.GetUnderlyingType(type) ?? type);
    }

    /// <summary>
    /// Selects an original table or an independent snapshot.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="inPlace">Whether to use the source.</param>
    /// <returns>The target table.</returns>
    public static DataTable Target(DataTable table,
        bool inPlace)
    {
        ArgumentNullException.ThrowIfNull(table);
        return inPlace ? table : Project(table,
            table.Columns
                .Cast<DataColumn>(),
            InspectionHelper.Rows(table));
    }

    /// <summary>
    /// Removes selected current rows without accepting other changes.
    /// </summary>
    /// <param name="table">The target table.</param>
    /// <param name="rows">The rows to remove.</param>
    public static void RemoveRows(DataTable table,
        IEnumerable<DataRow> rows)
    {
        foreach (var row in rows
            .ToArray())
        {
            table.Rows.Remove(row);
        }
    }

    #endregion
}
