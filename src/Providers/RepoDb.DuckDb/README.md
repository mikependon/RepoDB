<div align="center">
    <image src="DuckDB.png" style="width:256px;" />
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-duckdb/"><strong>RepoDb.DuckDb</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for DuckDB in .NET.</span>
</div>

-----

<br/>

[![DuckDbBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-duckdb.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-duckdb.yml)
[![DuckDbHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![DuckDbVersion](https://img.shields.io/nuget/v/RepoDb.DuckDb?&logo=nuget)](https://www.nuget.org/packages/RepoDb.DuckDb)

# [RepoDb.DuckDb](https://repodb.net/tutorial/get-started-duckdb) — RepoDB for DuckDB

The DuckDB provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [DuckDB.NET](https://www.nuget.org/packages/DuckDB.NET.Data.Full).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [DuckDB.NET.Data.Full](https://www.nuget.org/packages/DuckDB.NET.Data.Full/) — the ADO.NET data provider for DuckDB.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.DuckDb
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseDuckDb();
```

Then use any RepoDB operation directly on your `DuckDBConnection`:

### Query

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
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
using (var connection = new DuckDBConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (Id = $Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET FirstName = $FirstName WHERE (Id = $Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Customer\";");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-duckdb) page for the full DuckDB guide.

## Notes

DuckDB is an embedded, in-process analytical database engine — there is no server to connect to, and `ConnectionString` is either a file path (`Data Source=my.db`) or `Data Source=:memory:`. DuckDB also uses `$name` as its parameter placeholder sigil in raw SQL text (not `@name`), as reflected in the `ExecuteQuery`/`ExecuteNonQuery` examples above.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)
