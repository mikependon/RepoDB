using System.Data;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Combines materialized current table values.
/// </summary>
internal static class CombiningHelper
{
    #region Public Methods

    /// <summary>
    /// Appends a table with an identical ordered schema.
    /// </summary>
    /// <param name="target">The output table.</param>
    /// <param name="source">The appended table.</param>
    public static void Append(DataTable target,
        DataTable source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var left = target.Columns
            .Cast<DataColumn>();
        var right = source.Columns
            .Cast<DataColumn>();
        if (!left
            .Select(column => (column.ColumnName,
                column.DataType))
            .SequenceEqual(right
            .Select(column => (column.ColumnName,
                column.DataType))))
        {
            throw new ArgumentException("Concatenated schemas must have matching ordered names and types.",
                nameof(source));
        }
        for (var index = 0; index < target.Columns.Count; index++)
        {
            target.Columns[index].AllowDBNull |= source.Columns[index].AllowDBNull;
        }
        foreach (var row in InspectionHelper.Rows(source))
        {
            target.Rows.Add(row.ItemArray);
        }
    }

    /// <summary>
    /// Executes a composite-key hash join.
    /// </summary>
    /// <param name="left">The left table.</param>
    /// <param name="right">The right table.</param>
    /// <param name="leftOn">The left keys.</param>
    /// <param name="rightOn">The right keys.</param>
    /// <param name="how">The join mode.</param>
    /// <param name="suffix">The duplicate-name suffix.</param>
    /// <returns>The joined table.</returns>
    public static DataTable Join(DataTable left,
        DataTable right,
        string[] leftOn,
        string[] rightOn,
        JoinType how,
        string suffix)
    {
        var leftKeys = TableHelper.Columns(left,
            leftOn);
        var rightKeys = TableHelper.Columns(right,
            rightOn);
        ValidateKeys(leftKeys,
            rightKeys,
            how);
        ArgumentException.ThrowIfNullOrWhiteSpace(suffix);
        var result = JoinSchema(left,
            right,
            suffix);
        foreach (var pair in Matches(left,
            right,
            leftKeys,
            rightKeys,
            how))
        {
            var values = Values(pair.Left,
                left.Columns.Count)
                    .Concat(Values(pair.Right,
                        right.Columns.Count))
                    .ToArray();
            result.Rows.Add(values);
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Checks key count, types, and join mode.
    /// </summary>
    /// <param name="left">The left key columns.</param>
    /// <param name="right">The right key columns.</param>
    /// <param name="how">The join mode.</param>
    private static void ValidateKeys(DataColumn[] left,
        DataColumn[] right,
        JoinType how)
    {
        if (!Enum.IsDefined(how))
        {
            throw new ArgumentOutOfRangeException(nameof(how));
        }
        if (left.Length == 0 || left.Length != right.Length ||
            !left
                .Select(column => column.DataType)
                .SequenceEqual(right
                .Select(column => column.DataType)))
        {
            throw new ArgumentException("Join keys require equal nonzero counts and matching types.",
                nameof(left));
        }
    }

    /// <summary>
    /// Creates nullable output columns with distinct names.
    /// </summary>
    /// <param name="left">The left table.</param>
    /// <param name="right">The right table.</param>
    /// <param name="suffix">The duplicate-name suffix.</param>
    /// <returns>The joined schema.</returns>
    private static DataTable JoinSchema(DataTable left,
        DataTable right,
        string suffix)
    {
        var result = TableHelper.Schema(left,
            left.Columns
                .Cast<DataColumn>());
        foreach (DataColumn column in right.Columns)
        {
            var name = result.Columns.Contains(column.ColumnName) ? column.ColumnName + suffix : column.ColumnName;
            if (result.Columns.Contains(name))
            {
                throw new ArgumentException("The right suffix produces duplicate output names.",
                    nameof(suffix));
            }
            TableHelper.CopyColumn(result,
                column,
                name);
        }
        foreach (DataColumn column in result.Columns)
        {
            column.AllowDBNull = true;
        }
        return result;
    }

    /// <summary>
    /// Enumerates matched and outer row pairs.
    /// </summary>
    /// <param name="left">The left table.</param>
    /// <param name="right">The right table.</param>
    /// <param name="leftKeys">The left keys.</param>
    /// <param name="rightKeys">The right keys.</param>
    /// <param name="how">The join mode.</param>
    /// <returns>The output row pairs.</returns>
    private static IEnumerable<(DataRow? Left,
        DataRow? Right)> Matches(DataTable left,
            DataTable right,
            DataColumn[] leftKeys,
            DataColumn[] rightKeys,
            JoinType how)
    {
        var rightRows = InspectionHelper.Rows(right)
            .ToArray();
        var lookup = rightRows
            .ToLookup(row => new RowKey(rightKeys
            .Select(column => row[column])));
        var matched = new HashSet<DataRow>();
        foreach (var row in InspectionHelper.Rows(left))
        {
            var key = new RowKey(leftKeys
                .Select(column => row[column]));
            var matches = key.HasNull ? Array.Empty<DataRow>() : lookup[key]
                .ToArray();
            foreach (var other in matches)
            {
                matched.Add(other);
                yield return (row,
                    other);
            }
            if (matches.Length == 0 && how is JoinType.Left or JoinType.Full)
            {
                yield return (row,
                    null);
            }
        }
        foreach (var row in rightRows
            .Where(row => !matched.Contains(row) && how is JoinType.Right or JoinType.Full))
        {
            yield return (null,
                row);
        }
    }

    /// <summary>
    /// Gets current row values or outer null placeholders.
    /// </summary>
    /// <param name="row">The optional row.</param>
    /// <param name="count">The column count.</param>
    /// <returns>The output values.</returns>
    private static IEnumerable<object?> Values(DataRow? row,
        int count)
    {
        return row is null ? Enumerable.Repeat<object?>(DBNull.Value,
            count) : row.ItemArray;
    }

    #endregion
}
