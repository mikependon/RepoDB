
[![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr.yml)
[![Version](https://img.shields.io/nuget/v/RepoDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb)
[![MsTeams](https://img.shields.io/badge/chat-microsoft%20teams-6264A7?&logo=microsoftteams&logoColor=white&style=for-the-badge)](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ)

# RepoDB — a Hybrid ORM for .NET

RepoDB is a fast, lightweight, and open-source .NET ORM that gives you the best of both worlds: the simplicity of a micro-ORM and the power of a full ORM — without the overhead.

Write raw SQL when you need full control. Use fluent methods when you want productivity. Switch between them freely, in the same codebase.

## Packages and Build Status

| Project | Nuget | Status |
|---------|-------|--------|
| [RepoDb](https://www.nuget.org/packages/RepoDb) | [![](https://img.shields.io/nuget/v/RepoDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-core.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-core.yml) |
| [RepoDb.SqlServer](https://www.nuget.org/packages/RepoDb.SqlServer) | [![](https://img.shields.io/nuget/v/RepoDb.SqlServer?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr.yml) |
| [RepoDb.MySql](https://www.nuget.org/packages/RepoDb.MySql) | [![](https://img.shields.io/nuget/v/RepoDb.MySql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql.yml) |
| [RepoDb.MySqlConnector](https://www.nuget.org/packages/RepoDb.MySqlConnector) | [![](https://img.shields.io/nuget/v/RepoDb.MySqlConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySqlConnector) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysqlconnector.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysqlconnector.yml) |
| [RepoDb.PostgreSql](https://www.nuget.org/packages/RepoDb.PostgreSql) | [![](https://img.shields.io/nuget/v/RepoDb.PostgreSql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql.yml) |
| [RepoDb.SQLite.System](https://www.nuget.org/packages/RepoDb.SQLite.System) | [![](https://img.shields.io/nuget/v/RepoDb.SQLite.System?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SQLite.System) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-system.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-system.yml) |
| [RepoDb.Sqlite.Microsoft](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft) | [![](https://img.shields.io/nuget/v/RepoDb.Sqlite.Microsoft?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-microsoft.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-microsoft.yml) |
| [RepoDb.SqlServer.BulkOperations](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![](https://img.shields.io/nuget/v/RepoDb.SqlServer.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr-bulk.yml) |
| [RepoDb.PostgreSql.BulkOperations](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations) | [![](https://img.shields.io/nuget/v/RepoDb.PostgreSql.BulkOperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations) | [![Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql-bulk.yml) |

## Why RepoDB?

<details>
<summary><b>Expand to learn more</b></summary>

<p>

**Easy to Use** — all operations are extension methods on `IDbConnection`. Open a connection and you're ready to go.

**High Performance** — compiled expressions are cached and reused. RepoDB understands your schema to generate the most efficient execution path ahead of time.

**Memory Efficient** — object properties, execution contexts, mappings, and SQL statements are extracted once and reused throughout the lifetime of your application.

**Hybrid by Design** — use fluent methods for everyday CRUD, drop down to raw SQL for complex queries, or mix both — all within the same connection.

**Battle-Tested** — backed by thousands of unit and integration tests, and used in production systems worldwide.

**Always Free** — Apache 2.0 licensed, forever open source.

</p>

</details>

## Get Started

Choose your database and follow the quick-start guide:

- [SQL Server](http://repodb.net/tutorial/get-started-sqlserver)
- [MySQL](http://repodb.net/tutorial/get-started-mysql)
- [PostgreSQL](http://repodb.net/tutorial/get-started-postgresql)
- [SQLite](http://repodb.net/tutorial/get-started-sqlite)

Explore individual features in the [documentation](http://repodb.net/docs).

## Supported Databases

Raw SQL execution methods work with **any** ADO.NET-compatible provider:

- [ExecuteQuery](http://repodb.net/operation/executequery)
- [ExecuteNonQuery](http://repodb.net/operation/executenonquery)
- [ExecuteScalar](http://repodb.net/operation/executescalar)
- [ExecuteReader](http://repodb.net/operation/executereader)
- [ExecuteQueryMultiple](http://repodb.net/operation/executequerymultiple)

Fluent operations (Query, Insert, Merge, Delete, Update, and [more](http://repodb.net/operation)) are supported for SQL Server, MySQL, PostgreSQL, and SQLite.

## Type Coercion

RepoDB uses ADO.NET's native coercion by default, keeping type mismatches visible and explicit. To enable automatic conversion:

```csharp
RepoDb.Converter.ConversionType = ConversionType.Automatic;
```

## Contributions

We welcome contributions of all kinds — code, docs, bug reports, and ideas.

- Browse [for-grabs](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) issues and submit a PR.
- File a [new issue](https://github.com/mikependon/RepoDb/issues/new) to start a discussion.
- Contribute to the [documentation site](https://github.com/mikependon/RepoDb.NET).
- Blog about it, share it, or simply give us a :star:

### Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?tab=newest&q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A and community chat.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

### Contributor Resources

- [Building the Solutions](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/building-the-solutions.md)
- [Coding Standards](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/coding-standards.md)
- [Issuing a Pull Request](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/issuing-a-pull-request.md)
- [Reporting an Issue](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Docs/reporting-an-issue.md)
- [Support Policy](https://github.com/mikependon/RepoDB/blob/master/RepoDb.Docs/support-policy.md)

## Credits

<a href="https://github.com/mikependon/RepoDB/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=mikependon/RepoDB" />
</a>

Thanks to all [contributors](https://github.com/mikependon/RepoDb/graphs/contributors) and to [Scott Hanselman](https://www.hanselman.com/) for [featuring RepoDB](https://www.hanselman.com/blog/ExploringTheNETOpenSourceHybridORMLibraryRepoDB.aspx).

Tools and projects that make RepoDB possible: [GitHub](https://github.com/), [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ), [Moq](https://github.com/moq/moq4), [NuGet](https://www.nuget.org/), [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher), [Shields](https://shields.io/), [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/), [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/), [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/), [Npgsql](https://www.nuget.org/packages/Npgsql/).

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://twitter.com/mike_pendon)
