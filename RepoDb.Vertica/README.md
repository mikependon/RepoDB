[![VerticaBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-vertica.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-vertica.yml)
[![VerticaHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![VerticaVersion](https://img.shields.io/nuget/v/RepoDb.Vertica?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Vertica)

# [RepoDb.Vertica](https://repodb.net/tutorial/get-started-vertica) — RepoDB for Vertica

The Vertica provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [Vertica.Data.VerticaClient](https://www.nuget.org/packages/Vertica.Data.VerticaClient).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Vertica.Data.VerticaClient](https://www.nuget.org/packages/Vertica.Data.VerticaClient/) — the Vertica ADO.NET data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## Known limitations

- Targets **Vertica 3.0 and later**. Identity-column detection relies on `RDB$RELATION_FIELDS.RDB$IDENTITY_TYPE`, which does not exist on Vertica 2.5 and earlier; tables using the pre-3.0 trigger + generator pattern for auto-increment will not be detected as identity columns.
- `InsertAll`/`MergeAll` always execute one statement per row (`batchSize` is effectively 1). Vertica's ADO.NET provider does not support executing multiple statements in a single round-trip, unlike SQL Server/MySQL/PostgreSql.
- `Merge`/`MergeAll` are implemented with Vertica's native `UPDATE OR INSERT ... MATCHING (...)` statement rather than an ANSI `MERGE`. When the identity column is also a qualifier (the common default case, since qualifiers default to the primary key), a plain `UPDATE OR INSERT` can't tell "match this literal 0/null" apart from "auto-generate me" - `Merge`/`MergeAll` compile to an `EXECUTE BLOCK` in that case, which branches at runtime between a plain `INSERT` (when the identity value is null/0, letting Vertica auto-generate it) and the ordinary `MATCHING`-based `UPDATE OR INSERT` (when a real identity value is supplied).
- There is no session-wide "last identity" (no equivalent of `SCOPE_IDENTITY()`/`LAST_INSERT_ID()`); the generated key is returned directly by `Insert`/`Merge` via Vertica's `RETURNING` clause.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Vertica
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseVertica();
```

Then use any RepoDB operation directly on your `VerticaConnection`:

### Query

```csharp
using (var connection = new VerticaConnection(ConnectionString))
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
using (var connection = new VerticaConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (\"Id\" = @Id)", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET \"FirstName\" = @FirstName WHERE (\"Id\" = @Id)", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Customer\"");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-vertica) page for the full Vertica guide.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)
