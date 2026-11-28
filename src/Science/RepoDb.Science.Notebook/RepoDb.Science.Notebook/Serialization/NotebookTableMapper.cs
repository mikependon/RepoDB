namespace RepoDb.Science.Serialization;

/// <summary>
/// Converts tables to and from their file shape.
/// </summary>
internal static class NotebookTableMapper
{
    #region Public Methods

    /// <summary>
    /// Reads a saved table.
    /// </summary>
    /// <param name="model">The saved table.</param>
    /// <returns>The table.</returns>
    public static NotebookTable FromModel(NotebookTableFileModel model) => new(ColumnsOf(model),
        RowsOf(model),
        model.IsTruncated,
        model.Title ?? string.Empty);

    /// <summary>
    /// Writes a table.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <returns>The saved table.</returns>
    public static NotebookTableFileModel ToModel(NotebookTable table) => new()
    {
        Columns = table.Columns
            .Select(column => (NotebookColumnFileModel?)new NotebookColumnFileModel { Name = column.Name, TypeName = column.TypeName })
            .ToList(),
        IsTruncated = table.IsTruncated,
        Rows = table.Rows
            .Select(row => (List<string?>?)row
                .ToList())
            .ToList(),
        Title = table.Title
    };

    #endregion

    #region Private Methods

    /// <summary>
    /// Reads the saved columns.
    /// </summary>
    /// <param name="model">The saved table.</param>
    /// <returns>The columns.</returns>
    private static List<NotebookColumn> ColumnsOf(NotebookTableFileModel model) => (model.Columns ?? [])
        .OfType<NotebookColumnFileModel>()
        .Select(column => new NotebookColumn(column.Name ?? string.Empty,
            string.IsNullOrWhiteSpace(column.TypeName) ? "string" : column.TypeName))
        .ToList();

    /// <summary>
    /// Reads the saved rows.
    /// </summary>
    /// <param name="model">The saved table.</param>
    /// <returns>The rows.</returns>
    private static List<IReadOnlyList<string?>> RowsOf(NotebookTableFileModel model) => (model.Rows ?? [])
        .OfType<List<string?>>()
        .Cast<IReadOnlyList<string?>>()
        .ToList();

    #endregion
}
