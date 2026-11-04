# Schema and data unit test coverage

Measured on Windows x64 using .NET SDK 10.0.402 on 2026-11-04. This work adds tests for all 20 RepoDb.Schema packages and RepoDb.Data.Core. Production code and existing test methods are unchanged.

## Validation

The suite increased from 8966 to 11869 cases per target framework, adding 2903 cases. All 63 test assemblies passed on net8.0, net9.0 and net10.0: 35607 executions, no failures and no skips.

```powershell
dotnet test --solution src/RepoDb.Schema.Data.UnitTests.slnx --max-parallel-test-modules 4
```

The same suite passed with trim and AOT analyzers enabled:

```powershell
dotnet test --solution src/RepoDb.Schema.Data.UnitTests.slnx --max-parallel-test-modules 4 -p:EnableAotAnalyzer=true -p:EnableTrimAnalyzer=true
```

No IL trimming/AOT diagnostics were reported against the added source files. Existing diagnostics remain in other repository code and dependencies. A full NativeAOT publish was not performed. The new fixtures use concrete ADO.NET types and statically registered settings rather than runtime-generated mocks.

## Coverage results

Each row reports only the named production assembly using Microsoft.Testing.Platform coverage in Cobertura format on net10.0. Referenced provider/core assemblies and test fixtures do not contribute to the row. Baselines were collected before adding the tests, after aligning the Db2 test driver with the version used by its production dependency.

| Package | Before | After | Cases on net10.0 |
| --- | ---: | ---: | ---: |
| RepoDb.Data.Core | 96.04% | 97.05% | 918 |
| RepoDb.Schema.AuroraDbMySqlConnector | 59.60% | 84.96% | 525 |
| RepoDb.Schema.AuroraDbPostgreSql | 53.06% | 81.82% | 449 |
| RepoDb.Schema.ClickHouse | 46.18% | 77.25% | 472 |
| RepoDb.Schema.CockroachDb | 52.78% | 81.48% | 452 |
| RepoDb.Schema.Core | 98.20% | 99.22% | 1496 |
| RepoDb.Schema.Db2 | 56.94% | 83.93% | 521 |
| RepoDb.Schema.DuckDb | 49.86% | 76.09% | 516 |
| RepoDb.Schema.Firebird | 51.42% | 77.73% | 507 |
| RepoDb.Schema.MariaDb | 60.40% | 85.69% | 525 |
| RepoDb.Schema.MariaDbConnector | 60.40% | 85.69% | 525 |
| RepoDb.Schema.MySql | 59.60% | 84.96% | 525 |
| RepoDb.Schema.MySqlConnector | 59.60% | 84.96% | 525 |
| RepoDb.Schema.Oracle | 55.92% | 83.95% | 516 |
| RepoDb.Schema.PostgreSql | 53.06% | 81.82% | 449 |
| RepoDb.Schema.SapHana | 56.36% | 83.41% | 514 |
| RepoDb.Schema.Sqlite.Ahtola | 32.18% | 63.19% | 495 |
| RepoDb.Schema.Sqlite.Microsoft | 33.18% | 62.71% | 495 |
| RepoDb.Schema.Sqlite.Turso | 30.35% | 59.85% | 493 |
| RepoDb.Schema.SqlServer | 56.14% | 82.94% | 440 |
| RepoDb.Schema.Vertica | 49.41% | 80.07% | 511 |

## Added coverage

- Schema readers: empty and populated catalog results, column record mapping, table identity and existence, optional metadata, synchronous and asynchronous execution, SQLite master-table projections and Vertica current-schema lookup.
- Type resolvers: aliases, case and whitespace normalization, nullable CLR types, blank inputs and unknown native types.
- Schema composition: native MySQL enum/set declarations and escaped column comments.
- Schema core: portable logical types, explicit and inferred encodings, SQL Server code pages and collations, wide numeric precision and negative scale, optional metadata and invalid XML/YAML structures.
- Data core: payload-size estimates, integer boundaries, nonfinite floating-point values, binary streams and portable JSON/export representations.
- Shared fixtures: connection state, command execution, parameter metadata and parameter collection operations.

The Db2 schema test project referenced IBM driver 10.0.0.200 while its production dependency requires 10.0.0.300. Aligning the test reference fixes four pre-existing assembly-load failures on net10.0; production dependency versions are unchanged.

## Reproducing coverage

Collect a single project's coverage using:

```powershell
dotnet test --project src/Data/RepoDb.Data.Core/RepoDb.Data.Core.UnitTests/RepoDb.Data.Core.UnitTests.csproj --framework net10.0 --coverage --coverage-output-format cobertura
```

Append the following filter to run only the original tests:

```powershell
--filter 'FullyQualifiedName!~CoverageTest'
```

The fixtures verify catalog processing without databases. They do not validate server acceptance of SQL, real driver/network behavior or every backend-specific catalog variant. The remaining uncovered paths include those scenarios and additional error and boundary branches; these results do not imply complete behavioral coverage.
