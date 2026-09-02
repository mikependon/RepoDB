# Add RepoDb.EnterpriseDb.BulkOperations

## Summary

Adds bulk operations support (`BulkInsert`, `BulkMerge`, `BulkUpdate`, `BulkDelete`, `BulkDeleteByKey`) for EnterpriseDB (EDB Postgres Advanced Server), targeting `RepoDb.Connector.EnterpriseDb` rather than the official `EnterpriseDB.EDBClient` package.

This package was scaffolded from `RepoDb.MariaDbConnector.BulkOperations` and then rewritten where the underlying database dialect diverges - it is not a rename-only port. MariaDB's pseudo-table pipeline leans on session user variables, `PREPARE`/`EXECUTE` dynamic SQL, and `AUTO_INCREMENT`; none of that exists in Postgres/EDB, which instead offers `RETURNING`, `INSERT ... ON CONFLICT DO UPDATE`, `UPDATE ... FROM`, and `DELETE ... USING` as direct, simpler replacements.

## What changed

### New package: `RepoDb.EnterpriseDb.BulkOperations`

- **`Helpers/EDBText.cs`** - rewritten SQL generator for the pseudo-table pipeline:
  - Pseudo table creation is a `CREATE TABLE ... AS SELECT ... WHERE (1 = 0)` + `ALTER TABLE ... ADD COLUMN ... BIGINT GENERATED ALWAYS AS IDENTITY` pair, since Postgres's `CREATE TABLE ... AS` can't combine an explicit extra column with a `SELECT`. Unlike MariaDB's CTAS, no `NOT NULL`/constraint carries over from the source table, so no `AllowNullForColumn` workaround is needed.
  - Insert-with-identity is a single `INSERT ... SELECT ... ORDER BY ... RETURNING` statement - Postgres generates and reports the identity value in one round trip, eliminating MariaDB's/Oracle's session-variable pre-assignment step entirely.
  - Merge is `INSERT ... SELECT ... ON CONFLICT (qualifiers) DO UPDATE SET ... RETURNING`, replacing MariaDB's two-statement anti-join technique.
  - Update/Delete use `UPDATE ... FROM` / `DELETE ... USING`, the native equivalents of MariaDB's multi-table `... INNER JOIN` syntax.
- **`Helpers/EDBExecution.cs`** - thin orchestration layer over `EDBText`; no longer needs the `GetIdentitySequenceMetadata`/`AllowNullForColumn` steps MariaDB's version required.
- **`Base/WriteToServer.cs`** - uses `EDBBulkCopy` (from `RepoDb.Connector.EnterpriseDb`, built on Npgsql's binary `COPY ... FROM STDIN` protocol) instead of `MariaDbBulkCopy`. `EDBBulkCopy` resolves column mappings by ordinal/name internally, so the `ColumnFilteredDataReader` shim MariaDB's version needed was dropped.
- Fixed a dead ternary in `ResolvePseudoTableType` that always resolved to `Physical` regardless of row count; it now correctly falls back to `Memory` (a `TEMP` table) below the row-count threshold.
- `EDBBulkInsertMapItem`, `EDBBulkImportIdentityBehavior`, `EDBBulkImportPseudoTableType`, `EDBTraceKeys`, `EDBConstants`, and the `BaseRepository`/`DbRepository`/`EDBConnection` extension wrappers - renamed and re-scoped for EnterpriseDB.

### Core `RepoDb.EnterpriseDb` changes

- Added a `ProjectReference` to `RepoDb.Connector.EnterpriseDb` (not published to NuGet - referenced locally from the sibling `RepoDB.Connectors` repository).
- `EnterpriseDbBootstrap` now registers `DbSettingMapper`/`DbHelperMapper`/`StatementBuilderMapper` for **both** the official `EnterpriseDB.EDBClient.EDBConnection` and `RepoDb.Connector.EnterpriseDb.EDBConnection` - the SQL dialect, quoting, and catalog queries are identical either way, so one `EnterpriseDbDbSetting`/`EnterpriseDbDbHelper`/`EnterpriseDbStatementBuilder` triple serves both.
- `EnterpriseDbDbHelper` no longer hard-codes `new EDBConnection(...)` against the official type in its retry-on-a-new-connection path; it now reconstructs a connection of whatever concrete type was passed in (via `Activator.CreateInstance`) and matches an "operation already in progress" exception from either driver by type name.
- `RepoDb.Core`: added `RepoDb.EnterpriseDb.BulkOperations` to the `InternalsVisibleTo` list.

### Docs / solution

- `README.md` and `RepoDb.EnterpriseDb.BulkOperations.sln`, matching the structure of the other provider bulk-operations packages.
- Integration test project (`Setup/Database.cs`, models, `Helper.cs`) adapted to native Postgres/EDB types (`UUID`, `GENERATED ALWAYS AS IDENTITY`, `TIMESTAMP(6)`, `DOUBLE PRECISION`) and connection defaults matching what `build-enterprisedb-bulk.yml` actually spins up in CI (`edb` maintenance database, port `5444`, user `enterprisedb`).

## Notable design decisions

- **Row order for `RETURNING`**: `BulkInsert`'s and `BulkMerge`'s return-identity paths both `ORDER BY` the pseudo table's surrogate row-order column in the source `SELECT`, and rely on `RETURNING` emitting rows in that scan order to match them back to the original entity list positionally. This isn't a hard SQL-standard guarantee, but holds for a single, non-parallel `INSERT ... SELECT ... RETURNING` plan - the same class of assumption already used elsewhere in this pipeline.
- **`EDBBulkCopy` for the load step**: per direction to use the `EDBBulkCopy` class already present in `RepoDb.Connector.EnterpriseDb`, which does the actual staging-table load via Npgsql's native binary `COPY` protocol.

## Testing

- `dotnet build` succeeds with zero errors/warnings for `RepoDb.EnterpriseDb.BulkOperations`, its `IntegrationTests` project, and the touched core `RepoDb.EnterpriseDb`/`RepoDb.Core` projects, across `net8.0`/`net9.0`/`net10.0`.
- The original `RepoDb.MariaDbConnector.BulkOperations` source this was scaffolded from still builds unmodified - no regression.
- **Not verified**: runtime behavior against a live EDB Postgres Advanced Server instance. No such instance was available during development, so the generated SQL, `EDBBulkCopy`'s binary-`COPY` type inference (e.g. `byte` → `smallint`, decimal/timestamp precision), and the `RETURNING` row-order assumption above are unexercised. Recommend a full integration test run before merging.
