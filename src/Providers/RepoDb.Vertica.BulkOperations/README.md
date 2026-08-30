[![VerticaBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-vertica-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-vertica-bulk.yml)
[![VerticaBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![VerticaBulkVersion](https://img.shields.io/nuget/v/repodb.vertica.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Vertica.BulkOperations)

# [RepoDb.Vertica.BulkOperations](https://www.nuget.org/packages/RepoDb.Vertica.BulkOperations)

An extension library of RepoDB that does bulk operations towards a Vertica database. It uses `Vertica.Data`'s native `VerticaCopyStream` - Vertica's own `COPY ... FROM STDIN` streaming bulk-load primitive, the same category as `SqlBulkCopy`/`NpgsqlBinaryImporter` - wrapped behind a `SqlBulkCopy`-shaped façade (`VerticaBulkCopy.DestinationTableName`, `ColumnMappings`, `WriteToServer`/`WriteToServerAsync`) kept consistent with the rest of RepoDB's bulk-operations packages.

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

- `BulkInsert` stages rows through `VerticaBulkCopy`/`VerticaCopyStream` directly. Every other operation (`BulkMerge`, `BulkUpdate`, `BulkDelete`, `BulkDeleteByKey`) first bulk-loads rows into a short, per-call uniquely-named pseudo (staging) table via the same mechanism, then applies them to the real table with a single SQL statement - so concurrent callers targeting the same table never race on a shared staging-table name.
- There is no `bulkCopyOptions` parameter (unlike `SqlBulkCopy`/`DB2BulkCopy`-based packages) - `VerticaCopyStream` has no equivalent concept; COPY-specific behavior (delimiter, NULL representation, error handling) is fixed by this package rather than caller-configurable.
- **Verification status:** this package was ported from `RepoDb.Firebird.BulkOperations` and rebuilt around Vertica's real `VerticaCopyStream` for the `BulkInsert` path, but the pseudo-table create/merge/drop SQL text (`VerticaText`/`VerticaExecution`) still carries over assumptions from Firebird's dialect (e.g. `EXECUTE BLOCK ... SUSPEND` loops for multi-row identity retrieval) that have **not** been verified against a live Vertica instance. Treat `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey` and `identityBehavior: ReturnIdentity` as unverified until exercised against a real database.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Vertica.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseVertica();
```

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: VerticaBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new VerticaConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```
