<div align="center">
    <image src="logo.png" style="width:256px;" />
    <br/>
    <span style="font-size:16px;font-weight:bold;">A production-ready data access platform for .NET applications.</span>
</div>

-----

RepoDB is a high-performance data productivity platform for .NET developers. At its core is the popular hybrid-ORM library for .NET with clean and easy APIs.

It provides the flexibility to work the way you want — all through the [IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection) interface. Write raw SQL when you need absolute control, or use the fluent APIs for more productivity, and switch seamlessly between both without sacrificing performance or maintainability.

## Why RepoDB?

RepoDB solves a complex problem in the .NET data access space: making bulk operations simple. It is designed to efficiently move millions of records across different database providers. Imagine migrating massive datasets from legacy databases to modern, cloud-native platforms — efficiently, reliably, and with high performance.

### As a Productivity Platform

It goes beyond the ORM with enterprise-grade capabilities for building, operating, and scaling with confidence:

- **📦 Bulk Operations** — High-performance bulk inserts, updates, merges, and deletes built for demanding production workloads.
- **🔄 Data Replication** — Scalable data movement and synchronization across multiple database platforms.
- **📊 Telemetry** — Immediate visibility into execution times, failures, and application behavior, with minimal configuration.

### As a Hybrid-ORM

It stays close to the metal and remaining lightweight:

- **👌 Easy to Use** — All operations are extension methods on [IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection). Open a connection and you're ready to go.
- **🚀 High Performance** — Compiled expressions are cached and reused. RepoDB understands your schema to generate the most efficient execution path ahead of time.
- **🧠 Memory Efficient** — Object properties, execution contexts, mappings, and SQL statements are extracted once and reused throughout the lifetime of your application.
- **🔀 Hybrid** — Use fluent methods for everyday CRUD, drop down to raw SQL for complex queries, or mix both — all within the same connection.
- **🏆 Battle-Tested** — Backed by thousands of unit and integration tests, and used in production systems worldwide.
- **🆓 Always Free** — Apache 2.0 licensed, forever open source.

### As a Universal Connector

It bridges different database systems with a single, unified connectivity layer:

- **🔌 Universal Connectivity** — Works with any provider that implements [IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection), from SQL Server and PostgreSQL to ClickHouse, DB2, Oracle, and more.
- **🌉 Cross-Provider Data Movement** — Move data between different database engines without writing hand-rolled conversion logic.
- **🧩 Consistent API Surface** — The same fluent methods and conventions work identically regardless of the underlying provider.
- **⚙️ Provider-Native Optimizations** — Each connector is tuned to exploit the bulk and native capabilities of its target database.

### Roadmap

RepoDB is evolving from an ORM into a broader data productivity platform, with Data Engineering capabilities — moving data between providers, on-premise or cloud-native — on the roadmap.

<img src="https://raw.githubusercontent.com/mikependon/RepoDB.Resources/refs/heads/main/blogs/images/posts/2026-08/repodb-new-architecture.png" style="max-width:768px" />

## Packages and Build Status

RepoDB ships as a core package plus provider-specific packages for each supported database, with optional bulk-operations add-ons.

See the full [package list and build status](PACKAGES.md).

## Get Started

Choose your database and follow the quick-start guide:

- [ClickHouse](http://repodb.net/tutorial/get-started-clickhouse)
- [Db2](http://repodb.net/tutorial/get-started-db2)
- [MariaDB](http://repodb.net/tutorial/get-started-mariadb)
- [MySQL](http://repodb.net/tutorial/get-started-mysql)
- [Oracle](http://repodb.net/tutorial/get-started-oracle)
- [PostgreSQL](http://repodb.net/tutorial/get-started-postgresql)
- [SQL Server](http://repodb.net/tutorial/get-started-sqlserver)
- [SQLite](http://repodb.net/tutorial/get-started-sqlite)

Explore individual features in the [documentation](http://repodb.net/docs).

## Supported Databases

Raw SQL execution methods work with **any** ADO.NET-compatible provider:

- [ExecuteQuery](http://repodb.net/operation/executequery)
- [ExecuteNonQuery](http://repodb.net/operation/executenonquery)
- [ExecuteScalar](http://repodb.net/operation/executescalar)
- [ExecuteReader](http://repodb.net/operation/executereader)
- [ExecuteQueryMultiple](http://repodb.net/operation/executequerymultiple)

Fluent operations (Query, Insert, Merge, Delete, Update, and [more](http://repodb.net/operation)) are supported for DB providers mentioned at [get-started](#get-started) section.

## Type Coercion

RepoDB uses ADO.NET's native coercion by default, keeping type mismatches visible and explicit. To enable automatic conversion:

```csharp
GlobalConfiguration
    .Setup(new Options.GlobalConfigurationOptions()
    {
        ConversionType = ConversionType.Automatic
    })
    .UseSqlServer();
```

## How RepoDB Compares

RepoDB sits between a micro-ORM and a full ORM. Each tool below makes different tradeoffs — pick the one that fits your project:

| | RepoDB | Dapper | Entity Framework |
|---|---|---|---|
| **Abstraction level** | ✅ Hybrid — fluent CRUD + raw SQL | ❌ Micro-ORM — raw SQL mapping | ✅ Full ORM — LINQ, change tracking |
| **Fluent CRUD API** | ✅ Yes (Insert, Query, Update, Delete, Merge, [more](http://repodb.net/operation)) | ❌ No — SQL per call | ✅ Yes, via LINQ/`DbSet` |
| **Raw SQL** | ✅ Yes, mixed freely with fluent calls | ✅ Yes — its core model | ✅ Yes, via `FromSql` |
| **Change tracking** | ❌ None | ❌ None | ✅ Yes |
| **Migrations** | ❌ None built-in | ❌ None built-in | ✅ Yes (EF Migrations) |
| **Native Bulk** | ✅ Built-in, cross-provider | ❌ Via extensions | ❌ Via extensions/third-party |
| **Insights / telemetry** | ✅ Built-in ([RepoDb.Telemetry.Default](RepoDb.Telemetry.Default/README.md)) | ❌ None built-in — manual or third-party (e.g. MiniProfiler) | ✅ Built-in logging/interceptors; OTel via community packages |
| **Performance** | ✅ Close to raw ADO.NET | ✅ Close to raw ADO.NET | ❌ Overhead from tracking/materialization |
| **Best fit** | ✅ EF-like productivity without losing SQL control | ✅ Thinnest possible SQL-to-object mapper | ✅ Rich object graphs, LINQ, migrations |

Dapper and Entity Framework are both excellent, mature tools — this reflects design tradeoffs, not a ranking.

## Telemetry

RepoDB includes opt-in, drop-in telemetry via [RepoDb.Telemetry.Default](RepoDb.Telemetry.Default/README.md). Enable it once at startup and every operation (Insert, Query, Update, Delete, etc.) is captured and published to your insights collector automatically — no custom `ITrace` required.

<img src="https://raw.githubusercontent.com/mikependon/RepoDB.Blogs.Resources/refs/heads/main/images/repodb-insights-default-telemetry-banner.png" style="max-width:768px;" />

It comes with great and simple dashboards visualization.

Simply `docker compose up -d` the [docker-compose.yml](https://raw.githubusercontent.com/mikependon/RepoDB/refs/heads/master/RepoDb.Telemetry.Default/docker-compose.yml) and [.env](https://raw.githubusercontent.com/mikependon/RepoDB/refs/heads/master/RepoDb.Telemetry.Default/.env) files and integrate your code.

```csharp
GlobalConfiguration
    .Setup(new GlobalConfigurationOptions { UseRegisteredGlobalTraces = true })
    .UseDefaultTelemetry(new DefaultTelemetryOption("<YOUR_APPLICATION_NAME>")
    {
        Host = "https://your-collector-host",
        ApiKey = "YOUR_API_KEY",
        Group = "<YOUR_APPLICATION_GROUP>",
        Frequency = TimeSpan.FromSeconds(1)
    });
```

It's intentionally lightweight rather than OTel-based, keeping RepoDB's thin, fast footprint intact. See the [package README](RepoDb.Telemetry.Default/README.md) for configuration options, the full OTel rationale, and the roadmap.

## Contributions

We welcome contributions of all kinds — code, docs, bug reports, and ideas.

- Browse [for-grabs](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) issues and submit a PR.
- File a [new issue](https://github.com/mikependon/RepoDb/issues/new) to start a discussion.
- Contribute to the [documentation site](https://github.com/mikependon/RepoDb.NET).
- Blog about it, share it, or simply give us a :star:

### Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A and community chat.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

Read our [contributing](CONTRIBUTING.md) page for more.

### Resources

- [Building the Solutions](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/building-the-solutions.md)
- [Coding Standards](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/coding-standards.md)
- [Issuing a Pull Request](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/issuing-a-pull-request.md)
- [Reporting an Issue](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/reporting-an-issue.md)
- [Support Policy](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/support-policy.md)
- [Limitations](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/limitations.md)

### Contributors

<a href="https://github.com/mikependon/RepoDB/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=mikependon/RepoDB" />
</a>

## Credits

Thanks to all [contributors](https://github.com/mikependon/RepoDb/graphs/contributors) and to [Scott Hanselman](https://www.hanselman.com/) for [featuring RepoDB](https://www.hanselman.com/blog/ExploringTheNETOpenSourceHybridORMLibraryRepoDB.aspx).

Tools and projects that make RepoDB possible: [GitHub](https://github.com/), [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ), [Moq](https://github.com/moq/moq4), [NuGet](https://www.nuget.org/), [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher), [Shields](https://shields.io/), [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/), [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/), [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/), [Npgsql](https://www.nuget.org/packages/Npgsql/).

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2018 [Michael Camara Pendon](https://x.com/mike_pendon)
