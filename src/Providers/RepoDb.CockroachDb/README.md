<div align="center">
    <a href="https://repodb.net/tutorial/get-started-cockroachdb/"><image src="CockroachDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-cockroachdb/"><strong>RepoDb.CockroachDb</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for CockroachDB in .NET.</span>
</div>

-----

<br/>

[![CockroachDbBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-cockroachdb.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-cockroachdb.yml)
[![CockroachDbHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![CockroachDbVersion](https://img.shields.io/nuget/v/RepoDb.CockroachDb?&logo=nuget)](https://www.nuget.org/packages/RepoDb.CockroachDb)

# [RepoDb.CockroachDb](https://repodb.net/tutorial/get-started-cockroachdb) — RepoDB for CockroachDB

The CockroachDB provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [RepoDb.Connector.CockroachDb](https://www.nuget.org/packages/RepoDb.Connector.CockroachDb).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.
- [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#cockroachdb) — CockroachDB-specific caveats.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [RepoDb.Connector.CockroachDb](https://www.nuget.org/packages/RepoDb.Connector.CockroachDb/) — the ADO.NET connector for CockroachDB (built on Npgsql).
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.CockroachDb
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseCockroachDb();
```

Then use any RepoDB operation directly on your `CockroachDbConnection`:

### Query

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
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
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
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
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var id = connection.Merge<Customer>(customer, qualifiers: e => new { e.Email });
}
```

### Delete

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (\"Id\" = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET \"FirstName\" = @FirstName WHERE (\"Id\" = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<long>("SELECT COUNT(*) FROM \"Customer\";");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-cockroachdb) page for the full CockroachDB guide.

## Notes

CockroachDB speaks the PostgreSQL wire protocol, so identifiers are quoted with `"` and raw SQL uses `@name` parameter placeholders. The connection string follows the Npgsql format, e.g. `Server=127.0.0.1;Port=26257;Database=RepoDb;User Id=root;` for a local insecure node. CockroachDB's `INT` is a 64-bit `INT8`, so map integer columns to `long` properties (and read `COUNT(*)` as `long`), as shown above.

See the [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#cockroachdb) page before relying on transactions that mix data changes with schema changes.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)
