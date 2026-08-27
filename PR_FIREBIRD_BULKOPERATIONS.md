## Summary

Adds `RepoDb.Firebird.BulkOperations` — a new extension package that brings `BulkInsert`, `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` to the Firebird provider, matching the shape and conventions of RepoDB's other bulk-operations packages (SQL Server, Oracle, DB2, MySQL, PostgreSQL).

Firebird's ADO.NET provider (`FirebirdSql.Data.FirebirdClient`) has no `SqlBulkCopy`-equivalent bulk-copy class, so this package is built on `FbBatchCommand` instead — a single prepared, parameterized statement executed once per batch (one or more round trips depending on row count), wrapped behind a `SqlBulkCopy`-shaped façade (`FirebirdCommandBatcher` with `DestinationTableName`, `ColumnMappings`, `WriteToServer`/`WriteToServerAsync`) so it feels familiar to anyone who has used the other providers' bulk packages.

Ref: #1243

## What's included

- **New package**: `RepoDb.Extensions/RepoDb.Firebird.BulkOperations/RepoDb.Firebird.BulkOperations` — targets net8.0/net9.0/net10.0, with its own `.sln`, `README.md` (usage + known limitations), and `Trace` support (`FirebirdTraceKeys`).
- **`FirebirdCommandBatcher`** — the `SqlBulkCopy`-style wrapper around `FbBatchCommand`/`FbBatchParameterCollection`, with `FirebirdCommandBatcherMapItem` (renamed from an earlier `FirebirdBulkInsertMapItem`) and `FirebirdCommandBatcherColumnMappingCollection` for column mapping.
- **All five operations**, each with sync/async and entity/table-name/`DataTable`/`IDataReader` overloads, plus `FirebirdConnection`, `BaseRepository`, and `DbRepository` tiers:
  - `BulkInsert` — including `identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity` to read back server-generated identities.
  - `BulkMerge` — upsert by qualifiers (defaults to the primary/identity key), with `ReturnIdentity` support.
  - `BulkUpdate` — update-only by qualifiers.
  - `BulkDelete` — delete by qualifiers, driven by entities/`DataTable`.
  - `BulkDeleteByKey` — delete by a bare list of primary/identity key values, no entities needed.
- **Pseudo (staging) table pipeline**: every operation except a plain no-return-identity `BulkInsert` stages its rows in a short-lived, per-call uniquely-named pseudo table (`FirebirdBulkImportPseudoTableType.Physical` or a genuine `GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS` for `Memory`) before applying them to the real table — this avoids the shared/deterministic staging-table-name collisions some other providers' bulk packages are prone to under concurrent callers.
- **`Expression<Func<TEntity, object>>` qualifiers overloads** added across `FirebirdConnection`/`BaseRepository`/`DbRepository` for `BulkMerge`, `BulkUpdate`, and `BulkDelete`, matching the qualifiers convention already established by the Oracle/DB2/PostgreSQL bulk packages.
- **`RepoDb.Docs/limitations.md`** — new "Firebird" section documenting provider-level caveats (batching behavior, `IN (...)` limits, merge semantics around identity-as-qualifier, timestamp precision, etc.).
- **Full integration test suite** — 590 `[TestMethod]`s, ported 1:1 from `RepoDb.Oracle.BulkOperations`'s test suite and adapted to Firebird (models, `Helper.cs` generators, `Setup/Database.cs` DDL), covering every operation across entity/anonymous-object/`ExpandoObject`/mapped/with-extra-fields variants, sync and async, single and batched, identity and non-identity tables.

## Design notes / departures from the DB2 bulk package

This package started as a port of `RepoDb.Db2.BulkOperations` (per the original ask), refactored end-to-end for Firebird rather than left as a reskin:

- DB2's `DB2BulkCopy`/`Db2BulkArrayBinder` (positional `?` parameters, CTAS-based pseudo tables, deterministic pseudo-table names) is replaced by `FirebirdCommandBatcher` over `FbBatchCommand`, real `CREATE TABLE`-based (or `GLOBAL TEMPORARY TABLE`) pseudo tables with GUID-suffixed names, and Firebird-native SQL (`MERGE INTO ... USING ... ON ...`, `UPDATE OR INSERT ... MATCHING (...)`, `EXECUTE BLOCK ... RETURNS (...) ... SUSPEND` for multi-row identity/count round-trips, since Firebird's `RETURNING` — like Oracle's — is single-row only).
- Multi-row generated-identity retrieval and non-trivial merge/upsert row counts are read back through an `EXECUTE BLOCK` loop rather than relying on `ExecuteNonQuery`'s records-affected count, since Firebird's engine does not report a reliable row count for `EXECUTE BLOCK` or native `MERGE` statements.
- Row order through the pseudo table is guaranteed by a client-assigned row-order column rather than depending on server-assigned ordering, so return-identity correlation is correct even when rows aren't written back in input order.

## Testing

- `dotnet build RepoDb.Firebird.BulkOperations.sln` across net8.0/net9.0/net10.0 — clean build, no warnings.
- Full 590-test integration suite run against a live Firebird 3.0+ instance; failures found during that process were root-caused and fixed rather than worked around (parameter/identifier quoting, reader-disposal-vs-`DROP TABLE` ordering, pseudo-table row-order defaults, identity-column exclusion across all `BulkInsert` code paths, `DataTable` column typing for `Guid`-mapped columns, `Assert.Throws` expectations updated to match the Firebird driver's actual client-side type-coercion behavior, and a `NullReferenceException` guard for return-identity operations against read-only/anonymous-typed entities).

## Known limitations

See the new "Known limitations" section in the package's own `README.md` and in `RepoDb.Docs/limitations.md`'s "Firebird" section — notably: no `bulkCopyOptions` equivalent (no concept in `FbBatchCommand`), and `BulkMerge` where the identity column is itself a merge qualifier compiles to a per-row branching `EXECUTE BLOCK` rather than a single set-based statement.

## Checklist

- [x] Builds cleanly across all target frameworks (net8.0/net9.0/net10.0)
- [x] Integration tests added/ported and passing against a live Firebird database
- [x] Documentation updated (`README.md`, `RepoDb.Docs/limitations.md`)
- [x] Follows existing bulk-operations package conventions (naming, tiers, qualifiers overloads)
