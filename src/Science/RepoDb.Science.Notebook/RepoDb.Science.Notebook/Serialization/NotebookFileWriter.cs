using System.Text.Json;

namespace RepoDb.Science.Serialization;

/// <summary>
/// Builds the content of the file of a notebook.
/// </summary>
internal static class NotebookFileWriter
{
    #region Public Methods

    /// <summary>
    /// Builds the content, as a copy that later changes do not affect.
    /// </summary>
    /// <param name="document">The notebook.</param>
    /// <returns>The content.</returns>
    public static NotebookFileModel Write(NotebookDocument document) => new()
    {
        Sections = document.Sections
            .Select(section => (NotebookSectionFileModel?)WriteSection(section))
            .ToList(),
        HostData = HostDataOf(document)
    };

    #endregion

    #region Private Methods

    /// <summary>
    /// Copies the host data, leaving out the names the package owns.
    /// </summary>
    /// <param name="document">The notebook.</param>
    /// <returns>The data.</returns>
    private static Dictionary<string, JsonElement> HostDataOf(NotebookDocument document) => document.HostData
        .Where(pair => pair.Key is not (nameof(NotebookFileModel.Sections) or nameof(NotebookFileModel.FormatVersion)))
        .ToDictionary(pair => pair.Key,
            pair => pair.Value,
            StringComparer.Ordinal);

    /// <summary>
    /// Writes the values of a chart.
    /// </summary>
    /// <param name="section">The chart.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteChart(NotebookChartSection section,
        NotebookSectionFileModel model)
    {
        model.Language = NotebookFileSectionKind.Chart;
        model.ChartSource = section.Source;
        model.Script = section.SqlScript;
        model.ChartCode = section.CSharpScript;
        model.ChartType = NotebookChartNames.NameOf(section.ChartType);
        model.ChartBucket = NotebookChartNames.NameOf(section.Bucket);
        model.ChartColors = new Dictionary<string, string>(section.Colors,
            StringComparer.Ordinal);
    }

    /// <summary>
    /// Writes the values of a code section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteCode(NotebookCodeSection section,
        NotebookSectionFileModel model)
    {
        model.Language = section.Language == NotebookCodeLanguage.Sql ? NotebookFileSectionKind.Sql : NotebookFileSectionKind.CSharp;
        model.Name = section.Name;
        model.Script = section.Script;
    }

    /// <summary>
    /// Writes the values of the kind of a section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteContent(NotebookSection section,
        NotebookSectionFileModel model)
    {
        switch (section)
        {
            case NotebookHeaderSection header:
                WriteHeader(header,
                    model);
                break;
            case NotebookImportsSection imports:
                model.Language = NotebookFileSectionKind.Imports;
                model.Script = imports.Script;
                break;
            case NotebookCodeSection code:
                WriteCode(code,
                    model);
                break;
            default:
                WriteChart((NotebookChartSection)section,
                    model);
                break;
        }
    }

    /// <summary>
    /// Writes the values of a header.
    /// </summary>
    /// <param name="section">The header.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteHeader(NotebookHeaderSection section,
        NotebookSectionFileModel model)
    {
        model.Language = NotebookFileSectionKind.Header;
        model.Title = section.Title;
        model.Description = section.Description;
    }

    /// <summary>
    /// Writes what the last run produced.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteRun(NotebookExecutableSection section,
        NotebookSectionFileModel model)
    {
        model.EditorHeight = section.EditorHeight;
        model.ElapsedSeconds = section.Elapsed.TotalSeconds;
        model.Logs = section.Log.Entries
            .Select(entry => (NotebookLogFileModel?)new NotebookLogFileModel
            {
                Level = entry.Level,
                Message = entry.Message,
                Time = entry.Time
            })
            .ToList();
    }

    /// <summary>
    /// Writes the connection and results of a script section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <param name="model">The saved section.</param>
    private static void WriteScript(NotebookScriptSection section,
        NotebookSectionFileModel model)
    {
        model.ConnectionId = section.ConnectionId;
        model.TableHeight = section.TableHeight;
        model.Output = section.Output;
        model.ResultStatus = section.ResultStatus;
        model.Results = section.Results
            .Select(table => (NotebookTableFileModel?)NotebookTableMapper.ToModel(table))
            .ToList();
    }

    /// <summary>
    /// Writes a section.
    /// </summary>
    /// <param name="section">The section.</param>
    /// <returns>The saved section.</returns>
    private static NotebookSectionFileModel WriteSection(NotebookSection section)
    {
        var model = new NotebookSectionFileModel { IsCollapsed = section.IsCollapsed };
        WriteContent(section,
            model);
        if (section is NotebookExecutableSection executable)
        {
            WriteRun(executable,
                model);
        }
        if (section is NotebookScriptSection script)
        {
            WriteScript(script,
                model);
        }
        return model;
    }

    #endregion
}
