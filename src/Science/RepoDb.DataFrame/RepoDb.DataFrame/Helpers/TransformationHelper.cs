using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Materializes typed column transformations.
/// </summary>
internal static class TransformationHelper
{
    #region Public Methods

    /// <summary>
    /// Creates or replaces a column with computed values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="name">The output name.</param>
    /// <param name="type">The output CLR type.</param>
    /// <param name="values">The computed current values.</param>
    /// <param name="inPlace">Whether to update the source.</param>
    /// <returns>The transformed table.</returns>
    public static DataTable SetColumn(DataTable table,
        string name,
        Type type,
        object?[] values,
        bool inPlace)
    {
        var existing = table.Columns[name];
        var storedType = Nullable.GetUnderlyingType(type) ?? type;
        if (inPlace && existing is not null && existing.DataType != storedType)
        {
            throw new InvalidOperationException("Changing an existing column type requires a new table.");
        }
        var staged = inPlace ? StageColumn(name,
            storedType,
            values) : BuildColumn(table,
                name,
                storedType,
                values);
        return inPlace ? Commit(table,
            staged,
            name) : staged;
    }

    /// <summary>
    /// Converts a present value using invariant culture.
    /// </summary>
    /// <param name="value">The input value.</param>
    /// <param name="type">The target type.</param>
    /// <returns>The converted value or DBNull.</returns>
    public static object ConvertValue(object? value,
        Type type)
    {
        if (InspectionHelper.IsMissing(value))
        {
            return DBNull.Value;
        }
        var target = Nullable.GetUnderlyingType(type) ?? type;
        return target.IsInstanceOfType(value) ? value! : Convert.ChangeType(value,
            target,
            CultureInfo.InvariantCulture)!;
    }

    /// <summary>
    /// Evaluates assignments against the same source snapshot.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="assignments">The column computations.</param>
    /// <param name="inPlace">Whether to update the source.</param>
    /// <returns>The transformed table.</returns>
    public static DataTable Apply(DataTable table,
        ColumnAssignment[] assignments,
        bool inPlace)
    {
        ArgumentNullException.ThrowIfNull(assignments);
        ValidateAssignments(assignments);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        var values = assignments
            .Select(assignment => rows
            .Select(assignment.Selector)
            .ToArray())
            .ToArray();
        var result = TableHelper.Target(table,
            false);
        for (var index = 0; index < assignments.Length; index++)
        {
            var assignment = assignments[index];
            var existing = result.Columns[assignment.Name];
            var canEdit = existing is null || existing.DataType == (Nullable.GetUnderlyingType(assignment.DataType) ?? assignment.DataType);
            result = SetColumn(result,
                assignment.Name,
                assignment.DataType,
                values[index],
                canEdit);
        }
        return inPlace ? CommitAssignments(table,
            result,
            assignments) : result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validates computed values using a single-column staging table.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="type">The stored CLR type.</param>
    /// <param name="values">The computed values.</param>
    /// <returns>The validated column values.</returns>
    private static DataTable StageColumn(string name,
        Type type,
        object?[] values)
    {
        var result = new DataTable();
        TableHelper.AddColumn(result,
            name,
            type);
        foreach (var value in values)
        {
            result.Rows.Add(value ?? DBNull.Value);
        }
        return result;
    }

    /// <summary>
    /// Builds a staged replacement column and its values.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="name">The output name.</param>
    /// <param name="type">The output type.</param>
    /// <param name="values">The computed values.</param>
    /// <returns>The staged table.</returns>
    private static DataTable BuildColumn(DataTable table,
        string name,
        Type type,
        object?[] values)
    {
        var result = new DataTable(table.TableName) { Locale = table.Locale, CaseSensitive = table.CaseSensitive };
        foreach (DataColumn column in table.Columns)
        {
            if (ReferenceEquals(column,
                table.Columns[name]))
            {
                TableHelper.CopyColumn(result,
                    column,
                    name,
                    type);
            }
            else
            {
                TableHelper.CopyColumn(result,
                    column,
                    column.ColumnName);
            }
        }
        return PopulateColumn(table,
            result,
            name,
            type,
            values);
    }

    /// <summary>
    /// Populates a staged replacement table.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="result">The output schema.</param>
    /// <param name="name">The output name.</param>
    /// <param name="type">The output type.</param>
    /// <param name="values">The computed values.</param>
    /// <returns>The populated table.</returns>
    private static DataTable PopulateColumn(DataTable table,
        DataTable result,
        string name,
        Type type,
        object?[] values)
    {
        var column = result.Columns[name] ?? TableHelper.AddColumn(result,
            name,
            type);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        for (var index = 0; index < rows.Length; index++)
        {
            var row = result.NewRow();
            foreach (DataColumn target in result.Columns)
            {
                row[target] = target == column ? values[index] ?? DBNull.Value : rows[index][target.ColumnName];
            }
            result.Rows.Add(row);
        }
        return result;
    }

    /// <summary>
    /// Commits a staged column to an existing table.
    /// </summary>
    /// <param name="table">The target table.</param>
    /// <param name="staged">The staged values.</param>
    /// <param name="name">The output name.</param>
    /// <returns>The target table.</returns>
    private static DataTable Commit(DataTable table,
        DataTable staged,
        string name)
    {
        var column = table.Columns[name] ?? TableHelper.AddColumn(table,
            name,
            staged.Columns[name]!.DataType);
        var rows = InspectionHelper.Rows(table)
            .ToArray();
        for (var index = 0; index < rows.Length; index++)
        {
            rows[index][column] = staged.Rows[index][name];
        }
        return table;
    }

    /// <summary>
    /// Validates assignment names before execution.
    /// </summary>
    /// <param name="assignments">The computations.</param>
    private static void ValidateAssignments(ColumnAssignment[] assignments)
    {
        foreach (var assignment in assignments)
        {
            ArgumentNullException.ThrowIfNull(assignment,
                nameof(assignments));
        }
        if (assignments
            .Select(assignment => assignment.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() != assignments.Length)
        {
            throw new ArgumentException("Assignment names must be distinct.",
                nameof(assignments));
        }
    }

    /// <summary>
    /// Commits staged assignments after checking type compatibility.
    /// </summary>
    /// <param name="table">The target table.</param>
    /// <param name="staged">The staged result.</param>
    /// <param name="assignments">The computations.</param>
    /// <returns>The target table.</returns>
    private static DataTable CommitAssignments(DataTable table,
        DataTable staged,
        ColumnAssignment[] assignments)
    {
        foreach (var assignment in assignments)
        {
            var existing = table.Columns[assignment.Name];
            if (existing is not null && existing.DataType != staged.Columns[assignment.Name]!.DataType)
            {
                throw new InvalidOperationException("Changing an existing column type requires a new table.");
            }
        }
        foreach (var assignment in assignments)
        {
            Commit(table,
                staged,
                assignment.Name);
        }
        return table;
    }

    #endregion
}
