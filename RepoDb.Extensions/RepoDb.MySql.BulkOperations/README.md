[![MySqlBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql-bulk.yml)
[![MySqlBulkHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![MySqlBulkVersion](https://img.shields.io/nuget/v/repodb.mysql.bulkoperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations)

# [RepoDb.MySql.BulkOperations](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations)

High-performance bulk operations for RepoDB on MySql. Row loading goes through this package's own internal
`MySqlBulkCopy` class - a `LOAD DATA LOCAL INFILE`-based stand-in built on top of `MySql.Data`'s
`MySqlBulkLoader`, since `MySql.Data` ships no genuine streaming bulk-copy API of its own. Every bulk
operation in this package - including the generated-identity read-back for `BulkInsert`/`BulkMerge` - goes
through this same class; there's no separate array-bind fallback and no dependency on the third-party
`MySqlConnector` package or its own `MySqlBulkCopy` type.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Special Arguments](#special-arguments)
- [How Rows Are Loaded: MySqlBulkCopy and the Transaction Boundary](#how-rows-are-loaded-mysqlbulkcopy-and-the-transaction-boundary)
- [The Staging Table Lifecycle: Auto, Memory, and Physical](#the-staging-table-lifecycle-auto-memory-and-physical)
- [Async Methods](#async-methods)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)
- [BulkDelete](#bulkdelete)
- [BulkDeleteByKey](#bulkdeletebykey)

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.MySql.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
RepoDb.MySqlBootstrap.Initialize();
```

The connection string needs `AllowLoadLocalInfile=True;AllowUserVariables=True;` - the former lets the
client send `LOAD DATA LOCAL INFILE`, which this package's internal `MySqlBulkCopy` uses for every row-load,
and the latter lets the staging-table SQL use session user variables (`SET @repodb_...`) and
`PREPARE`/`EXECUTE` for its identity pre-assignment and nullability-toggling steps. The server also needs its
`local_infile` global variable turned on (`SET GLOBAL local_infile = 1;`, requires
`SUPER`/`SYSTEM_VARIABLES_ADMIN`) - it's off by default.

Or visit the [installation](https://repodb.net/tutorial/installation) page for more options.

## Special Arguments

**`qualifiers`** — defines the fields used in the matching criteria for `BulkMerge`, `BulkUpdate`, and
`BulkDelete`. Defaults to the primary key column.

**`mappings`** (`BulkInsert`, `BulkMerge`, `BulkUpdate`) — an explicit list of `MySqlBulkInsertMapItem`
describing which source properties/columns map to which destination columns. When omitted, the matching
properties/columns from the target table are used automatically. Each mapping can optionally carry a
`MySql.Data.MySqlClient.MySqlDbType` override, but this package's internal `MySqlBulkCopy` (see
[How Rows Are Loaded](#how-rows-are-loaded-mysqlbulkcopy-and-the-transaction-boundary)) has no per-column
type slot to feed it into - it infers each field's on-the-wire representation from the value's own CLR type
when serializing rows to the `LOAD DATA LOCAL INFILE` temp file, so the override currently has no effect.

**`identityBehavior`** — controls identity handling for `BulkInsert` and `BulkMerge`:

- `KeepIdentity` *(default)* — the identity column is left out of the bulk-loaded row set entirely, so
  MySQL's own `AUTO_INCREMENT` assigns each row's value as usual.
- `ReturnIdentity` — the row load is redirected into a staging pseudo table instead of the real table, an
  identity value is pre-assigned to each staged row (see
  [The Staging Table Lifecycle](#the-staging-table-lifecycle-auto-memory-and-physical)), then copied into
  the real table and read back onto each entity/row - in the original bulk-load order - via a follow-up
  `SELECT`.

**`pseudoTableType`** (`BulkMerge`, `BulkUpdate`, `BulkDelete`, `BulkDeleteByKey`, and `BulkInsert` with
`ReturnIdentity`) — a `MySqlBulkImportPseudoTableType` controlling what kind of staging table backs the
operation:

- `Auto` *(default)* — picks `Physical` when the entity/row count being bulk-written is 5,000 or more,
  otherwise `Memory`.
- `Memory` — a MySQL `TEMPORARY TABLE`. Session-private rows, safe for concurrent callers writing to the
  same table from different connections.
- `Physical` — an ordinary persistent table. No session isolation - see the caveat below before using this.

> **Currently, every value above resolves to `Physical` at runtime**, including `Memory` and `Auto`'s
> row-count threshold. See [The Staging Table Lifecycle](#the-staging-table-lifecycle-auto-memory-and-physical)
> for why.

MySQL has no native `MERGE` statement, so unlike the PostgreSQL or SQL Server bulk packages there is no
`BulkImportMergeCommandType` to pick between alternate upsert strategies here. `BulkMerge` always performs
the same two-statement translation: an `UPDATE ... INNER JOIN` against the rows that match on `qualifiers`,
followed by an `INSERT ... SELECT` guarded by a `LEFT JOIN ... WHERE ... IS NULL` anti-join for the rows
that don't.

## How Rows Are Loaded: MySqlBulkCopy and the Transaction Boundary

Every bulk operation in this package moves rows through exactly one mechanism: this package's own internal
`MySqlBulkCopy` class (`Helpers/MySqlBulkCopy.cs`) - not a type from the third-party `MySqlConnector` NuGet
package, and not something `MySql.Data` ships itself (it has no class of that name). `MySql.Data`'s only
genuine bulk-load primitive is `MySqlBulkLoader`, which can only load from a file via
`LOAD DATA [LOCAL] INFILE` - unlike `SqlBulkCopy`, it has no reader-streaming `WriteToServer(IDataReader)`
overload. So this package's `MySqlBulkCopy` first serializes whatever rows it's given (entities, a
`DataTable`, or a reader) to a temporary tab-delimited file, hands that file to `MySqlBulkLoader`, then
deletes it once the load completes.

This is the *only* row-load path in the package - a plain `BulkInsert` writes straight to the destination
table with it; `BulkInsert` with `ReturnIdentity` and every `BulkMerge`/`BulkUpdate`/`BulkDelete` call route
their rows through it into a staging pseudo table first (see
[The Staging Table Lifecycle](#the-staging-table-lifecycle-auto-memory-and-physical)). There is no separate
array-bind or parameterized fallback for any scenario, including returning generated identities - those are
read back with a follow-up `SELECT` against the staging table instead (see `identityBehavior` above).

**The transaction boundary.** This package's `MySqlBulkCopy` is constructed from a bare `MySqlConnection`
and never receives a `MySqlTransaction`, and it issues `LOAD DATA LOCAL INFILE` directly against that
connection rather than through a `MySqlCommand` enlisted in your transaction. Whether that means a
rolled-back transaction leaves already-loaded rows behind has not been verified against a live server (see
the verification-status note at the top of this document) - treat it as unconfirmed until you've checked the
behavior for your MySQL version and storage engine.

## The Staging Table Lifecycle: Auto, Memory, and Physical

`BulkMerge`, `BulkUpdate`, `BulkDelete`, `BulkDeleteByKey`, and `BulkInsert` with `ReturnIdentity` stage rows
into a per-call pseudo table before running a set-based statement against it. Every call - not just the
first one for a given table - issues a fresh `DROP TABLE IF EXISTS` followed by `CREATE TABLE ... AS SELECT ... WHERE (1 = 0)`
(or `CREATE TEMPORARY TABLE ...` for `Memory`) to (re)create the pseudo table, shaped after the real table's
columns, plus one extra surrogate column - `__RepoDbBulkRowOrder__ BIGINT AUTO_INCREMENT PRIMARY KEY` - that
gives the staged rows a deterministic order to read back in. The pseudo table is dropped again once the
operation finishes.

Because `CREATE TABLE`/`DROP TABLE` are DDL, and DDL causes an **implicit COMMIT** in MySQL, **every**
`BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey`/`BulkInsert`-with-`ReturnIdentity` call implicitly commits any other
uncommitted work already pending on that connection - both when the pseudo table is (re)created at the start
of the call and again when it's dropped at the end. This happens on every call, not just the first one for a
table. Keep this in mind if you're bulk-writing inside a larger transaction alongside other statements.

The `pseudoTableType` argument picks which kind of table backs this:

- **`Auto`** *(default)* — resolves to `Physical` when the number of entities/rows being bulk-written is
  5,000 or more, otherwise resolves to `Memory`.
- **`Memory`** — `CREATE TEMPORARY TABLE`. Rows are private to each session, so concurrent connections
  bulk-writing to the same target table never see or interfere with each other's staged data, even though
  they share one table definition. This is the safe choice for concurrent/multi-connection workloads.
- **`Physical`** — `CREATE TABLE ... AS SELECT ...`, an ordinary persistent table. It carries **no
  per-session data isolation** - every session/connection reads and writes the *same* rows. Two connections
  bulk-writing to the same target table concurrently with `Physical` will corrupt or race each other's
  staged data. Only use this for workloads where calls against the same table are known to be sequential
  (e.g. a single-threaded batch job). `Memory` and `Physical` staging tables for the same real table are
  named distinctly, so switching between them (directly or via `Auto`) for the same table is safe and won't
  collide.

**`Memory` is currently not reachable - every pseudo table is `Physical` for now, regardless of what you
pass.** The `TEMPORARY TABLE` branch is fully implemented in the SQL builder, but the code that resolves
`pseudoTableType` before it gets there (`ResolvePseudoTableType` in `Base/WriteToServer.cs`) currently maps
every input - including an explicit `Memory` and `Auto`'s row-count threshold - to `Physical`
unconditionally, until that path has been enabled and verified against a live server. This means the
concurrency caveat for `Physical` above currently applies unconditionally, not just when you explicitly
request it.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: MySqlBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined
qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

When `identityBehavior: MySqlBulkImportIdentityBehavior.ReturnIdentity` is requested, a matched row keeps
its existing identity value and an unmatched row gets a freshly pre-assigned one - both are read back onto
the corresponding entity/row via the same staging-table `SELECT` described in
[Special Arguments](#special-arguments).

`BulkMerge`, `BulkUpdate`, and `BulkDelete` also accept `pseudoTableType` (see
[Special Arguments](#special-arguments) and
[The Staging Table Lifecycle](#the-staging-table-lifecycle-auto-memory-and-physical)) to pick between
auto-selection (the default), a session-isolated temporary table, and a shared physical table:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    // Only safe for sequential, single-threaded workloads against this table - see the caveat above.
    var mergedRows = connection.BulkMerge<Customer>(customers, pseudoTableType: MySqlBulkImportPseudoTableType.Physical);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of
updated rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

`BulkUpdate` has no identity-related arguments - like the PostgreSQL bulk package, this operation never
generates or reports back identity values. It accepts `pseudoTableType` the same way `BulkMerge` does.

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of
deleted rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

`BulkDelete` only ever stages the qualifier columns (not the whole row) - it's the lightest of the
entity/`DataTable`-based operations. It accepts `pseudoTableType` the same way `BulkMerge` does. When you
only have the primary key values on hand (no entities or `DataTable`), use
[BulkDeleteByKey](#bulkdeletebykey) instead.

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone -
no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted
rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```

`BulkDeleteByKey` stages only the key column - the same one `qualifiers` would default to for `BulkDelete`
- into its own pseudo table (named distinctly from `BulkDelete`'s, so the two never collide even against the
same real table). It has no `qualifiers` argument of its own, since the key values themselves are the match
criteria; it accepts `pseudoTableType` the same way `BulkMerge` does.
