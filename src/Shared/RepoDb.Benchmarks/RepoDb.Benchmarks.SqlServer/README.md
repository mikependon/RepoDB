# 🟦 RepoDb.Benchmarks.SqlServer

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, Linq2Db, and NHibernate on SQL Server. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on SQL Server, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for SQL Server workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own SQL Server configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## ▶️ Running the Benchmark

1. Start a SQL Server instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d mssql
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.SqlServer
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
Server=tcp:127.0.0.1,1433;Database=RepoDbBulk;User ID=sa;Password=RepoDB2026;TrustServerCertificate=True;
```

To target a different instance, set the `REPODB_CONSTR` environment variable before running:

```bash
export REPODB_CONSTR="Server=tcp:<your-host>,1433;Database=RepoDbBulk;User ID=sa;Password=<your-password>;TrustServerCertificate=True;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.
