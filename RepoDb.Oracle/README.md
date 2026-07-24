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
