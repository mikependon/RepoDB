[![FirebirdBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-firebird-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-firebird-bulk.yml)
[![FirebirdBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![FirebirdBulkVersion](https://img.shields.io/nuget/v/repodb.firebird.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Firebird.BulkOperations)

# [RepoDb.Firebird.BulkOperations](https://www.nuget.org/packages/RepoDb.Firebird.BulkOperations)

An extension library of RepoDB that does bulk operations towards a Firebird database. Firebird's ADO.NET provider has no `SqlBulkCopy`-equivalent bulk-copy class, so this uses `FbBatchCommand` - a single parameterized statement executed once per row through one round trip (or one round trip per batch, for very large row counts) - kept API-shaped like `SqlBulkCopy` (`FirebirdCommandBatcher.DestinationTableName`, `ColumnMappings`, `WriteToServer`/`WriteToServerAsync`).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Async Methods](#async-methods)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)
- [BulkDelete](#bulkdelete)
- [BulkDeleteByKey](#bulkdeletebykey)

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Known limitations

- Every bulk operation except a plain (no-return-identity) `BulkInsert` stages rows through a pseudo (staging) table first. Each pseudo table gets a short, per-call unique name, so - unlike some other providers' bulk-operations packages - concurrent callers targeting the same table never race on a shared staging-table name, for either `pseudoTableType: Physical` or `Memory` (a genuine Firebird `GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS`).
- `BulkMerge`/`BulkInsert` with `identityBehavior: ReturnIdentity` read generated identities back via an `EXECUTE BLOCK ... SUSPEND` loop over the pseudo table - Firebird's `RETURNING` clause, like Oracle's, only ever returns a single row, so a multi-row result set needs this "loop and yield" shape. Row order is guaranteed by a client-assigned row-order column (not a server-generated one), so correctness never depends on rows being written back in input order.
- `BulkMerge` where the identity column is also a merge qualifier (the common default case) compiles to a per-row branching `EXECUTE BLOCK`, for the same reason documented for the core `RepoDb.Firebird` package's own `Merge` - see its README's "Known limitations" section.
- There is no `bulkCopyOptions` parameter (unlike `SqlBulkCopy`/`DB2BulkCopy`-based packages) - `FbBatchCommand` has no equivalent concept.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Firebird.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseFirebird();
```

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: FirebirdBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new FbConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```
