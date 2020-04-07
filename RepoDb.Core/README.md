[![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
[![Home](https://img.shields.io/badge/home-github-important)](https://github.com/mikependon/RepoDb)
[![Website](https://img.shields.io/badge/website-information-yellow)](http://repodb.net)
[![GetStarted](https://img.shields.io/badge/tutorial-getstarted-pink)](http://repodb.net/tutorial/get-started-sqlserver)
[![CoreVersion](https://img.shields.io/nuget/v/RepoDb)](https://www.nuget.org/packages/RepoDb)
[![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx?label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)
[![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)

## RepoDb - a hybrid ORM library for .NET.

RepoDb is an open-source .NET ORM that bridge the gaps between micro-ORMs and macro-ORMs. It helps the developer to simplify the switch-over of when to use the “BASIC” and “ADVANCE” operations during the development.

It is the best alternative ORM to both Dapper and EntityFramework.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Core Features
 
- Asynchronous Operations
- Batch Operations
- Bulk Operations
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
- Property Handlers (Transformations)
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

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

--------

## Installation

At the Package Manager Console, write the command below.

```
Install-Package RepoDb
```

## Getting Started

After the installation, any library operation can then be called. Please see below for the samples.

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
