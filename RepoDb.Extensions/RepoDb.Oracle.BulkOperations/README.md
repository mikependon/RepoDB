[![OracleBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-oracle-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-oracle-bulk.yml)
[![OracleBulkHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![OracleBulkVersion](https://img.shields.io/nuget/v/repodb.oracle.bulkoperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations)

# [RepoDb.Oracle.BulkOperations](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations)

High-performance bulk operations for RepoDB on Oracle. Uses ODP.NET array binding to transfer data in a
single round trip per operation.

> **Verification status:** this package has been implemented and reviewed but not yet exercised against a
> live Oracle instance. In particular, the array-bind `RETURNING ... INTO` identity read-back used by
> `BulkInsert` and the Global Temporary Table staging strategy used by `BulkMerge`/`BulkUpdate`/`BulkDelete`
> should be verified end-to-end before relying on this package in production. This mirrors the same
> caveat already called out on `OracleStatementBuilder`'s `DBMS_SQL.RETURN_RESULT` identity trick in the
> core `RepoDb.Oracle` package.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Special Arguments](#special-arguments)
- [Why No Staging Table for BulkInsert](#why-no-staging-table-for-bulkinsert)
- [The Staging Table Lifecycle: Temporary vs Physical](#the-staging-table-lifecycle-temporary-vs-physical)
- [Async Methods](#async-methods)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)
- [BulkDelete](#bulkdelete)

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
Install-Package RepoDb.Oracle.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
RepoDb.OracleBootstrap.Initialize();
```

Or visit the [installation](https://repodb.net/tutorial/installation) page for more options.

## Special Arguments

**`qualifiers`** — defines the fields used in the matching criteria for `BulkMerge`, `BulkUpdate`, and
`BulkDelete`. Defaults to the primary key column.

**`mappings`** (`BulkInsert` only) — an explicit list of `OracleBulkInsertMapItem` describing which
source properties/columns map to which destination columns, and (optionally) which
`Oracle.ManagedDataAccess.Client.OracleDbType` to bind each one as. When omitted, the matching
properties/columns from the target table are used automatically, honoring each property's
`[OracleDbType]`/`[OracleDbTypeEx]` attribute exactly like the rest of this Oracle provider.

**`identityBehavior`** — controls identity handling for `BulkInsert` and `BulkMerge`:

- `Unspecified` *(default)* — the identity column is neither sent nor read back.
- `KeepIdentity` — the identity property's existing value is sent and used as-is.
- `ReturnIdentity` — the database-generated (or matched) identity value is read back and written onto
  each entity/row.

**`pseudoTableType`** (`BulkMerge`, `BulkUpdate`, `BulkDelete` only) — an `OracleBulkImportPseudoTableType`
controlling what kind of staging table backs the operation:

- `Temporary` *(default)* — a Global Temporary Table (GTT). Session-private rows, safe for concurrent
  callers writing to the same table from different connections.
- `Physical` — an ordinary heap table. No session isolation - see the caveat below before using this.

Unlike the PostgreSQL bulk package, there is no `BulkImportMergeCommandType` (Oracle has exactly one
native upsert construct, `MERGE INTO`).

## Why No Staging Table for BulkInsert

Every other provider's bulk insert loads rows into a staging table first. Oracle's `BulkInsert` skips
that step entirely: ODP.NET's array binding supports a `RETURNING <col> INTO :out` clause directly on an
array-bound `INSERT ... VALUES (...)` statement, and returns one identity value per bound row - in the
same order the rows were bound - as a single output parameter array. That gives `BulkInsert` a true
single-round-trip load with reliable per-row identity correlation, with no server-side table needed at
all.

## The Staging Table Lifecycle: Temporary vs Physical

`BulkMerge`, `BulkUpdate`, and `BulkDelete` stage rows into a per-table pseudo table before running one
set-based `MERGE INTO` / `DELETE ... WHERE EXISTS` statement against it. Oracle's `CREATE TABLE` and
`DROP TABLE` are DDL and cause an **implicit COMMIT** - so unlike PostgreSQL, which creates and drops its
pseudo table on every call, this package creates the staging table **once** per (table name, pseudo table
type) the first time it's needed in the process, and merely `DELETE`s its contents (plain DML,
transaction-safe) before every subsequent call. The `pseudoTableType` argument picks which kind of table
backs this:

- **`Temporary`** *(default)* — `CREATE GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS`. Rows are
  private to each session, so concurrent connections bulk-writing to the same target table never see or
  interfere with each other's staged data, even though they share one table definition. This is the safe
  choice for concurrent/multi-connection workloads and should be left as the default in almost all cases.
- **`Physical`** — `CREATE TABLE ... AS SELECT ...`, an ordinary heap table. It carries **no per-session
  data isolation** - every session/connection reads and writes the *same* rows. Two connections
  bulk-writing to the same target table concurrently with `Physical` will corrupt or race each other's
  staged data. Only use this for workloads where calls against the same table are known to be sequential
  (e.g. a single-threaded batch job), in exchange for avoiding whatever session-temporary-object overhead
  your Oracle environment attaches to GTTs. `Temporary` and `Physical` staging tables for the same real
  table are named distinctly, so switching between them for the same table is safe and won't collide.

**Practical implication:** the very first `BulkMerge`/`BulkUpdate`/`BulkDelete` call against a given table
(for a given `pseudoTableType`) in a process will issue a `CREATE TABLE` or `CREATE GLOBAL TEMPORARY TABLE`
statement. If that first call happens inside a transaction that already has other uncommitted work
pending, that work will be implicitly committed at that point. Consider "warming up" the staging table for
tables you'll bulk-write to (e.g. with a throwaway call at application startup, outside of any transaction
you care about) if this matters for your workload.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: BulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined
qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

`BulkMerge` never uses Oracle's `RETURNING` clause on the `MERGE` statement itself (that's only supported
starting with Oracle Database 23ai). When `identityBehavior: BulkImportIdentityBehavior.ReturnIdentity` is
requested, a second, version-independent query correlates the staged rows back to the real table by the
same qualifiers immediately after the `MERGE` completes.

`BulkMerge`, `BulkUpdate`, and `BulkDelete` also accept `pseudoTableType` (see
[Special Arguments](#special-arguments) and
[The Staging Table Lifecycle](#the-staging-table-lifecycle-temporary-vs-physical)) to pick between a
session-isolated Global Temporary Table (the default) and a shared physical table:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    // Only safe for sequential, single-threaded workloads against this table - see the caveat above.
    var mergedRows = connection.BulkMerge<Customer>(customers, pseudoTableType: OracleBulkImportPseudoTableType.Physical);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of
updated rows.

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new OracleConnection(ConnectionString))
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
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new OracleConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

`BulkDelete` only ever stages the qualifier columns (not the whole row) - it's the lightest of the four
operations. It accepts `pseudoTableType` the same way `BulkMerge` does.
