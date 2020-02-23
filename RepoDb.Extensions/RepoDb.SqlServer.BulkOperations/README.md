[![BulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a)](https://ci.appveyor.com/project/mikependon/repodb-uai8a)
[![BulkVersion](https://img.shields.io/nuget/v/RepoDb.SqlServer.BulkOperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![BulkDL](https://img.shields.io/nuget/dt/repodb.sqlserver.bulkoperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![BulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)

## RepoDb.SqlServer.BulkOperations

An extension library that contains the official Bulk Operations for RepoDb.

## Why use Bulk Operations?

Basically, we do the normal *Delete*, *Insert*, *Merge* and *Update* operations when interacting with the database. The data is processed in an atomic way. If we do call the batch operations, the multiple single operation is just being batched and executed at the same time. There will be round-trips in between your application and database. Thus does not give you the maximum performance when doing the operation.

With bulk operations, the data is brought from the client application to the database via *BulkInsert* process. It ignores the audit and any other database special handling. After that, the data is being processed at the same time in the database. The performance is hugely improved from the basic operations by more than 90%.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Core Features
 
- [BulkDelete via PrimaryKeys](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkdelete-via-primarykeys)
- [BulkDelete via DataEntities](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkdelete-via-dataentities)
- [BulkDelete via DataTable](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkdelete-via-datatable)
- [BulkDelete via DbDataReader](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkdelete-via-dbdatareader)
- [BulkInsert via DataEntities](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkinsert-via-dataentities)
- [BulkInsert via DataTable](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkinsert-via-datatable)
- [BulkInsert via DbDataReader](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkinsert-via-dbdatareader)
- [BulkMerge via DataEntities](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkmerge-via-dataentities)
- [BulkMerge via DataTable](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkmerge-via-datatable)
- [BulkMerge via DbDataReader](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkmerge-via-dbdatareader)
- [BulkUpdate via DataEntities](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkupdate-via-dataentities)
- [BulkUpdate via DataTable](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkupdate-via-datatable)
- [BulkUpdate via DbDataReader](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkupdate-via-dbdatareader)
- [Special Arguments](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#special-arguments)
- [BulkOperations Asynchronous Methods](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Extensions/RepoDb.SqlServer.BulkOperations#bulkoperations-asynchronous-methods)

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

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkInsert<Customer>(customers, qualifiers: qualifiers);
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

Or via table-name with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkInsert("Customer", customers, qualifiers: qualifiers);
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

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkInsert<Customer>(table, qualifiers: qualifiers);
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

Or via table-name with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var qualifiers = Field.From("LastName", "BirthDate");
	var rows = connection.BulkInsert("Customer", table, qualifiers: qualifiers);
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

Or with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkInsert<Customer>(reader, qualifiers: qualifiers);
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

Or via table-name with qualifiers

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customer];"))
	{
		var qualifiers = Field.From("LastName", "BirthDate");
		var rows = connection.BulkInsert("Customer", reader, qualifiers: qualifiers);
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

## Special Arguments

The arguments *qualifiers* and *usePhysicalPseudoTempTable* is provided at *BulkDelete*, *BulkMerge* and *BulkUpdate* operations.

The argument *qualifiers* is used to define the qualifier fields to be used in the operation. It usually refers to the *WHERE* expression of SQL Statements. If not given, the primary key (or identity) field will be used.

The argument *usePhysicalPseudoTempTable* is used to define whether a physical pseudo-table will be created during the operation. By default, a temporary table (ie: *#TableName*) is used.

## BulkOperations Asynchronous Methods

All synchronous methods has an equivalent asynchronous methods.