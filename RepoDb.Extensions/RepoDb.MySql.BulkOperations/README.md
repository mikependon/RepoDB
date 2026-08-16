[![MySqlBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql-bulk.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql-bulk.yml)
[![MySqlBulkHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![MySqlBulkVersion](https://img.shields.io/nuget/v/repodb.mysql.bulkoperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations)

# [RepoDb.MySql.BulkOperations](https://www.nuget.org/packages/RepoDb.MySql.BulkOperations)

A high-performant extension library of RepoDB that does bulk operations towards a MySQL database. It uses its own internal class `MySqlBulkCopy` to load the data towards the database. It uses the `LOAD DATA LOCAL INFILE`-based implementation on top of `MySql.Data`'s `MySqlBulkLoader`.

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
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.MySql.BulkOperations
```

Then call the setup for `MySql`.

```csharp
RepoDb
    .Setup()
    .UseMySql();
```

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

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

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

## BulkUpdate

Updates existing rows in the database in bulk, matched by the defined qualifiers. Returns the number of updated rows.

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

## BulkDelete

Deletes existing rows from the database in bulk, matched by the defined qualifiers. Returns the number of deleted rows.

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

## BulkDeleteByKey

Deletes existing rows from the database in bulk, matched by their primary (or identity) key value alone — no entities or `DataTable` involved, just the list of key values to remove. Returns the number of deleted rows.

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, 10046, 10047 };
    var deletedRows = connection.BulkDeleteByKey("Customer", primaryKeys);
}
```

## License

[Apache License 2.0](https://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon) 

This project depends on Oracle's MySQL Connector/NET (`MySql.Data`),
which is separately licensed under GPL-2.0 with the Universal FOSS
Exception, Version 1.0.
