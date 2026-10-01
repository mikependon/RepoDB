<div align="center">
    <a href="https://repodb.net/tutorial/get-started-duckdb/"><image src="DuckDB.png" style="width:256px;" /></a>
    <br/>
    <span style="font-size:28px;font-weight:bold;"><a href="https://repodb.net/tutorial/get-started-duckdb/"><strong>RepoDb.DuckDb.BulkOperations</strong></a></span>
    <br/>
    <span style="font-size:16px;">A high-performance data productivity platform for bulk operations on DuckDB in .NET.</span>
</div>

-----

<br/>

[![DuckDbBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-duckdb-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-duckdb-bulk.yml)
[![DuckDbBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![DuckDbBulkVersion](https://img.shields.io/nuget/v/repodb.duckdb.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.DuckDb.BulkOperations)

# [RepoDb.DuckDb.BulkOperations](https://www.nuget.org/packages/RepoDb.DuckDb.BulkOperations)

A high-performant extension library of RepoDB that does bulk operations towards a DuckDB database. It uses `DuckDB.NET.Data`'s native `DuckDBAppender` class to load the data into the database.

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
- [Column Mappings Behavior](#column-mappings-behavior)

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.DuckDb.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseDuckDb();
```

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: DuckDbBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new DuckDBConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
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

`Automatic` is the default and is the behavior of the earlier versions. `DuckDbBulkOperationsDbSetting` inherits `DuckDbDbSetting`, and `UseDuckDb(...)` registers it as the setting of the connection in place of the default one. `Strict` and `StrictBypass` throw a `DuckDbBulkColumnMappingsException` (naming the offending columns) before any data is written.

```csharp
GlobalConfiguration
    .Setup()
    .UseDuckDb(new DuckDbBulkOperationsDbSetting
    {
        BulkColumnMappingsBehavior = DuckDbBulkColumnMappingsBehavior.StrictBypass
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
catch (DuckDbBulkColumnMappingsException ex)
{
    // e.g. "... Source column(s) that do not exist on the destination table: 'Nickname'."
    logger.LogError(ex, "Bulk insert rejected due to column misalignment.");
    throw;
}
```

Explicitly passed `mappings` always take precedence: the setting is not applied to them. `BulkDeleteByKey` is not affected, as its source is a list of key values rather than a set of columns.
