# 🌅 RepoDb.Benchmarks.AuroraDb.MySqlConnector

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, Linq2Db, and NHibernate on Amazon Aurora MySQL, using the [MySqlConnector](https://mysqlconnector.net/) client library. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

This is a sibling of [RepoDb.Benchmarks.MySqlConnector](../RepoDb.Benchmarks.MySqlConnector) — same schema, same benchmarks — pointed at an Aurora MySQL target through [RepoDb.AuroraDb.MySqlConnector](../../Providers/RepoDb.AuroraDb.MySqlConnector).

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on Aurora MySQL when built on MySqlConnector, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for Aurora MySQL workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own Aurora configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## ▶️ Running the Benchmark

1. Start a MySQL-compatible instance using the repository's root [docker-compose.yml](../../../docker-compose.yml):

   ```bash
   docker compose up -d auroradbmysql
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Benchmarks/RepoDb.Benchmarks.AuroraDb.MySqlConnector
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=127.0.0.1;Port=3308;Database=RepoDb;User Id=root;Password=RepoDB2026;
```

and uses the following admin connection (without a target database) to create the `RepoDb` database if it doesn't already exist:

```
Server=127.0.0.1;Port=3308;User Id=root;Password=RepoDB2026;
```

To target a different instance or credentials, set these environment variables before running:

```bash
export REPODB_CONSTR="Server=<your-host>;Port=3308;Database=RepoDb;User Id=root;Password=<your-password>;"
export REPODB_CONSTR_AURORADB_MYSQL="Server=<your-host>;Port=3308;User Id=root;Password=<your-password>;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [MySqlConnector](https://www.nuget.org/packages/MySqlConnector) throughout — including [RepoDb.AuroraDb.MySqlConnector](../../Providers/RepoDb.AuroraDb.MySqlConnector), [RepoDb.AuroraDb.MySqlConnector.BulkOperations](../../Providers/RepoDb.AuroraDb.MySqlConnector.BulkOperations) (via `AuroraDbConnection`), and NHibernate (via the community-maintained [NHibernate.Driver.MySqlConnector](https://www.nuget.org/packages/NHibernate.Driver.MySqlConnector) driver). Linq2Db targets it via `MySqlProvider.MySqlConnector`.

Entity Framework Core uses [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql) — the standard EF Core provider built on top of MySqlConnector. As of this writing, Pomelo has not yet released EF Core 10 support, so this project's `Microsoft.EntityFrameworkCore`/EF Core packages are pinned to **9.x**, one major version behind the 10.0.11 used by the other benchmark projects. Once Pomelo ships EF Core 10 support, this pin should be lifted to stay in line with the rest of the suite.


### A couple of things specific to Aurora MySQL

- **The local container is plain MySQL 8.0.** The `auroradbmysql` service in `docker-compose.yml` (port `3308`) runs a stock `mysql:8.0` image, since Aurora itself is a managed AWS service. Point `REPODB_CONSTR` at a real Aurora cluster to benchmark the real thing.
