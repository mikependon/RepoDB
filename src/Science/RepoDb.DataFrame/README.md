# RepoDb.DataFrame

Non-mutating `DataTable` inspection extensions in the `RepoDb.Science` namespace.
Targets .NET 10 and .NET 11. The library has no database-provider or Studio dependencies.

Extension methods are organized by capability under `RepoDb.DataFrame/Extensions/DataTable`.
Each inspection operation has its own file in `Inspection`, contributing to the partial
`DataTableExtensions` class in `RepoDb.Science`. The remaining capability folders are reserved
for future operations and contain `.gitkeep` placeholders.

```csharp
using RepoDb.Science;

var preview = sales.Head(10);
var sample = sales.Sample(10,
    seed: 42);
var dimensions = sales.Shape();
var metadata = sales.Info();
var statistics = sales.Describe();
var types = sales.DTypes();
var estimatedBytes = sales.MemoryUsage();
var countries = sales.Unique("Country");
var countryCount = sales.NUnique("Country");
var frequencies = sales.ValueCounts("Country",
    includeNulls: true);
```

## Inspection API

| Method | Result | Behavior |
| --- | --- | --- |
| `Head(count = 5)` | `DataTable` | First current rows, in source order |
| `Tail(count = 5)` | `DataTable` | Last current rows, in source order |
| `Sample(count = 5, seed = null)` | `DataTable` | Random rows without replacement, in sample order |
| `Shape()` | `(int Rows, int Columns)` | Current row and schema column counts |
| `Info()` | `TableInfo` | Snapshot of types, nullability, and missing/present counts |
| `Describe()` | `IReadOnlyList<ColumnStatistics>` | Statistics for declared numeric columns |
| `DTypes()` | `IReadOnlyDictionary<string, Type>` | Read-only declared type mapping in column order |
| `MemoryUsage()` | `long` | Estimated stored-value payload bytes |
| `Unique(column, includeNulls = true)` | `IReadOnlyList<object?>` | Distinct values in first-seen order |
| `NUnique(column, includeNulls = false)` | `int` | Distinct value count |
| `ValueCounts(column, includeNulls = false)` | `IReadOnlyList<ValueCount>` | Frequencies in first-seen order |

## Behavior

- All operations inspect current values and skip deleted rows. None modifies the source.
- Row selections use `DataTable.Clone` and `ImportRow`, preserving supported schema metadata,
  constraints, expressions, and original/current row versions. Object-valued cells retain their
  existing reference semantics; this is not recursive object cloning. Dataset relations are not
  copied. Schemas with relation-dependent expressions may not support standalone selection.
- Negative row limits throw `ArgumentOutOfRangeException`. Head and tail clamp oversized limits.
  Sampling more rows than are available throws, including the default count on small tables.
  Zero returns an empty table with the source schema. Seeds reproduce samples for the same
  source ordering and runtime random implementation.
- Missing values are `null` or `DBNull.Value`, normalized to `null` in distinct results.
  Distinct operations use CLR equality: strings are case sensitive, different boxed numeric
  types remain distinct, and arrays use reference equality. Mutable returned values are not cloned.
- `Describe` supports signed/unsigned integer primitives, `float`, `double`, and `decimal`.
  Boolean, enum, object, date/time, and string columns are excluded. Missing values are skipped.
  NaN and infinity throw `ArgumentException`; other inspections treat them as present values.
  Statistics use double precision, which can lose decimal and large-integer precision and can
  overflow for extreme magnitudes. Standard deviation uses the sample denominator (`count - 1`).
  Quartiles use linear interpolation at `(count - 1) * fraction`.
  Empty/all-null columns have count zero and null statistics; singleton deviation is null.
- Memory estimates count UTF-16 string characters, byte-array contents, and fixed-size scalar
  payloads. Nulls contribute zero; unsupported objects contribute one pointer-sized estimate.
  CLR/table overhead, indexes, original row versions, null bitmaps, and shared-object accounting
  are excluded. This is not a measurement of heap allocations or process memory.
- Null tables and invalid column names throw argument exceptions. Source mutation during an
  inspection is unsupported.
- Inspection has no `inPlace` option: summaries return results and previews leave the source intact.
  Mutation options are reserved for future transformation capabilities.

## Validation

Tests are explicitly invoked methods in an executable; no test-discovery or data-driven
attributes are used. An assertion failure exits with an error and stack trace. Run both TFMs:

```powershell
dotnet run --project src/Science/RepoDb.DataFrame/RepoDb.DataFrame.UnitTests -f net10.0
dotnet run --project src/Science/RepoDb.DataFrame/RepoDb.DataFrame.UnitTests -f net11.0
```

The suite includes negative cases for invalid columns, extreme row limits, insufficient
sample rows, NaN and infinities, and attempted mutation of read-only results. Boundary tests
cover deleted-only tables, snapshots, numeric type classification, and raw C# null handling.
The test assembly has internal visibility to verify helper contracts that normal DataTable
storage cannot exercise because it normalizes nulls to `DBNull.Value`.

To collect library-only coverage for both target frameworks:

```powershell
./src/Science/RepoDb.DataFrame/RepoDb.DataFrame.UnitTests/Measure-Coverage.ps1
```

The script installs `dotnet-coverage` 18.12.0 into `artifacts/Science/tools` if needed, runs
the tests, and writes Cobertura reports under `artifacts/Science/Coverage`. It fails if
either framework has less than 100% line or branch coverage or an empty report. Use
`-NoRestore` when project dependencies have already been restored. Coverage includes
all library code and excludes the test runner and framework dependencies.

Both projects enable NativeAOT compatibility analysis. To compile and run all tests natively,
install the platform NativeAOT prerequisites (including the C++ toolchain on Windows), then:

```powershell
dotnet publish src/Science/RepoDb.DataFrame/RepoDb.DataFrame.UnitTests -f net11.0 -r win-x64 -c Release -p:PublishAot=true -o artifacts/Science/NativeAot/net11.0
./artifacts/Science/NativeAot/net11.0/RepoDb.DataFrame.UnitTests.exe
```
