[![SqlServerBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr-bulk.yml)
[![SqlServerBulkHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![SqlServerBulkVersion](https://img.shields.io/nuget/v/repodb.sqlserver.bulkoperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)

# [RepoDb.SqlServer.BulkOperations](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)

High-performance bulk operations for RepoDB on SQL Server. Process millions of rows in a single pass — up to 90% faster than row-by-row or batch operations.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Special Arguments](#special-arguments)
- [Async Methods](#async-methods)
- [Caveats](#caveats)
- [BulkDelete](#bulkdelete)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)

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
Install-Package RepoDb.SqlServer.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
RepoDb.SqlServerBootstrap.Initialize();
```

Or visit the [installation](https://repodb.net/tutorial/installation) page for more options.

## Special Arguments

**`qualifiers`** — defines the fields used in the `WHERE` clause for delete, merge, and update operations. Defaults to the primary key or identity column.

**`isReturnIdentity`** — when enabled, newly generated identity values are written back to the entity objects after the operation completes.

**`usePhysicalPseudoTempTable`** — controls whether a physical temp table or a session-scoped `#TempTable` is used internally. Defaults to session-scoped.

### Identity Setting Alignment

When `isReturnIdentity` is enabled, RepoDB adds an internal `__RepoDb_OrderColumn` to the pseudo-temporary table. This column preserves the original index of each entity in the `IEnumerable<T>` collection, ensuring that identity values are mapped back to the correct objects after the bulk operation completes.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## Caveats

RepoDB automatically sets `SqlBulkCopyOptions.KeepIdentity` for BulkDelete, BulkMerge, and BulkUpdate when no qualifiers are provided and the target table has an `IDENTITY` primary key column.

These operations require the connected user to have CREATE TABLE permission in the target database, since a pseudo-temporary table is created internally during execution.

## BulkDelete

Deletes a list of entities or primary keys from the database in bulk. Returns the number of deleted rows.

### BulkDelete via PrimaryKeys

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var primaryKeys = new object[] { 10045, ..., 11902 };
	var rows = connection.BulkDelete<Customer>(primaryKeys);
}
```

Or:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var primaryKeys = new object[] { 10045, ..., 11902 };
	var rows = connection.BulkDelete("Customer", primaryKeys);
}
```

### BulkDelete via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkDelete via DataTable

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkDelete("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkDelete("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkDelete via DbDataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkDelete("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkDelete("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

### BulkInsert via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkInsert("Customer", customers);
}
```

### BulkInsert via DataTable

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkInsert("Customer", table);
}
```

### BulkInsert via DbDataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkInsert("Customer", reader);
	}
}
```

## BulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

### BulkMerge via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkMerge via DataTable

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkMerge("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkMerge("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkMerge via DbDataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkMerge("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkMerge("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BulkUpdate

Updates existing rows in the database in bulk. Returns the number of updated rows.

### BulkUpdate via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkUpdate via DataTable

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkUpdate("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkUpdate("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BulkUpdate via DbDataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```
