## Disclaimer

#### This code-line is still under-development as [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) v1.10.1.

[![PostgreSqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![PostgreSqlVersion](https://img.shields.io/nuget/v/RepoDb.PostgreSql?style=flat-square)](https://www.nuget.org/packages/RepoDb.PostgreSql)
[![PostgreSqlDL](https://img.shields.io/nuget/dt/repodb.postgresql?style=flat-square)](https://www.nuget.org/packages/RepoDb.PostgreSql)
[![PostgreSqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![PostgreSqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

## RepoDb.PostgreSql - a hybrid .NET ORM library for PostgreSql.

This is the official repository for **RepoDb.PostgreSql** solution.

## What is with this library

- It has all the functionalities of [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) implementation.
- It has batch operations; optimized the execution of multiple operations(ie: ***QueryMultiple***, ***InsertAll***, ***MergeAll*** and ***UpdateAll***).
- It is a unique and hybrid solution for ***PostgreSql*** data-provider within ***.NET Technology***.
- It is well-covered by Unit and Integration Tests.

## Bulk Operations 

IThe bulk-insert has been implemented through `NpgsqlConnection` default implementation. Below is the implementation (as extended methods).

```
BaseRepository<TEntity, TDbConnection>.BulkInsert(...);
DbRepository<TDbConnection>.BulkInsert<TEntity>(...);
NpgsqlConnection.BulkInsert<TEntity>(...);
```

## Installation

At the ***Package Manager Console***, write the command below.

```
Install-Package RepoDb.PostgreSql
```

## Getting Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.PostgreSqlBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

### Query

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
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
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

## Dependencies

- [Npgsql (>= 4.1.2)](https://www.nuget.org/packages/Npgsql/)
- [RepoDb (>= 1.10.1)](https://www.nuget.org/packages/RepoDb.SqLite/)

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.
