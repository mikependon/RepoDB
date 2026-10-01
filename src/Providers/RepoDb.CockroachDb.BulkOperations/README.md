<div align="center">
    <a href="https://repodb.net/tutorial/get-started-cockroachdb/"><image src="CockroachDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-cockroachdb/"><strong>RepoDb.CockroachDb.BulkOperations</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for bulk operations on CockroachDB in .NET.</span>
</div>

-----

<br/>

[![CockroachDbBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-cockroachdb-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-cockroachdb-bulk.yml)
[![CockroachDbBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![CockroachDbBulkVersion](https://img.shields.io/nuget/v/repodb.cockroachdb.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.CockroachDb.BulkOperations)

# [RepoDb.CockroachDb.BulkOperations](https://www.nuget.org/packages/RepoDb.CockroachDb.BulkOperations)

A high-performant extension library of RepoDB that does bulk operations towards a CockroachDB database. It uses the `CockroachDbBulkCopy` class of [RepoDb.Connector.CockroachDb](https://www.nuget.org/packages/RepoDb.Connector.CockroachDb) (Npgsql binary `COPY`) to load the data into the database.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.
- [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#cockroachdb) — CockroachDB-specific caveats, including bulk staging tables.

## Core Features

- [Async Methods](#async-methods)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)
- [BulkDelete](#bulkdelete)
- [BulkDeleteByKey](#bulkdeletebykey)
- [Column Mappings Behavior](#column-mappings-behavior)

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.CockroachDb.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseCockroachDb();
```

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: CockroachDbBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new CockroachDbConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045L, 10046L, 10047L };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```

## Column Mappings Behavior

When no `mappings` are passed to `BulkInsert`, `BulkMerge`, or `BulkUpdate` (or their async variants), and for `BulkDelete` (which has no `mappings` argument), the columns of the source (entities, `DataTable`, or `DbDataReader`) are aligned with the columns of the destination table based on the `BulkColumnMappingsBehavior` setting:

| Value | Column names must match | Column order must match | Destination columns missing from the source | Source columns missing from the destination |
|---|---|---|---|---|
| `Automatic` (default) | Only the matching ones | No | Bypassed | Ignored |
| `Strict` | Yes | Yes | Throws | Throws |
| `StrictBypass` | Yes | No | Bypassed | Throws |

`Automatic` is the default and is the behavior of the earlier versions. `CockroachDbBulkOperationsDbSetting` inherits `CockroachDbDbSetting`, and `UseCockroachDb(...)` registers it as the setting of the connection in place of the default one. `Strict` and `StrictBypass` throw a `CockroachDbBulkColumnMappingsException` (naming the offending columns) before any data is written.

```csharp
GlobalConfiguration
    .Setup()
    .UseCockroachDb(new CockroachDbBulkOperationsDbSetting
    {
        BulkColumnMappingsBehavior = CockroachDbBulkColumnMappingsBehavior.StrictBypass
    });
```

For example, with a `Customer` table that has the `Id`, `Name`, `Email`, and `CreatedDateUtc` columns, and a list of entities (the source) with the `Email`, `Name`, and `Nickname` properties:

- `Strict` throws, because the order differs, `Id`/`CreatedDateUtc` are missing, and `Nickname` does not exist in the destination.
- `StrictBypass` throws, because `Nickname` does not exist in the destination.
- `Automatic` inserts `Name` and `Email`, and ignores `Nickname`.

```csharp
try
{
    await connection.BulkInsertAsync("Customer", customers);
}
catch (CockroachDbBulkColumnMappingsException ex)
{
    // e.g. "... Source column(s) that do not exist on the destination table: 'Nickname'."
    logger.LogError(ex, "Bulk insert rejected due to column misalignment.");
    throw;
}
```

Explicitly passed `mappings` always take precedence: the setting is not applied to them. `BulkDeleteByKey` is not affected, as its source is a list of key values rather than a set of columns.

## Notes

`BulkMerge`, `BulkUpdate`, `BulkDelete`, `BulkDeleteByKey`, and `BulkInsert` with `ReturnIdentity` stage rows in a pseudo table before applying them. Staging runs DDL (`CREATE`/`ALTER`/`DROP TABLE`), and CockroachDB commits any open transaction before DDL by default (`autocommit_before_ddl`). `Memory` staging (what the default `Auto` uses below 5,000 rows) also switches on `experimental_enable_temp_tables` for the session. See the [Limitations](https://github.com/mikependon/RepoDB/blob/master/LIMITATIONS.md#cockroachdb) page for details.
