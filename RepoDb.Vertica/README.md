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
- `InsertAll` batches real multi-row `INSERT INTO t (...) VALUES (...), (...), ...` statements (`IDbSetting.IsInsertAllBatchable = true`) - a single statement, not multiple `;`-separated ones, so it doesn't hit Vertica's compound-statement restriction. This matters: Vertica creates a new storage container (ROS) per *statement*, regardless of whether inserts share a transaction/commit, so one-row-at-a-time inserts exhaust the per-projection ROS container limit ("Too many ROS containers exist") on any sizeable `InsertAll`. A generated IDENTITY value for a batch is read back with a single `LAST_INSERT_ID()` query after the batch, then assigned to each row by computed offset - this assumes Vertica generates IDENTITY values contiguously and in input row order within one multi-row INSERT, which is unverified against a real (especially multi-node) cluster. `MergeAll` always executes one statement per row regardless (see below) - Vertica's ADO.NET provider does not support executing multiple separate statements in a single round-trip, unlike SQL Server/MySQL/PostgreSql.
- Vertica has neither a native single-statement upsert nor a usable `MERGE` for this purpose: it has no `UPDATE OR INSERT ... MATCHING` statement, and its ANSI `MERGE` fails outright (error 4711, "Sequence or IDENTITY/AUTO_INCREMENT column in merge query is not supported") against any table carrying an IDENTITY column, whether or not that column is referenced. Nor can a compound (`stmt1; stmt2`) statement stand in for it: `VerticaCommand` refuses to execute one at all once it carries a parameter, on every `Execute*` call (it always prepares first internally), not just when `.Prepare()` is called explicitly. `Merge`/`MergeAll` therefore set `IsUseUpsert = true`, routing through RepoDb.Core's `Exists` + `Update`/`Insert` fallback - genuinely separate, single-statement round-trips - instead of a single atomic statement.
- Vertica has no `RETURNING` clause on any DML. A generated IDENTITY value is instead read back via a separate `SELECT LAST_INSERT_ID()` query (Vertica's session-scoped equivalent of `SCOPE_IDENTITY()`, valid only for IDENTITY columns, not named sequences) - see `VerticaDbHelper.GetScopeIdentity`, called automatically by RepoDb.Core's `Insert`/`InsertAll` as a fallback whenever the insert's own `ExecuteScalar()` doesn't yield a value.
- Vertica's driver materializes a `TIME` column's value as a full `DateTime` combined with the *current* date at the time of the read, not a fixed placeholder date - so a value written and read back would otherwise never compare equal to itself. There is no accessor that returns a bare `TimeSpan` for a `TIME` column. Apply `VerticaTimeToDateTimePropertyHandler` to any `DateTime`-typed property mapped to a `TIME` column to re-base it onto `DateTime`'s default date; it is opt-in (not auto-registered) since `PropertyHandlerMapper` registrations by CLR type are global and would otherwise also affect `DATE`/`TIMESTAMP` columns.
  - This only fixes typed-entity queries (`Query<TEntity>`, `Merge<TEntity>`, etc.), since `PropertyHandlerMapper` is keyed by `(EntityType, PropertyInfo)` - an actual CLR property to attach to. Dynamic/`ExpandoObject` reads (`*ViaTableName` operations, a dynamic `Query`/`ExecuteQuery`) go through RepoDb.Core's dictionary-binding compiler, which reads the raw driver value directly with no property-handler hook at all. Fixing that path would require wrapping Vertica's (sealed, so un-subclassable) `VerticaConnection`/`VerticaCommand`/`VerticaDataReader` behind a from-scratch delegating `DbConnection`/`DbCommand`/`DbDataReader` triplet - not implemented. A `TIME` column read back through a dynamic path will show today's date instead of a placeholder.

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
