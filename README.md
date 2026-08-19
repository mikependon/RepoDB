<div align="center">
    <image src="logo.png" style="width:256px;" />
    <br/>
    <span style="font-size:16px;font-weight:bold;">A production-ready data access platform for .NET applications.</span>
</div>

-----

RepoDB is a high-performance data productivity platform for .NET developers. At its core is the popular hybrid-ORM library for .NET.

It provides the flexibility to work the way you want — all through the [IDbConnection](https://learn.microsoft.com/en-us/dotnet/api/system.data.idbconnection) interface. Write raw SQL when you need absolute control, or use the fluent APIs for more productivity, and switch seamlessly between both without sacrificing performance or maintainability.

## Why RepoDB?

RepoDB solves a complex problem in the data access space: making bulk operations simple. It is designed to efficiently move millions of records across different database providers. Imagine migrating massive datasets from legacy databases to modern, cloud-native platforms — efficiently, reliably, and with high performance.

It also addresses a common tension in data access: choosing between the raw performance and control of manual ADO.NET and the productivity of a full-featured ORM. RepoDB brings both together — without forcing a trade-off.

### As a Hybrid-ORM

It stays close to the metal while remaining easy to use:

| Feature | Description |
|---|---|
| **👌 Easy to Use** | All operations are extension methods on `IDbConnection`. Open a connection and you're ready to go. |
| **🚀 High Performance** | Compiled expressions are cached and reused. RepoDB understands your schema to generate the most efficient execution path ahead of time. |
| **🧠 Memory Efficient** | Object properties, execution contexts, mappings, and SQL statements are extracted once and reused throughout the lifetime of your application. |
| **🔀 Hybrid** | Use fluent methods for everyday CRUD, drop down to raw SQL for complex queries, or mix both — all within the same connection. |
| **🏆 Battle-Tested** | Backed by thousands of unit and integration tests, and used in production systems worldwide. |
| **🆓 Always Free** | Apache 2.0 licensed, forever open source. |

### As a Productivity Platform

It goes beyond the ORM with enterprise-grade capabilities for building, operating, and scaling with confidence:

| Feature | Description |
|---|---|
| **📦 Bulk Operations** | High-performance bulk inserts, updates, merges, and deletes built for demanding production workloads. |
| **🔄 Data Replication** | Scalable data movement and synchronization across multiple database platforms. |
| **📊 Telemetry** | Immediate visibility into execution times, failures, and application behavior, with minimal configuration. |
| **🗄️ Multi-DB Support** | A growing range of relational database providers with a consistent development experience. |

## Packages and Build Status

| Project | Nuget | Downloads | Status |
|---------|-------|-----------|--------|
| [Core](https://www.nuget.org/packages/RepoDb) | [![](https://img.shields.io/nuget/v/RepoDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb) | [![](https://img.shields.io/nuget/dt/RepoDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-core.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-core.yml) |
| [SQL Server](https://www.nuget.org/packages/RepoDb.SqlServer) | [![](https://img.shields.io/nuget/v/RepoDb.SqlServer?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer) | [![](https://img.shields.io/nuget/dt/RepoDb.SqlServer?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr.yml) |
| [SQL Server (Bulk)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![](https://img.shields.io/nuget/v/RepoDb.SqlServer.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.SqlServer.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr-bulk.yml) |
| [Oracle](https://www.nuget.org/packages/RepoDb.Oracle) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Oracle?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Oracle) | [![](https://img.shields.io/nuget/dt/RepoDb.Oracle?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Oracle) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-oracle.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-oracle.yml) |
| [Oracle (Bulk)](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Oracle.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.Oracle.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-oracle-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-oracle-bulk.yml) |
| [PostgreSQL](https://www.nuget.org/packages/RepoDb.PostgreSql) | [![](https://img.shields.io/nuget/v/RepoDb.PostgreSql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql) | [![](https://img.shields.io/nuget/dt/RepoDb.PostgreSql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql.yml) |
| [PostgreSQL (Bulk)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations) | [![](https://img.shields.io/nuget/v/RepoDb.PostgreSql.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.PostgreSql.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql-bulk.yml) |
| [MySQL](https://www.nuget.org/packages/RepoDb.MySql) | [![](https://img.shields.io/nuget/v/RepoDb.MySql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql) | [![](https://img.shields.io/nuget/dt/RepoDb.MySql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql.yml) |
| [MySQL (Bulk)](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MySql.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.MySql.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql-bulk.yml) |
| [MySQL Connector](https://www.nuget.org/packages/RepoDb.MySqlConnector) | [![](https://img.shields.io/nuget/v/RepoDb.MySqlConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySqlConnector) | [![](https://img.shields.io/nuget/dt/RepoDb.MySqlConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySqlConnector) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysqlconnector.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysqlconnector.yml) |
| [MySQL Connector (Bulk)](https://www.nuget.org/packages/RepoDb.MySqlConnector.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MySqlConnector.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySqlConnector.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.MySqlConnector.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySqlConnector.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysqlconnector-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysqlconnector-bulk.yml) |
| [MariaDB](https://www.nuget.org/packages/RepoDb.MariaDb) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MariaDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDb) | [![](https://img.shields.io/nuget/dt/RepoDb.MariaDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDb) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mariadb.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mariadb.yml) |
| [MariaDB (Bulk)](https://www.nuget.org/packages/RepoDb.MariaDb.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MariaDb.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDb.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.MariaDb.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDb.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mariadb-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mariadb-bulk.yml) |
| [MariaDB Connector](https://www.nuget.org/packages/RepoDb.MariaDbConnector) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MariaDbConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDbConnector) | [![](https://img.shields.io/nuget/dt/RepoDb.MariaDbConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDbConnector) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mariadbconnector.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mariadbconnector.yml) |
| [MariaDB Connector (Bulk)](https://www.nuget.org/packages/RepoDb.MariaDbConnector.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.MariaDbConnector.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDbConnector.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.MariaDbConnector.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDbConnector.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mariadbconnector-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mariadbconnector-bulk.yml) |
| [IBM DB2](https://www.nuget.org/packages/RepoDb.Db2) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Db2?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2) | [![](https://img.shields.io/nuget/dt/RepoDb.Db2?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-db2.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-db2.yml) |
| [IBM DB2 (Bulk)](https://www.nuget.org/packages/RepoDb.Db2.BulkOperations) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Db2.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2.BulkOperations) | [![](https://img.shields.io/nuget/dt/RepoDb.Db2.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-db2-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-db2-bulk.yml) |
| [SQLite](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft) | [![](https://img.shields.io/nuget/v/RepoDb.Sqlite.Microsoft?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft) | [![](https://img.shields.io/nuget/dt/RepoDb.Sqlite.Microsoft?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-microsoft.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-microsoft.yml) |
| [Telemetry (Core)](https://www.nuget.org/packages/RepoDb.Telemetry.Core) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Telemetry.Core?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Telemetry.Core) | [![](https://img.shields.io/nuget/dt/RepoDb.Telemetry.Core?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Telemetry.Core) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-telemetry-core.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-telemetry-core.yml) |
| [Telemetry (Default)](https://www.nuget.org/packages/RepoDb.Telemetry.Default) 🆕 | [![](https://img.shields.io/nuget/v/RepoDb.Telemetry.Default?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Telemetry.Default) | [![](https://img.shields.io/nuget/dt/RepoDb.Telemetry.Default?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Telemetry.Default) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-telemetry-default.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-telemetry-default.yml) |

## Get Started

Choose your database and follow the quick-start guide:

- [SQL Server](http://repodb.net/tutorial/get-started-sqlserver)
- [Oracle](http://repodb.net/tutorial/get-started-oracle)
- [PostgreSQL](http://repodb.net/tutorial/get-started-postgresql)
- [MySQL](http://repodb.net/tutorial/get-started-mysql)
- [MariaDB](http://repodb.net/tutorial/get-started-mariadb) — covers both [RepoDb.MariaDb](RepoDb.MariaDb/README.md) and [RepoDb.MariaDbConnector](RepoDb.MariaDbConnector/README.md)
- [Db2](http://repodb.net/tutorial/get-started-db2)
- [SQLite](http://repodb.net/tutorial/get-started-sqlite)

Explore individual features in the [documentation](http://repodb.net/docs).

Want visibility into what your operations are doing in production? See [Telemetry](#telemetry) 🆕 below to enable opt-in insights with a couple lines of code.

## Supported Databases

Raw SQL execution methods work with **any** ADO.NET-compatible provider:

- [ExecuteQuery](http://repodb.net/operation/executequery)
- [ExecuteNonQuery](http://repodb.net/operation/executenonquery)
- [ExecuteScalar](http://repodb.net/operation/executescalar)
- [ExecuteReader](http://repodb.net/operation/executereader)
- [ExecuteQueryMultiple](http://repodb.net/operation/executequerymultiple)

Fluent operations (Query, Insert, Merge, Delete, Update, and [more](http://repodb.net/operation)) are supported for SQL Server, Oracle, MySQL, PostgreSQL, IBM DB2 and SQLite.

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

Read our [contibuting](CONTRIBUTING.md) page for more.

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
