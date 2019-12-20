[![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlDL](https://img.shields.io/nuget/dt/repodb.mysql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

## RepoDb.MySql - a hybrid .NET ORM library for MySql.

This is the official repository for **RepoDb.MySql** solution.

## Introduction

- It has all the functionalities of [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) implementation.
- It has [batch operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations).
- It is a unique and hybrid solution for ***MySql*** data-provider within ***.NET Technology***.
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
Install-Package RepoDb.MySql
```

## Learnings (In-Progress)

This section is specialized for MySql. To read more about the library, please visit the [Learnings](https://github.com/mikependon/RepoDb#learnings-in-progress) section of our [GitHub](https://github.com/mikependon/RepoDb) page.

- [Working with Spatial DataTypes (Geometry, LineString, Point and Polygon)](https://www.nuget.org/packages/RepoDb).

## Getting Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.MySqlBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

### Query

```csharp
using (var connection = new MySqlConnection(ConnectionString))
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
using (var connection = new MySqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

## Dependencies

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/)
- [RepoDb](https://www.nuget.org/packages/RepoDb/)

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

Please refer to RepoDb [GitHub](https://github.com/mikependon/RepoDb) page for further information.
