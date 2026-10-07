# 🪶 RepoDb.Benchmarks.Sqlite.Turso

Benchmarks for RepoDB (and Dapper, as a reference micro-ORM) on [Turso](https://www.nuget.org/packages/Turso.Data.Sqlite.Provider), the SQLite-compatible engine rewritten in Rust by Turso, exposed through a `Microsoft.Data.Sqlite`-style ADO.NET facade (`Turso.Data.Sqlite`). It mirrors [RepoDb.Benchmarks.Sqlite.Microsoft](../RepoDb.Benchmarks.Sqlite.Microsoft) and [RepoDb.Benchmarks.Sqlite.Ahtola](../RepoDb.Benchmarks.Sqlite.Ahtola) one-to-one (same schema, dataset, operations and configuration) so the result sets can be compared directly. See the [main Benchmarks README](../README.md) for the full methodology.

## ❓ Why this Benchmark?

To quantify the cost/benefit of swapping the native SQLite engine (via `Microsoft.Data.Sqlite`) for the Turso engine when using [RepoDb.Sqlite.Turso](../../Providers/RepoDb.Sqlite.Turso).

Entity Framework Core and Linq2Db are not included: neither ships a provider for the Turso client library.

## ▶️ Running the Benchmark

1. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Benchmarks/RepoDb.Benchmarks.Sqlite.Turso
   dotnet run -c Release
   ```

2. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu (or pass `--filter *RepoDb*`).

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark uses a `RepoDb.Sqlite.Turso.db` file created next to the compiled binaries (connection string `Data Source=<path>;`). Any leftover file is deleted at startup. To target a different database, set:

```bash
export REPODB_CONSTR="Data Source=/path/to/your.db;"
```

## 📦 Notes

- **No bulk operations** — like SQLite, there is no bulk-copy protocol, so only plain `InsertAll`/`UpdateAll` are measured.
- **`Id INTEGER PRIMARY KEY`** is used as the identity column (ROWID alias), same as the SQLite benchmark.
- `DATETIME` values are stored as `TEXT` and read back through the same `SqliteDateTimePropertyHandler` as the SQLite benchmark.
