[![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-uai8a)](https://ci.appveyor.com/project/mikependon/repodb-uai8a)
[![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-oap1j?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-oap1j/build/tests)

## RepoDb.SqlServer.BulkOperations

An extension library that contains the official Bulk Operations for RepoDb.

## Core Features
 
- BulkInsert via DataEntities
- BulkInsert via DataTable
- BulkInsert via DbDataReader
- BulkInsert Asynchronous Methods

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

```
Install-Package RepoDb.SqlServer.BulkOperations
```

### Query

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

### Insert

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

To learn more, please visit our [reference implementations](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md) page.
