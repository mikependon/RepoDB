<div align="center">
    <a href="https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Ahtola"><image src="SQLite.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Ahtola"><strong>RepoDb.Sqlite.Ahtola</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for Ahtola (SQLite-compatible) in .NET.</span>
</div>

-----

<br/>

[![SqLiteAhtolaBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-ahtola.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-ahtola.yml)
[![SqLiteAhtolaHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![SqLiteAhtolaVersion](https://img.shields.io/nuget/v/RepoDb.Sqlite.Ahtola?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Sqlite.Ahtola)

# [RepoDb.Sqlite.Ahtola](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Ahtola) — RepoDB for Ahtola (Devolutions.Ahtola.Data.Sqlite)

The Ahtola provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [Devolutions.Ahtola.Data.Sqlite](https://www.nuget.org/packages/Devolutions.Ahtola.Data.Sqlite).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Devolutions.Ahtola.Data.Sqlite](https://www.nuget.org/packages/Devolutions.Ahtola.Data.Sqlite/) — Ahtola data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Sqlite.Ahtola
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseAhtola();
```

Then use any RepoDB operation directly on your `SqliteConnection` (`using Ahtola.Data.Sqlite;`):

### Query

```csharp
using (var connection = new SqliteConnection(ConnectionString))
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
using (var connection = new SqliteConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE [Customer] SET FirstName = @FirstName WHERE (Id = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [Customer];");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-sqlite) page for the full SQLite guide; the same usage applies to Ahtola.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2018 [Michael Camara Pendon](https://x.com/mike_pendon)
