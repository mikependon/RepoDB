# RepoDb.Science.DataFrame

`DataTable` inspection, transformation, statistics, windows, reshaping, and ML preparation in `RepoDb.Science`.
Targets .NET 10 and .NET 11. The library has no database-provider or Studio dependencies.

Extension methods are organized by capability under `RepoDb.Science.DataFrame/Extensions/DataTable`.
Each operation has its own file in its capability folder, contributing to the partial
`DataTableExtensions` class in `RepoDb.Science`.

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
| Missing data | `DropNulls`, `FillNulls`, `ForwardFill`, `BackwardFill` |
| Transformation | `DropDuplicates`, `Replace`, `CastColumn<T>`, `Assign<T>`, `Apply`, `MapColumn<TSource, TResult>` |
| Aggregation | `Sum`, `Mean`, `Min`, `Max`, `Aggregate` |
| Grouping | `GroupBy` returning `DataTableGroupBy` |
| Combining | `Join`, explicit-key `Merge`, `Concat` |
| ML preparation | `TrainTestSplit`, `ToFeatureMatrix` |

## Additional analytical operations

| Capability | Operations |
| --- | --- |
| Missing data | `IsNull`, `NotNull`, `Interpolate` |
| Selection | `SelectDTypes`, `TopN`, `BottomN` |
| Numeric transformation | `Clip`, `Round`, `Abs`, `Explode` |
| Statistics | `Count`, `Median`, `Mode`, `Std`, `Variance`, `Quantile` |
| Cumulative calculations | `CumSum`, `CumProduct`, `CumMin`, `CumMax` |
| Row comparisons | `Shift`, `Diff`, `PctChange` |
| Windows | `Rolling`, `Expanding`, `Ewm` |
| Reshaping | `Pivot`, `PivotTable`, `Melt`, `Transpose` |
| Relationships | `Corr`, `CorrMatrix`, `Cov` |
| Grouping | `DataTableGroupBy.Transform`, `DataTableGroupBy.Having` |
| Data quality | `Duplicated`, `IsIn`, `Between` |

```csharp
var interpolated = readings
    .Interpolate("Temperature");
var movingAverage = readings
    .Rolling("Temperature",
        window: 3,
        minPeriods: 1)
    .Mean("MovingAverage");
var expandingDeviation = readings
    .Expanding("Temperature")
    .Std(outputColumn: "Deviation");
var weighted = readings
    .Ewm("Temperature",
        alpha: 0.3)
    .Mean("WeightedTemperature");

var wide = sales
    .PivotTable("Country",
        "Year",
        "Revenue",
        AggregateFunction.Sum);
var longForm = wide
    .Melt(new[] { "Country" },
        variableName: "Year",
        valueName: "Revenue");

var countryTotals = sales
    .GroupBy("Country")
    .Transform("Revenue",
        AggregateFunction.Sum,
        "CountryRevenue");
var largeCountries = sales
    .GroupBy("Country")
    .Having(row => row.Field<decimal>("Total") > 1000m,
        new AggregateDefinition("Revenue",
            AggregateFunction.Sum,
            "Total"));
```

### Masks and selection

`IsNull(column)` and `NotNull(column)` return `bool[]` masks for current rows in source order.
`Duplicated` marks subsequent occurrences after the first; its empty key list means all columns.
`IsIn` uses CLR value equality and normalizes nulls, so boxed `1` and `1L` remain distinct while
`null` and `DBNull.Value` match. `Between` excludes missing values and compares compatible
CLR types with optional inclusion of both endpoints. Its string comparisons follow the CLR
current-culture comparer. Masks are independent arrays, not table columns.

`SelectDTypes` matches exact declared CLR types and retains source column order. Repeated
type requests do not duplicate columns; an empty type list selects no columns. `TopN` and
`BottomN` exclude missing values and stably sort comparable values before taking the requested
nonnegative count. Counts beyond available rows are clamped. These operations return copies.

### Numeric transformations and statistics

`Clip`, `Round`, and `Abs` preserve the numeric CLR type and support `inPlace: true`.
Integers and decimals use decimal arithmetic; floats and doubles use floating arithmetic.
`Clip` accepts optional decimal bounds and rejects fractional bounds for integral columns;
cast to a fractional type first when necessary. `Round` supports 0 through 15 decimal places
and explicit `MidpointRounding`. Checked conversions can overflow, including the absolute
value of the minimum signed integer. Missing values remain missing and non-finite inputs
are rejected.

`Count` counts non-missing values, including nonnumeric columns. `Median` and `Quantile` use
double precision and linear interpolation; probabilities must be finite and between 0 and 1.
`Variance` and `Std` use `degreesOfFreedom: 1` for sample statistics, or 0 for population
statistics. They return null when there are too few observations. `Mode` returns a read-only
list of all tied most-frequent CLR values in first-seen order, optionally including nulls.
Empty input has no modes. Numeric statistics skip missing values and require finite inputs.

`Interpolate` fills only gaps bounded by present numeric endpoints, with equal spacing by row
position. It leaves leading and trailing gaps missing. It does not infer a date index or perform
extrapolation. Interpolation produces a double-valued column. All floating calculations can lose
decimal or large-integer precision; computed non-finite results throw `ArithmeticException`.

### Cumulative and row-order calculations

`CumSum`, `CumProduct`, `CumMin`, and `CumMax` preserve missing positions. With
`skipNulls: true` they resume from the previous running value after a gap; false leaves all
subsequent results missing. `Shift` accepts positive lag, negative lead, and zero offsets and
preserves the stored CLR type. Out-of-range positions become missing, including extreme offsets.

`Diff` subtracts the offset value from the current value. `PctChange` returns fractional changes
(`0.1` means 10%) and returns missing for zero denominators rather than infinity. Neither method
implicitly fills gaps. Interpolation, cumulative functions, `Diff`, and `PctChange` produce double
columns. Their optional in-place mode requires an existing double column because changing the
type of a populated source column is not supported. Sorting first determines chronological order;
deleted rows do not occupy positions in these calculations.

### Window definitions

`Rolling(column, window, minPeriods)` uses a positive trailing row width. Its default minimum
present observation count equals the width. `Expanding(column, minPeriods: 1)` starts at the
first current row. Both return `DataTableWindow`, with `Sum`, `Mean`, `Min`, `Max`, `Count`,
`Std`, `Variance`, and basic `Aggregate` operations. A zero minimum is supported: empty-observation
sum and count are zero, while means, extrema, and sample statistics are missing.

`Ewm(column, alpha, minPeriods: 1)` supports finite `0 < alpha <= 1` and positive minimum
observations. It returns `DataTableEwm`, currently supporting `Mean`. The first present value
seeds the recursion; subsequent present values use `(1 - alpha) * previous + alpha * current`.
Missing observations are ignored and carry the previous mean once the minimum has been met.
This corresponds to recursive, unadjusted weighting with gaps ignored, rather than every Pandas
EWM option.

Window builders read current source values at execution time. They operate on row counts,
not calendar durations, and have no centered-window or implicit index behavior. Results use
double columns; pass an output name to retain the original input column. `inPlace: true` can
add that output column or update an existing compatible double column. Trailing summaries
currently evaluate each window separately; exponential means use a single pass.

### Reshaping and grouped results

`Pivot(index, columns, values)` requires unique row-key/column-key pairs. `PivotTable` handles
duplicates using an existing `AggregateFunction` (mean by default). Both retain an explicit
index column and first-seen key order. Missing row keys form groups; missing column keys use
the name `(null)`. Column keys become invariant string names. Blank names, collisions with the
index, and distinct keys producing identical names are rejected. Unobserved cells are missing.

`Melt(idColumns, valueColumns)` retains explicit identifiers and emits `Variable`/`Value` columns
by default. A null value-column list means all non-identifiers; explicit empty lists are allowed.
Identifiers and value selections must be disjoint. Variable names are strings, cells are objects,
and rows follow value-column then source-row order. Output names must not collide.

`Explode` expands non-string `IEnumerable` cells by one level. Strings and other scalar cells
remain single rows. Empty collections produce one missing row, and null elements remain missing.
The expanded column becomes `object`; other columns repeat their current values. `Transpose`
uses an explicit `Column` label followed by `Row0`, `Row1`, and so on, with object-valued cells.
Both transposition labels can be configured. All reshaping operations return new tables.

`GroupBy(...).Transform(column, function, outputColumn)` broadcasts a basic group summary to each
original row, retaining row order and missing-key groups. It supports optional in-place output
under the existing type rules. `Having(predicate, aggregates)` evaluates named aggregate columns
and returns matching summary rows. It filters summaries, not the original detail rows.

### Correlation and covariance

`Corr(left, right)` computes Pearson correlation; `Cov(left, right, degreesOfFreedom: 1)` computes
sample covariance. Missing observations are excluded pairwise. Correlation requires two complete
pairs and returns null for constant series. Covariance requires more complete pairs than its
nonnegative denominator adjustment. Present values must be finite even if their counterpart is
missing. These calculations use double precision and reject non-finite computed results.

`CorrMatrix` selects all numeric columns by default or accepts an explicit ordered selection.
It returns a labeled `DataTable` with a reserved `Column` row-label field and nullable double
coefficients. A selected column named `Column` conflicts with that label and is rejected.
Undefined correlations remain missing. No operation in this section mutates its source.

## MVP workflow examples

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

Operations copy by default. `Filter`, `RenameColumns`, `FillNulls`, `ForwardFill`, `BackwardFill`, `Replace`, `Assign`,
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

`ForwardFill(column, inPlace: false)` fills missing values from the preceding present value;
`BackwardFill(column, inPlace: false)` uses the following present value. Both operate on a
single column in current row order, skip deleted rows, and preserve unresolved leading or
trailing gaps respectively. Empty and all-missing columns are supported. Missing means
`null` or `DBNull.Value`; empty strings and NaN remain present values. Sort first when the
intended order is chronological. In-place mode uses the same staging and ADO.NET constraint
behavior as other value transformations; copy mode leaves the source unchanged.

```csharp
var forward = readings
    .ForwardFill("Temperature");
var backward = readings
    .BackwardFill("Temperature");
readings
    .ForwardFill("Temperature",
        inPlace: true);
```

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
dotnet run --project src/Science/RepoDb.Science.DataFrame/RepoDb.Science.DataFrame.UnitTests -f net10.0
dotnet run --project src/Science/RepoDb.Science.DataFrame/RepoDb.Science.DataFrame.UnitTests -f net11.0
```

The suite includes negative cases for invalid columns, extreme row limits, insufficient
sample rows, NaN and infinities, and attempted mutation of read-only results. Boundary tests
cover deleted-only tables, snapshots, numeric type classification, and raw C# null handling.
The test assembly has internal visibility to verify helper contracts that normal DataTable
storage cannot exercise because it normalizes nulls to `DBNull.Value`.

To collect library-only coverage for both target frameworks:

```powershell
./src/Science/RepoDb.Science.DataFrame/RepoDb.Science.DataFrame.UnitTests/Measure-Coverage.ps1
```

The script installs `dotnet-coverage` 18.12.0 into `artifacts/Science/tools` if needed, runs
the tests, and writes Cobertura reports under `artifacts/Science/Coverage`. It fails if
either framework has less than 100% line or branch coverage or an empty report. Use
`-NoRestore` when project dependencies have already been restored. Coverage includes
all library code and excludes the test runner and framework dependencies.

Both projects enable NativeAOT compatibility analysis. To compile and run all tests natively,
install the platform NativeAOT prerequisites (including the C++ toolchain on Windows), then:

```powershell
dotnet publish src/Science/RepoDb.Science.DataFrame/RepoDb.Science.DataFrame.UnitTests -f net11.0 -r win-x64 -c Release -p:PublishAot=true -o artifacts/Science/NativeAot/net11.0
./artifacts/Science/NativeAot/net11.0/RepoDb.Science.DataFrame.UnitTests.exe
```
