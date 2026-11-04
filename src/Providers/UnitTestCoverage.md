# Provider unit test coverage

Measured on Windows x64 with .NET SDK 10.0.402 on 2026-11-04. Production code and existing unit tests are unchanged. RepoDb.Core is excluded from these results.

The provider-only solution contains all 37 buildable packages: 20 providers and 17 bulk-operation extensions. Six KingbaseES, Redshift and Snowflake folders are placeholders without projects or source code.

## Validation

All 111 test assemblies passed on net8.0, net9.0 and net10.0, with no failed or skipped tests. The suite increased from 3767 cases to 7207 cases per framework, adding 3440 cases. The final run passed 21621 test executions across the three frameworks.

```powershell
dotnet test --solution src/Providers/RepoDb.UnitTests.slnx --max-parallel-test-modules 4
```

The additions cover type aliases and casing, null and unsupported inputs, undefined enum values, nullable CLR types, parameter attribute values, SQL field validation, unsigned key handling, property conversions, numeric precision and overflow, vector formats, bulk settings, column mappings and diagnostic exceptions. Sixteen bulk unit test projects were added and registered in the provider solutions.

New test code uses direct calls and statically referenced types, without dynamic code generation or reflection-based discovery helpers. No NativeAOT publish validation was performed; the test runner and existing database drivers retain their own compatibility requirements.

## Line coverage

Each row measures only the named production assembly using Microsoft.Testing.Platform code coverage in Cobertura format on net10.0. Referenced assemblies, including RepoDb.Core, do not contribute to the row. The baseline runs the original tests by excluding the added BoundaryTest and QualifiedNameTest classes. The new bulk projects had no prior unit test suite; their baseline is not presented as a measured percentage.

| Package | Before | After | Cases on net10.0 |
| --- | ---: | ---: | ---: |
| RepoDb.AuroraDb.MySqlConnector.BulkOperations | No unit test project | 2.3% | 26 |
| RepoDb.AuroraDb.MySqlConnector | 87.2% | 90.1% | 380 |
| RepoDb.AuroraDb.PostgreSql.BulkOperations | No unit test project | 2.3% | 26 |
| RepoDb.AuroraDb.PostgreSql | 76.1% | 83.4% | 565 |
| RepoDb.ClickHouse.BulkOperations | No unit test project | 3.3% | 26 |
| RepoDb.ClickHouse | 60.7% | 68.8% | 248 |
| RepoDb.CockroachDb.BulkOperations | No unit test project | 2.3% | 26 |
| RepoDb.CockroachDb | 61.8% | 78.0% | 387 |
| RepoDb.Db2.BulkOperations | No unit test project | 2.0% | 26 |
| RepoDb.Db2 | 74.8% | 79.5% | 449 |
| RepoDb.DuckDb.BulkOperations | No unit test project | 1.9% | 22 |
| RepoDb.DuckDb | 79.9% | 83.7% | 261 |
| RepoDb.EnterpriseDb.BulkOperations | No unit test project | 2.4% | 26 |
| RepoDb.EnterpriseDb | 61.4% | 78.2% | 404 |
| RepoDb.Firebird.BulkOperations | No unit test project | 2.1% | 20 |
| RepoDb.Firebird | 60.3% | 62.5% | 273 |
| RepoDb.MariaDb.BulkOperations | No unit test project | 2.2% | 26 |
| RepoDb.MariaDb | 85.5% | 89.0% | 347 |
| RepoDb.MariaDbConnector.BulkOperations | No unit test project | 2.3% | 26 |
| RepoDb.MariaDbConnector | 85.5% | 89.0% | 347 |
| RepoDb.MySql.BulkOperations | No unit test project | 2.2% | 26 |
| RepoDb.MySql | 85.8% | 88.1% | 348 |
| RepoDb.MySqlConnector.BulkOperations | No unit test project | 2.3% | 26 |
| RepoDb.MySqlConnector | 85.0% | 88.2% | 354 |
| RepoDb.Oracle.BulkOperations | No unit test project | 2.2% | 26 |
| RepoDb.Oracle | 69.2% | 79.7% | 462 |
| RepoDb.PostgreSql.BulkOperations | No unit test project | 1.1% | 31 |
| RepoDb.PostgreSql | 63.6% | 84.3% | 433 |
| RepoDb.SapHana.BulkOperations | Existing 2-test suite | 2.2% | 28 |
| RepoDb.SapHana | 75.4% | 83.9% | 281 |
| RepoDb.Sqlite.Ahtola | 76.0% | 77.4% | 176 |
| RepoDb.Sqlite.Microsoft | 72.7% | 78.4% | 214 |
| RepoDb.Sqlite.Turso | 76.0% | 77.4% | 178 |
| RepoDb.SqlServer.BulkOperations | No unit test project | 1.1% | 22 |
| RepoDb.SqlServer | 75.0% | 79.5% | 372 |
| RepoDb.Vertica.BulkOperations | No unit test project | 2.6% | 26 |
| RepoDb.Vertica | 77.9% | 81.5% | 293 |

## Remaining coverage

These are database-independent unit tests. Schema discovery, native bulk writers, connection and transaction behavior, and execution of generated SQL still require the existing integration suites and suitable databases. Bulk-operation assembly percentages remain low because most of their code executes database operations; the new tests cover their public configuration, mappings and diagnostics. Some providers already covered their mappings well, so additional cases can strengthen behavior checks without increasing line coverage.

The tests intentionally exercise supported legacy APIs where the shipping provider still uses them, so existing obsolete-driver warnings may appear alongside repository analyzer and package-audit warnings.

To collect coverage for a single package:

```powershell
dotnet test --project src/Providers/RepoDb.PostgreSql/RepoDb.PostgreSql.UnitTests/RepoDb.PostgreSql.UnitTests.csproj --framework net10.0 --coverage --coverage-output-format cobertura
```

To reproduce its original-test baseline, append:

```powershell
--filter 'FullyQualifiedName!~BoundaryTest&FullyQualifiedName!~QualifiedNameTest'
```
