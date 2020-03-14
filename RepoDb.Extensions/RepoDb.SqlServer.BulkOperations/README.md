[![SqlServerBulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a)](https://ci.appveyor.com/project/mikependon/repodb-uai8a)
[![Home](https://img.shields.io/badge/home-github-important)](https://github.com/mikependon/RepoDb)
[![Wiki](https://img.shields.io/badge/wiki-information-yellow)](https://github.com/mikependon/RepoDb/wiki)
[![SqlServerBulkVersion](https://img.shields.io/nuget/v/repodb.sqlserver.bulkoperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![SqlServerBulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)

## RepoDb.SqlServer.BulkOperations

An extension library that contains the official Bulk Operations for RepoDb.

## Why use Bulk Operations?

Basically, we do the normal *Delete*, *Insert*, *Merge* and *Update* operations when interacting with the database. The data is processed in an atomic way. If we do call the batch operations, the multiple single operation is just being batched and executed at the same time. In short, there are round-trips between your application and the database. Thus does not give you the maximum performance when doing the CRUD operations.

With bulk operations, all data is brought from the client application to the database via *BulkInsert* process. It ignores the audit, logs, constraints and any other database special handling. After that, the data is being processed at the same time in the database (server).

The bulk operations can hugely improve the performance by more than 90% when processing a large datasets.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Core Features

- [Special Arguments](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#special-arguments)
- [Async Methods](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#async-methods)
- [Caveats](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#caveats)
- [BulkDelete](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkdelete)
- [BulkInsert](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkinsert)
- [BulkMerge](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkmerge)
- [BulkUpdate](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkupdate)

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

--------

## Installation

At the *Package Manager Console*, write the command below.

```csharp
> Install-Package RepoDb.SqlServer.BulkOperations
```

Then call the bootstrapper once.

```csharp
RepoDb.SqlServerBootstrap.Initialize();
```

## Special Arguments

The arguments *qualifiers* and *usePhysicalPseudoTempTable* is provided at *BulkDelete*, *BulkMerge* and *BulkUpdate* operations.

The argument *qualifiers* is used to define the qualifier fields to be used in the operation. It usually refers to the *WHERE* expression of SQL Statements. If not given, the primary key (or identity) field will be used.

The argument *usePhysicalPseudoTempTable* is used to define whether a physical pseudo-table will be created during the operation. By default, a temporary table (ie: *#TableName*) is used.

## Async Methods

All synchronous methods has an equivalent asynchronous (Async) methods (ie: *BulkDeleteAsync*, *BulkInsertAsync*, *BulkMergeAsync* and *BulkUpdateAsync*).

## Caveats

RepoDb is automatically setting the value of *options* argument to *SqlBulkCopyOptions.KeepIdentity* when calling the *BulkDelete*, *BulkMerge* and *BulkUpdate* if you have not passed any *qualifiers* and if your table has an *IDENTITY* primary key column. The same logic will apply if there is no primary key but has an *IDENTITY* column defined in the table.

In addition, when calling the *BulkDelete*, *BulkMerge* and *BulkUpdate* operations, the library is creating a pseudo temporary table behind the scene. It requires your user to have the correct privilege to create a table in the database, otherwise a *SqlException* will be thrown.

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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkDelete<Customer>(customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkDelete("Customer", customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkDelete<Customer>(table, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkDelete("Customer", table, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkDelete<Customer>(reader, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkDelete("Customer", reader, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkMerge<Customer>(customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkMerge("Customer", customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkMerge<Customer>(table, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkMerge("Customer", table, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkMerge<Customer>(reader, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkMerge("Customer", reader, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkUpdate<Customer>(customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkUpdate("Customer", customers, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkUpdate<Customer>(table, qualifiers: qualifiers);
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
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkUpdate("Customer", table, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkUpdate<Customer>(reader, qualifiers: qualifiers);
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
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkUpdate("Customer", reader, qualifiers: qualifiers);
	}
}
```
