[![CoreBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlsvr.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlsvr.yml)
[![CoreHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![CoreVersion](https://img.shields.io/nuget/v/RepoDb?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb)

# [RepoDB](http://repodb.net) — a production-ready data access platform for .NET applications.

RepoDB is a high-performance, open-source data access platform for .NET applications. It combines the simplicity of a micro-ORM with the productivity of a full ORM, giving developers the freedom to use raw SQL for precise control or fluent operations for faster development—all through the same IDbConnection.

Write raw SQL when you need full control. Use fluent methods when you want productivity. Switch between them freely, in the same codebase.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Core Features

- [Batch Operations](http://repodb.net/feature/batchoperations)
- [Bulk Operations](http://repodb.net/feature/bulkoperations)
- [Caching](http://repodb.net/feature/caching)
- [Class Handlers](http://repodb.net/feature/classhandlers)
- [Class Mapping](http://repodb.net/feature/classmapping)
- [Connection Persistency](http://repodb.net/feature/connectionpersistency)
- [Dynamics](http://repodb.net/feature/dynamics)
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

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDB
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

All RepoDB operations are extension methods on `IDbConnection` — no repository classes or extra setup required.

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

### Stored Procedure

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcedure).FirstOrDefault();
}
```

Or via inline SQL:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("EXEC [dbo].[sp_GetCustomer](@Id);", new { Id = 10045 }).FirstOrDefault();
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-sqlserver) page for the full SQL Server guide.
