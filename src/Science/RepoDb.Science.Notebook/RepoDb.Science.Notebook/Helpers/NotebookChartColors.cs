using System.Collections.ObjectModel;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Validates the custom colors of chart series.
/// </summary>
public static class NotebookChartColors
{
    #region Public Methods

    /// <summary>
    /// Copies the valid colors of series.
    /// </summary>
    /// <param name="colors">The colors by series name.</param>
    /// <returns>The valid colors in uppercase.</returns>
    public static IReadOnlyDictionary<string, string> Normalize(IReadOnlyDictionary<string, string>? colors)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in colors ?? ReadOnlyDictionary<string, string>.Empty)
        {
            if (NormalizeColor(pair.Value) is { } color)
            {
                result[pair.Key] = color;
            }
        }
        return new ReadOnlyDictionary<string, string>(result);
    }

    /// <summary>
    /// Normalizes a hexadecimal color such as #1A2B3C.
    /// </summary>
    /// <param name="color">The color text.</param>
    /// <returns>The color in uppercase, or null when it is not valid.</returns>
    public static string? NormalizeColor(string? color)
    {
        var text = color?.Trim();
        return text is { Length: 7 } && text[0] == '#' && text
            .Skip(1)
            .All(char.IsAsciiHexDigit)
            ? text.ToUpperInvariant()
            : null;
    }

    #endregion

    #region Internal Methods

    /// <summary>
    /// Writes colors as text that does not depend on their order.
    /// </summary>
    /// <param name="colors">The colors.</param>
    /// <returns>The text.</returns>
    internal static string Snapshot(IReadOnlyDictionary<string, string> colors) => NotebookText.Compose(colors
        .OrderBy(pair => pair.Key,
            StringComparer.Ordinal)
        .SelectMany(pair => new[] { pair.Key, pair.Value }));

    #endregion
}
