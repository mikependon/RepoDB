# RepoDb.Oracle

A hybrid .NET ORM library for Oracle Database, built on top of [RepoDb](https://repodb.net) and [ODP.NET (Oracle.ManagedDataAccess.Core)](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core).

## Target

Oracle Database 12c and later. Earlier versions are not supported (the provider relies on native `IDENTITY` columns, `OFFSET/FETCH` paging, and implicit result sets, all of which require 12c+).

## Get Started

```csharp
using Oracle.ManagedDataAccess.Client;

GlobalConfiguration
    .Setup()
    .UseOracle();

using var connection = new OracleConnection(connectionString);
var customers = connection.QueryAll<Customer>();
```

## Known limitations (v1)

- `InsertAll` / `MergeAll` execute one row per round-trip for now (`IsMultiStatementExecutable = false`); true multi-row batching with a single implicit-result-set return will follow in a later release.
- Identity/primary-key retrieval on `Insert`/`Merge` relies on an Oracle 12c+ implicit result set (`DBMS_SQL.RETURN_RESULT`) wrapped in an anonymous PL/SQL block, since Oracle's native `RETURNING ... INTO` binds to an output parameter that RepoDb's core execution pipeline does not read back. This should be verified against your own Oracle instance before relying on it in production.
- A `RETURNING` clause on `MERGE` specifically is only supported starting with **Oracle Database 23ai** - it does not work on 12c/18c/19c/21c at all (fails with `ORA-00933`). This provider otherwise targets 12c+, but `Merge` against a table with a primary/identity key requires 23ai+ to get the key value back. On older versions, `Insert`/`Update`/`Query`/etc. are unaffected - only identity-returning `Merge` calls are impacted.
- Oracle has no native GUID/`UNIQUEIDENTIFIER` type. A `System.Guid` data entity property will throw `ArgumentException: Value does not fall within the expected range.` from `OracleParameter.Value` if bound directly, because (unlike `SqlParameter`/`NpgsqlParameter`) ODP.NET does not accept a raw `Guid` value. If a column stores a GUID as `RAW(16)`, map it as `byte[]` on the entity, or keep it as `Guid` and register `RepoDb.Oracle.PropertyHandlers.GuidToByteArrayPropertyHandler` for that specific property:
  ```csharp
  PropertyHandlerMapper.Add<YourEntity, GuidToByteArrayPropertyHandler>(
      e => e.YourGuidProperty, new GuidToByteArrayPropertyHandler(), true);
  ```
  Register it per-property (not globally for `typeof(Guid)`) if your process also uses another RepoDb provider that handles `Guid` natively, since a type-level `PropertyHandlerMapper` registration applies process-wide across all connections.
