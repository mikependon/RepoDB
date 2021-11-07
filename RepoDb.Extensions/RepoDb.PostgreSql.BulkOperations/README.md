[![PostgreSqlBulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ninm5?&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-ninm5)
[![PostgreSqlBulkHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![PostgreSqlBulkVersion](https://img.shields.io/nuget/v/repodb.postgresql.bulkoperations?&logo=nuget)](https://www.nuget.org/packages/RepoDb.PostgreSql.BulkOperations)
[![PostgreSqlBulkReleases](https://img.shields.io/badge/releases-core-important?&logo=nuget)](https://repodb.net/release/postgresqlbulk)
[![PostgreSqlBulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-ck79r?&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-ck79r/build/tests)

# RepoDb.PostgreSql.BulkOperations

An extension library that contains the official Bulk Operations of RepoDB for PostgreSQL.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Why use Bulk Operations?

Basically, we do the normal [Delete](https://repodb.net/operation/delete), [Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge) and [Update](https://repodb.net/operation/update) operations when interacting with the database. The data is processed in an atomic way. If we do call the batch operations, the multiple single operation is just being batched and executed at the same time. In short, there are round-trips between your application and the database. Thus does not give you the maximum performance when doing the CRUD operations.

With bulk operations, all data is brought from the client application to the database via [BinaryImport](https://repodb.net/operation/binaryimport) process. It ignores the audit, logs, constraints and any other database special handling. After that, the data is being processed at the same time in the database (server).

The bulk operations can hugely improve the performance by more than 90% when processing a large datasets.

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

The argument `keepIdentity` is used to define a value whether the identity property of the entity/model will be kept during the operation. Whereas,the argument `identityBehavior` is used to define a value whether an identity property of the entity/model will be kept, or, the newly generated identity values from the database will be returned after the operation. 

The argument `pseudoTableType` is used to define a value whether a physical pseudo-table will be created during the operation. By default, a temporary table is used.

The argument `mergedCommandType` is used to define a value whether the existing `ON CONFLICT DO UPDATE` will be used over the `UPDATE/INSERT` SQL commands during operations.

### Identity Setting Alignment

The library has enforced an additional logic to ensure the identity setting alignment if the `keepIdentity` is enabled (or the `identityBehavior` is set to `ReturnIdentity`) during the calls. This affects both the [BinaryBulkInsert](https://repodb.net/operation/binarybulkinsert) and [BinaryBulkMerge](https://repodb.net/operation/binarybulkmerge) operations.

Basically, a new column named `__RepoDb_OrderColumn` is being added into the pseudo-temporary table if the identity field is present on the underlying target table. This column will contain the actual index of the entity model from the `IEnumerable<T>` object.

During the bulk operation, a dedicated index value is passed that targets this additional column with a value of the entity model index, thus ensuring that the index value is really equating the index of the entity data from the `IEnumerable<T>` object. The resultsets of the pseudo-temporary table are being ordered using this newly generated column prior the actual merge to the underlying table.

When the newly generated identity value is being set back to the data model, the value of the `__RepoDb_OrderColumn` column is being used to look-up the proper index of the equating entity model from the `IEnumerable<T>` object, then, the compiled identity-setter function is used to assign back the identity value into the identity property.

## Async Methods

All synchronous methods has an equivalent asynchronous (Async) methods.

## BinaryBulkDelete

Delete the existing rows from the database by bulk. It returns the number of rows deleted during the operation.

### BinaryBulkDelete via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkDelete<Customer>(customers);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkDelete("Customer", customers);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BinaryBulkDelete("Customer", customers, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkDelete via DataTable

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BinaryBulkDelete("Customer", table);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BinaryBulkDelete("Customer", table, qualifiers: Field.From("LastName", "DateOfBirth"));
}
```

### BinaryBulkDelete via DbDataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BinaryBulkDelete("Customer", reader);
	}
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BinaryBulkDelete("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```

## BinaryBulkDeleteByKey

Soon to be updated.

## BulkInsert

Bulk insert a list of data entity objects into the database. All data entities will be inserted as new records in the database. It returns the number of rows inserted in the database.

### BulkInsert via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkInsert<Customer>(customers);
}
```

Or via table-name

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

Bulk merge a list of data entity objects into the database. A record is being inserted in the database if it is not exists using the defined qualifiers. It returns the number of rows inserted/updated in the database.

### BulkMerge via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge<Customer>(customers);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkMerge("Customer", customers);
}
```

Or with qualifiers

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

Or with qualifiers

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

Or with qualifiers

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

Bulk update a list of data entity objects into the database. A record is being updated in the database based on the defined qualifiers. It returns the number of rows updated in the database.

### BulkUpdate via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate<Customer>(customers);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkUpdate("Customer", customers);
}
```

Or with qualifiers

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

Or with qualifiers

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

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```
