namespace RepoDb.Science.Models;

/// <summary>
/// Carries an XML scalar and its declared type.
/// </summary>
internal readonly struct XmlLoadValue
{
    #region Constructors

    /// <summary>
    /// Stores a validated scalar and its lexical representation.
    /// </summary>
    /// <param name="type">The declared CLR type.</param>
    /// <param name="text">The original lexical value.</param>
    /// <param name="value">The parsed value.</param>
    public XmlLoadValue(Type type,
        string text,
        object? value)
    {
        Type = type;
        Text = text;
        Value = value;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the original lexical value.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the declared CLR type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the parsed value.
    /// </summary>
    public object? Value { get; }

    #endregion
}
