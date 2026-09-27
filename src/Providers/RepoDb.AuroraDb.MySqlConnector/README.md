<div align="center">
    <a href="https://repodb.net/tutorial/get-started-auroradb-mysqlconnector/"><image src="AuroraDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-auroradb-mysqlconnector/"><strong>RepoDb.AuroraDb.MySqlConnector</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for Amazon Aurora MySQL in .NET.</span>
</div>

-----

<br/>

[![AuroraDbBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-auroradb-mysqlconnector.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-auroradb-mysqlconnector.yml)
[![AuroraDbHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![AuroraDbVersion](https://img.shields.io/nuget/v/RepoDb.AuroraDb.MySqlConnector?&logo=nuget)](https://www.nuget.org/packages/RepoDb.AuroraDb.MySqlConnector)

# [RepoDb.AuroraDb.MySqlConnector](https://repodb.net/tutorial/get-started-auroradb-mysqlconnector) — RepoDB for Amazon Aurora MySQL

The Amazon Aurora MySQL provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [RepoDb.Connector.AuroraDb.MySqlConnector](https://www.nuget.org/packages/RepoDb.Connector.AuroraDb.MySqlConnector).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.
- [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#auroradb-mysql) — Amazon Aurora MySQL-specific caveats.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [RepoDb.Connector.AuroraDb.MySqlConnector](https://www.nuget.org/packages/RepoDb.Connector.AuroraDb.MySqlConnector/) — the ADO.NET connector for Amazon Aurora MySQL (built on the AWS Advanced .NET Data Provider Wrapper and MySqlConnector).
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.AuroraDb.MySqlConnector
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseAuroraDb();
```

Then use any RepoDB operation directly on your `AuroraDbConnection`:

### Query

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

### Insert

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Merge

```csharp
var customer = GetCustomer();
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var id = connection.Merge<Customer>(customer, qualifiers: e => new { e.Email });
}
```

### Delete

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM `Customer` WHERE (`Id` = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE `Customer` SET `FirstName` = @FirstName WHERE (`Id` = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM `Customer`;");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-auroradb-mysqlconnector) page for the full Amazon Aurora MySQL guide.

## Notes

- **Connection strings.** Amazon Aurora MySQL speaks the MySQL wire protocol, so identifiers are quoted with backticks and raw SQL uses `@name` parameter placeholders. The connection string follows the MySqlConnector format, e.g. `Server=my-cluster.cluster-xxxx.us-east-1.rds.amazonaws.com;Port=3306;Database=RepoDb;User ID=admin;Password=...;`. It can also carry the settings of the [AWS Advanced .NET Data Provider Wrapper](https://github.com/aws/aws-advanced-dotnet-data-provider-wrapper) (e.g. `Plugins=failover,efm`), which `AuroraDbConnection` is built on.
- **Server version.** The generated SQL targets Aurora MySQL version 3 (MySQL 8.0 compatible). `InsertAll` reads the identities back with `VALUES ROW(...)`, which needs MySQL 8.0.19 or later, so Aurora MySQL version 2 (MySQL 5.7 compatible) is not supported.
- **Spatial types.** `GEOMETRY` columns (and its subtypes) can be mapped to a `MySqlGeometry` property with the `AuroraDbGeometryToMySqlGeometryPropertyHandler`.

See the [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#auroradb-mysql) page before referencing this package together with `RepoDb.AuroraDb.PostgreSql`.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)
