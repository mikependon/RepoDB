[![Db2Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-db2.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-db2.yml)
[![Db2Home](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![Db2Version](https://img.shields.io/nuget/v/RepoDb.Db2?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2)

# RepoDb.Db2 — RepoDB for Db2 Database

The Db2 provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and the [IBM Data Server .NET Provider (Net.IBM.Data.Db2)](https://www.nuget.org/packages/Net.IBM.Data.Db2).

## Target

Db2 for Linux, UNIX, and Windows (LUW) 10.5 and later. Earlier versions are not supported (the provider relies on `OFFSET ... FETCH NEXT ... ROWS ONLY` paging, which requires 10.5+). Db2 for z/OS and Db2 for i are not currently tested against.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## Dependencies

- [Net.IBM.Data.Db2](https://www.nuget.org/packages/Net.IBM.Data.Db2/) — IBM's Data Server .NET provider for Db2.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Db2
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseDb2();
```

Every statement RepoDb.Db2 generates binds parameters using `":Name"`-style host variables (e.g. `WHERE "Id" = :Id`). IBM's Data Server .NET Provider disables host-variable support by default, so your connection string **must** include `HostVarParameters=True;`, otherwise every parameterized call fails with `DB2Exception` `SQL0313N`:

```
Server=localhost:50000;Database=REPODB;UID=db2inst1;PWD=yourpassword;HostVarParameters=True;
```

Then use any RepoDB operation directly on your `DB2Connection`:

### Query

```csharp
using (var connection = new DB2Connection(ConnectionString))
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
using (var connection = new DB2Connection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

## QueryMultiple Behavior

[`QueryMultiple`/`QueryMultipleAsync`](http://repodb.net/operation/executequerymultiple) return several result sets — one per target type — from a single call.

Db2's IBM Data Server provider rejects a command text containing more than one SQL statement (`IDbSetting.IsMultiStatementExecutable = false` for `RepoDb.Db2`), so `QueryMultiple` automatically falls back to issuing one round trip per requested type instead of one combined command. This fallback is transparent — the same `QueryMultiple<T1, T2, ...>` call works unchanged against Db2 — but it means a call that costs 1 round trip on SQL Server/MySQL/PostgreSQL costs *N* round trips (one per type) on Db2. Keep this in mind for latency-sensitive code paths that call `QueryMultiple` with many types against a Db2 database.

## Known limitations (v1)

### `InsertAll` / `MergeAll`

Execute one row per round-trip for now (`IsMultiStatementExecutable = false`); true multi-row batching in a single round trip will follow in a later release.

### Identity/primary-key retrieval

Db2's idiomatic way to read back a generated key on `Insert`/`Merge` is `SELECT ... FROM FINAL TABLE (INSERT INTO ... VALUES (...))` — an ANSI-SQL-adjacent construct that returns the post-insert row (including any identity-generated column) as an ordinary result set, with no PL/SQL block, output parameter, or cursor plumbing required. This same mechanism works uniformly for both `Insert` and `Merge`, on any Db2 version 9.7+ (well within this provider's 10.5+ target) — there is no version gate to worry about.

An earlier revision of this provider wrapped the key column in an Oracle-style `DECLARE ... DBMS_SQL.RETURN_RESULT(...)` PL/SQL block, which doesn't exist in Db2 — that has been replaced with the `FINAL TABLE` form described above. Verify `Insert`/`Merge` calls that request the generated key against your own Db2 instance before relying on this in production.

### GUID/UNIQUEIDENTIFIER

Db2 has no native GUID/`UNIQUEIDENTIFIER` type. A `System.Guid` data entity property cannot be bound directly to a `DB2Parameter` the way it can with `SqlParameter`/`NpgsqlParameter`. The idiomatic Db2 storage for a GUID is a fixed-length 16-byte `CHAR(16) FOR BIT DATA` column — map it as `byte[]` on the entity, or keep it as `Guid` and register `RepoDb.Db2.PropertyHandlers.Db2GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, Db2GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new Db2GuidToByteArrayPropertyHandler(), true);
```

Register it per-property (not globally for `typeof(Guid)`) if your process also uses another RepoDb provider that handles `Guid` natively, since a type-level `PropertyHandlerMapper` registration applies process-wide across all connections.
