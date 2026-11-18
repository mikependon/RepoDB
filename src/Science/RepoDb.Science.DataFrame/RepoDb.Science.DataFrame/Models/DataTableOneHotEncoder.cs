using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Applies a fitted one-hot encoding to DataTables.
/// </summary>
public sealed class DataTableOneHotEncoder
{
    #region Private Variables

    private readonly (string Name, string[] Categories)[] _encodedColumns;
    private readonly (string Name, Type Type)[] _retainedColumns;

    #endregion

    #region Constructors

    /// <summary>
    /// Stores the fitted columns and categories.
    /// </summary>
    /// <param name="encodedColumns">The categorical columns and their levels.</param>
    /// <param name="retainedColumns">The columns to copy.</param>
    private DataTableOneHotEncoder((string Name, string[] Categories)[] encodedColumns,
        (string Name, Type Type)[] retainedColumns)
    {
        _encodedColumns = encodedColumns;
        _retainedColumns = retainedColumns;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Learns ordinal string categories from current rows.
    /// </summary>
    /// <param name="table">The training table.</param>
    /// <param name="columns">The categorical column names.</param>
    /// <returns>The fitted encoder.</returns>
    public static DataTableOneHotEncoder Fit(DataTable table,
        params string[] columns)
    {
        var selected = TableHelper.Columns(table,
            columns);
        if (selected.Length == 0)
        {
            throw new ArgumentException("Select at least one categorical column.",
                nameof(columns));
        }
        var encoded = selected
            .Select(column => (column.ColumnName, ReadCategories(table,
                column)))
            .ToArray();
        var retained = table.Columns
            .Cast<DataColumn>()
            .Except(selected)
            .Select(column => (column.ColumnName, column.DataType))
            .ToArray();
        ValidateOutputNames(encoded,
            retained);
        return new DataTableOneHotEncoder(encoded,
            retained);
    }

    /// <summary>
    /// Applies the fitted schema, encoding null and unseen values as all zeros.
    /// </summary>
    /// <param name="table">The table containing the fitted source columns.</param>
    /// <returns>A new table with the fitted column order.</returns>
    public DataTable Transform(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var retained = _retainedColumns
            .Select(column => ResolveColumn(table,
                column.Name,
                column.Type))
            .ToArray();
        var encoded = _encodedColumns
            .Select(column => (Source: ResolveColumn(table,
                column.Name,
                typeof(string)), column.Categories))
            .ToArray();
        var result = CreateOutput(table,
            retained);
        foreach (var row in InspectionHelper.Rows(table))
        {
            result.Rows
                .Add(EncodeRow(row,
                    retained,
                    encoded,
                    result.Columns.Count));
        }
        return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates the fitted output schema.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="retained">The columns to copy.</param>
    /// <returns>The empty output table.</returns>
    private DataTable CreateOutput(DataTable table,
        DataColumn[] retained)
    {
        var result = TableHelper.Schema(table,
            Array.Empty<DataColumn>());
        for (var index = 0; index < retained.Length; index++)
        {
            TableHelper.CopyColumn(result,
                retained[index],
                _retainedColumns[index].Name);
        }
        foreach (var (name, categories) in _encodedColumns)
        {
            foreach (var category in categories)
            {
                TableHelper.AddColumn(result,
                    IndicatorName(name,
                        category),
                    typeof(int));
            }
        }
        return result;
    }

    /// <summary>
    /// Copies retained values and emits binary indicators for one row.
    /// </summary>
    /// <param name="row">The source row.</param>
    /// <param name="retained">The columns to copy.</param>
    /// <param name="encoded">The columns and fitted categories to encode.</param>
    /// <param name="width">The output column count.</param>
    /// <returns>The output values.</returns>
    private static object[] EncodeRow(DataRow row,
        DataColumn[] retained,
        (DataColumn Source, string[] Categories)[] encoded,
        int width)
    {
        var values = new object[width];
        var index = 0;
        foreach (var column in retained)
        {
            values[index++] = row[column];
        }
        foreach (var (source, categories) in encoded)
        {
            var value = row[source] as string;
            foreach (var category in categories)
            {
                values[index++] = string.Equals(value,
                    category,
                    StringComparison.Ordinal) ? 1 : 0;
            }
        }
        return values;
    }

    /// <summary>
    /// Forms the name of a category indicator.
    /// </summary>
    /// <param name="column">The source column name.</param>
    /// <param name="category">The fitted category.</param>
    /// <returns>The indicator column name.</returns>
    private static string IndicatorName(string column,
        string category) => $"{column}_{category}";

    /// <summary>
    /// Reads distinct non-null categories in ordinal order.
    /// </summary>
    /// <param name="table">The training table.</param>
    /// <param name="column">The categorical column.</param>
    /// <returns>The ordered categories.</returns>
    private static string[] ReadCategories(DataTable table,
        DataColumn column)
    {
        if (column.DataType != typeof(string))
        {
            throw new ArgumentException($"Column '{column.ColumnName}' must contain strings.",
                nameof(column));
        }
        return InspectionHelper.Rows(table)
            .Select(row => row[column])
            .OfType<string>()
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>
    /// Resolves a fitted column with its expected type.
    /// </summary>
    /// <param name="table">The input table.</param>
    /// <param name="name">The fitted column name.</param>
    /// <param name="type">The fitted column type.</param>
    /// <returns>The resolved source column.</returns>
    private static DataColumn ResolveColumn(DataTable table,
        string name,
        Type type)
    {
        var column = InspectionHelper.Column(table,
            name);
        if (column.DataType != type)
        {
            throw new ArgumentException($"Column '{name}' must have type '{type.Name}'.",
                nameof(table));
        }
        return column;
    }

    /// <summary>
    /// Rejects indicator names that conflict with fitted output names.
    /// </summary>
    /// <param name="encoded">The categorical columns and their levels.</param>
    /// <param name="retained">The columns to copy.</param>
    private static void ValidateOutputNames((string Name, string[] Categories)[] encoded,
        (string Name, Type Type)[] retained)
    {
        var names = new HashSet<string>(retained
            .Select(column => column.Name),
            StringComparer.Ordinal);
        foreach (var (name, categories) in encoded)
        {
            foreach (var category in categories)
            {
                var indicator = IndicatorName(name,
                    category);
                if (!names
                    .Add(indicator))
                {
                    throw new ArgumentException($"Indicator column '{indicator}' conflicts with another output column.",
                        nameof(encoded));
                }
            }
        }
    }

    #endregion
}
