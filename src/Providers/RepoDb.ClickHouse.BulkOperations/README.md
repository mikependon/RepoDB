[![ClickHouseBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-clickhouse-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-clickhouse-bulk.yml)
[![ClickHouseBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![ClickHouseBulkVersion](https://img.shields.io/nuget/v/repodb.clickhouse.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.ClickHouse.BulkOperations)

# [RepoDb.ClickHouse.BulkOperations](https://www.nuget.org/packages/RepoDb.ClickHouse.BulkOperations)

A high-performant extension library of RepoDB that does bulk operations towards a ClickHouse database. It uses its own internal class `ClickHouseBulkCopy` to load the data towards the database, built on top of `ClickHouse.Driver`'s native `ClickHouseBulkCopy` streaming-insert implementation.

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

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.ClickHouse.BulkOperations
```

Then call the setup for `ClickHouse`.

```csharp
RepoDb
    .Setup()
    .UseClickHouse();
```

> **Waiting for mutations:** `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey` all resolve to
> ClickHouse's asynchronous `ALTER TABLE ... UPDATE`/`DELETE` mutations under the hood, applied by a
> background merge rather than immediately - a query issued right after one of these calls returns is not
> guaranteed to see the change yet. Pass `isWaitForMutationsEnabled: true` to `UseClickHouse()` (it defaults
> to `false`) to have these operations block until each mutation actually finishes (up to a 5-second timeout)
> before returning, trading extra latency per call for read-your-writes consistency. This is read from
> `ClickHouseDbSetting.IsWaitForMutationsEnabled` on the mapped `IDbSetting` - see the
> [RepoDb.ClickHouse](https://github.com/mikependon/RepoDB/tree/master/RepoDb.ClickHouse) README for details.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

> **Note:** Unlike the SQL Server/MySQL/Oracle providers, ClickHouse has no identity, auto-increment, or
> sequence mechanism of any kind, so `ClickHouseBulkImportIdentityBehavior.ReturnIdentity` is not supported
> here - passing it throws a `NotSupportedException`. Use `KeepIdentity` (the default) instead, and assign
> your own key values before inserting.

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new ClickHouseConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```

## License

[Apache License 2.0](https://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon) 

This project depends on `ClickHouse.Driver` (the ADO.NET driver for ClickHouse,
formerly known as `ClickHouse.Client`), which is separately licensed under the
MIT License.
