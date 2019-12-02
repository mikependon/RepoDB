[![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o6787)
[![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteDL](https://img.shields.io/nuget/dt/repodb.sqlite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests)
[![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests)

## RepoDb.SqLite - a hybrid .NET ORM library for SqLite.

This is the official repository for **RepoDb.SqLite** solution.

Any contribution (pull-requests) regarding **SqLite** solution must be done on this code-line.

## What is with this library

- It has all the functionalities of [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) implementation.
- It has batch operations; optimized the execution of multiple operations(ie: ***QueryMultiple***, ***InsertAll***, ***MergeAll*** and ***UpdateAll***).
- It is a unique and hybrid solution for ***SqLite*** data-provider within ***.NET Technology***.
- It is well-covered by Unit and Integration Tests.

## Installation

At the ***Package Manager Console***, write the command below.

```
Install-Package RepoDb.SqLite
```

## Getting Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.SqLiteBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

### Query

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
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
using (var connection = new SQLiteConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

## Dependencies

- [RepoDb (>= 1.10.1)](https://www.nuget.org/packages/RepoDb.SqLite/)
- [System.Data.SQLite (>= 1.0.112)](https://www.nuget.org/packages/System.Data.SQLite/)

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.
