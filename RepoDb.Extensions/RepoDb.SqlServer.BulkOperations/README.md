[![BulkBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a)](https://ci.appveyor.com/project/mikependon/repodb-uai8a)
[![BulkVersion](https://img.shields.io/nuget/v/RepoDb.SqlServer.BulkOperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![BulkDL](https://img.shields.io/nuget/dt/repodb.sqlserver.bulkoperations)](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations)
[![BulkIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)

## RepoDb.SqlServer.BulkOperations

An extension library that contains the official Bulk Operations for RepoDb.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Core Features
 
- BulkDelete via PrimaryKeys
- BulkDelete via DataEntities
- BulkDelete via DataTable
- BulkDelete via DbDataReader
- BulkInsert via DataEntities
- BulkInsert via DataTable
- BulkInsert via DbDataReader
- BulkMerge via DataEntities
- BulkMerge via DataTable
- BulkUpdate via DataEntities
- BulkUpdate via DataTable
- BulkUpdate via DbDataReader
- BulkOperatations Asynchronous Methods

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

### BulkInsert via DataEntities

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customers = GetCustomers();
	var rows = connection.BulkInsert<Customer>(customers);
}
```

Or

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

Or

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var table = GetCustomersAsDataTable();
	var rows = connection.BulkInsert("Customer", table);
}
```

### BulkInsert via DataReader

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	using (var reader = connection.ExecuteReader("SELECT * FROM [dbo].[Customers];"))
	{
		var rows = connection.BulkInsert("Customer", reader);
	}
}
```
