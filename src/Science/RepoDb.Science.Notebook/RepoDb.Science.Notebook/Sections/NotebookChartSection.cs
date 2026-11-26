using RepoDb.Science.Helpers;

namespace RepoDb.Science;

/// <summary>
/// A chart drawn from the table of a SQL or a C# script.
/// </summary>
public sealed class NotebookChartSection : NotebookScriptSection
{
    #region Private Variables

    private NotebookChartBucket _bucket;
    private NotebookChartType _chartType;
    private IReadOnlyDictionary<string, string> _colors = NotebookChartColors.Normalize(null);
    private string _cSharpScript = string.Empty;
    private NotebookChartData? _data;
    private bool _failOnNonNumeric;
    private NotebookCodeLanguage _source;
    private string _sqlScript = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an empty chart that reads from SQL.
    /// </summary>
    public NotebookChartSection()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the width of the time buckets.
    /// </summary>
    public NotebookChartBucket Bucket
    {
        get => _bucket;
        set
        {
            if (SetContent(ref _bucket,
                NotebookValues.Defined(value,
                    nameof(value))))
            {
                Redraw();
            }
        }
    }

    /// <summary>
    /// Gets or sets how the series are drawn.
    /// </summary>
    public NotebookChartType ChartType
    {
        get => _chartType;
        set
        {
            if (SetContent(ref _chartType,
                NotebookValues.Defined(value,
                    nameof(value))))
            {
                Redraw();
            }
        }
    }

    /// <summary>
    /// Gets the custom colors by series name.
    /// </summary>
    public IReadOnlyDictionary<string, string> Colors => _colors;

    /// <summary>
    /// Gets or sets the C# script.
    /// </summary>
    public string CSharpScript
    {
        get => _cSharpScript;
        set => SetContent(ref _cSharpScript,
            value ?? string.Empty);
    }

    /// <summary>
    /// Gets what the chart draws, or null when it draws nothing.
    /// </summary>
    public NotebookChartData? Data
    {
        get => _data;
        private set => SetProperty(ref _data,
            value);
    }

    /// <summary>
    /// Gets or sets whether a column that is not numeric fails the chart. It is not saved.
    /// </summary>
    public bool FailOnNonNumeric
    {
        get => _failOnNonNumeric;
        set
        {
            if (SetProperty(ref _failOnNonNumeric,
                value))
            {
                Redraw();
            }
        }
    }

    /// <summary>
    /// Gets the language of the script that runs.
    /// </summary>
    public override NotebookCodeLanguage ScriptLanguage => Source;

    /// <summary>
    /// Gets the script that runs.
    /// </summary>
    public override string ScriptToRun => Source == NotebookCodeLanguage.CSharp ? CSharpScript : SqlScript;

    /// <summary>
    /// Gets or sets which script gives the table.
    /// </summary>
    public NotebookCodeLanguage Source
    {
        get => _source;
        set => SetContent(ref _source,
            NotebookValues.Defined(value,
                nameof(value)));
    }

    /// <summary>
    /// Gets or sets the SQL script.
    /// </summary>
    public string SqlScript
    {
        get => _sqlScript;
        set => SetContent(ref _sqlScript,
            value ?? string.Empty);
    }

    /// <summary>
    /// Gets the table the chart is drawn from, or null.
    /// </summary>
    public NotebookTable? Table => Results.Count > 0 ? Results[0] : null;

    /// <summary>
    /// Gets the kind of section.
    /// </summary>
    public override NotebookSectionType Type => NotebookSectionType.Chart;

    #endregion

    #region Public Methods

    /// <summary>
    /// Replaces the custom colors and redraws without running the script.
    /// </summary>
    /// <param name="colors">The colors by series name. Invalid colors are ignored.</param>
    public void SetColors(IReadOnlyDictionary<string, string> colors)
    {
        ArgumentNullException.ThrowIfNull(colors);
        var normalized = NotebookChartColors.Normalize(colors);
        if (string.Equals(NotebookChartColors.Snapshot(normalized),
            NotebookChartColors.Snapshot(_colors),
            StringComparison.Ordinal))
        {
            return;
        }
        CaptureContent();
        _colors = normalized;
        NotifyContent(nameof(Colors));
        Redraw();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Draws the table a C# script gave to the chart.
    /// </summary>
    /// <param name="chart">The table, already limited, or null.</param>
    /// <returns>The result.</returns>
    private protected override NotebookSectionResult CompleteCSharp(NotebookTable? chart) => Draw(chart,
        "The script did not give a table to the chart, so there is nothing to chart.");

    /// <summary>
    /// Draws the first table of a SQL script.
    /// </summary>
    /// <param name="tables">The tables.</param>
    /// <returns>The result.</returns>
    private protected override NotebookSectionResult CompleteSql(IReadOnlyList<NotebookTable> tables) => Draw(tables.Count > 0 ? tables[0] : null,
        "The script returned no result set to chart.");

    /// <summary>
    /// Gets the authored values.
    /// </summary>
    /// <returns>The values.</returns>
    private protected override IEnumerable<string?> GetContent() =>
    [
        Source.ToString(),
        SqlScript,
        CSharpScript,
        ConnectionId?.ToString(),
        ChartType.ToString(),
        Bucket.ToString(),
        NotebookChartColors.Snapshot(_colors)
    ];

    /// <summary>
    /// Redraws the chart for new results.
    /// </summary>
    private protected override void OnResultsChanged() => Redraw();

    /// <summary>
    /// Keeps and draws a table when it can be charted.
    /// </summary>
    /// <param name="table">The table, or null.</param>
    /// <param name="missing">Why there is no table.</param>
    /// <returns>The result.</returns>
    private NotebookSectionResult Draw(NotebookTable? table,
        string missing)
    {
        Results = [];
        ResultStatus = string.Empty;
        if (table is null)
        {
            return Fail(missing);
        }
        var check = NotebookChartValidator.Check(table,
            FailOnNonNumeric);
        if (!check.IsValid)
        {
            return Fail(check.Error!);
        }
        Results = [table];
        ResultStatus = $"{table.Rows.Count} row(s)";
        LogChart(table,
            check);
        return NotebookSectionResult.Succeeded(ResultStatus);
    }

    /// <summary>
    /// Logs what was charted.
    /// </summary>
    /// <param name="table">The table.</param>
    /// <param name="check">The check.</param>
    private void LogChart(NotebookTable table,
        NotebookChartCheck check)
    {
        Log.Add(NotebookLogLevel.Information,
            $"Charted {table.Rows.Count} rows as {check.ValueColumns.Count} series.");
        if (!check.IsTimeBased)
        {
            Log.Add(NotebookLogLevel.Information,
                $"The first column '{table.Columns[0].Name}' is not a timestamp: each row is a point, labeled by it.");
        }
        if (check.Skipped.Count > 0)
        {
            Log.Add(NotebookLogLevel.Warning,
                $"Skipped the columns that are not numeric: {string.Join(", ",
                    check.Skipped)}.");
        }
    }

    /// <summary>
    /// Builds the chart data from the table.
    /// </summary>
    private void Redraw()
    {
        if (Table is not { } table)
        {
            Data = null;
            return;
        }
        var check = NotebookChartValidator.Check(table,
            FailOnNonNumeric);
        Data = check.IsValid
            ? NotebookChartBuilder.Build(table,
                check,
                Bucket,
                ChartType,
                _colors)
            : null;
    }

    #endregion
}
