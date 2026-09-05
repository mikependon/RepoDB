# 🐳 RepoDb.Benchmarks.Db2

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, Linq2Db, and NHibernate on IBM Db2. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on Db2, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for Db2 workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own Db2 configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## ▶️ Running the Benchmark

1. Start a Db2 instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d db2
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.Db2
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=127.0.0.1:50000;Database=repodb;UID=db2inst1;PWD=RepoDB2026;
```

Unlike SQL Server/PostgreSQL/MySQL, this benchmark does not create a database — the `db2` Docker image provisions the `repodb` database itself at container startup (via its `DBNAME` environment variable), so the benchmark only creates the `PERSON` table inside it.

To target a different instance or credentials, set this environment variable before running:

```bash
export REPODB_CONSTR="Server=<your-host>:50000;Database=<your-database>;UID=<your-user>;PWD=<your-password>;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [Net.IBM.Data.Db2](https://www.nuget.org/packages/Net.IBM.Data.Db2) (IBM's official managed Db2 driver — `Net.IBM.Data.Db2-lnx` on non-Windows) throughout — including [RepoDb.Db2](../../../Providers/RepoDb.Db2), [RepoDb.Db2.BulkOperations](../../../Providers/RepoDb.Db2.BulkOperations), and NHibernate (via `DB2NetDriver`). Entity Framework Core uses [IBM.EntityFrameworkCore](https://www.nuget.org/packages/IBM.EntityFrameworkCore) (`IBM.EntityFrameworkCore-lnx` on non-Windows), IBM's official EF Core provider, which already supports EF Core 10 so no version pin was needed here. Note that `RepoDb.Db2` itself references the driver with `PrivateAssets="all"` on Windows, so it does not flow transitively — this project references it directly.

### A note on identifier casing

Db2 folds unquoted identifiers to uppercase, but RepoDb and Linq2Db quote identifiers (case-sensitively) by default, while IBM's EF Core provider does not quote at all. To keep every ORM pointed at the same physical objects regardless of that difference, the `PERSON` table and its columns are created unquoted and in uppercase (`PERSON`, `ID`, `NAME`, `AGE`, `CREATEDDATEUTC`), and every ORM's mapping spells those names out explicitly in that exact uppercase form — see [Models/Person.cs](Models/Person.cs) and [Linq2db/Models/PersonDatabase.cs](Linq2db/Models/PersonDatabase.cs).
