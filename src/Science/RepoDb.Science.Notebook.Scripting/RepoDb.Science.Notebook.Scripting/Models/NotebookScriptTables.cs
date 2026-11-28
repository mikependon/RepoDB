using System.Collections;
using System.Data;

namespace RepoDb.Science;

/// <summary>
/// The tables a script shows, each one once and in the order they were first shown.
/// </summary>
internal sealed class NotebookScriptTables
{
    #region Public Variables

    /// <summary>
    /// The most items of a collection that are looked at.
    /// </summary>
    public const int ItemLimit = 100;

    #endregion

    #region Private Variables

    private readonly List<DataTable> _items = [];

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty list of tables.
    /// </summary>
    public NotebookScriptTables()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the tables.
    /// </summary>
    public IReadOnlyList<DataTable> Items => _items;

    #endregion

    #region Public Methods

    /// <summary>
    /// Keeps the tables of a value: a data table, a data set or a collection that holds data tables.
    /// </summary>
    /// <param name="value">The value.</param>
    public void AddFrom(object? value)
    {
        switch (value)
        {
            case DataTable table:
                Add(table);
                break;
            case DataSet set:
                AddSet(set);
                break;
            case IEnumerable items and not string:
                AddItems(items);
                break;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Keeps a table once.
    /// </summary>
    /// <param name="table">The table.</param>
    private void Add(DataTable table)
    {
        if (!_items.Contains(table))
        {
            _items.Add(table);
        }
    }

    /// <summary>
    /// Keeps the tables among the first items of a collection.
    /// </summary>
    /// <param name="items">The collection.</param>
    private void AddItems(IEnumerable items)
    {
        foreach (var table in items
            .Cast<object?>()
            .Take(ItemLimit)
            .OfType<DataTable>())
        {
            Add(table);
        }
    }

    /// <summary>
    /// Keeps the tables of a data set.
    /// </summary>
    /// <param name="set">The data set.</param>
    private void AddSet(DataSet set)
    {
        foreach (DataTable table in set.Tables)
        {
            Add(table);
        }
    }

    #endregion
}
