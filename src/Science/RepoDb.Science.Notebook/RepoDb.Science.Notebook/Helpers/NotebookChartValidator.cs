namespace RepoDb.Science.Helpers;

/// <summary>
/// Checks whether a table can be charted.
/// </summary>
internal static class NotebookChartValidator
{
    #region Public Methods

    /// <summary>
    /// Checks a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="failOnNonNumeric">Whether a column that is not numeric fails the check.</param>
    /// <returns>The outcome.</returns>
    public static NotebookChartCheck Check(NotebookTable table,
        bool failOnNonNumeric)
    {
        if (table.Rows.Count == 0)
        {
            return NotebookChartCheck.Failure("The result has no rows to chart.");
        }
        if (table.Columns.Count < 2)
        {
            return NotebookChartCheck.Failure("A chart needs a first column that labels its points and at least one numeric column after it.");
        }
        var timeBased = IsTimeBased(table);
        if (timeBased && !table.Rows
            .Any(HasTime))
        {
            return NotebookChartCheck.Failure("No row has a valid timestamp in its first column.");
        }
        return CheckColumns(table,
            failOnNonNumeric,
            timeBased);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Picks the columns that are drawn.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="failOnNonNumeric">Whether a column that is not numeric fails the check.</param>
    /// <param name="timeBased">Whether the first column is a time.</param>
    /// <returns>The outcome.</returns>
    private static NotebookChartCheck CheckColumns(NotebookTable table,
        bool failOnNonNumeric,
        bool timeBased)
    {
        var positions = Enumerable
            .Range(1,
                table.Columns.Count - 1)
            .ToLookup(index => NotebookChartValues.IsNumeric(table.Columns[index].TypeName));
        var skipped = positions[false]
            .Select(index => table.Columns[index])
            .ToList();
        if (failOnNonNumeric && skipped.Count > 0)
        {
            return NotebookChartCheck.Failure($"The column '{skipped[0].Name}' is not numeric ({skipped[0].TypeName}): every column after the first one must be numeric.");
        }
        return positions[true]
            .Any()
            ? new NotebookChartCheck(null,
                positions[true]
                    .ToList(),
                skipped
                    .Select(column => column.Name)
                    .ToList(),
                timeBased)
            : NotebookChartCheck.Failure("The result has no numeric column after the first one.");
    }

    /// <summary>
    /// Checks whether a row starts with a time.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <returns>Whether it does.</returns>
    private static bool HasTime(IReadOnlyList<string?> row) => NotebookChartValues.TryTimestamp(NotebookChartValues.TextAt(row,
            0),
        out _);

    /// <summary>
    /// Checks whether every text of the first column is a time.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>Whether they are.</returns>
    private static bool HoldsTimes(NotebookTable table)
    {
        var rows = table.Rows
            .Where(row => NotebookChartValues.TextAt(row,
                0) is not null)
            .ToList();
        return rows.Count > 0 && rows
            .All(row => HasTime(row));
    }

    /// <summary>
    /// Checks whether the first column is a time.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>Whether it is.</returns>
    private static bool IsTimeBased(NotebookTable table)
    {
        var typeName = table.Columns[0].TypeName;
        return NotebookChartValues.IsTimestamp(typeName) || (string.Equals(typeName,
            "string",
            StringComparison.Ordinal) && HoldsTimes(table));
    }

    #endregion
}
