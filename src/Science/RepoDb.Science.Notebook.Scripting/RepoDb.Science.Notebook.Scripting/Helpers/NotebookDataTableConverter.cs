using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Converts between notebook tables, which hold text, and typed data tables.
/// </summary>
internal static class NotebookDataTableConverter
{
    #region Public Methods

    /// <summary>
    /// Creates a typed data table. A value that cannot be read as the type of its column becomes a database null.
    /// </summary>
    /// <param name="source">The notebook table.</param>
    /// <param name="name">The name of the data table.</param>
    /// <returns>The data table.</returns>
    public static DataTable ToDataTable(NotebookTable source,
        string name)
    {
        var table = new DataTable(name) { Locale = CultureInfo.InvariantCulture };
        AddColumns(table,
            source);
        AddRows(table,
            source);
        return table;
    }

    /// <summary>
    /// Creates a notebook table from the rows of a data table that are not deleted.
    /// </summary>
    /// <param name="source">The data table. One without a name is titled Table.</param>
    /// <param name="rowLimit">The most rows kept.</param>
    /// <returns>The notebook table.</returns>
    public static NotebookTable ToNotebookTable(DataTable source,
        int rowLimit)
    {
        var rows = LiveRowsOf(source);
        return new NotebookTable(ColumnsOf(source),
            rows
                .Take(rowLimit)
                .Select(TextsOf)
                .ToList(),
            rows.Count > rowLimit,
            source.TableName.Length > 0 ? source.TableName : "Table");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a typed column under a name no other column has.
    /// </summary>
    /// <param name="columns">The columns of the data table.</param>
    /// <param name="column">The notebook column.</param>
    /// <param name="position">The position of the column.</param>
    [UnconditionalSuppressMessage("Trimming",
        "IL2072",
        Justification = "A notebook column type is a primitive or another type the runtime always keeps.")]
    private static void AddColumn(DataColumnCollection columns,
        NotebookColumn column,
        int position) => columns.Add(UniqueName(columns,
            column.Name,
            position),
        NotebookValueConverter.TypeOf(column.TypeName));

    /// <summary>
    /// Adds the columns of a notebook table.
    /// </summary>
    /// <param name="table">The data table.</param>
    /// <param name="source">The notebook table.</param>
    private static void AddColumns(DataTable table,
        NotebookTable source)
    {
        for (var position = 0; position < source.Columns.Count; position++)
        {
            AddColumn(table.Columns,
                source.Columns[position],
                position);
        }
    }

    /// <summary>
    /// Adds the rows of a notebook table.
    /// </summary>
    /// <param name="table">The data table, which has its columns.</param>
    /// <param name="source">The notebook table.</param>
    private static void AddRows(DataTable table,
        NotebookTable source)
    {
        foreach (var row in source.Rows)
        {
            table.Rows.Add(ValuesOf(table.Columns,
                source.Columns,
                row));
        }
    }

    /// <summary>
    /// Gets the rows that are not deleted.
    /// </summary>
    /// <param name="source">The data table.</param>
    /// <returns>The rows.</returns>
    private static List<DataRow> LiveRowsOf(DataTable source) => source.Rows
        .Cast<DataRow>()
        .Where(row => row.RowState != DataRowState.Deleted)
        .ToList();

    /// <summary>
    /// Describes the columns of a data table.
    /// </summary>
    /// <param name="source">The data table.</param>
    /// <returns>The columns.</returns>
    private static List<NotebookColumn> ColumnsOf(DataTable source) => source.Columns
        .Cast<DataColumn>()
        .Select(column => new NotebookColumn(column.ColumnName,
            NotebookValueConverter.TypeNameOf(column.DataType)))
        .ToList();

    /// <summary>
    /// Writes the values of a row as text.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>The texts.</returns>
    private static IReadOnlyList<string?> TextsOf(DataRow row) => row.ItemArray
        .Select(TextOf)
        .ToList();

    /// <summary>
    /// Writes a value as text. A value whose text cannot be written is replaced by why, so the table is still shown.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The text, or null for a database null.</returns>
    private static string? TextOf(object? value)
    {
        try
        {
            return NotebookValueConverter.ToText(value);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return NotebookScriptPrinter.FailureOf(exception);
        }
    }

    /// <summary>
    /// Names a column without a name after its position, and ends a name that is taken with underscores. An exported
    /// notebook names its columns the same way.
    /// </summary>
    /// <param name="columns">The columns of the data table.</param>
    /// <param name="name">The wanted name.</param>
    /// <param name="position">The position of the column.</param>
    /// <returns>The free name.</returns>
    private static string UniqueName(DataColumnCollection columns,
        string name,
        int position)
    {
        var candidate = name.Length > 0
            ? name
            : string.Create(CultureInfo.InvariantCulture,
                $"Column{position + 1}");
        while (columns.Contains(candidate))
        {
            candidate += "_";
        }
        return candidate;
    }

    /// <summary>
    /// Reads a text as the value of a column.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="typeName">The type name of the column.</param>
    /// <param name="type">The type of the column.</param>
    /// <returns>The value.</returns>
    private static object ValueOf(string? text,
        string typeName,
        Type type)
    {
        var value = NotebookValueConverter.FromText(text,
            typeName);
        return type.IsInstanceOfType(value) ? value! : DBNull.Value;
    }

    /// <summary>
    /// Reads a row. A row shorter than the columns is completed with database nulls.
    /// </summary>
    /// <param name="columns">The columns of the data table.</param>
    /// <param name="sources">The notebook columns.</param>
    /// <param name="row">The texts of the row.</param>
    /// <returns>The values.</returns>
    private static object[] ValuesOf(DataColumnCollection columns,
        IReadOnlyList<NotebookColumn> sources,
        IReadOnlyList<string?> row) => sources
        .Select((source,
            index) => ValueOf(index < row.Count ? row[index] : null,
            source.TypeName,
            columns[index].DataType))
        .ToArray();

    #endregion
}
