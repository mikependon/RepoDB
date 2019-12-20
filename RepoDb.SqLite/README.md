[![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o6787)
[![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteDL](https://img.shields.io/nuget/dt/repodb.sqlite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests)
[![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests)

## RepoDb.SqLite - a hybrid .NET ORM library for SqLite.

This is the official repository for **RepoDb.SqLite** solution.

## Introduction

- It has all the functionalities of [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) implementation.
- It has [batch operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations).
- It is a unique and hybrid solution for ***SqLite*** data-provider within ***.NET Technology***.
- It is well-covered by Unit and Integration Tests.

## Core Features
 
- Asynchronous Operations
- Batch Operations
- Caching
- Connection Persistency
- Database Helpers
- Database Settings
- Expression Trees
- Extension Methods
- Field Mapping
- Inline Hints
- Massive Operations (Generics/Explicits/MethodCalls/TableBased)
- Multi-Resultset Query
- Query Builder
- Repositories
- Resolvers (CLR Types, DB Types)
- Statement Builder
- Tracing
- Transaction
- Type Mapping

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

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

- [RepoDb](https://www.nuget.org/packages/RepoDb/)
- [System.Data.SQLite](https://www.nuget.org/packages/System.Data.SQLite/)

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.
