[![PostgreSqlBulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ninm5?&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-ninm5)
[![PostgreSqlBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![PostgreSqlBulkVersion](https://img.shields.io/nuget/v/repodb.postgresql.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations)
[![PostgreSqlBulkReleases](https://img.shields.io/badge/releases-core-important?&logo=nuget)](https://repodb.net/release/postgresqlbulk)
[![PostgreSqlBulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-ck79r?&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-ck79r/build/tests)

# [RepoDb.PostgreSql.BulkOperations](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations)

An extension library that contains the official Bulk Operations of RepoDB for PostgreSQL.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Why use the Bulk Operations?

Basically, you do the normal [Delete](https://repodb.net/operation/delete), [Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge) and [Update](https://repodb.net/operation/update) operations when interacting with the database. Through this, the data is processed in an atomic way.

If you use the [Batch Operations](https://repodb.net/feature/batchoperations) (the process of wrapping the multiple single operations and executing one-go), it does not completely eliminate the round-trips between your application and your database, thus does not give you the maximum performance during the CRUD operations.

With the [Bulk Operations](https://repodb.net/feature/bulkoperations), all data is brought from your client application towards your database in one-go via the [BinaryImport](https://repodb.net/operation/binaryimport) operation (a real bulk process). It then post processed the data altogether in the database server to maximize the performance.

During the operation, the process ignores the audit, logs, constraints and any other database special handling. It hugely improve the performance of your application by more than 90%, especially when processing the large datasets.

## Core Features

- [Special Arguments](#special-arguments)
- [Async Methods](#async-methods)
- [Caveats](#caveats)
- [BinaryBulkDelete](#binarybulkdelete)
- [BinaryBulkDeleteByKey](#binarybulkdeletebykey)
- [BinaryBulkInsert](#binarybulkinsert)
- [BinaryBulkMerge](#binarybulkmerge)
- [BinaryBulkUpdate](#binarybulkupdate)

## Community Engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2020 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.PostgreSql.BulkOperations
```

Then call the bootstrapper once.

```csharp
RepoDb.PostgreSqlBootstrap.Initialize();
```

Or, visit our [installation](https://repodb.net/tutorial/installation) page for more information.

## Special Arguments

The arguments `qualifiers`, `keepIdentity`, `identityBehavior`, `pseudoTableType` and `mergeCommanType` were provided in most operations.

The argument `qualifiers` is used to define the qualifier fields to be used in the operations. It usually refers to the `WHERE` expression of SQL Statements. If not given, the primary key field will be used.

The argument `keepIdentity` is used to define a value whether the identity property of the entity/model will be kept during the operation.

The argument `identityBehavior` is used to define a value like with the `keepIdentity` argument, together-with, a value that is used to return the newly generated identity values from the database. 

The argument `pseudoTableType` is used to define a value whether a physical pseudo-table will be created during the operation. By default, a temporary table is used.

The argument `mergedCommandType` is used to define a value whether the existing `ON CONFLICT DO UPDATE` will be used over the `UPDATE/INSERT` SQL commands during operations.

### Identity Setting Alignment

Behind the scene, the library has enforced an additional logic to ensure the identity setting alignment. Basically, a new column named `__RepoDb_OrderColumn` is being added into the pseudo-temporary table if the identity field is present on the underlying table. This column will contain the actual index of the entity model from the `IEnumerable<T>` object.

During the bulk operation, a dedicated index (entity model index) value is passed to this column, thus ensuring that the index value is really equating to the index of the item from the `IEnumerable<T>` object. The resultsets of the pseudo-temporary table are being ordered using this column, prior the actual merge to the underlying table.

For both the [BinaryBulkInsert](https://repodb.net/operation/binarybulkinsert) and [BinaryBulkMerge](https://repodb.net/operation/binarybulkmerge) operations, when the newly generated identity value is being set back to the data model, the value of the `__RepoDb_OrderColumn` column is being used to look-up the proper index of the equating item from the `IEnumerable<T>` object, then, the compiled identity-setter function is used to assign back the identity value into the identity property.

## BatchSize

All the provided operations has a `batchSize` attribute that enables you to override the size of the items being wired-up to the server during the operation. By default it is `null`, all the items are being sent together in one-go.

Use this attribute if you wish to optimize the operation based on certain sitution (i.e.: No. of Columns, Type/Size of Data, Network Latency).

## Async Methods

All the provided synchronous operations has its equivalent asynchronous (Async) operations.

## BinaryBulkDelete

Delete the existing rows from the database by bulk. It returns the number of rows that has been deleted during the operation.

### BinaryBulkDelete via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete<Customer>(customers);
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var deletedRows = connection.BinaryBulkDelete("Customer", customers);
}
```

And with qualifiers.

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

Or with qualifiers.

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
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var deletedRows = connection.BinaryBulkDelete("Customer", reader);
	}
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var deletedRows = connection.BinaryBulkDelete("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BinaryBulkDeleteByKey

Delete the existing rows from the database by bulk via a list of primary keys. It returns the number of rows that has been deleted during the operation.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var primaryKeys = new [] { 1, 2, ..., 10045 };
	var deletedRows = connection.BinaryBulkDeleteByKey(primaryKeys);
}
```

## BinaryBulkInsert

Insert a list of entities into the database by bulk. It returns the number of rows that has been inserted in the database.

### BinaryBulkInsert via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var insertedRows = connection.BinaryBulkInsert<Customer>(customers);
}
```

Or via table-name.

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
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var insertedRows = connection.BinaryBulkInsert("Customer", reader);
	}
}
```

## BinaryBulkMerge

Merge a list of entities into the database by bulk. A new row is being inserted (if not present) and an existing row is being updated (if present) through the defined qualifiers. It returns the number of rows that has been inserted/updated in the database.

### BinaryBulkMerge via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge<Customer>(customers);
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var mergedRows = connection.BinaryBulkMerge("Customer", customers);
}
```

And with qualifiers.

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

Or with qualifiers.

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
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var mergedRows = connection.BinaryBulkMerge("Customer", reader);
	}
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var mergedRows = connection.BinaryBulkMerge("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BinaryBulkUpdate

Update the existing rows from the database by bulk. The affected rows are strongly bound to the values of the qualifier fields when calling the operation. It returns the number of rows that has been updated in the database.

### BinaryBulkUpdate via DataEntities

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate<Customer>(customers);
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkUpdate("Customer", customers);
}
```

And with qualifiers.

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

Or with qualifiers.

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
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BinaryBulkUpdate("Customer", reader);
	}
}
```

Or with qualifiers.

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BinaryBulkUpdate("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```
