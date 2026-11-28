using System.Globalization;
using System.Numerics;

namespace RepoDb.Science;

/// <summary>
/// Converts table values to saved text and back.
/// </summary>
public static class NotebookValueConverter
{
    #region Private Variables

    private static readonly Dictionary<string, Func<string, object?>> Parsers = new(StringComparer.Ordinal)
    {
        ["binary"] = ParseBinary,
        ["bool"] = text => bool.TryParse(text,
            out var flag) ? flag : null,
        ["byte"] = text => ParseNumber<byte>(text,
            NumberStyles.Integer),
        ["datetime"] = text => DateTime.TryParse(text,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var date) ? date : null,
        ["datetimeoffset"] = text => DateTimeOffset.TryParse(text,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var offset) ? offset : null,
        ["decimal"] = text => ParseNumber<decimal>(text,
            NumberStyles.Number),
        ["double"] = text => ParseNumber<double>(text,
            NumberStyles.Float),
        ["float"] = text => ParseNumber<float>(text,
            NumberStyles.Float),
        ["guid"] = text => Guid.TryParse(text,
            out var guid) ? guid : null,
        ["int"] = text => ParseNumber<int>(text,
            NumberStyles.Integer),
        ["long"] = text => ParseNumber<long>(text,
            NumberStyles.Integer),
        ["short"] = text => ParseNumber<short>(text,
            NumberStyles.Integer),
        ["timespan"] = text => TimeSpan.TryParse(text,
            CultureInfo.InvariantCulture,
            out var span) ? span : null
    };
    private static readonly Dictionary<string, Type> Types = new(StringComparer.Ordinal)
    {
        ["binary"] = typeof(byte[]),
        ["bool"] = typeof(bool),
        ["byte"] = typeof(byte),
        ["datetime"] = typeof(DateTime),
        ["datetimeoffset"] = typeof(DateTimeOffset),
        ["decimal"] = typeof(decimal),
        ["double"] = typeof(double),
        ["float"] = typeof(float),
        ["guid"] = typeof(Guid),
        ["int"] = typeof(int),
        ["long"] = typeof(long),
        ["short"] = typeof(short),
        ["timespan"] = typeof(TimeSpan)
    };

    #endregion

    #region Public Methods

    /// <summary>
    /// Reads a saved text as a value of a type.
    /// </summary>
    /// <param name="text">The text, where null is a database null.</param>
    /// <param name="typeName">The type name.</param>
    /// <returns>The value, or the text when it cannot be read.</returns>
    public static object? FromText(string? text,
        string typeName)
    {
        if (text is null)
        {
            return null;
        }
        return Parsers.TryGetValue(typeName,
            out var parse) ? parse(text) ?? text : text;
    }

    /// <summary>
    /// Gets the name of a type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name, or string for an unknown type.</returns>
    public static string TypeNameOf(Type? type)
    {
        var plain = type is null ? typeof(string) : Nullable.GetUnderlyingType(type) ?? type;
        return NumberNameOf(plain) ?? OtherNameOf(plain);
    }

    /// <summary>
    /// Gets the type of a type name.
    /// </summary>
    /// <param name="typeName">The type name.</param>
    /// <returns>The type, or string for an unknown name.</returns>
    public static Type TypeOf(string typeName) => Types.GetValueOrDefault(typeName) ?? typeof(string);

    /// <summary>
    /// Writes a value the same way on every machine.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The text, or null for a database null.</returns>
    public static string? ToText(object? value) => value switch
    {
        null or DBNull => null,
        DateTime date => date.ToString("O",
            CultureInfo.InvariantCulture),
        DateTimeOffset offset => offset.ToString("O",
            CultureInfo.InvariantCulture),
        TimeSpan span => span.ToString("c",
            CultureInfo.InvariantCulture),
        byte[] bytes => Convert.ToBase64String(bytes),
        IFormattable formattable => formattable.ToString(null,
            CultureInfo.InvariantCulture),
        _ => value.ToString()
    };

    #endregion

    #region Private Methods

    /// <summary>
    /// Gets the name of a numeric type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name, or null.</returns>
    private static string? NumberNameOf(Type type) => type == typeof(int) ? "int"
        : type == typeof(long) ? "long"
        : type == typeof(short) ? "short"
        : type == typeof(byte) ? "byte"
        : type == typeof(decimal) ? "decimal"
        : type == typeof(double) ? "double"
        : type == typeof(float) ? "float"
        : null;

    /// <summary>
    /// Gets the name of a type that is not numeric.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name.</returns>
    private static string OtherNameOf(Type type) => type == typeof(bool) ? "bool"
        : type == typeof(DateTime) ? "datetime"
        : type == typeof(DateTimeOffset) ? "datetimeoffset"
        : type == typeof(Guid) ? "guid"
        : type == typeof(TimeSpan) ? "timespan"
        : type == typeof(byte[]) ? "binary"
        : "string";

    /// <summary>
    /// Reads base64 text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The bytes, or null.</returns>
    private static byte[]? ParseBinary(string text)
    {
        var buffer = new byte[text.Length];
        return Convert.TryFromBase64String(text,
            buffer,
            out var written) ? buffer[..written] : null;
    }

    /// <summary>
    /// Reads a number.
    /// </summary>
    /// <typeparam name="T">The number type.</typeparam>
    /// <param name="text">The text.</param>
    /// <param name="style">The accepted format.</param>
    /// <returns>The number, or null.</returns>
    private static object? ParseNumber<T>(string text,
        NumberStyles style)
        where T : struct, INumber<T> => T.TryParse(text,
            style,
            CultureInfo.InvariantCulture,
            out var number) ? number : null;

    #endregion
}
