# 🔥 RepoDb.Benchmarks.Firebird

Benchmarks comparing RepoDB against Dapper, Entity Framework Core, Linq2Db, and NHibernate on Firebird. See the [main Benchmarks README](../README.md) for the full methodology, ORM list, and Enterprise Notice.

## ❓ Why this Benchmark?

This benchmark exists to give **visibility** into how RepoDB performs against other widely used .NET ORMs on Firebird, and to do so in a way that avoids bias — the same schema, the same dataset, and the same operations are run against every ORM in a single pass, and all of the benchmarking code is open for anyone to read or challenge.

That said, results produced here run on infrastructure local to this repository. If your organization is evaluating RepoDB for Firebird workloads, we strongly encourage you to run this benchmark on your **own environment** — your own hardware, your own Firebird configuration, and your own data shape — before making a collective and conclusive decision. See the [Enterprise Notice](../README.md#-enterprise-notice) in the main Benchmarks README for more on why this matters.

## ▶️ Running the Benchmark

1. Start a Firebird instance using the repository's root [docker-compose.yml](../../../../docker-compose.yml):

   ```bash
   docker compose up -d firebird
   ```

2. Run the benchmark project in `Release` configuration:

   ```bash
   cd src/Shared/RepoDb.Benchmarks/RepoDb.Benchmarks.Firebird
   dotnet run -c Release
   ```

3. Select the benchmark(s) you want to run from the interactive BenchmarkDotNet menu.

> ⚠️ Always run in `Release` configuration — Debug builds produce misleading results.

By default, the benchmark connects using:

```
DataSource=127.0.0.1;Port=3050;Database=/firebird/data/repodb.fdb;User=SYSDBA;Password=RepoDB2026;Charset=UTF8;Pooling=false;
```

Unlike SQL Server/PostgreSQL/MySQL, this benchmark does not create a database — the `firebird` Docker image provisions the `repodb.fdb` database file itself at container startup (via its `FIREBIRD_DATABASE` environment variable), so the benchmark only creates the `"Person"` table inside it. Pooling is disabled because the `RECREATE TABLE` used to set up that table (see below) needs an exclusive metadata lock, which a pooled connection carrying leftover transaction state from a previous run could collide with.

To target a different instance or credentials, set this environment variable before running:

```bash
export REPODB_CONSTR="DataSource=<your-host>;Port=3050;Database=<your-database-path>;User=SYSDBA;Password=<your-password>;Charset=UTF8;Pooling=false;"
```

Results are written to `BenchmarkDotNet.Artifacts` as Markdown, HTML, and console reports.

## 📦 Client Library

This benchmark uses [FirebirdSql.Data.FirebirdClient](https://www.nuget.org/packages/FirebirdSql.Data.FirebirdClient) throughout — including [RepoDb.Firebird](../../../Providers/RepoDb.Firebird), [RepoDb.Firebird.BulkOperations](../../../Providers/RepoDb.Firebird.BulkOperations), and NHibernate (via `FirebirdClientDriver` + `Firebird4Dialect`). Linq2Db targets it via `FirebirdVersion.v4`. Entity Framework Core uses [FirebirdSql.EntityFrameworkCore.Firebird](https://www.nuget.org/packages/FirebirdSql.EntityFrameworkCore.Firebird), the official EF Core provider, which already supports EF Core 10 (its own versioning has since diverged from EF Core's — the compatible release is `13.0.0`, not `10.x`), so no version pin was needed here.

### A couple of things specific to Firebird

- **Identifiers are quoted, exact-case, throughout** — RepoDb, Linq2Db, and the EF Core provider all quote identifiers with double quotes and preserve exact case, so the `"Person"` table and its columns are declared with the same mixed-case names used in the .NET models (unlike Oracle/Db2, no uppercase reconciliation was needed here).
- **The `"Person"` table is created with `RECREATE TABLE`**, Firebird's idempotent create-or-replace statement, rather than `CREATE TABLE IF NOT EXISTS` — it unconditionally drops and recreates the table on every run, guaranteeing a clean slate. Cleanup between benchmark classes uses `DELETE FROM "Person"`, since Firebird has no native `TRUNCATE TABLE` statement.
