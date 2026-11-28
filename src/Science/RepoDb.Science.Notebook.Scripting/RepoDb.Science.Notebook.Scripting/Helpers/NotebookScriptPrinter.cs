using System.Collections;
using System.Data;
using System.Globalization;

namespace RepoDb.Science.Helpers;

/// <summary>
/// Writes a printed value as its type and content.
/// </summary>
internal static class NotebookScriptPrinter
{
    #region Public Variables

    /// <summary>
    /// The most items of a collection that are written.
    /// </summary>
    public const int ItemLimit = 10;

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes a value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The line.</returns>
    public static string Format(object? value) => value switch
    {
        null => "null",
        DataTable table => FormatTable(table),
        DataSet set => FormatSet(set),
        IEnumerable items and not string => FormatItems(items),
        _ => $"{NotebookScriptTypeNames.Of(value.GetType())}: {TextOf(value)}"
    };

    /// <summary>
    /// Writes what stands for a value whose text cannot be written.
    /// </summary>
    /// <param name="exception">Why it cannot be written.</param>
    /// <returns>The text.</returns>
    public static string FailureOf(Exception exception) => $"<ToString failed: {MessageOf(exception)}>";

    /// <summary>
    /// Reads the message of an exception, which never fails for an exception whose message cannot be read.
    /// </summary>
    /// <param name="exception">The exception, which a script may have declared.</param>
    /// <returns>The message.</returns>
    public static string MessageOf(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        try
        {
            return exception.Message;
        }
        catch (Exception failure) when (failure is not OutOfMemoryException)
        {
            return "<Message failed>";
        }
    }

    /// <summary>
    /// Writes the text of a value, which never fails for a value that cannot be written.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The text.</returns>
    public static string TextOf(object? value)
    {
        try
        {
            return value is IFormattable formattable
                ? formattable.ToString(null,
                    CultureInfo.InvariantCulture)
                : value is null ? "null" : value.ToString() ?? string.Empty;
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            return FailureOf(exception);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Writes the first items of a collection.
    /// </summary>
    /// <param name="items">The collection.</param>
    /// <returns>The line.</returns>
    private static string FormatItems(IEnumerable items)
    {
        var shown = items
            .Cast<object?>()
            .Take(ItemLimit + 1)
            .ToList();
        var text = string.Join(", ",
            shown
                .Take(ItemLimit)
                .Select(ItemOf));
        var more = shown.Count > ItemLimit ? ", ..." : string.Empty;
        return $"{NotebookScriptTypeNames.Of(items.GetType())}: [{text}{more}]";
    }

    /// <summary>
    /// Writes the shape of a data set.
    /// </summary>
    /// <param name="set">The data set.</param>
    /// <returns>The line.</returns>
    private static string FormatSet(DataSet set) => string.Create(CultureInfo.CurrentCulture,
        $"{NotebookScriptTypeNames.Of(set.GetType())}: '{set.DataSetName}', {set.Tables.Count:N0} tables");

    /// <summary>
    /// Writes the shape of a data table.
    /// </summary>
    /// <param name="table">The data table.</param>
    /// <returns>The line.</returns>
    private static string FormatTable(DataTable table) => string.Create(CultureInfo.CurrentCulture,
        $"{NotebookScriptTypeNames.Of(table.GetType())}: '{table.TableName}', {table.Columns.Count:N0} columns, {table.Rows.Count:N0} rows");

    /// <summary>
    /// Writes one item of a collection.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The text.</returns>
    private static string ItemOf(object? item) => item is DataTable table
        ? string.Create(CultureInfo.CurrentCulture,
            $"'{table.TableName}' ({table.Rows.Count:N0} rows)")
        : TextOf(item);

    #endregion
}
