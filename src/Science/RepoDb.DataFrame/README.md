# RepoDb.DataFrame

`DataTable` inspection, transformation, aggregation, joining, and ML preparation in `RepoDb.Science`.
Targets .NET 10 and .NET 11. The library has no database-provider or Studio dependencies.

Extension methods are organized by capability under `RepoDb.DataFrame/Extensions/DataTable`.
Each operation has its own file in its capability folder, contributing to the partial
`DataTableExtensions` class in `RepoDb.Science`. `Reshaping` and `Statistics` remain reserved
for future operations.

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

## MVP operations

| Capability | Operations |
| --- | --- |
| Selection | `SelectColumns`, `DropColumns`, `Filter`, `RenameColumns` |
| Sorting | `SortValues` with one or multiple keys and directions |
| Missing data | `DropNulls`, `FillNulls` |
| Transformation | `DropDuplicates`, `Replace`, `CastColumn<T>`, `Assign<T>`, `Apply`, `MapColumn<TSource, TResult>` |
| Aggregation | `Sum`, `Mean`, `Min`, `Max`, `Aggregate` |
| Grouping | `GroupBy` returning `DataTableGroupBy` |
| Combining | `Join`, explicit-key `Merge`, `Concat` |
| ML preparation | `TrainTestSplit`, `ToFeatureMatrix` |

```csharp
var cleaned = sales
    .DropNulls("Revenue")
    .Filter(row => row.Field<decimal>("Revenue") > 0)
    .SelectColumns("Country",
        "Revenue")
    .SortValues("Revenue",
        ascending: false);

var totals = cleaned
    .GroupBy("Country")
    .Aggregate(new AggregateDefinition("Revenue",
        AggregateFunction.Sum,
        "TotalRevenue"));

var combined = customers
    .Join(orders,
        leftOn: "Id",
        rightOn: "CustomerId",
        how: JoinType.Left);

var transformed = sales
    .Apply(new ColumnAssignment("Revenue",
        typeof(decimal),
        row => row.Field<decimal?>("Revenue") ?? 0m),
        new ColumnAssignment("OriginalRevenue",
            typeof(decimal),
            row => row["Revenue"]));

var (train, test) = cleaned
    .TrainTestSplit(testSize: 0.2,
        seed: 42);
var features = train
    .ToFeatureMatrix("Revenue");
```

### Copies and in-place operations

Operations copy by default. `Filter`, `RenameColumns`, `FillNulls`, `Replace`, `Assign`,
and `MapColumn` accept `inPlace: true` directly. `DropColumns`, `DropNulls`,
`DropDuplicates`, and `Apply` have array overloads followed by the `inPlace` flag:

```csharp
var result = sales
    .FillNulls("Revenue",
        0m,
        inPlace: true)
    .DropDuplicates(new[] { "Country", "Revenue" },
        inPlace: true);
// result and sales reference the same DataTable.
```

In-place value changes stage computed values before writing, using a single-column staging
table. `Apply` evaluates all selectors against the original rows, then stages its output;
assignments do not see preceding assignments from the same call. Staging can allocate.
Existing column type changes require a new table: use `CastColumn<T>` or the default copy mode.
Sorting, projection, joins, aggregation, concatenation, and splitting return independent tables.

Callbacks must not mutate their source rows. Original table constraints, expressions,
read-only columns, relations, events, and row versions continue to follow ADO.NET rules in
in-place mode. Callback and conversion failures happen before source writes; failures during
commit due to constraints or event handlers are not transactional and can leave partial changes.
In-place filtering physically removes excluded current rows without accepting other changes.

New analytical tables materialize current values into ordinary columns. They preserve names,
types, captions, extended column properties, locale, and case sensitivity. Unchanged columns
preserve nullability; replacement columns permit missing values. They do not carry constraints,
expression evaluation, auto-increment behavior, relations, read-only flags, or original row
versions. Inspection previews retain their original `Clone`/`ImportRow` behavior described below.
Object-valued cells retain their reference semantics in both kinds of output.

### Selection and transformation rules

Selections retain requested column order and source row order. Duplicate or unknown names
are rejected; name resolution follows `DataColumnCollection`. Empty `SelectColumns` produces
a zero-column table retaining its row count. Empty keys in `DropNulls` and `DropDuplicates`
mean all columns. `DropNulls` drops a row if any selected value is missing; duplicates keep
the first row. Rename mappings allow name swaps and require distinct nonblank final names.

Sorting is stable, supports one direction per key, and places nulls last by default in either
direction. Strings use ordinal ordering; other present values must be comparable.
`Replace` and duplicate keys use CLR equality. `CastColumn<T>` uses invariant .NET conversion,
preserves missing values, and throws for bad formats, overflow, or unsupported conversions.
`MapColumn` skips missing inputs and requires input values compatible with `TSource`.

### Aggregation and joining rules

`Sum` returns `decimal?`, `Mean` returns `double?`, and `Min`/`Max` retain the source CLR type
in an `object?`. Numeric `Sum<T>`, `Min<T>`, and `Max<T>` overloads convert the result using
normal invariant .NET numeric conversions, including their rounding and overflow behavior.
Numeric summaries skip missing values and reject non-finite inputs. Decimal summation can
overflow for values outside decimal range; double-based means can lose precision or overflow
at extreme magnitudes. Empty or all-null numeric input returns null.

`AggregateDefinition` selects the input column, function, and optional output alias. Supported
functions are `Sum`, `Mean`, `Min`, `Max`, and non-null `Count`. Count also supports nonnumeric
columns. Output aliases must be distinct from each other and grouping keys. Global `Aggregate`
returns one row even for empty input; empty grouped input returns an empty table with the
output schema. Groups preserve first-seen order and include missing keys. Grouping captures
key names and reads current source data when `Aggregate` executes.

Joins support inner, left, right, and full modes, including composite-key array overloads.
Key counts and declared CLR types must match. Missing key components never match. Duplicate
keys produce every matching pair. Left rows retain source order with right matches in right
source order; unmatched right rows follow for right/full joins. All left columns precede
right columns. Duplicate right names receive `rightSuffix` (default `_right`); unresolved
collisions throw. Outer results allow missing values. `Concat` requires identical ordered names
and CLR types, broadens nullability as needed, and preserves all duplicates.

`Merge` is an in-memory join only when explicit `leftOn` and `rightOn` arguments are supplied.
`table.Merge(other)` still calls ADO.NET's existing mutating method. Prefer `Join` in new code
to make the intended operation clear; neither join API performs a database upsert.

### Machine learning preparation

`TrainTestSplit` requires at least two current rows and a finite fraction strictly between zero
and one. It samples row positions without replacement, uses the ceiling of the requested test
size, and clamps it to keep both partitions nonempty. Seeds reproduce the split for the same
source order and runtime random implementation. No preprocessing parameters are learned here;
fit future scalers and encoders on training data only.

`ToFeatureMatrix` returns a row-major `double[,]` in requested column order. With no column
arguments it selects all declared numeric columns. Explicit nonnumeric columns, missing
features, and non-finite values throw; impute or drop missing values first. Conversion can lose
decimal or large-integer precision. An empty numeric selection returns a matrix with zero
columns; deleted rows are excluded.

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

- Inspection operations use current values and skip deleted rows. None modifies the source.
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
