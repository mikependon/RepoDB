# RepoDb.Science.Notebook.Scripting

Runs the C# sections of RepoDB notebooks with Roslyn scripting, in the `RepoDb.Science` namespace.
It adds `NotebookCSharpExecutor`, the C# counterpart of the `NotebookSqlExecutor` of `RepoDb.Science.Notebook`.
Targets .NET 10 and .NET 11.

## Running

```csharp
using RepoDb.Science;

var context = new NotebookExecutionContext
{
    SqlExecutor = new NotebookSqlExecutor(connectionFactory),
    CSharpExecutor = new NotebookCSharpExecutor()
};

await notebook.ExecuteAsync(context);
```

**C# scripts have no database access unless the host allows it.** The function is asked before every run:

```csharp
var executor = new NotebookCSharpExecutor(allowDatabaseAccess: () => options.AllowDatabaseAccess);
```

## What a script can use

```csharp
var customers = Notebook.Context.FromSqlResult("Section1")[0];

Notebook.Context.Set("UpdatedCustomers", customers);
Print(customers);
Notebook.Chart.FromDataTable(customers);
```

| Member | Use |
|---|---|
| `Print(value)`, `Notebook.Print(value)` | Writes the type and value, and shows the data tables it holds |
| `Notebook.Context.FromSqlResult(name)` | The result sets of a SQL code section as `DataTable`s. The name ignores case |
| `Notebook.Context.Set(name, value)` | Stores a value for later scripts |
| `Notebook.Context.Get(name)`, `Get<T>(name)` | Reads a stored value. The name is case-sensitive |
| `Notebook.Context.Has(name)`, `Remove(name)`, `Names` | Asks for, removes and lists the stored values |
| `Notebook.ToDataTable(...)` | A table from a data reader, or from a list of `ExpandoObject`s, dictionaries or objects |
| `Notebook.Chart.FromDataTable(table)` | Gives a table to a chart section |
| `Notebook.Chart.FromSqlResult(name)` | Gives the first result set of a SQL section to a chart section |

Data tables that are printed or returned, alone, in a `DataSet` or in a collection, become result tables, each one
once. A table without a name is titled `Table`. At most 100 items of a collection are looked at.

`System`, `System.Collections.Generic`, `System.Data`, `System.IO`, `System.Linq` and `System.Threading.Tasks` are
imported already. The imports section of the notebook adds to them.

## Messages

The texts of the problems come from `NotebookScriptingMessages`, whose defaults name no product. A host overrides
what differs. The texts of the import checks come from `NotebookMessages`.

```csharp
public sealed class MyScriptingMessages : NotebookScriptingMessages
{
    public override string DatabaseAccessNotAllowed() => "Turn on database access in the settings.";
}

var executor = new NotebookCSharpExecutor(
    messages: new MyScriptingMessages(),
    notebookMessages: new MyNotebookMessages());
```

| Texts | Methods |
|---|---|
| Database access | `DatabaseAccessNotAllowed`, `ForbiddenUse` and the other `Forbidden...` methods |
| Errors | `CodeError`, `ImportsError`, `InvalidImports`, `RuntimeError` |
| Script members | `SqlResultMissing`, `ValueMissing`, `ValueTypeMismatch`, `DataReaderClosed` |

How printed and returned values are written is not replaceable.

## Assemblies and editors

A script can use every assembly the application runs with. `NotebookScriptFactory` narrows that, and gives an editor
the documentation of the assemblies:

```csharp
var scripts = new NotebookScriptFactory(
    documentation: path => documentationOf(path),
    isReferenced: path => !path.EndsWith("RepoDb.Science.Notebook.dll", StringComparison.OrdinalIgnoreCase));

var executor = new NotebookCSharpExecutor(scripts: scripts);

// An editor compiles exactly what runs.
var compilation = scripts.Create(imports, code).GetCompilation();
```

The source of a script is `NotebookScriptFactory.PrefixOf(imports)` followed by the code, and
`ImportLinesOf(imports)` is the number of lines before the code.

## Data

- A SQL section must have run before a script reads it. Each script run gets fresh `DataTable`s, named after the
  title of the result, so changing one does not change the section. Pass a changed table on with `Set`.
- Values stored with `Set` live in `executor.Context`. They are not saved with the notebook, and clearing the
  output of the notebook keeps them. Use one executor for each notebook.
- Only the host removes everything, with `executor.Reset()`. Scripts cannot.
- A table has at most `RowLimit` rows.
- A value that cannot be read as the type of its column is a database null. A column with a taken name ends with
  underscores, for example `Id`, `Id_` and `Id__`, and one without a name is named after its position, such as
  `Column2`.
- The rows of a SQL result are in the `Added` state.
- A value of a result table whose text cannot be written shows as `<ToString failed: ...>`.

## Limits

- **Security:** the database check refuses database types, reflection, native code, other processes, `dynamic`, `#r`
  and `#load`. It is **not a sandbox**: a script can still, for example, delete files. Use `INotebookExecutionGuard`
  to confirm a script before it runs, and a database user with limited rights.
- **Cancellation:** the token stops the wait and the compilation. A script that loops runs to its end.
- **One at a time:** the console of the process is redirected while a script runs, so scripts run one after the
  other, across every executor, and output written by other threads meanwhile is captured too. A script that never
  ends blocks every later script.
- **Memory:** Roslyn loads a new assembly for each run and never unloads it, so the memory of a long-running
  application grows with the number of runs.
- **Threads:** a script runs on the thread pool, so it must not touch the user interface directly.
- **Native AOT:** the package is annotated, but Roslyn cannot run scripts in a Native AOT application. The executor
  has `RequiresDynamicCode` and `RequiresUnreferencedCode`, and reports itself as unavailable when the runtime has no
  dynamic code, so C# sections are skipped.
- **Single-file applications:** the references come from the assembly files of the application. Assemblies bundled
  into a single file cannot be referenced.

## Tests

The tests are a console program. Coverage must stay at 100% of lines and branches:

```powershell
dotnet run --project RepoDb.Science.Notebook.Scripting.UnitTests -f net10.0
./RepoDb.Science.Notebook.Scripting.UnitTests/Measure-Coverage.ps1
```
