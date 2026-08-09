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

- [Net.IBM.Data.Db2](https://www.nuget.org/packages/Net.IBM.Data.Db2/) — IBM's Data Server .NET provider for Db2. IBM ships this as separate, platform-specific packages rather than one cross-platform package: `Net.IBM.Data.Db2` (Windows x64), `Net.IBM.Data.Db2-lnx` (Linux AMD64), and others (`-osx`, `-zlnx`, `-ppc`) not referenced by this project. The published `RepoDb.Db2` package currently depends on the Windows package (`RepoDb.Db2.csproj` selects it via an `$(OS)`-conditional `PackageReference`, resolved when the package is built/packed) — if you consume `RepoDb.Db2` on Linux, add a direct `PackageReference` to `Net.IBM.Data.Db2-lnx` yourself.
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

Db2's IBM Data Server provider accepts more than one SQL statement in a single command text and executes them all in one round trip (confirmed against a live Db2 LUW instance - `IDbSetting.IsMultiStatementExecutable = true` for `RepoDb.Db2`), so `QueryMultiple` runs as a single combined command, same as SQL Server/MySQL/PostgreSQL - no special handling needed on your part, and no per-type round-trip cost to plan around.

`RepoDb.Core` builds each target type's `SELECT` independently and joins them into one command text using `BaseDbSetting.QueryMultipleSeparator` (a new, additive property - every other provider keeps the default `" "`, since their `CreateQuery` already self-terminates each statement with `" ;"`). Unlike those providers, `Db2StatementBuilder.CreateQuery` never appends a trailing `;` (a lone trailing statement terminator is a Db2 syntax error - see "Identity/primary-key retrieval" below), so `Db2DbSetting.QueryMultipleSeparator` is set to `"; "` instead, giving `"SELECT ... T1; SELECT ... T2"` - an interior separator with nothing dangling at the end, the same pattern already confirmed working for `Merge`'s follow-up `SELECT` and in `ExecuteQueryMultipleTest.cs`. Confirmed live to fix the SQL0104N that a plain space-joined command text previously produced.

## Known limitations (v1)

### `InsertAll` / `MergeAll` / `UpdateAll` batching

All three now batch multiple entities into a single round trip (`IsMultiStatementExecutable = true`), instead of one round trip per row:

* **`InsertAll`** generates a genuine multi-row `INSERT INTO t (...) VALUES (row0), (row1), ...` statement, wrapped the same way as a single-row `Insert` (`SELECT <key> FROM FINAL TABLE (...)`) when the generated key is needed. **This relies on an assumption not verified against a live Db2 instance**: that `FINAL TABLE`'s result rows come back in the same order as the source `VALUES` list, the same way `RepoDb.PostgreSql`'s `CreateInsertAll` in this same solution trusts a multi-row `RETURNING` to preserve `VALUES` order. If that assumption is ever wrong for Db2, a generated identity value would be paired with the wrong entity. Verify `InsertAll` calls that request the generated key thoroughly against your own Db2 instance before relying on this in production - `RepoDb.Db2.IntegrationTests`' `InsertAllTest`/`EnumTests` exercise exactly this path (matching each queried-back row to its entity by the `Id` `InsertAll` set).

* **`UpdateAll`** generates `<batchSize>` concatenated `UPDATE ... WHERE ... ;` statements (one per row) executed via `ExecuteNonQuery()`. There's no generated value to read back here (only an aggregate affected-row count), so there's no row-correlation concern - lower risk than `InsertAll`/`MergeAll`.

* **`MergeAll`** extends the single-row `Merge`'s `USING` clause to a multi-row source via `UNION ALL`, so one `MERGE` statement matches/inserts every row in the batch. The follow-up key-retrieval `SELECT` (see "Identity/primary-key retrieval" below) re-queries each row independently by its own qualifier value and explicitly echoes back a caller-bound order/index parameter as a second result column, so `RepoDb.Core`'s row-to-entity correlation doesn't depend on Db2 returning rows in any particular order for that part. **This only works when every row is deterministically re-findable by its own qualifier value after the `MERGE` runs** - which breaks down specifically when the identity column is (part of) the qualifier set, since a freshly-inserted row's caller-bound qualifier value is a placeholder, not the real generated identity, and there's no safe way to compute per-row fallback values for a batch that may mix matched and unmatched rows (unlike the single-row case's `MAX(<key>)` fallback - one row, one fallback value; a batch could need many, and which rows need one isn't known until the `MERGE` actually runs). `Db2StatementBuilder.CreateMergeAll` throws `NotSupportedException` for a `batchSize` greater than 1 in that situation instead of silently mis-attributing generated identities to the wrong entities - pass an explicit non-identity qualifier (a natural key), or call `MergeAll` with `batchSize: 1`, to work around it.

### Identity/primary-key retrieval

`Insert` reads back a generated key using `SELECT ... FROM FINAL TABLE (INSERT INTO ... VALUES (...))` — an ANSI-SQL-adjacent construct that returns the post-insert row (including any identity-generated column) as an ordinary result set, with no PL/SQL block, output parameter, or cursor plumbing required. Confirmed working against a live Db2 LUW instance.

`Merge` cannot use the same construct — Db2 LUW's `MERGE` statement does not support `FINAL TABLE` (confirmed against a live instance: it fails with `SQL0104N`, and Db2 LUW's official `MERGE` reference has no `FINAL TABLE` mention at all). Instead, `Merge`/`MergeAll` append a follow-up `SELECT COALESCE((SELECT <key> FROM <table> WHERE <qualifier predicate>), (SELECT MAX(<key>) FROM <table>))` statement to the same command text: it re-queries by the same qualifier predicate the `MERGE`'s own `ON` clause used, and falls back to re-reading `MAX(<key>)` off the table for the one case that predicate can't cover — a qualifier that is itself the identity column, on a row that was just inserted (so the caller's bound value for it doesn't match the newly generated one). This relies on the IBM Data Server .NET Provider executing multiple statements in one command text in a single round trip, which is confirmed for this read-only case even though `IDbSetting.IsMultiStatementExecutable` is `false` for `RepoDb.Db2` — that flag only governs whether RepoDb.Core batches *multiple entities* into one round trip (see `InsertAll`/`MergeAll` above), not whether a single statement builder call may itself return multi-statement SQL text.

An earlier revision of this fallback used `IDENTITY_VAL_LOCAL()` instead of `MAX(<key>)`, on the assumption that packing the `MERGE` and the follow-up `SELECT` into one command text/round trip would keep them in the same unit of work. Confirmed live, that assumption was wrong: `IDENTITY_VAL_LOCAL()` returns NULL once a COMMIT has occurred since the identity value was generated, and Db2 LUW's autocommit fires after *each individual statement*, not after the whole command text — so the register was already cleared by the time the follow-up `SELECT` ran, and every fresh `MergeAll` insert came back with `Id == 0`. Re-reading `MAX(<key>)` sidesteps this since it reads ordinary committed table data rather than a connection-scoped register, at the cost of being a best-effort read, not a guarantee — it assumes no other connection concurrently inserts into the same table between the `MERGE` and this `SELECT`.

An earlier revision of this provider wrapped the key column in an Oracle-style `DECLARE ... DBMS_SQL.RETURN_RESULT(...)` PL/SQL block, which doesn't exist in Db2. Verify `Insert`/`Merge` calls that request the generated key against your own Db2 instance before relying on this in production.

`InsertAll` and `MergeAll` extend these same two mechanisms (`FINAL TABLE` and the qualifier-based follow-up `SELECT`, respectively) to a multi-row batch - see "`InsertAll` / `MergeAll` / `UpdateAll` batching" above for the mechanism and its caveats.

### `Merge`/`MergeAll` against LOB columns (`CLOB`/`DBCLOB`/`BLOB`) and XML columns

Not exercised by this repo's test suite. `CompleteTable`/`NonIdentityCompleteTable` originally included `ColumnClob` (`CLOB(1M)`), `ColumnNClob` (`DBCLOB(1M)`), `ColumnBlob` (`BLOB(1M)`), and `ColumnXml` (`XML`), but all four were removed from the fixtures - `Insert`/`Update`/`Query` against columns of these types are unaffected by any of this and still work the same way they always did for any `RepoDb.Db2` consumer with columns of their own; only `Merge`/`MergeAll` are implicated, for two unrelated reasons:

* **Temp space.** A Db2 `MERGE` has to flow two full sets of columns through system temporary space at once — one for the row it might update, one for the row it might insert. Against a table with several LOB columns, that's wide enough to exceed the default 4K-page system temporary tablespace most Db2 installs (including the community Docker image) provision out of the box, failing with `SQL1585N`. This isn't something `RepoDb.Db2` can work around in the generated SQL — it's inherent to how Db2 LUW processes `MERGE` against wide/LOB-bearing tables. If you hit this with your own LOB-bearing table, provision a system temporary tablespace at a large enough page size (32K, Db2's maximum, is the safest choice):

  ```sql
  CREATE BUFFERPOOL BP32K SIZE 1000 PAGESIZE 32768;
  CREATE SYSTEM TEMPORARY TABLESPACE TMPSPACE32K PAGESIZE 32768 MANAGED BY AUTOMATIC STORAGE BUFFERPOOL BP32K;
  ```

  There's nothing in the library itself that provisions this automatically - it's a one-time, per-database setup step, not something a statement builder can (or should) do per-call. (An earlier attempt to provision it automatically via the community Db2 image's `/var/custom` container-startup-script hook made the container fail to start entirely, so that approach was abandoned.)

* **XML specifically is unsupported, full stop**, independent of the temp space issue above. Db2's `MERGE` requires every source value to flow through a derived table (`USING (SELECT :Field1 AS "Field1", ...) S`) before it's referenced as `S.Field1` in the `WHEN MATCHED`/`WHEN NOT MATCHED` clauses. A plain `INSERT`/`UPDATE` implicitly runs `XMLPARSE(DOCUMENT ...)` when a string is assigned directly to an XML column, but that implicit conversion doesn't reach a value merely selected into this derived table - confirmed live, it fails with `SQL0301N` ("... cannot be used because of its data type") regardless of whether the parameter is left bare or explicitly `CAST`. An explicit `XMLPARSE(DOCUMENT CAST(:Field AS VARCHAR(32672)))` does work around it, but doing so correctly requires the statement builder to know a given field targets an XML column specifically (a data entity property mapped to a Db2 `VARCHAR` column and one mapped to an `XML` column are both just CLR `string`, indistinguishable from `RepoDb.Field.Type` alone) - which was judged not worth the added complexity for this provider. Don't include an XML-mapped field in a `Merge`/`MergeAll` call.

### GUID/UNIQUEIDENTIFIER

Db2 has no native GUID/`UNIQUEIDENTIFIER` type. A `System.Guid` data entity property cannot be bound directly to a `DB2Parameter` the way it can with `SqlParameter`/`NpgsqlParameter`. The idiomatic Db2 storage for a GUID is a fixed-length 16-byte `CHAR(16) FOR BIT DATA` column — map it as `byte[]` on the entity, or keep it as `Guid` and register `RepoDb.Db2.PropertyHandlers.Db2GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, Db2GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new Db2GuidToByteArrayPropertyHandler(), true);
```

Register it per-property (not globally for `typeof(Guid)`) if your process also uses another RepoDb provider that handles `Guid` natively, since a type-level `PropertyHandlerMapper` registration applies process-wide across all connections.
