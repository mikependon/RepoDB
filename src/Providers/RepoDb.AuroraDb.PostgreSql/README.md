<div align="center">
    <a href="https://repodb.net/tutorial/get-started-auroradb-postgresql/"><image src="AuroraDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-auroradb-postgresql/"><strong>RepoDb.AuroraDb.PostgreSql</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for Amazon Aurora PostgreSQL in .NET.</span>
</div>

-----

<br/>

[![AuroraDbBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-auroradb-postgresql.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-auroradb-postgresql.yml)
[![AuroraDbHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![AuroraDbVersion](https://img.shields.io/nuget/v/RepoDb.AuroraDb.PostgreSql?&logo=nuget)](https://www.nuget.org/packages/RepoDb.AuroraDb.PostgreSql)

# [RepoDb.AuroraDb.PostgreSql](https://repodb.net/tutorial/get-started-auroradb-postgresql) — RepoDB for Amazon Aurora PostgreSQL

The Amazon Aurora PostgreSQL provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [RepoDb.Connector.AuroraDb.Npgsql](https://www.nuget.org/packages/RepoDb.Connector.AuroraDb.Npgsql).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.
- [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#auroradb-postgresql) — Amazon Aurora PostgreSQL-specific caveats.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [RepoDb.Connector.AuroraDb.Npgsql](https://www.nuget.org/packages/RepoDb.Connector.AuroraDb.Npgsql/) — the ADO.NET connector for Amazon Aurora PostgreSQL (built on the AWS Advanced .NET Data Provider Wrapper and Npgsql).
- [Npgsql](https://www.nuget.org/packages/Npgsql/) — the PostgreSQL driver (pinned to version 10).
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.AuroraDb.PostgreSql
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
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (\"Id\" = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET \"FirstName\" = @FirstName WHERE (\"Id\" = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new AuroraDbConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM \"Customer\";");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-auroradb-postgresql) page for the full Amazon Aurora PostgreSQL guide.

## Notes

- **Connection strings.** Amazon Aurora PostgreSQL speaks the PostgreSQL wire protocol, so identifiers are quoted with `"` and raw SQL uses `@name` parameter placeholders. The connection string follows the Npgsql format, e.g. `Server=my-cluster.cluster-xxxx.us-east-1.rds.amazonaws.com;Port=5432;Database=RepoDb;User Id=postgres;Password=...;`. It can also carry the settings of the [AWS Advanced .NET Data Provider Wrapper](https://github.com/aws/aws-advanced-dotnet-data-provider-wrapper) (e.g. `Plugins=failover,efm`), which `AuroraDbConnection` is built on.
- **Date and time types.** `DATE` and `TIME` columns are read as `DateOnly` and `TimeOnly`, the same as in `RepoDb.PostgreSql`. This is why the package references Npgsql 10.
- **Spatial types.** PostGIS `GEOMETRY`/`GEOGRAPHY` columns are exchanged as Well-Known Text through the PostGIS functions (i.e. `ST_GeomFromText`, `ST_AsText`). Reading them natively requires an Npgsql spatial plugin.

See the [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#auroradb-postgresql) page before relying on `TransactionScope` against a server that is not an Aurora cluster.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)
