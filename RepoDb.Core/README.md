[![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
[![CoreVersion](https://img.shields.io/nuget/v/RepoDb)](https://www.nuget.org/packages/RepoDb)
[![CoreDL](https://img.shields.io/nuget/dt/repodb)](https://www.nuget.org/packages/RepoDb)
[![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)
[![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)

## RepoDb - a hybrid ORM library for .NET.

RepoDb provides certain features of both “micro-ORMs” and “macro-ORMs”. It helps the developer to simplify the “switchover” of when to use the “basic” and “advance” operations during the development.

All [operations](https://github.com/mikependon/RepoDb#operations) were implemented as extended methods of the *IDbConnection* object. Once you hold the opened-state of your database connection object, you can then do all the activities you would like to do with your database through those extended methods.

## Links and Resources

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Highlights

- RepoDb is the fastest and the most-efficient ORM as per the official [result](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netcore.txt) of [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) tool.
- RepoDb is covered by thousand of major business related [Unit Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.UnitTests) and [Integration Tests](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests).

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

At the ***Package Manager Console***, write the command below.

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
