# 🪶 RepoDb.Benchmarks.Sqlite.Microsoft

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, and Linq2Db on SQLite (via `Microsoft.Data.Sqlite`). See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on SQLite, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for SQLite workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## 🐝 NHibernate is not included

NHibernate does ship a SQLite dialect and driver (`NHibernate.Dialect.SQLiteDialect` / `NHibernate.Driver.SQLite20Driver`), but that driver is hardcoded to the `System.Data.SQLite` ADO.NET provider (`System.Data.SQLite.SQLiteConnection`/`SQLiteCommand`) — a different client library from [`Microsoft.Data.Sqlite`](https://www.nuget.org/packages/Microsoft.Data.Sqlite), the one [RepoDb.Sqlite.Microsoft](../../../Providers/RepoDb.Sqlite.Microsoft) (and this benchmark) is built on. The two aren't interchangeable, so NHibernate ships nothing that actually works against this benchmark's target client, the same outcome as [RepoDb.Benchmarks.SapHana](../RepoDb.Benchmarks.SapHana) excluding it for a driver that's bound to the wrong assembly. (Third-party community packages exist that bridge NHibernate to `Microsoft.Data.Sqlite`, but consistent with how this suite treats every other exclusion, only what an ORM ships itself counts.)

## ▶️ Running the Benchmark

Unlike every other benchmark in this suite, SQLite needs no server and no [docker-compose.yml](../../../../docker-compose.yml) entry — it's an embedded, file-based database, so the "connection" is just a local file next to the compiled benchmark.

1. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.Sqlite.Microsoft
   dotnet run -c Release
   ```

2. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects to a `RepoDb.db` file created next to the compiled binaries (`bin/Release/net10.0/RepoDb.db`). Any file left over from a previous run is deleted at startup so every run starts from a clean slate — there's no server-side `DROP`/`RECREATE` to reach for, the database is just a file.

To target a different file (or an in-memory database) instead, set this environment variable before running:

```bash
export REPODB_CONSTR="Data Source=/path/to/your.db;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite) throughout — including [RepoDb.Sqlite.Microsoft](../../../Providers/RepoDb.Sqlite.Microsoft) and Linq2Db (via `options.UseSQLite(connectionString, SQLiteProvider.Microsoft)`, distinguishing it from linq2db's other `SQLiteProvider.System` option, which wraps `System.Data.SQLite` instead). Entity Framework Core uses [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite), Microsoft's own official EF Core provider, also built directly on `Microsoft.Data.Sqlite`.

### A couple of things specific to SQLite

- **No bulk operations.** There is no `RepoDb.Sqlite.Microsoft.BulkOperations` package — SQLite has no server-side bulk-copy protocol to wrap — so [RepoDb/InsertAllRepoDbBenchmarks.cs](RepoDb/InsertAllRepoDbBenchmarks.cs) only exercises the plain `InsertAll`, and there's no `BulkUpdateAll` in [RepoDb/UpdateAllRepoDbBenchmarks.cs](RepoDb/UpdateAllRepoDbBenchmarks.cs) either.
- **`Id INTEGER PRIMARY KEY` is the identity column**, not an `AUTOINCREMENT` one. In SQLite, a plain `INTEGER PRIMARY KEY` column is already an alias for the row's `ROWID` and auto-increments on its own — the `AUTOINCREMENT` keyword only changes ID-reuse behavior after deletes, at an extra cost (see [sqlite.org/autoinc.html](https://sqlite.org/autoinc.html)), so it's deliberately left off here, the same choice [RepoDb.Sqlite.Microsoft.IntegrationTests](../../../Providers/RepoDb.Sqlite.Microsoft/RepoDb.Sqlite.Microsoft.IntegrationTests) makes.
- **No `REPEAT()`/`RPAD()`.** SQLite's built-in function set doesn't include either, so the 128-character `Name` padding used to seed data is built in C# (`new string('x', 128)`) and bound as an ordinary parameter instead of generated in SQL.
