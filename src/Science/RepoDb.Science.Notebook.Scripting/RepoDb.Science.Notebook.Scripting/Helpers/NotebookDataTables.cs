using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Builds a data table from what a script has: the rows of a data reader, or a list of rows.
/// </summary>
internal static class NotebookDataTables
{
    #region Public Variables

    /// <summary>
    /// Why rows made of objects cannot be trimmed.
    /// </summary>
    public const string PropertiesRequirement = "The columns are the public properties of the objects.";

    #endregion

    #region Public Methods

    /// <summary>
    /// Builds a table from objects: a dictionary is a row by its keys, and another object by its public properties.
    /// </summary>
    /// <param name="rows">The objects. A null one is left out.</param>
    /// <returns>The data table.</returns>
    [RequiresUnreferencedCode(PropertiesRequirement)]
    public static DataTable FromObjects(IEnumerable<object?> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        return FromRows(rows
            .OfType<object>()
            .Select(RowOf)
            .ToList());
    }

    /// <summary>
    /// Reads the rows of the first result of an open data reader.
    /// </summary>
    /// <param name="reader">The data reader, which stays open.</param>
    /// <returns>The data table.</returns>
    public static DataTable FromReader(IDataReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var table = new DataTable { Locale = CultureInfo.InvariantCulture };
        AddReaderColumns(table,
            reader);
        ReadRows(table,
            reader);
        return table;
    }

    /// <summary>
    /// Builds a table whose columns are the keys of the rows. A row without a key has a database null there.
    /// </summary>
    /// <param name="rows">The rows. A null one is left out.</param>
    /// <returns>The data table.</returns>
    public static DataTable FromRows(IEnumerable<IDictionary<string, object?>?> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var items = rows
            .OfType<IDictionary<string, object?>>()
            .ToList();
        var table = new DataTable { Locale = CultureInfo.InvariantCulture };
        AddKeyColumns(table,
            items);
        AddRows(table,
            items);
        return table;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a column for each key of the rows.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="items">The rows.</param>
    [UnconditionalSuppressMessage("Trimming",
        "IL2072",
        Justification = "The type of a column is the type of a value that exists, so it is kept.")]
    private static void AddKeyColumns(DataTable table,
        List<IDictionary<string, object?>> items)
    {
        foreach (var name in items
            .SelectMany(row => row.Keys)
            .Distinct(StringComparer.Ordinal))
        {
            table.Columns.Add(name,
                TypeOf(items,
                    name));
        }
    }

    /// <summary>
    /// Adds the columns of a reader, naming those without a name and numbering a name that is taken.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="reader">The data reader.</param>
    private static void AddReaderColumns(DataTable table,
        IDataReader reader)
    {
        for (var position = 0; position < reader.FieldCount; position++)
        {
            table.Columns.Add(UniqueName(table,
                    reader.GetName(position),
                    position),
                reader.GetFieldType(position));
        }
    }

    /// <summary>
    /// Adds a row to the table for each row.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="items">The rows.</param>
    private static void AddRows(DataTable table,
        List<IDictionary<string, object?>> items)
    {
        table.BeginLoadData();
        foreach (var item in items)
        {
            table.Rows.Add(ValuesOf(table,
                item));
        }
        table.EndLoadData();
    }

    /// <summary>
    /// Reads the rows of the reader into the table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="reader">The data reader.</param>
    private static void ReadRows(DataTable table,
        IDataReader reader)
    {
        var values = new object[reader.FieldCount];
        table.BeginLoadData();
        while (values.Length > 0 && reader.Read())
        {
            reader.GetValues(values);
            table.Rows.Add(values);
        }
        table.EndLoadData();
    }

    /// <summary>
    /// Gets an object as a row.
    /// </summary>
    /// <param name="item">The object.</param>
    /// <returns>The object when it is a dictionary, and its public properties otherwise.</returns>
    [RequiresUnreferencedCode(PropertiesRequirement)]
    private static IDictionary<string, object?> RowOf(object item) => item as IDictionary<string, object?> ?? item
        .GetType()
        .GetProperties()
        .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
        .ToDictionary(property => property.Name,
            property => property.GetValue(item),
            StringComparer.Ordinal);

    /// <summary>
    /// Gets the type of a column.
    /// </summary>
    /// <param name="items">The rows.</param>
    /// <param name="name">The column.</param>
    /// <returns>The type of the values when they all have one type, and object otherwise.</returns>
    private static Type TypeOf(List<IDictionary<string, object?>> items,
        string name)
    {
        var types = items
            .Select(item => item.TryGetValue(name,
                out var value) ? value?.GetType() : null)
            .OfType<Type>()
            .Distinct()
            .ToList();
        return types.Count == 1 ? types[0] : typeof(object);
    }

    /// <summary>
    /// Gets a name for a column of a reader that no other column has.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="name">The name the reader gives.</param>
    /// <param name="position">The position of the column.</param>
    /// <returns>The name.</returns>
    private static string UniqueName(DataTable table,
        string name,
        int position)
    {
        var baseName = string.IsNullOrWhiteSpace(name)
            ? string.Create(CultureInfo.InvariantCulture,
                $"Column{position + 1}")
            : name;
        var unique = baseName;
        for (var number = 2; table.Columns.Contains(unique); number++)
        {
            unique = string.Create(CultureInfo.InvariantCulture,
                $"{baseName}_{number}");
        }
        return unique;
    }

    /// <summary>
    /// Gets the values of a row in the order of the columns.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="item">The row.</param>
    /// <returns>The values.</returns>
    private static object[] ValuesOf(DataTable table,
        IDictionary<string, object?> item) => table.Columns
        .Cast<DataColumn>()
        .Select(column => item.TryGetValue(column.ColumnName,
            out var value)
            ? value ?? DBNull.Value
            : DBNull.Value)
        .ToArray();

    #endregion
}
