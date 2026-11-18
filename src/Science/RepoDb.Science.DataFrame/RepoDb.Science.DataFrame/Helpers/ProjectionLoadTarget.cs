using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Infers projected columns or fills the destination table.
/// </summary>
internal sealed class ProjectionLoadTarget
{
    #region Private Variables

    private readonly Dictionary<string, DataColumn> _columns;
    private readonly Dictionary<string, Type?> _declared = new(StringComparer.Ordinal);
    private readonly bool _inferText;
    private readonly SortedDictionary<int, DataRow> _indexed = new();
    private readonly DataTable? _table;
    private readonly Dictionary<string, Type?> _types = new(StringComparer.Ordinal);
    private int _nextRow;
    private int? _width;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an inference or materialization target.
    /// </summary>
    /// <param name="table">The destination table, when filling.</param>
    /// <param name="inferText">Whether to infer text types.</param>
    /// <param name="positionNames">The caller's positional schema.</param>
    public ProjectionLoadTarget(DataTable? table,
        bool inferText,
        IReadOnlyList<string>? positionNames)
    {
        _table = table;
        _inferText = inferText;
        _columns = table?.Columns
            .Cast<DataColumn>()
            .ToDictionary(column => column.ColumnName,
                StringComparer.Ordinal) ?? new(StringComparer.Ordinal);
        PositionNames = positionNames;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the caller's column names for the Values layout.
    /// </summary>
    public IReadOnlyList<string>? PositionNames { get; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Registers a column and an optional declared type.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="type">The declared column type.</param>
    public void AddColumn(string name,
        Type? type = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new FormatException("A projected column name cannot be blank.");
        }
        if (type is not null)
        {
            _declared.Add(name,
                type);
        }
        _types.TryAdd(name,
            null);
    }

    /// <summary>
    /// Adds a named record in document or saved index order.
    /// </summary>
    /// <param name="values">The record fields.</param>
    /// <param name="index">The saved zero-based row position.</param>
    public void AddRow(IReadOnlyDictionary<string, object?> values,
        int? index = null)
    {
        if (_table is null)
        {
            foreach (var (name, value) in values)
            {
                Observe(name,
                    value);
            }
            return;
        }
        var row = NewRow();
        var number = (long)(index ?? _nextRow++) + 1;
        foreach (var (name, value) in values)
        {
            Assign(row,
                name,
                value,
                number);
        }
        AddRecord(row,
            index);
    }

    /// <summary>
    /// Adds a positional record using the saved or supplied column names.
    /// </summary>
    /// <param name="values">The values in column order.</param>
    /// <param name="columns">The saved column names.</param>
    /// <param name="index">The saved zero-based row position.</param>
    public void AddPositional(IReadOnlyList<object?> values,
        IReadOnlyList<string>? columns,
        int? index = null)
    {
        _width ??= columns?.Count ?? values.Count;
        if (_width != values.Count)
        {
            throw new FormatException("A projected row has an unexpected number of values.");
        }
        var named = new Dictionary<string, object?>(values.Count,
            StringComparer.Ordinal);
        for (var position = 0; position < values.Count; position++)
        {
            named.Add(columns?[position] ?? "Column" + (position + 1).ToString(CultureInfo.InvariantCulture),
                values[position]);
        }
        AddRow(named,
            index);
    }

    /// <summary>
    /// Adds one column-oriented value at its row position.
    /// </summary>
    /// <param name="index">The saved zero-based row position.</param>
    /// <param name="name">The column name.</param>
    /// <param name="value">The scalar value.</param>
    public void AddCell(int index,
        string name,
        object? value)
    {
        if (_table is null)
        {
            Observe(name,
                value);
            return;
        }
        if (!_indexed.TryGetValue(index,
            out var row))
        {
            row = NewRow();
            _indexed.Add(index,
                row);
        }
        Assign(row,
            name,
            value,
            (long)index + 1);
    }

    /// <summary>
    /// Creates the inferred columns in discovery order.
    /// </summary>
    /// <param name="table">The destination table.</param>
    public void ApplySchema(DataTable table)
    {
        foreach (var (name, type) in _types)
        {
            TableHelper.AddColumn(table,
                name,
                _declared.GetValueOrDefault(name) ?? type ?? typeof(string));
        }
    }

    /// <summary>
    /// Adds indexed records in ascending position order.
    /// </summary>
    public void Complete()
    {
        foreach (var row in _indexed.Values)
        {
            _table!.Rows.Add(row);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Combines a value type with previous observations.
    /// </summary>
    /// <param name="name">The column name.</param>
    /// <param name="value">The scalar value.</param>
    private void Observe(string name,
        object? value)
    {
        _types.TryGetValue(name,
            out var previous);
        _types[name] = TableLoadHelper.CombineTypes(previous,
            TableLoadHelper.InferType(value,
                _inferText));
    }

    /// <summary>
    /// Creates a destination record with missing-value defaults.
    /// </summary>
    private DataRow NewRow()
    {
        var row = _table!.NewRow();
        foreach (var column in _columns.Values)
        {
            if (column.Expression.Length == 0)
            {
                row[column] = DBNull.Value;
            }
        }
        return row;
    }

    /// <summary>
    /// Assigns an input field to a matching writable column.
    /// </summary>
    /// <param name="row">The destination row.</param>
    /// <param name="name">The column name.</param>
    /// <param name="value">The scalar value.</param>
    /// <param name="number">The one-based diagnostic row number.</param>
    private void Assign(DataRow row,
        string name,
        object? value,
        long number)
    {
        if (_columns.TryGetValue(name,
            out var column) && column.Expression.Length == 0)
        {
            TableLoadHelper.SetValue(row,
                column,
                value,
                number);
        }
    }

    /// <summary>
    /// Adds a record immediately or keeps its saved position.
    /// </summary>
    /// <param name="row">The destination row.</param>
    /// <param name="index">The saved zero-based row position.</param>
    private void AddRecord(DataRow row,
        int? index)
    {
        if (index is null)
        {
            _table!.Rows.Add(row);
        }
        else if (!_indexed.TryAdd(index.Value,
            row))
        {
            throw new FormatException($"Duplicate row position {index.Value}.");
        }
    }

    #endregion
}
