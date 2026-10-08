<div align="center">
    <a href="https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Turso"><image src="SQLite.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Turso"><strong>RepoDb.Sqlite.Turso</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for Turso (SQLite-compatible) in .NET.</span>
</div>

-----

<br/>

[![SqLiteTursoBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-turso.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-turso.yml)
[![SqLiteTursoHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![SqLiteTursoVersion](https://img.shields.io/nuget/v/RepoDb.Sqlite.Turso?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Sqlite.Turso)

# [RepoDb.Sqlite.Turso](https://github.com/mikependon/RepoDB/tree/master/src/Providers/RepoDb.Sqlite.Turso) — RepoDB for Turso (Turso.Data.Sqlite)

The Turso provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [Turso's .NET bindings](https://github.com/tursodatabase/turso/tree/main/bindings/dotnet) (`Turso.Data.Sqlite`). Targets .NET 8, 9, and 10; .NET Standard is not supported by the underlying driver.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Turso.Data.Sqlite.Provider](https://www.nuget.org/packages/Turso.Data.Sqlite.Provider/) — Turso data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon), [Marc-André Moreau](https://x.com/awakecoding)

--------

## Installation

```
Install-Package RepoDb.Sqlite.Turso
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup(new()
    {
        ConversionType = RepoDb.Enumerations.ConversionType.Automatic
    })
    .UseTurso();
```

Then use any RepoDB operation directly on your `SqliteConnection` (`using Turso.Data.Sqlite;`). `Data Source=:memory:` creates a private in-memory database; keep the connection open while using it.

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

Visit the [get-started](http://repodb.net/tutorial/get-started-sqlite) page for the full SQLite guide; the same usage applies to Turso.

## Compatibility

The provider retains SQLite SQL generation, parameter mapping, schema discovery,
pagination, transactions, batched inserts, and RepoDB's insert/update-based merge
behavior. There is no separate bulk-copy package.

Automatic conversion is recommended when entity types differ from the driver's
SQLite storage types (for example decimal, numeric, boolean, or date columns).
It is an explicit, process-wide RepoDB option; `UseTurso()` does not change it.
`Truncate` deletes rows without running `VACUUM`, which is experimental in the
Turso engine. It does not reclaim file space or reset an `AUTOINCREMENT` sequence.

Only `Turso.Data.Sqlite.SqliteConnection` is registered. The lower-level
`Turso.TursoConnection` is not interchangeable with the facade for this provider.
Provider-specific parameter attributes are in
`RepoDb.Attributes.Parameter.Turso`, avoiding conflicts with
`RepoDb.Sqlite.Microsoft`. Both packages can be referenced together; initialize
each with its own `UseTurso()` / `UseSqlite()` call.

Registration coexistence does not guarantee shared execution-cache compatibility.
Local benchmarking exposed an `InsertAll` parameter-setter cache collision when
the same entity type is used with both providers in one process: a Microsoft
SQLite parameter handler can be reused for a Turso parameter and throw
`InvalidCastException`. That mixed-provider scenario is not currently supported.

This provider includes a corresponding RepoDB core fix to resolve reader column
types after reading the first row. When publishing packages, release the core
from this revision as well; do not pin the provider to an older core package
without that fix.

Turso is not a complete SQLite implementation: raw SQLite handles, built-in FTS
modules, dirty reads, and remote client-side extension/function helpers have
limitations documented upstream. Local async methods may use synchronous native
execution. Remote/replica behavior requires an appropriate Turso endpoint and
credentials; the copied integration suite runs locally without either.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon) and [Marc-André Moreau](https://x.com/awakecoding)
