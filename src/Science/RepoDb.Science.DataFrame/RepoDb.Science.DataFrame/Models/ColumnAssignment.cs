using System.Data;

namespace RepoDb.Science;

/// <summary>
/// Defines a computed output column.
/// </summary>
public sealed class ColumnAssignment
{
    #region Constructors

    /// <summary>
    /// Creates a column computation.
    /// </summary>
    /// <param name="name">The output name.</param>
    /// <param name="dataType">The stored CLR type.</param>
    /// <param name="selector">The value computation.</param>
    public ColumnAssignment(string name,
        Type dataType,
        Func<DataRow, object?> selector)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(dataType);
        ArgumentNullException.ThrowIfNull(selector);
        Name = name;
        DataType = dataType;
        Selector = selector;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the output name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the stored CLR type.
    /// </summary>
    public Type DataType { get; }

    /// <summary>
    /// Gets the value computation.
    /// </summary>
    public Func<DataRow, object?> Selector { get; }

    #endregion
}
