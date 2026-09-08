# 🦭 RepoDb.Benchmarks.MariaDbConnector

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, and Linq2Db on MariaDB, using the [MySqlConnector](https://mysqlconnector.net/) client library. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

This is a sibling of [RepoDb.Benchmarks.MariaDb](../RepoDb.Benchmarks.MariaDb) — same schema, same benchmarks, same MariaDB server — the only thing that changes is the ADO.NET driver each ORM sits on top of (`RepoDb.Connector.MariaDb`/`MySql.Data` there, `RepoDb.Connector.MariaDbConnector`/`MySqlConnector` here).

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on MariaDB when built on MySqlConnector, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for MariaDB workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own MariaDB configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## 🐝 NHibernate is not included

Like [RepoDb.Benchmarks.MariaDb](../RepoDb.Benchmarks.MariaDb), this project does not have an NHibernate benchmark. NHibernate ships no MariaDB-specific dialect or driver — only MySQL ones — and there's no first-party (or well-established community) MariaDB dialect to point it at. Rather than approximate support by pointing NHibernate's MySQL dialect at a MariaDB server, it's left out until genuine support exists.

## ▶️ Running the Benchmark

1. Start a MariaDB instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d mariadb
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.MariaDbConnector
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=127.0.0.1;Port=3307;Database=RepoDb;User Id=root;Password=RepoDB2026;
```

and uses the following admin connection (without a target database) to create the `RepoDb` database if it doesn't already exist:

```
Server=127.0.0.1;Port=3307;User Id=root;Password=RepoDB2026;
```

> 📌 Port `3307`, not `3306` — the repository's `docker-compose.yml` maps the `mariadb` service to host port `3307` so it doesn't collide with the `mysql` service.

To target a different instance or credentials, set these environment variables before running:

```bash
export REPODB_CONSTR="Server=<your-host>;Port=3307;Database=RepoDb;User Id=root;Password=<your-password>;"
export REPODB_CONSTR_MARIADB="Server=<your-host>;Port=3307;User Id=root;Password=<your-password>;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

RepoDB, Dapper, and the schema/seed logic in `DatabaseHelper` all go through [RepoDb.MariaDbConnector](../../../Providers/RepoDb.MariaDbConnector) and [RepoDb.MariaDbConnector.BulkOperations](../../../Providers/RepoDb.MariaDbConnector.BulkOperations), built on top of [RepoDb.Connector.MariaDbConnector](https://www.nuget.org/packages/RepoDb.Connector.MariaDbConnector) (which itself wraps [MySqlConnector](https://www.nuget.org/packages/MySqlConnector)), exposing `MariaDbConnection`.

Linq2Db targets MariaDB explicitly via `MySqlVersion.MariaDB10` with `MySqlProvider.MySqlConnector`.

Entity Framework Core uses [Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql) — the only EF Core provider that officially supports MariaDB (Oracle's `MySql.EntityFrameworkCore` does not). Unlike the sibling `RepoDb.Benchmarks.MariaDb` project, Pomelo's underlying driver (MySqlConnector) matches every other ORM here, so this project talks to the server through a single client library end to end. Pomelo has not yet released EF Core 10 support, so this project's `Microsoft.EntityFrameworkCore`/EF Core packages are pinned to **9.x**, one major version behind the 10.0.11 used by the other benchmark projects. Once Pomelo ships EF Core 10 support, this pin should be lifted to stay in line with the rest of the suite.
