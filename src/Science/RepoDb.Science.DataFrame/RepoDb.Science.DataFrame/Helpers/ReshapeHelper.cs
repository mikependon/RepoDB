using System.Collections;
using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Materializes long, expanded, and transposed analytical tables.
/// </summary>
internal static class ReshapeHelper
{
    #region Public Methods

    /// <summary>
    /// Expands non-string enumerable cells by one level.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="name">The expanded column.</param>
    /// <returns>The expanded table.</returns>
    public static DataTable Explode(DataTable table,
        string name)
    {
        var column = InspectionHelper.Column(table,
            name);
        var result = ExpandedSchema(table,
            column);
        foreach (var row in InspectionHelper.Rows(table))
        {
            foreach (var value in Elements(row[column]))
            {
                var values = row.ItemArray;
                values[column.Ordinal] = value ?? DBNull.Value;
                result.Rows.Add(values);
            }
        }
        return result;
    }

    /// <summary>
    /// Unpivots value columns into a variable and value pair.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="idNames">The retained identifiers.</param>
    /// <param name="valueNames">The value names, or all non-identifiers.</param>
    /// <param name="variableName">The variable output name.</param>
    /// <param name="valueName">The value output name.</param>
    /// <returns>The long table.</returns>
    public static DataTable Melt(DataTable table,
        string[] idNames,
        string[]? valueNames,
        string variableName,
        string valueName)
    {
        var ids = TableHelper.Columns(table,
            idNames);
        var values = valueNames is null ? table.Columns
            .Cast<DataColumn>()
            .Except(ids)
            .ToArray() : TableHelper.Columns(table,
                valueNames);
        if (ids
            .Intersect(values)
            .Any())
        {
            throw new ArgumentException("Identifier and value columns must be disjoint.",
                nameof(valueNames));
        }
        var result = TableHelper.Schema(table,
            ids);
        TableHelper.AddColumn(result,
            variableName,
            typeof(string));
        TableHelper.AddColumn(result,
            valueName,
            typeof(object));
        PopulateMelt(table,
            result,
            ids,
            values);
        return result;
    }

    /// <summary>
    /// Transposes current cells into object-valued row columns.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="rowName">The original-column output name.</param>
    /// <param name="prefix">The generated row-column prefix.</param>
    /// <returns>The transposed table.</returns>
    public static DataTable Transpose(DataTable table,
        string rowName,
        string prefix)
    {
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        ArgumentException.ThrowIfNullOrWhiteSpace(prefix);
        var result = TableHelper.Schema(table,
            Array.Empty<DataColumn>());
        TableHelper.AddColumn(result,
            rowName,
            typeof(string));
        for (var index = 0; index < rows.Length; index++)
        {
            TableHelper.AddColumn(result,
                prefix + index.ToString(CultureInfo.InvariantCulture),
                typeof(object));
        }
        foreach (DataColumn column in table.Columns)
        {
            result.Rows.Add(new object[] { column.ColumnName }
                .Concat(rows
                .Select(row => row[column]))
                .ToArray());
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates a nullable object column for expanded values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="expanded">The expanded column.</param>
    /// <returns>The expanded schema.</returns>
    private static DataTable ExpandedSchema(DataTable table,
        DataColumn expanded)
    {
        var result = TableHelper.Schema(table,
            Array.Empty<DataColumn>());
        foreach (DataColumn column in table.Columns)
        {
            TableHelper.CopyColumn(result,
                column,
                column.ColumnName,
                column == expanded ? typeof(object) : null);
        }
        return result;
    }

    /// <summary>
    /// Returns scalar values or expanded elements with one missing row for empties.
    /// </summary>
    /// <param name="value">The source cell.</param>
    /// <returns>The expanded values.</returns>
    private static object?[] Elements(object value)
    {
        var elements = value is IEnumerable sequence && value is not string ? sequence
            .Cast<object?>()
            .ToArray() : new[] { value };
        return elements.Length == 0 ? new object?[] { DBNull.Value } : elements;
    }

    /// <summary>
    /// Writes long values in source-column then source-row order.
    /// </summary>
    /// <param name="source">The source table.</param>
    /// <param name="result">The output schema.</param>
    /// <param name="ids">The identifier columns.</param>
    /// <param name="values">The value columns.</param>
    private static void PopulateMelt(DataTable source,
        DataTable result,
        DataColumn[] ids,
        DataColumn[] values)
    {
        foreach (var column in values)
        {
            foreach (var row in InspectionHelper.Rows(source))
            {
                result.Rows.Add(ids
                    .Select(id => row[id])
                    .Concat(new object[] { column.ColumnName, row[column] })
                    .ToArray());
            }
        }
    }

    #endregion
}
