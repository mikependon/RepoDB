[![EnterpriseDbBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-enterprisedb-bulk.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-enterprisedb-bulk.yml)
[![EnterpriseDbBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![EnterpriseDbBulkVersion](https://img.shields.io/nuget/v/repodb.enterprisedb.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.EnterpriseDb.BulkOperations)

# [RepoDb.EnterpriseDb.BulkOperations](https://www.nuget.org/packages/RepoDb.EnterpriseDb.BulkOperations)

A high-performant extension library of RepoDB that does bulk operations towards an EDB Postgres Advanced Server database. It stages rows through a pseudo (staging) table and loads them via `RepoDb.Connector.EnterpriseDb`'s `EDBBulkCopy`, which itself is built on top of Npgsql's native binary `COPY` protocol.

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
Install-Package RepoDb.EnterpriseDb.BulkOperations
```

Then call the setup for `EnterpriseDb`.

```csharp
GlobalConfiguration
    .Setup()
    .UseEnterpriseDb();
```

`EDBConnection` in the samples below is `RepoDb.Connector.EnterpriseDb.EDBConnection` — the connector this package's bulk operations are built against, not the official `EnterpriseDB.EDBClient` driver's connection type of the same name.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var insertedRows = connection.BulkInsert("Customer", customers);
}
```

Or via a `DataTable`:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var insertedRows = connection.BulkInsert("Customer", table);
}
```

Returning generated identities:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers(); // Id not set
    connection.BulkInsert<Customer>(customers, identityBehavior: EDBBulkImportIdentityBehavior.ReturnIdentity);
    // customers[i].Id now holds the generated identity for each row
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var mergedRows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

Or via a `DataTable`:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var mergedRows = connection.BulkMerge("Customer", table);
}
```

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var rows = connection.BulkUpdate("Customer", table);
}
```

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var customers = GetCustomers();
    var deletedRows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via a `DataTable`:

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var table = GetCustomersAsDataTable();
    var deletedRows = connection.BulkDelete("Customer", table);
}
```

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new EDBConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```
