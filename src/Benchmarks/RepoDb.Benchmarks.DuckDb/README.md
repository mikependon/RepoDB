# 🦆 RepoDb.Benchmarks.DuckDb

Benchmarks comparing RepoDB against Dapper and Linq2Db on DuckDB. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET data-access libraries on DuckDB, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every library in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for DuckDB workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## 🚫 Entity Framework Core and NHibernate are not included

Neither ships a DuckDB provider — Entity Framework Core has no official DuckDB provider and NHibernate has no DuckDB dialect or driver. Consistent with how this suite treats every other exclusion, only what an ORM ships itself counts, so only RepoDB, Dapper (which is provider-agnostic), and Linq2Db (which ships a DuckDB provider) are benchmarked.

## ▶️ Running the Benchmark

DuckDB needs no server and no [docker-compose.yml](../../../docker-compose.yml) entry — it's an embedded, file-based database, so the "connection" is just a local file next to the compiled benchmark.

1. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Benchmarks/RepoDb.Benchmarks.DuckDb
   dotnet run -c Release
   ```

2. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects to a `RepoDb.duckdb` file created next to the compiled binaries (`bin/Release/net10.0/RepoDb.duckdb`). Any file left over from a previous run is deleted at startup so every run starts from a clean slate.

To target a different file (or an in-memory database) instead, set this environment variable before running:

```bash
export REPODB_CONSTR="Data Source=/path/to/your.duckdb"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [DuckDB.NET.Data](https://www.nuget.org/packages/DuckDB.NET.Data.Full) throughout — including [RepoDb.DuckDb](../../Providers/RepoDb.DuckDb), [RepoDb.DuckDb.BulkOperations](../../Providers/RepoDb.DuckDb.BulkOperations) (built on the DuckDB appender), and Linq2Db (via `options.UseDuckDB(connectionString)`).

### A couple of things specific to DuckDB

- **`Id` is a sequence-backed column.** DuckDB has no `AUTO_INCREMENT`/`IDENTITY` keyword, so `Id` is declared as `BIGINT DEFAULT nextval('Person_Id_Seq') PRIMARY KEY`.
- **Seeding is a single set-based insert.** The data is generated server-side with `INSERT ... SELECT ... FROM range(...)` rather than row-by-row, since an embedded engine has no network round-trip to amortize.
- **Bulk insert uses the DuckDB appender**, which is the engine's native fast path for loading data, so `BulkInsertAll` is expected to sit well ahead of the SQL-statement-based `InsertAll`.
