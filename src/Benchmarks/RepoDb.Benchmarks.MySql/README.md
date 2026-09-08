# 🐬 RepoDb.Benchmarks.MySql

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, Linq2Db, and NHibernate on MySQL. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on MySQL, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for MySQL workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own MySQL configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## ▶️ Running the Benchmark

1. Start a MySQL instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d mysql
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.MySql
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=127.0.0.1;Port=3306;Database=RepoDb;User Id=root;Password=RepoDB2026;
```

and uses the following admin connection (without a target database) to create the `RepoDb` database if it doesn't already exist:

```
Server=127.0.0.1;Port=3306;User Id=root;Password=RepoDB2026;
```

To target a different instance or credentials, set these environment variables before running:

```bash
export REPODB_CONSTR="Server=<your-host>;Port=3306;Database=RepoDb;User Id=root;Password=<your-password>;"
export REPODB_CONSTR_MYSQLDB="Server=<your-host>;Port=3306;User Id=root;Password=<your-password>;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [MySql.Data](https://www.nuget.org/packages/MySql.Data) (Oracle's official ADO.NET connector) throughout — including [RepoDb.MySql](../../../Providers/RepoDb.MySql), [RepoDb.MySql.BulkOperations](../../../Providers/RepoDb.MySql.BulkOperations), NHibernate (via `MySqlDataDriver`), and Linq2Db (via `MySqlProvider.MySqlData`). Entity Framework Core uses [MySql.EntityFrameworkCore](https://www.nuget.org/packages/MySql.EntityFrameworkCore), Oracle's official EF Core provider, which is also built on `MySql.Data`.
