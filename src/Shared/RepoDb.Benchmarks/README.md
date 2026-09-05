# 📊 RepoDB Benchmarks

Independent, reproducible performance benchmarks comparing RepoDB against the most widely used .NET ORMs, across every database provider RepoDB supports.

-----

## 🎯 Intentions

Benchmarks are easy to game and hard to trust. Our goal here is the opposite: give the .NET community — and the enterprise companies evaluating RepoDB for production workloads — a **transparent, unbiased, and reproducible** way to compare data access libraries.

To keep results honest, we hold ourselves to a few principles:

- **🔬 Same rules for everyone** — Every ORM benchmarked runs the same operation, against the same schema, same dataset size, and same hardware/database instance, in the same run.
- **📖 Open methodology** — The benchmark code lives in this repository. Nothing is hidden; anyone can read it, question it, or challenge it.
- **🧪 Real, runnable code** — Every number is backed by a [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) project you can clone and run yourself against your own infrastructure.
- **🙅 No cherry-picking** — We report the standard operations (CRUD, batch, and bulk) rather than hand-picking the scenarios where RepoDB happens to look best.
- **🔓 Community-auditable** — If you believe a benchmark is unfair to RepoDB or to another ORM, open an issue or a PR. Corrections are welcome and expected.

We built RepoDB because we believe developers deserve fast, low-overhead data access without giving up productivity. These benchmarks exist to prove that claim, not just state it.

## 🏢 Enterprise Notice

The numbers published alongside this repository are produced on infrastructure local to the RepoDB project (e.g., local Docker containers, CI runners, contributor machines). They are a useful signal, but they are **not** a substitute for validating performance in your own environment.

Hardware, network topology, database configuration, cloud provider, virtualization/container overhead, and data volume all materially affect ORM performance — and none of these match between our environment and yours.

If your organization is evaluating RepoDB for production use, we strongly recommend that you:

- **🖥️ Run the benchmarks yourself** — Clone this repository and execute the relevant benchmark project(s) against infrastructure that mirrors your production environment.
- **📐 Use your own data shape** — Adjust row counts, schema, and data types to reflect your actual workload rather than relying solely on the defaults used here.
- **🚫 Avoid bias from our environment** — Treat any numbers you see in this README, in issues, or in marketing material as a starting point for your own investigation, not as a guarantee of the performance you'll see in your own environment.

This is by design: the benchmarking code is fully open (see [Benchmarking Process](#️-benchmarking-process) below) specifically so it can be independently reproduced, rather than taken on faith.

## 📁 List of Projects with Benchmark

Each supported database provider has its own dedicated benchmark project. This list grows as RepoDB extends support to more providers.

| Provider | Project | Status |
|---|---|---|
| 🟦 SQL Server | [RepoDb.Benchmarks.SqlServer](RepoDb.Benchmarks.SqlServer) | ✅ Available |
| 🐘 PostgreSQL | [RepoDb.Benchmarks.PostgreSql](RepoDb.Benchmarks.PostgreSql) | ✅ Available |
| 🐬 MySQL | [RepoDb.Benchmarks.MySql](RepoDb.Benchmarks.MySql) | ✅ Available |
| ⚡ MySQL (MySqlConnector) | [RepoDb.Benchmarks.MySqlConnector](RepoDb.Benchmarks.MySqlConnector) | ✅ Available |
| 🔺 Oracle | [RepoDb.Benchmarks.Oracle](RepoDb.Benchmarks.Oracle) | ✅ Available |
| 🦭 MariaDB | [RepoDb.Benchmarks.MariaDb](RepoDb.Benchmarks.MariaDb) | ✅ Available |
| ⚙️ Shared infrastructure | [RepoDb.Benchmarks.Core](RepoDb.Benchmarks.Core) | Common models, base classes, and configurations shared across all providers |

> More providers (SQLite, DB2, ClickHouse, and others already supported by RepoDB) will be added here over time. Contributions that add a new provider's benchmark project are very welcome.

## ⚗️ Benchmarking Process

All benchmarks are built on top of [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet), the de-facto standard for .NET micro-benchmarking, which handles process isolation, warm-up, and statistical rigor for us.

1. **🏗️ Setup** — Each provider spins up (or reuses) a real database instance, creates the schema, and seeds it with data via `DatabaseHelper`.
2. **🥇 Bootstrap** — A single "throwaway" call per ORM is issued before measurement starts, so JIT warm-up and connection pool initialization don't skew the first real result.
3. **📏 Measurement** — BenchmarkDotNet runs each `[Benchmark]` method through its own configured iterations and warm-up count, executing every ORM under identical conditions within the same process run.
4. **🧹 Cleanup** — Data is truncated/reset between benchmark classes so every ORM starts from the same baseline.
5. **📈 Reporting** — Results are emitted as BenchmarkDotNet reports (Markdown, HTML, and console tables) under `BenchmarkDotNet.Artifacts`, including mean, error, standard deviation, and allocated memory.

To run a benchmark project yourself, spin up the target database provider with the repository's root [docker-compose.yml](../../../docker-compose.yml), then run the matching benchmark project:

```bash
# 🟦 SQL Server
docker compose up -d mssql
cd RepoDb.Benchmarks.SqlServer
dotnet run -c Release

# 🐘 PostgreSQL
docker compose up -d postgresql
cd RepoDb.Benchmarks.PostgreSql
dotnet run -c Release

# 🐬 MySQL
docker compose up -d mysql
cd RepoDb.Benchmarks.MySql
dotnet run -c Release

# ⚡ MySQL (MySqlConnector)
docker compose up -d mysql
cd RepoDb.Benchmarks.MySqlConnector
dotnet run -c Release

# 🔺 Oracle
docker compose up -d oracle
cd RepoDb.Benchmarks.Oracle
dotnet run -c Release

# 🦭 MariaDB
docker compose up -d mariadb
cd RepoDb.Benchmarks.MariaDb
dotnet run -c Release
```

> ⚠️ Always run benchmarks in `Release` configuration. Debug builds produce misleading results.

Connection strings default to a local Dockerized instance, but can be overridden via the `REPODB_CONSTR` environment variable (and `REPODB_CONSTR_POSTGRESDB` / `REPODB_CONSTR_MYSQLDB` / `REPODB_CONSTR_MARIADB` for the PostgreSQL/MySQL/MariaDB admin connection) if your container uses different credentials or a different host/port.

## 🥊 ORMs Benchmarked

| ORM | Style | NuGet |
|---|---|---|
| 🧩 **RepoDb** | Hybrid-ORM (fluent + raw SQL) | [RepoDb](https://www.nuget.org/packages/RepoDb) |
| 🥤 **Dapper** | Micro-ORM (raw SQL + mapping) | [Dapper](https://www.nuget.org/packages/Dapper) |
| 🏢 **Entity Framework Core** | Full-featured ORM | [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore) |
| 🔗 **Linq2Db** | LINQ-first ORM | [linq2db](https://www.nuget.org/packages/linq2db) |
| 🐝 **NHibernate** | Full-featured ORM | [NHibernate](https://www.nuget.org/packages/NHibernate) |

Each ORM is exercised through its own idiomatic API (e.g., `DbContext` for EF Core, `Connection.Query` for Dapper, `Connection.QueryAll` for RepoDB) rather than forcing a shared abstraction, so every library is measured doing what it does best.

> 📌 Not every ORM supports every database provider. Only the ORMs that officially support a given provider are included in that provider's benchmark project — so the list above may vary slightly from one provider to another. For example, [RepoDb.Benchmarks.MariaDb](RepoDb.Benchmarks.MariaDb) excludes NHibernate, which ships no MariaDB-specific dialect or driver.

## 🛠️ Operations

Benchmarks are grouped by operation category so you can compare libraries on the workload that matters to you:

- **✏️ CRUD** — Single/first-record reads (`GetFirst`), full-table reads (`GetAll`), and row updates (`UpdateAll`).
- **📦 Batch** — Multi-row operations executed as a set (e.g., `UpdateAll` across N rows) rather than one-row-at-a-time.
- **🚛 Bulk** — Native bulk operations (e.g., `BulkInsertAll`, `BulkUpdateAll`) that leverage provider-specific bulk-copy mechanisms for very large datasets — a category most general-purpose ORMs don't support natively.

Row counts are parameterized (typically `10`, `100`, and `1000`) so you can see how each ORM scales as dataset size grows, not just how it performs at a single fixed size.

## ⚖️ Pros and Cons

No ORM is universally "best" — each makes different trade-offs. Here's how they generally compare based on these benchmarks:

| ORM | Pros | Cons |
|---|---|---|
| 🧩 **RepoDb** | Near-ADO.NET performance, native bulk operation support, low memory allocation, minimal setup | Smaller community than EF Core; less "magic" (by design) |
| 🥤 **Dapper** | Extremely lightweight, very fast for raw SQL | No native bulk operations; more manual SQL to write |
| 🏢 **EF Core** | Rich change tracking, migrations, LINQ provider, huge ecosystem | Higher overhead on reads/writes, larger memory footprint, slower on batch/bulk workloads |
| 🔗 **Linq2Db** | Fast LINQ-to-SQL translation, low overhead | Smaller community, fewer bulk-operation conveniences than RepoDB |
| 🐝 **NHibernate** | Mature, feature-rich (caching, mapping strategies) | Heaviest overhead of the group, steeper learning curve |

> 📌 Exact numbers vary by hardware, database engine, and dataset size — always run the benchmarks against your own target environment before drawing conclusions for production use.

-----

## 🤝 Contributions

Found a way to make a comparison fairer, more complete, or want to add a new provider? Contributions are very welcome.

- Add a new provider by following the pattern in [RepoDb.Benchmarks.SqlServer](RepoDb.Benchmarks.SqlServer), [RepoDb.Benchmarks.PostgreSql](RepoDb.Benchmarks.PostgreSql), [RepoDb.Benchmarks.MySql](RepoDb.Benchmarks.MySql), [RepoDb.Benchmarks.MySqlConnector](RepoDb.Benchmarks.MySqlConnector), [RepoDb.Benchmarks.Oracle](RepoDb.Benchmarks.Oracle), or [RepoDb.Benchmarks.MariaDb](RepoDb.Benchmarks.MariaDb).
- File a [new issue](https://github.com/mikependon/RepoDb/issues/new) if you spot a methodology concern.
- Read the main [contributing guide](../../../CONTRIBUTING.md) before submitting a PR.

## 📜 License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2018 [Michael Camara Pendon](https://x.com/mike_pendon)
