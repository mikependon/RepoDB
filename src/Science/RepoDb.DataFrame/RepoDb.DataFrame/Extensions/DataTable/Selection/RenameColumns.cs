using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Provides analytical DataTable operations.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Renames columns using a validated name mapping.
    /// </summary>
    /// <param name="table">The source table.</param>
    /// <param name="names">The original and replacement names.</param>
    /// <param name="inPlace">Whether to modify the source.</param>
    /// <returns>The operation result.</returns>
    public static DataTable RenameColumns(this DataTable table,
        IReadOnlyDictionary<string, string> names,
        bool inPlace = false)
    {
        ArgumentNullException.ThrowIfNull(table);
        ArgumentNullException.ThrowIfNull(names);
        var mapping = names
            .ToDictionary(pair => InspectionHelper.Column(table,
                pair.Key),
                pair => pair.Value);
        var outputs = table.Columns
            .Cast<DataColumn>()
            .Select(column => mapping.GetValueOrDefault(column,
                column.ColumnName))
                .ToArray();
        foreach (var name in outputs)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name,
                nameof(names));
        }
        if (outputs
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count() != outputs.Length)
        {
            throw new ArgumentException("Output names must be distinct.",
                nameof(names));
        }
        return RenameHelper.Execute(table,
            outputs,
            inPlace);
    }

    #endregion
}
