[![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)
[![CoreHome](https://img.shields.io/badge/home-github-important?style=flat-square&logo=github)](https://github.com/mikependon/RepoDb)
[![CoreVersion](https://img.shields.io/nuget/v/RepoDb?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RepoDb)
[![CoreReleases](https://img.shields.io/badge/releases-core-important?style=flat-square&logo=nuget)](http://repodb.net/release/core)
[![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx?style=flat-square&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)
[![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas?style=flat-square&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)

# [RepoDB](http://repodb.net) - a hybrid ORM library for .NET.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

It is your best alternative ORM to both Dapper and EntityFramework.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Core Features
 
- [Batch Operations](http://repodb.net/feature/batchoperations)
- [Bulk Operations](http://repodb.net/feature/bulkoperations)
- [Caching](http://repodb.net/feature/caching)
- [Class Handlers](http://repodb.net/feature/classhandlers)
- [Class Mapping](http://repodb.net/feature/classmapping)
- [Dynamics](http://repodb.net/feature/dynamics)
- [Connection Persistency](http://repodb.net/feature/connectionpersistency)
- [Enumeration](http://repodb.net/feature/enumeration)
- [Expression Trees](http://repodb.net/feature/expressiontrees)
- [Hints](http://repodb.net/feature/hints)
- [Implicit Mapping](http://repodb.net/feature/implicitmapping)
- [Multiple Query](http://repodb.net/feature/multiplequery)
- [Property Handlers](http://repodb.net/feature/propertyhandlers)
- [Repositories](http://repodb.net/feature/repositories)
- [Targeted Operations](http://repodb.net/feature/targeted)
- [Tracing](http://repodb.net/feature/tracing)
- [Transaction](http://repodb.net/feature/transaction)
- [Type Mapping](http://repodb.net/feature/typemapping)

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```
Install-Package RepoDB
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

After the installation, any library operation can then be called. Please see below for the samples.

### Query

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### Insert

```csharp
var customer = new
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.ExecuteScalar<int>("INSERT INTO [dbo].[Customer](FirstName, LastName, IsActive) VALUES (@FirstName, @LastName, @IsActive); SELECT SCOPE_IDENTITY();", customer);
}
```

### Update

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		Id = 10045,
		FirstName = "John",
		LastName = "Doe"
	};
	var affectedRows = connection.ExecuteNonQuery("UPDATE [dbo].[Customer] SET FirstName = @FirstName, LastName = @LastName, LastUpdatedUtc = GETUTCDATE() WHERE (Id = @Id);", customer);
}
```

### Delete

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedRows = connection.ExecuteNonQuery("DELETE FROM [dbo].[Customer] WHERE (Id = @Id)", new { Id = 10045 });
}
```

### StoredProcedure

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcdure).FirstOrDefault();
}
```

Or via direct calls.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("EXEC [dbo].[sp_GetCustomer](@Id);", new { Id = 10045 }).FirstOrDefault();
}
```

Or, visit the official [get-started](http://repodb.net/tutorial/get-started-sqlserver) page for SQL Server.
