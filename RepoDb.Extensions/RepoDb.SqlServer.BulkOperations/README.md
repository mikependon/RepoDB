[![SqlServerBulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-uai8a)
[![SqlServerBulkHome](https://img.shields.io/badge/home-github-important?style=flat-square&logo=github)](https://github.com/mikependon/RepoDb)
[![SqlServerBulkVersion](https://img.shields.io/nuget/v/repodb.sqlserver.bulkoperations?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![SqlServerBulkReleases](https://img.shields.io/badge/releases-core-important?style=flat-square&logo=nuget)](https://repodb.net/release/sqlserverbulk)
[![SqlServerBulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?style=flat-square&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)

# RepoDb.SqlServer.BulkOperations

An extension library that contains the official Bulk Operations of RepoDB for SQL Server.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Why use Bulk Operations?

Basically, we do the normal [Delete](https://repodb.net/operation/delete), [Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge) and [Update](https://repodb.net/operation/update) operations when interacting with the database. The data is processed in an atomic way. If we do call the batch operations, the multiple single operation is just being batched and executed at the same time. In short, there are round-trips between your application and the database. Thus does not give you the maximum performance when doing the CRUD operations.

With bulk operations, all data is brought from the client application to the database via [BulkInsert](https://repodb.net/operation/bulkinsert) process. It ignores the audit, logs, constraints and any other database special handling. After that, the data is being processed at the same time in the database (server).

The bulk operations can hugely improve the performance by more than 90% when processing a large datasets.

## Core Features

- [Special Arguments](#special-arguments)
- [Async Methods](#async-methods)
- [Caveats](#caveats)
- [BulkDelete](#bulkdelete)
- [BulkInsert](#bulkinsert)
- [BulkMerge](#bulkmerge)
- [BulkUpdate](#bulkupdate)

## Community engagements

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
> Install-Package RepoDb.SqlServer.BulkOperations
```

Then call the bootstrapper once.

```csharp
RepoDb.SqlServerBootstrap.Initialize();
```

Or, visit our [installation](https://repodb.net/tutorial/installation) page for more information.

## Special Arguments

The arguments `qualifiers`, `isReturnIdentity` and `usePhysicalPseudoTempTable` is provided at [BulkDelete](https://repodb.net/operation/bulkdelete), [BulkMerge](https://repodb.net/operation/bulkmerge) and [BulkUpdate](https://repodb.net/operation/bulkupdate) operations.

The argument `qualifiers` is used to define the qualifier fields to be used in the operation. It usually refers to the `WHERE` expression of SQL Statements. If not given, the primary key (or identity) field will be used.

The argument `isReturnIdentity` is used to define the behaviour of the execution whether the newly generated identity will be set-back to the data entities. By default, it is disabled.

The argument `usePhysicalPseudoTempTable` is used to define whether a physical pseudo-table will be created during the operation. By default, a temporary table (ie: `#TableName`) is used.

### Identity Setting Alignment

The library has enforced an additional logic to ensure the identity setting alignment if the `isReturnIdentity` is enabled during the calls. This affects both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations.

Basically, a new column named `__RepoDb_OrderColumn` is being added into the pseudo-temporary table if the identity field is present on the underlying target table. This column will contain the actual index of the entity model from the `IEnumerable<T>` object.

During the bulk operation, a dedicated `DbParameter` object is created that targets this additional column with a value of the entity model index, thus ensuring that the index value is really equating the index of the entity data from the `IEnumerable<T>` object. The resultsets of the pseudo-temporary table are being ordered using this newly generated column prior the actual merge to the underlying table.

When the newly generated identity value is being set back to the data model, the value of the `__RepoDb_OrderColumn` column is being used to look-up the proper index of the equating entity model from the `IEnumerable<T>` object, then, the compiled identity-setter function is used to assign back the identity value into the identity property.

## Async Methods

All synchronous methods has an equivalent asynchronous (Async) methods.

## Caveats

RepoDB is automatically setting the value of `options` argument to `SqlBulkCopyOptions.KeepIdentity` when calling the [BulkDelete](https://repodb.net/operation/bulkdelete), [BulkMerge](https://repodb.net/operation/bulkmerge) and [BulkUpdate](https://repodb.net/operation/bulkupdate) if you have not passed any `qualifiers` and if your table has an `IDENTITY` primary key column. The same logic will apply if there is no primary key but has an `IDENTITY` column defined in the table.

In addition, when calling the [BulkDelete](https://repodb.net/operation/bulkdelete), [BulkMerge](https://repodb.net/operation/bulkmerge) and [BulkUpdate](https://repodb.net/operation/bulkupdate) operations, the library is creating a pseudo temporary table behind the scene. It requires your user to have the correct privilege to create a table in the database, otherwise a `SqlException` will be thrown.

## BulkDelete

Bulk delete a list of data entity objects (or via primary keys) from the database. It returns the number of rows deleted from the database.

### BulkDelete via PrimaryKeys

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var primaryKeys = new object[] { 10045, ..., 11902 };
	var rows = connection.BulkDelete<Customer>(primaryKeys);
}
```

Or

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

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or with primary keys

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var primaryKeys = new [] { 10045, ..., 11011 };
	var rows = connection.BulkDelete<Customer>(primaryKeys);
}
```

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete<Customer>(customers, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkDelete("Customer", customers);
}
```

Or via table-name with qualifiers

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
	var rows = connection.BulkDelete<Customer>(table);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkDelete<Customer>(table, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkDelete("Customer", table);
}
```

Or via table-name with qualifiers

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
		var rows = connection.BulkDelete<Customer>(reader);
	}
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkDelete<Customer>(reader, qualifiers: e => new { e.LastName, e.DateOfBirth });
	}
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkDelete("Customer", reader);
	}
}
```

Or via table-name with qualifiers

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
	var rows = connection.BulkInsert<Customer>(table);
}
```

Or via table-name

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
		var rows = connection.BulkInsert<Customer>(reader);
	}
}
```

Or via table-name

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

Or via table-name with qualifiers

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
	var rows = connection.BulkMerge<Customer>(table);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkMerge<Customer>(table, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkMerge("Customer", table);
}
```

Or via table-name with qualifiers

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
		var rows = connection.BulkMerge<Customer>(reader);
	}
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkMerge<Customer>(reader, qualifiers: e => new { e.LastName, e.DateOfBirth });
	}
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkMerge("Customer", reader);
	}
}
```

Or via table-name with qualifiers

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

Or via table-name with qualifiers

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
	var rows = connection.BulkUpdate<Customer>(table);
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkUpdate<Customer>(table, qualifiers: e => new { e.LastName, e.DateOfBirth });
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkUpdate("Customer", table);
}
```

Or via table-name with qualifiers

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
		var rows = connection.BulkUpdate<Customer>(reader);
	}
}
```

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate<Customer>(reader, qualifiers: e => new { e.LastName, e.DateOfBirth });
	}
}
```

Or via table-name

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate("Customer", reader);
	}
}
```

Or via table-name with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var rows = connection.BulkUpdate("Customer", reader, qualifiers: Field.From("LastName", "DateOfBirth"));
	}
}
```
