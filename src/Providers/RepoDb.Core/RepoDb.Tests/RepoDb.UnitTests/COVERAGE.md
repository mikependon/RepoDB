# Core unit-test coverage

Measured on 2026-11-04, using only `RepoDb.UnitTests` and the built-in Microsoft.Testing.Platform coverage extension. Percentages below describe the shipping `RepoDb` assembly, not the test assembly or the referenced SQL Server provider.

| Target | Passing test cases | Line coverage | Branch coverage |
| --- | ---: | ---: | ---: |
| .NET 10 baseline | 3,499 | 33.40% | 37.91% |
| .NET 8 expanded | 7,226 | 94.56% | 81.57% |
| .NET 9 expanded | 7,226 | 94.56% | 81.57% |
| .NET 10 expanded | 7,226 | 94.57% | 81.62% |

The additions provide 3,727 additional executed test cases per framework, in 2,109 new test methods. Each new test uses Setup / Act / Assert, with one to three assertions. The tests use MSTest, existing fake-provider conventions, and additional test-only helpers. New sources introduce no analyzer warnings. Existing test files, project files, coverage exclusions, and production sources were not modified for this work.

## Added behavior checks

- Shared and entity repository overloads: predicates, fields, query groups, dynamic filters, generic keys, aggregate results, queries, writes, and sync/async behavior.
- Transactions supplied by callers, connection reuse, forced connections, per-call disposal, and disposal after command failures.
- Two through seven result sets, typed and scalar dynamic results, multi-statement providers and providers requiring separate commands, cancellation, and original error propagation.
- Raw SQL query/scalar/non-query APIs, cached raw query results, caller transactions, and execution failures.
- Reader extraction, result progression, scalar conversion, extraction caches, connection ownership, disposal, and cancellation.
- Entity and scalar materialization, immutable constructors, nulls, schema-dependent function caching, compiled entity/dictionary parameter setters, parameter metadata, and output assignments.
- Request identity, hint-sensitive keys, equality, builder composition, pagination, empty inputs, quoting, parameters, and aggregate fields.
- GUID, network address, signed/unsigned 128-bit integer, bit/byte-array, float vector, XML, JSON, enum, array/list, and separated-string property handlers.
- Entity readers: typed access, ordering columns, dictionaries, invalid state, unsupported operations, reset, and value-array validation.

## Remaining coverage

**This is not 100% coverage.** Both line and branch gaps remain. The largest remaining line gaps are listed below; these counts combine duplicate line entries from generated state machines by source line.

| Source relative to `RepoDb` | Uncovered executable lines |
| --- | ---: |
| `Extensions/DbConnectionExtension.cs` | 396 |
| `Reflection/Compiler/Compiler.cs` | 334 |
| `Operations/DbConnection/MergeAll.cs` | 307 |
| `Operations/DbConnection/InsertAll.cs` | 161 |
| `Extensions/DbCommandExtension.cs` | 156 |
| `Extensions/ExpressionExtension.cs` | 125 |
| `QueryGroup.cs` | 120 |
| `Operations/DbConnection/Merge.cs` | 111 |
| `Mappers/EntityMapFluentDefinition.cs` | 99 |
| `Mappers/PropertyValueAttributeMapper.cs` | 78 |
| `Mappers/PropertyHandlerMapper.cs` | 75 |
| `BaseRepository.cs` | 71 |
| `Extensions/QueryFields/DirectionalQueryField.cs` | 68 |
| `DbRepository.cs` | 64 |
| `Operations/DbConnection/QueryMultiple.cs` | 48 |

Much of the remainder involves conversion/compiler combinations, specialized mapper behavior, command preparation, upsert/batch variations, and error branches. Those are still test opportunities, rather than all being unreachable code.

The existing library also contains hardware-specific branches: `ByteArrayToFloatArrayPropertyHandler` reverses bytes only on big-endian hardware. These Windows x64 runs cannot execute those branches. No exclusions or runtime-field manipulation were added to inflate coverage.

The `netstandard2.0` library build is a distinct implementation with conditional source paths. The three existing unit-test targets select modern library builds. The results above do not claim coverage of the .NET Standard implementation or NativeAOT execution.

## Production issues found while testing

These were reproduced during development and left unchanged. The final passing suites do not silently redefine their expected behavior to approve these failures.

### Multi-result cache keys are ignored

The cache lookup and insertion helpers in `Operations/DbConnection/QueryMultiple.cs` guard their work with:

```csharp
cache != null && string.IsNullOrEmpty(cacheKey)
```

Consequently a supplied non-empty key never reaches the cache. Tests expecting a preloaded value of 42 instead received the database fake's value of 1. Cache-population checks found no entry for the supplied key. Expected cache-hit and cache-population tests cannot pass without addressing the production behavior.

With the same provider registration as `RepositoryOperationTestBase`, this reproduces the cache-hit problem:

```csharp
using var connection = new CoverageDbConnection();
var cache = new MemoryCache();
cache.Add<IEnumerable<CoverageEntity>>("first",
    new[] { new CoverageEntity { Id = 42 } });
var actual = connection.QueryMultiple<CoverageEntity, CoverageEntity>(
    where1: new QueryGroup(new QueryField("Id", 1)),
    where2: new QueryGroup(new QueryField("Id", 1)),
    cacheKey1: "first", cacheKey2: "second", cache: cache);
// actual.Item1.Single().Id is 1; the expected cached value is 42.
```

### Dynamic multi-result queries reject multi-column rows

The non-generic multi-result path can route through `DataReader.ToEnumerable<object>`. That materializer requires one result column. With the fake's normal `Id` and `Name` columns, it throws `InvalidOperationException` instead of returning dynamic rows. The added passing multi-result tests cover typed rows and the currently supported single-column dynamic results.

With the same provider registration, this reproduces the multi-column failure:

```csharp
using var connection = new CoverageDbConnection();
var actual = connection.QueryMultiple(
    "CoverageEntity", new QueryField("Id", 1),
    "CoverageEntity", new QueryField("Id", 1));
// InvalidOperationException: number of fields must only be 1 for System.Object.
```

## Reproduce the measurements

From the repository root, build and execute only the unit-test project, separately for each target. No database or integration suite is required.

```powershell
$unitProject = 'src/Providers/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests/RepoDb.UnitTests.csproj'
foreach ($framework in 'net8.0', 'net9.0', 'net10.0') {
    dotnet build $unitProject -f $framework --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $framework" }
    dotnet run --project $unitProject -f $framework --no-build -- `
        --coverage --coverage-output-format cobertura `
        --coverage-output coverage.cobertura.xml `
        --results-directory "artifacts/core-unit-coverage/final-$framework" `
        --output Minimal --progress off
    if ($LASTEXITCODE -ne 0) { throw "Unit tests failed: $framework" }
}
```

Restore dependencies first if running from a fresh checkout. Local XML reports are under `artifacts/core-unit-coverage/final-net8`, `final-net9`, and `final-net10`; the command above creates corresponding framework-named report directories. Artifacts are ignored by Git. Inspect the `RepoDb` package within each Cobertura file for core-only rates and per-line/per-branch hits.
