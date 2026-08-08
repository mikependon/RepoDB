[![Db2Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-db2.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-db2.yml)
[![Db2Home](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![Db2Version](https://img.shields.io/nuget/v/RepoDb.Db2?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2)

# RepoDb.Db2 — RepoDB for Db2 Database

The Db2 provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [ODP.NET (Db2.ManagedDataAccess.Core)](https://www.nuget.org/packages/Db2.ManagedDataAccess.Core).

## Target

Db2 Database 12c and later. Earlier versions are not supported (the provider relies on native `IDENTITY` columns, `OFFSET/FETCH` paging, and implicit result sets, all of which require 12c+).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## Dependencies

- [Db2.ManagedDataAccess.Core](https://www.nuget.org/packages/Db2.ManagedDataAccess.Core/) — ODP.NET Db2 data provider.
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

Then use any RepoDB operation directly on your `Db2Connection`:

### Query

```csharp
using (var connection = new Db2Connection(ConnectionString))
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
using (var connection = new Db2Connection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new Db2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new Db2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

## QueryMultiple Behavior

[`QueryMultiple`/`QueryMultipleAsync`](http://repodb.net/operation/executequerymultiple) return several result sets — one per target type — from a single call.

ODP.NET rejects a command text containing more than one SQL statement (`IDbSetting.IsMultiStatementExecutable = false` for `RepoDb.Db2`), so `QueryMultiple` automatically falls back to issuing one round trip per requested type instead of one combined command. This fallback is transparent — the same `QueryMultiple<T1, T2, ...>` call works unchanged against Db2 — but it means a call that costs 1 round trip on SQL Server/MySQL/PostgreSQL costs *N* round trips (one per type) on Db2. Keep this in mind for latency-sensitive code paths that call `QueryMultiple` with many types against an Db2 database.

## Known limitations (v1)

### `InsertAll` / `MergeAll`

Execute one row per round-trip for now (`IsMultiStatementExecutable = false`); true multi-row batching with a single implicit-result-set return will follow in a later release.

### Identity/primary-key

Retrieval on `Insert`/`Merge` relies on an Db2 12c+ implicit result set (`DBMS_SQL.RETURN_RESULT`) wrapped in an anonymous PL/SQL block, since Db2's native `RETURNING ... INTO` binds to an output parameter that RepoDb's core execution pipeline does not read back.

```csharp
DECLARE l_repodb_result "CompleteTable"."Id"%TYPE; l_repodb_cursor SYS_REFCURSOR; BEGIN INSERT INTO "CompleteTable" ( "SessionId", "ColumnVarchar", "ColumnNumber", "ColumnDate", "ColumnTimestamp" ) VALUES ( :SessionId, :ColumnVarchar, :ColumnNumber, :ColumnDate, :ColumnTimestamp ) RETURNING "Id" INTO l_repodb_result; OPEN l_repodb_cursor FOR SELECT l_repodb_result AS "Result" FROM DUAL; DBMS_SQL.RETURN_RESULT(l_repodb_cursor); END;
```

This should be verified against your own Db2 instance before relying on it in production.

### RETURNING on MERGE

A `RETURNING` clause on `MERGE` specifically is only supported starting with **Db2 Database 23ai** - it does not work on 12c/18c/19c/21c at all (fails with `ORA-00933`). This provider otherwise targets 12c+, but `Merge` against a table with a primary/identity key requires 23ai+ to get the key value back. On older versions, `Insert`/`Update`/`Query`/etc. are unaffected - only identity-returning `Merge` calls are impacted.

### GUID/UNIQUEIDENTIFIER

Db2 has no native GUID/`UNIQUEIDENTIFIER` type. A `System.Guid` data entity property will throw `ArgumentException: Value does not fall within the expected range.` from `Db2Parameter.Value` if bound directly, because (unlike `SqlParameter`/`NpgsqlParameter`) ODP.NET does not accept a raw `Guid` value. If a column stores a GUID as `RAW(16)`, map it as `byte[]` on the entity, or keep it as `Guid` and register `RepoDb.Db2.PropertyHandlers.GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new GuidToByteArrayPropertyHandler(), true);
```

Register it per-property (not globally for `typeof(Guid)`) if your process also uses another RepoDb provider that handles `Guid` natively, since a type-level `PropertyHandlerMapper` registration applies process-wide across all connections.
