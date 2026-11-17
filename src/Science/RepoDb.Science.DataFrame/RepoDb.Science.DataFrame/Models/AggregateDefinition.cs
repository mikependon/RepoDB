namespace RepoDb.Science;

/// <summary>
/// Defines a named column aggregation.
/// </summary>
public sealed class AggregateDefinition
{
    #region Constructors

    /// <summary>
    /// Creates an aggregation definition.
    /// </summary>
    /// <param name="column">The input column.</param>
    /// <param name="function">The summary function.</param>
    /// <param name="outputColumn">The optional output name.</param>
    public AggregateDefinition(string column,
        AggregateFunction function,
        string? outputColumn = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(column);
        if (!Enum.IsDefined(function))
        {
            throw new ArgumentOutOfRangeException(nameof(function));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(outputColumn ?? column,
            nameof(outputColumn));
        Column = column;
        Function = function;
        OutputColumn = outputColumn ?? column;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the input column name.
    /// </summary>
    public string Column { get; }

    /// <summary>
    /// Gets the summary function.
    /// </summary>
    public AggregateFunction Function { get; }

    /// <summary>
    /// Gets the output column name.
    /// </summary>
    public string OutputColumn { get; }

    #endregion
}
