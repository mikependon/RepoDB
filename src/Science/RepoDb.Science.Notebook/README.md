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

`NotebookSqlExecutor` runs only scripts that look like plain `SELECT` or `WITH` queries, unless
`allowWrites` is `true`. The check errs on the side of refusing: for example, it rejects backslashes in
quoted text, `#`, `$` and `--` without a following space. It cannot see side effects of functions, so it is a
safeguard and **not a security boundary**. Use a read-only database user to enforce read access.

## Charts

`NotebookChartSection.Data` holds the labels and series to draw. It follows the table, `ChartType`, `Bucket`,
colors and `FailOnNonNumeric` without running the script again. Drawing and saving images belong to the host.

## Tests

The tests are a console program. Coverage must stay at 100% of lines and branches:

```powershell
dotnet run --project RepoDb.Science.Notebook.UnitTests -f net10.0
./RepoDb.Science.Notebook.UnitTests/Measure-Coverage.ps1
```
