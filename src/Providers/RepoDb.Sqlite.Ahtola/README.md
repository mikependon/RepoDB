<div align="center">
    <a href="https://github.com/Devolutions/ahtola"><image src="SQLite.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><strong>RepoDb.Sqlite.Ahtola</strong></span>
    <br/>
    <span style="font-size:16px;">RepoDB for Ahtola, a pure managed (C#) SQLite-compatible engine.</span>
</div>

-----

# RepoDb.Sqlite.Ahtola — RepoDB for Ahtola (Devolutions.Ahtola.Data.Sqlite)

The [Ahtola](https://github.com/Devolutions/ahtola) provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [Devolutions.Ahtola.Data.Sqlite](https://www.nuget.org/packages/Devolutions.Ahtola.Data.Sqlite), a pure managed port of Turso's SQLite-compatible engine (no native SQLite library required).

This provider is a copy of `RepoDb.Sqlite.Microsoft` re-targeted at Ahtola's `Microsoft.Data.Sqlite`-compatible facade (`Ahtola.Data.Sqlite.SqliteConnection`). The provider types are prefixed with `Ahtola` so both providers can be loaded side by side in the same process.

> ⚠️ Ahtola is an experimental engine and is not production-ready. Targets `net8.0`, `net9.0` and `net10.0` only.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Devolutions.Ahtola.Data.Sqlite](https://www.nuget.org/packages/Devolutions.Ahtola.Data.Sqlite/) — Ahtola ADO.NET data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Known Differences from Microsoft.Data.Sqlite

- **NUMERIC/DECIMAL column field types** — Ahtola's `SqliteDataReader.GetFieldType()` reports `System.String` for columns declared as `NUMERIC`/`DECIMAL` (or any unrecognized declared type), while `GetValue()` returns a `double`. RepoDB core handles this by converting string-typed reader fields into numeric target properties using the invariant culture.
- **Multi-statement `ExecuteNonQuery`** — for `DELETE ...; VACUUM;` Ahtola returns only the rows affected by the `DELETE` (native SQLite carries the previous change count over `VACUUM`, doubling it).
- **Truly asynchronous operations** — async methods may resume on a different thread, so wrap async work in `new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)`.
- **Performance** — Ahtola is fully managed (no native binaries); writes are slower than native SQLite. Use explicit transactions for batched writes. See `src/Benchmarks/RepoDb.Benchmarks.Ahtola`.
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

Then use any RepoDB operation directly on Ahtola's `SqliteConnection` (`using Ahtola.Data.Sqlite;`):

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
