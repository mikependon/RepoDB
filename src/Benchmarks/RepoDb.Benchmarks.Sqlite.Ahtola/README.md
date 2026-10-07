# 🪶 RepoDb.Benchmarks.Ahtola

Benchmarks for RepoDB (and Dapper, as a reference micro-ORM) on [Ahtola](https://www.nuget.org/packages/Devolutions.Ahtola.Data.Sqlite), a pure-managed, SQLite-compatible engine exposed through a `Microsoft.Data.Sqlite`-style ADO.NET facade (`Ahtola.Data.Sqlite`). It mirrors [RepoDb.Benchmarks.Sqlite.Microsoft](../RepoDb.Benchmarks.Sqlite.Microsoft) one-to-one (same schema, dataset, operations and configuration) so the two result sets can be compared directly. See the [main Benchmarks README](../README.md) for the full methodology.

## ❓ Why this Benchmark?

To quantify the cost/benefit of swapping the native SQLite engine (via `Microsoft.Data.Sqlite`) for the fully-managed Ahtola engine when using [RepoDb.Ahtola](../../Providers/RepoDb.Ahtola).

Entity Framework Core and Linq2Db are not included: neither ships a provider for the Ahtola client library.

## ▶️ Running the Benchmark

1. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Benchmarks/RepoDb.Benchmarks.Ahtola
   dotnet run -c Release
   ```

2. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu (or pass `--filter *RepoDb*`).

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark uses a `RepoDb.Ahtola.db` file created next to the compiled binaries (connection string `Data Source=<path>;Local Provider=Managed;`). Any leftover file is deleted at startup. To target a different database, set:

```bash
export REPODB_CONSTR="Data Source=/path/to/your.db;Local Provider=Managed;"
```

## 📦 Notes

- **No bulk operations** — like SQLite, there is no bulk-copy protocol, so only plain `InsertAll`/`UpdateAll` are measured.
- **`Id INTEGER PRIMARY KEY`** is used as the identity column (ROWID alias), same as the SQLite benchmark.
- `DATETIME` values are stored as `TEXT` and read back through the same `SqliteDateTimePropertyHandler` as the SQLite benchmark.
