# 🔻 RepoDb.Benchmarks.Vertica

Benchmarks comparing RepoDB against Dapper on Vertica. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET data-access libraries on Vertica, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every library in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for Vertica workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own Vertica configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## 🐝 Only RepoDb and Dapper are included

Unlike every other benchmark in this suite, this one has no Entity Framework Core, Linq2Db, or NHibernate benchmarks: none of the three ship a Vertica provider, official or community. Dapper is included because it's provider-agnostic (it works over any `IDbConnection`), so it needs no dedicated Vertica package to participate.

## ▶️ Running the Benchmark

1. Start a Vertica instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d vertica
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.Vertica
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Host=127.0.0.1;Port=5433;Database=RepoDb;User=dbadmin;Password=RepoDB2026;
```

Unlike SQL Server/PostgreSQL/MySQL, this benchmark does not create a database — the `vertica` Docker image provisions the `RepoDb` database itself at container startup (via its `VERTICA_DB_NAME` environment variable), so the benchmark only creates the `"Person"` table inside it.

To target a different instance or credentials, set this environment variable before running:

```bash
export REPODB_CONSTR="Host=<your-host>;Port=5433;Database=<your-database>;User=<your-user>;Password=<your-password>;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [Vertica.Data](https://www.nuget.org/packages/Vertica.Data) throughout — [RepoDb.Vertica](../../../Providers/RepoDb.Vertica), [RepoDb.Vertica.BulkOperations](../../../Providers/RepoDb.Vertica.BulkOperations), and Dapper all connect through it directly (`VerticaConnection`).

### A couple of things specific to Vertica

- **The `"Person"` table is dropped and recreated with two separate statements** (`DROP TABLE IF EXISTS ... CASCADE` then `CREATE TABLE ...`) rather than one — Vertica has no `RECREATE TABLE` or `CREATE TABLE IF NOT EXISTS`, and `RepoDb.Vertica`'s own `IsMultiStatementExecutable` setting is `false`, so a single command can only ever hold one statement.
- **`"Id"` uses Vertica's native `IDENTITY(1, 1)` column type** rather than a `GENERATED ... AS IDENTITY` clause, and cleanup uses `DELETE FROM "Person"` rather than `TRUNCATE TABLE`, since Vertica has no `TRUNCATE TABLE` statement (confirmed directly in `RepoDb.Vertica`'s own `CreateTruncate` implementation).
