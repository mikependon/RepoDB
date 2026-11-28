using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Keeps the last lines of what a script wrote.
/// </summary>
internal static class NotebookOutputTrimmer
{
    #region Public Methods

    /// <summary>
    /// Gets the last lines of a text, with a note about the lines left out.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="limit">The lines to keep.</param>
    /// <returns>The trimmed text.</returns>
    public static string LastLines(string text,
        int limit)
    {
        var lines = text
            .TrimEnd()
            .Split('\n')
            .ToList();
        var omitted = TakeMarker(lines);
        var keep = Math.Max(1,
            limit);
        return lines.Count <= keep
            ? text
            : Join(omitted + lines.Count - keep,
                lines
                    .Skip(lines.Count - keep));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Writes the note followed by the kept lines.
    /// </summary>
    /// <param name="omitted">The lines left out.</param>
    /// <param name="lines">The kept lines.</param>
    /// <returns>The text.</returns>
    private static string Join(int omitted,
        IEnumerable<string> lines) => Marker(omitted) + Environment.NewLine + string.Join('\n',
            lines);

    /// <summary>
    /// Writes the note about the lines left out.
    /// </summary>
    /// <param name="omitted">The lines left out.</param>
    /// <returns>The note.</returns>
    private static string Marker(int omitted) => string.Create(CultureInfo.InvariantCulture,
        $"... {omitted:N0} earlier {(omitted == 1 ? "line" : "lines")} not shown");

    /// <summary>
    /// Removes an earlier note from the lines.
    /// </summary>
    /// <param name="lines">The lines.</param>
    /// <returns>The number the note held, or zero.</returns>
    private static int TakeMarker(List<string> lines)
    {
        var words = lines[0]
            .Trim()
            .Split(' ');
        if (words is not ["...", var number, "earlier", "line" or "lines", "not", "shown"]
            || !int.TryParse(number,
                NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var omitted))
        {
            return 0;
        }
        lines.RemoveAt(0);
        return omitted;
    }

    #endregion
}
