using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// The values C# scripts share, and the results of the SQL sections. Nothing here is saved with the notebook.
/// </summary>
public sealed class NotebookScriptContext
{
    #region Private Variables

    private readonly NotebookScriptingMessages _messages;
    private readonly Dictionary<string, DataTable[]> _sqlResults = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, object?> _values = new(StringComparer.Ordinal);

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty context.
    /// </summary>
    /// <param name="messages">The texts of the problems.</param>
    internal NotebookScriptContext(NotebookScriptingMessages messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        _messages = messages;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the names of the stored values.
    /// </summary>
    public IReadOnlyCollection<string> Names => _values.Keys;

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the result tables of a SQL section. The name ignores case.
    /// </summary>
    /// <param name="sectionName">The name of the SQL section.</param>
    /// <returns>The tables, one for each result set.</returns>
    public DataTable[] FromSqlResult(string sectionName) => _sqlResults.TryGetValue(sectionName,
        out var tables)
        ? tables
        : throw new InvalidOperationException(_messages.SqlResultMissing(sectionName));

    /// <summary>
    /// Gets a value a script stored. The name is case-sensitive.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <returns>The value.</returns>
    public object? Get(string name) => _values.TryGetValue(name,
        out var value)
        ? value
        : throw new InvalidOperationException(_messages.ValueMissing(name));

    /// <summary>
    /// Gets a stored value as a type. The name is case-sensitive.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <returns>The value.</returns>
    public T Get<T>(string name) => As<T>(name,
        Get(name));

    /// <summary>
    /// Tells whether a value is stored.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <returns>Whether it is stored.</returns>
    public bool Has(string name) => _values.ContainsKey(name);

    /// <summary>
    /// Removes a stored value.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <returns>Whether there was one.</returns>
    public bool Remove(string name) => _values.Remove(name);

    /// <summary>
    /// Stores a value for the scripts that run later, replacing the value of the same name.
    /// </summary>
    /// <param name="name">The name of the value.</param>
    /// <param name="value">The value.</param>
    public void Set(string name,
        object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _values[name] = value;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Removes every value and SQL result. Only the host does this, through its executor.
    /// </summary>
    internal void Clear()
    {
        _sqlResults.Clear();
        _values.Clear();
    }

    /// <summary>
    /// Replaces the SQL results with fresh data tables.
    /// </summary>
    /// <param name="results">The tables of each SQL section, by name.</param>
    internal void Load(IReadOnlyDictionary<string, IReadOnlyList<NotebookTable>> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        _sqlResults.Clear();
        foreach (var (sectionName, tables) in results)
        {
            _sqlResults[sectionName] = ToDataTables(tables);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Converts the tables of a section, which are named after their titles.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>The data tables.</returns>
    private static DataTable[] ToDataTables(IReadOnlyList<NotebookTable> tables) => tables
        .Select(table => NotebookDataTableConverter.ToDataTable(table,
            table.Title))
        .ToArray();

    /// <summary>
    /// Reads a value as a type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="name">The name of the value.</param>
    /// <param name="value">The value.</param>
    /// <returns>The typed value.</returns>
    private T As<T>(string name,
        object? value)
    {
        if (value is T typed)
        {
            return typed;
        }
        if (value is null && default(T) is null)
        {
            return default!;
        }
        throw new InvalidCastException(_messages.ValueTypeMismatch(name,
            value?.GetType(),
            typeof(T)));
    }

    #endregion
}
