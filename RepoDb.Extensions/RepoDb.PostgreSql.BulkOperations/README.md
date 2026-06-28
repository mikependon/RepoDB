[![PostgreSqlBulkBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql-bulk.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql-bulk.yml)
[![PostgreSqlBulkHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![PostgreSqlBulkVersion](https://img.shields.io/nuget/v/repodb.postgresql.bulkoperations?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations)

# [RepoDb.PostgreSql.BulkOperations](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations)

High-performance bulk operations for RepoDB on PostgreSQL. Uses PostgreSQL's native binary import protocol to transfer data in a single pass — up to 90% faster than row-by-row or batch operations.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Special Arguments](#special-arguments)
- [Async Methods](#async-methods)
- [BinaryBulkDelete](#binarybulkdelete)
- [BinaryBulkDeleteByKey](#binarybulkdeletebykey)
- [BinaryBulkInsert](#binarybulkinsert)
- [BinaryBulkMerge](#binarybulkmerge)
- [BinaryBulkUpdate](#binarybulkupdate)

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
Install-Package RepoDb.PostgreSql.BulkOperations
```

Then initialize the bootstrapper once at application startup:

```csharp
RepoDb.PostgreSqlBootstrap.Initialize();
```

Or visit the [installation](https://repodb.net/tutorial/installation) page for more options.

## Special Arguments

**`qualifiers`** — defines the fields used in the matching criteria for delete, merge, and update operations. Defaults to the primary key column.

**`keepIdentity`** — when enabled, the identity property value on the entity is preserved during the operation.

**`identityBehavior`** — controls identity handling and whether newly generated identity values are returned and written back to the entities.

**`pseudoTableType`** — controls whether a physical or session-scoped temporary table is created internally during the operation.

**`mergeCommandType`** — controls whether `ON CONFLICT DO UPDATE` or separate `UPDATE`/`INSERT` SQL commands are used during merge.

### Identity Setting Alignment

RepoDB adds an internal `__RepoDb_OrderColumn` to the pseudo-temporary table when identity fields are present. This column preserves the original index of each entity in the `IEnumerable<T>` collection, ensuring generated identity values are mapped back to the correct objects after the operation completes.

## BatchSize

All operations accept a `batchSize` argument to control how many rows are sent to the server per round-trip. Defaults to `null` (all rows in one pass). Tune this based on column count, data size, and network characteristics.

## Enum Types

Npgsql supports the following .NET enum mappings:

- .NET Enum → PostgreSQL text-based types (e.g. `text`, `varchar`)
- .NET Enum → PostgreSQL integer types (e.g. `int4`, `int8`)
- .NET Enum → Native PostgreSQL enum type, when mapped via `NpgsqlDataSource.MapEnum()`

These mappings work correctly with standard fluent operations such as [Insert](https://repodb.net/operation/insert) and [InsertAll](https://repodb.net/operation/insertall). However, bulk operations such as [BinaryBulkInsert](https://repodb.net/operation/binarybulkinsert) may fail with the following error when an enum column is involved:

```
'RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations.Hands' is not supported for parameters having NpgsqlDbType 'Unknown'.
```

A fix for this is currently in progress. This section will be updated once a resolution is available.

## Async Methods

Every synchronous operation has a corresponding `Async` overload.

## BinaryBulkDelete

Deletes existing rows from the database in bulk. Returns the number of deleted rows.

### BinaryBulkDelete via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkDelete via DataTable

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var deletedRows = connection.BinaryBulkDelete("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var deletedRows = connection.BinaryBulkDelete("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkDelete via DbDataReader

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var deletedRows = connection.BinaryBulkDelete("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var deletedRows = connection.BinaryBulkDelete("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BinaryBulkDeleteByKey

Deletes existing rows from the database in bulk via a list of primary keys. Returns the number of deleted rows.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var primaryKeys = new [] { 1, 2, ..., 10045 };
	var deletedRows = connection.BinaryBulkDeleteByKey(primaryKeys);
}
```

## BinaryBulkInsert

Inserts a list of entities into the database in bulk. Returns the number of inserted rows.

### BinaryBulkInsert via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var insertedRows = connection.BinaryBulkInsert<Customer>(customers);
}
```

Or via table-name:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var insertedRows = connection.BinaryBulkInsert("Customer", customers);
}
```

### BinaryBulkInsert via DataTable

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var insertedRows = connection.BinaryBulkInsert("Customer", table);
}
```

### BinaryBulkInsert via DbDataReader

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var insertedRows = connection.BinaryBulkInsert("Customer", reader);
	}
}
```

## BinaryBulkMerge

Upserts a list of entities in bulk — inserts new rows and updates existing ones based on the defined qualifiers. Returns the number of affected rows.

### BinaryBulkMerge via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkMerge via DataTable

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var mergedRows = connection.BinaryBulkMerge("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var mergedRows = connection.BinaryBulkMerge("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkMerge via DbDataReader

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var mergedRows = connection.BinaryBulkMerge("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var mergedRows = connection.BinaryBulkMerge("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BinaryBulkUpdate

Updates existing rows in the database in bulk. Returns the number of updated rows.

### BinaryBulkUpdate via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate<Customer>(customers);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate("Customer", customers);
}
```

Or via table-name with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkUpdate via DataTable

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BinaryBulkUpdate("Customer", table);
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BinaryBulkUpdate("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkUpdate via DbDataReader

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var rows = connection.BinaryBulkUpdate("Customer", reader);
	}
}
```

Or with qualifiers:

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM \"Customer\";"))
	{
		var rows = connection.BinaryBulkUpdate("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```
