using System.Text.Json;
using RepoDb.Science.Helpers;

namespace RepoDb.Science.Serialization;

/// <summary>
/// Builds a notebook from the content of its file.
/// </summary>
internal static class NotebookFileReader
{
    #region Public Methods

    /// <summary>
    /// Builds the notebook.
    /// </summary>
    /// <param name="model">The content of the file.</param>
    /// <returns>The notebook.</returns>
    public static NotebookDocument Read(NotebookFileModel model)
    {
        if (model.FormatVersion > NotebookDocument.FormatVersion)
        {
            throw new InvalidDataException($"The notebook has format version {model.FormatVersion}, which is newer than this version reads.");
        }
        var document = new NotebookDocument();
        document.Sections.Load((model.Sections ?? [])
            .OfType<NotebookSectionFileModel>()
            .Select(ReadSection)
            .ToList());
        foreach (var pair in model.HostData ?? new Dictionary<string, JsonElement>(StringComparer.Ordinal))
        {
            document.HostData[pair.Key] = pair.Value;
        }
        return document;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Builds a chart section.
    /// </summary>
    /// <param name="model">The saved section.</param>
    /// <returns>The section.</returns>
    private static NotebookChartSection ReadChart(NotebookSectionFileModel model)
    {
        if (!Enum.IsDefined(model.ChartSource))
        {
            throw new InvalidDataException("The notebook has an unknown chart source.");
        }
        var section = new NotebookChartSection
        {
            Source = model.ChartSource,
            SqlScript = model.Script ?? string.Empty,
            CSharpScript = model.ChartCode ?? string.Empty,
            ChartType = NotebookChartNames.ParseType(model.ChartType),
            Bucket = NotebookChartNames.ParseBucket(model.ChartBucket)
        };
        section.SetColors(NotebookChartColors.Normalize(model.ChartColors));
        return section;
    }

    /// <summary>
    /// Builds a code section.
    /// </summary>
    /// <param name="model">The saved section.</param>
    /// <param name="language">The language.</param>
    /// <returns>The section.</returns>
    private static NotebookCodeSection ReadCode(NotebookSectionFileModel model,
        NotebookCodeLanguage language)
    {
        var section = new NotebookCodeSection
        {
            Language = language,
            Script = model.Script ?? string.Empty
        };
        section.SetName(model.Name ?? string.Empty);
        return section;
    }

    /// <summary>
    /// Reads the saved log.
    /// </summary>
    /// <param name="model">The saved section.</param>
    /// <returns>The entries.</returns>
    private static IEnumerable<NotebookLogEntry> ReadLog(NotebookSectionFileModel model) => (model.Logs ?? [])
        .OfType<NotebookLogFileModel>()
        .Select(entry => new NotebookLogEntry(NotebookLogLevelOf(entry),
            entry.Message ?? string.Empty,
            entry.Time));

    /// <summary>
    /// Requires a known log level.
    /// </summary>
    /// <param name="entry">The saved entry.</param>
    /// <returns>The level.</returns>
    private static NotebookLogLevel NotebookLogLevelOf(NotebookLogFileModel entry) => Enum.IsDefined(entry.Level)
        ? entry.Level
        : throw new InvalidDataException("The notebook has an unknown log level.");

    /// <summary>
    /// Restores what the last run produced.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void RestoreRun(NotebookExecutableSection section,
        NotebookSectionFileModel model)
    {
        section.EditorHeight = model.EditorHeight;
        section.RestoreRun(NotebookValues.Elapsed(model.ElapsedSeconds),
            ReadLog(model));
        if (section is NotebookImportsSection imports)
        {
            imports.RestoreCheck();
        }
        if (section is NotebookScriptSection script)
        {
            RestoreScript(script,
                model);
        }
    }

    /// <summary>
    /// Restores the connection and results of a script section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void RestoreScript(NotebookScriptSection section,
        NotebookSectionFileModel model)
    {
        section.ConnectionId = model.ConnectionId;
        section.TableHeight = model.TableHeight;
        var results = (model.Results ?? [])
            .OfType<NotebookTableFileModel>()
            .Select(NotebookTableMapper.FromModel)
            .ToList();
        section.RestoreResults(model.Output ?? string.Empty,
            results,
            model.ResultStatus ?? string.Empty);
    }

    /// <summary>
    /// Builds a section of the saved kind.
    /// </summary>
    /// <param name="model">The saved section.</param>
    /// <returns>The section.</returns>
    private static NotebookSection ReadSection(NotebookSectionFileModel model)
    {
        NotebookSection section = model.Language switch
        {
            NotebookFileSectionKind.Header => new NotebookHeaderSection
            {
                Title = model.Title ?? string.Empty,
                Description = model.Description ?? string.Empty
            },
            NotebookFileSectionKind.Imports => new NotebookImportsSection { Script = model.Script ?? string.Empty },
            NotebookFileSectionKind.Sql => ReadCode(model,
                NotebookCodeLanguage.Sql),
            NotebookFileSectionKind.CSharp => ReadCode(model,
                NotebookCodeLanguage.CSharp),
            NotebookFileSectionKind.Chart => ReadChart(model),
            _ => throw new InvalidDataException("The notebook has an unknown section kind.")
        };
        section.IsCollapsed = model.IsCollapsed;
        if (section is NotebookExecutableSection executable)
        {
            RestoreRun(executable,
                model);
        }
        return section;
    }

    #endregion
}
