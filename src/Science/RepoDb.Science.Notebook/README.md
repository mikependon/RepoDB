# RepoDb.Science.Notebook

The notebook model behind RepoDB notebooks (`.rdbnb`), in the `RepoDb.Science` namespace.
Targets .NET 10 and .NET 11, is trim and NativeAOT annotated, and has no UI or database-provider dependencies.

## Open and save

```csharp
using RepoDb.Science;

var notebook = NotebookDocument.Open("Sales.rdbnb");

var sql = notebook.Sections
    .OfType<NotebookCodeSection>()
    .First(section => section.Language == NotebookCodeLanguage.Sql);

sql.Script = "SELECT * FROM [dbo].[Sales];";
notebook.Save();
```

`NotebookDocument.Create("Sales")` starts a new notebook. `OpenAsync` and `SaveAsync` are also available.
Opening never runs a script. Saving writes a temporary file first and then replaces the original.
Properties of the file that the package does not own are kept in `HostData` and written back.
A connection is stored as its identifier only.

## Sections

| Class | Holds | Runs |
|---|---|---|
| `NotebookHeaderSection` | Title and description | No |
| `NotebookImportsSection` | Using directives | Checks their syntax |
| `NotebookCodeSection` | A SQL or C# script, its output and tables | Yes |
| `NotebookChartSection` | SQL and C# scripts, chart type, bucket, colors and table | Yes |

Each runnable section keeps the log and results of its last run only. `HasChanges` tracks authored content,
not runs or layout. A notebook holds one imports section at most, and SQL section names are unique.

| Member | Use |
|---|---|
| `Sections.Swap(first, second)` | Exchanges two sections. Assigning the sections to each other's index throws |
| `section.SetResultStatus(status)` | Replaces the saved result status, for example when another table is shown |
| `section.ApplyOutputLimit(lines)` | Cuts the saved output to its last lines |
| `notebook.GetRunPlan()` | The sections a run of the notebook runs, in order and with their names |

## Running

The host supplies the executors. Without a C# executor, C# scripts are skipped.

```csharp
var context = new NotebookExecutionContext
{
    SqlExecutor = new NotebookSqlExecutor((connectionId, cancellationToken) =>
        ValueTask.FromResult<DbConnection>(new SqlConnection(connectionString))),
    CSharpExecutor = csharpExecutor,
    Guard = guard,
    RowLimit = 1000
};

await sql.ExecuteAsync(context);

var result = await notebook.ExecuteAsync(context);
```

- `INotebookSqlExecutor` runs SQL, `INotebookCSharpExecutor` runs C#, and `INotebookExecutionGuard` can block
  a script, for example after asking the user to confirm it.
- `notebook.ExecuteAsync` runs the imports first and then the other sections, and stops at the first failure.
- Property change events are raised on the synchronization context that started the run.
- A cancelled run throws `OperationCanceledException`. Any error raised after the token is cancelled counts as the
  cancellation. A cancelled SQL code section keeps the tables and status it had before.

### Writing a SQL executor

An executor returns a `NotebookSqlResult`:

| Property | Meaning |
|---|---|
| `Tables` | The result tables |
| `RowsAffected` | The affected rows, or `null` when the script only read |
| `Status` | The result status of the host, or `null` for the default one |
| `Error` | Why the script failed without throwing, or `null` |

`NotebookSqlRequest.Log` is the log of the section. Lines added there appear while the script runs, so add them
on the context that called the executor. The package adds its own error line only when the log has none.
A C# executor returns its lines in `NotebookScriptResult.Logs` and may set `Status`.

`NotebookSqlExecutor`, the built-in implementation, runs only scripts that look like plain `SELECT` or `WITH`
queries, unless `allowWrites` is `true`. The check, also public as `NotebookSqlReadOnlyCheck.IsReadOnly`, errs on
the side of refusing: for example, it rejects backslashes in quoted text, `#`, `$` and `--` without a following
space. It cannot see side effects of functions, so it is a safeguard and **not a security boundary**. Use a
read-only database user to enforce read access.

It also refuses a query that uses a statement word, such as `Copy`, `Load`, `Move`, `Set` or `Use`, as a bare name.
Quote such a name (`[Copy]`, `"Copy"` or `` `Copy` ``, as the database allows). The words are listed in
[`NotebookSqlReadOnlyCheck`](RepoDb.Science.Notebook/Helpers/NotebookSqlReadOnlyCheck.cs).

## Messages

Every text the package writes in logs, statuses, import problems, chart problems and run step names comes from
`NotebookMessages`. A host overrides what differs and passes it to the notebook:

```csharp
public sealed class MyMessages : NotebookMessages
{
    public override string SqlFailed(NotebookScriptSection section, string error) => $"The query failed: {error}";

    // An empty text removes a log line.
    public override string SqlStarted(NotebookScriptSection section) => string.Empty;
}

var notebook = NotebookDocument.Open("Sales.rdbnb", new MyMessages());
```

`Create` accepts messages too. A section that belongs to no notebook uses the default ones. Two things are not
replaceable: the "earlier lines not shown" note, which is read back to keep its count, and the exceptions for
programming errors and unreadable files.

## Charts

`NotebookChartSection.Data` holds the labels and series to draw. It follows the table, `ChartType`, `Bucket`,
colors and `FailOnNonNumeric` without running the script again. Drawing and saving images belong to the host.
A chart takes a table only from a successful run: a script that fails, even after giving a table, draws nothing.

## Imports

`NotebookImportValidator.Check(text)` checks that a text holds only using directives, comments and blank lines.

## Tests

The tests are a console program. Coverage must stay at 100% of lines and branches:

```powershell
dotnet run --project RepoDb.Science.Notebook.UnitTests -f net10.0
./RepoDb.Science.Notebook.UnitTests/Measure-Coverage.ps1
```
