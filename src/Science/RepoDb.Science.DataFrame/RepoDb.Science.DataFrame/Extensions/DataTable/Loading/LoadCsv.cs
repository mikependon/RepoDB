using System.Data;
using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// Loads records from CSV.
/// </summary>
public static partial class DataTableExtensions
{
    #region Public Methods

    /// <summary>
    /// Loads a CSV file with a header.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="path">The input file.</param>
    /// <param name="separator">The field delimiter.</param>
    /// <param name="inferTypes">Whether to infer types for an empty schema.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadCsv(this DataTable table,
        string path,
        char separator = ',',
        bool inferTypes = false)
    {
        using var stream = File.OpenRead(path);
        return table.LoadCsv(stream,
            separator,
            inferTypes);
    }

    /// <summary>
    /// Loads CSV records and leaves the stream open.
    /// </summary>
    /// <param name="table">The optional column schema.</param>
    /// <param name="stream">The input stream.</param>
    /// <param name="separator">The field delimiter.</param>
    /// <param name="inferTypes">Whether to infer types for an empty schema.</param>
    /// <returns>The loaded table.</returns>
    public static DataTable LoadCsv(this DataTable table,
        Stream stream,
        char separator = ',',
        bool inferTypes = false) =>
        TableLoadHelper.Load(table,
            stream,
            inferTypes,
            (input, column, row) => CsvLoadHelper.Read(input,
                separator,
                column,
                row),
            inferText: inferTypes);

    #endregion
}
