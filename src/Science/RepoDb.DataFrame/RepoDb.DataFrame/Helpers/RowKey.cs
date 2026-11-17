namespace RepoDb.Science.Helpers;

/// <summary>
/// Compares composite keys using CLR value equality.
/// </summary>
internal sealed class RowKey : IEquatable<RowKey>
{
    #region Private Variables

    private readonly object?[] _values;

    #endregion

    #region Constructors

    /// <summary>
    /// Captures normalized key values.
    /// </summary>
    /// <param name="values">The key values.</param>
    public RowKey(IEnumerable<object?> values)
    {
        _values = values
            .Select(value => InspectionHelper.IsMissing(value) ? null : value)
            .ToArray();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets whether any key component is missing.
    /// </summary>
    public bool HasNull => _values
        .Any(value => value is null);

    #endregion

    #region Public Methods

    /// <summary>
    /// Compares composite keys.
    /// </summary>
    /// <param name="other">The other key.</param>
    /// <returns>Whether the keys match.</returns>
    public bool Equals(RowKey? other) => other is not null && _values
        .SequenceEqual(other._values);

    /// <summary>
    /// Compares an object with this key.
    /// </summary>
    /// <param name="obj">The other object.</param>
    /// <returns>Whether the keys match.</returns>
    public override bool Equals(object? obj) => obj is RowKey other && Equals(other);

    /// <summary>
    /// Computes an equality-compatible hash.
    /// </summary>
    /// <returns>The hash code.</returns>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (var value in _values)
        {
            hash.Add(value);
        }
        return hash.ToHashCode();
    }

    #endregion
}
