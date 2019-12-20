
<p align="center">
	<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Icons/RepoDb-64x64.png" height="64px" />
</p>

<p align="center">
	<b>A hybrid ORM Library for .NET</b>
</p>

-----------------

[![SolutionBuilds](https://img.shields.io/appveyor/ci/mikependon/repodb-h87g9?label=sln%20builds&style=for-the-badge)](https://ci.appveyor.com/project/mikependon/repodb-h87g9)
[![CodeSize](https://img.shields.io/github/languages/code-size/mikependon/repodb?style=for-the-badge)](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?color=48B293&style=for-the-badge)](https://gitter.im/RepoDb/community)

## Introduction

RepoDb provide certain features of both micro-ORMs and macro-ORMs. It helps the developer to simplify the “switchover” of when to use the “basic” and “advance” operations during the development.

### It is high-performant

This refers to “how fast” RepoDb converts the raw data into a class object and transport the class object as an actual data in the database.

RepoDb has its own compiler. It caches the “already-generated” compiled-ILs and compiled-Expressions and reusing them for the upcoming transformations. Furthermore, RepoDb also caches the “already-executed” operation-context and reusing it for future calls.

### It is efficient

This refers to “how well-managed” RepoDb uses the computer memory when manipulating the objects all throughout the cycle of the operations.

RepoDb caches the “already-extracted” object properties, mappings and SQL statements and reusing them throughout the process of transformations and executions. It helps eliminate the creation of unnecessary objects that leads to a low memory consumption.

## Builds and Tests Result

Project/Solution                                                                | Build                                                                                                                                                   | Version                                                                                                                          | Downloads                                                                                                                    | Unit Tests                                                                                                                                                                 | IntegrationTests                                                                                                                                                                  |
--------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
[RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core)     | [![CoreBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-ek0nw?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-ek0nw)   | [![CoreVersion](https://img.shields.io/nuget/v/RepoDb?style=flat-square)](https://www.nuget.org/packages/RepoDb)                 | [![CoreDL](https://img.shields.io/nuget/dt/repodb?style=flat-square)](https://www.nuget.org/packages/RepoDb)                 | [![CoreUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-yf1cx?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-yf1cx/build/tests)   | [![CoreIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qksas?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-qksas/build/tests)   |
[RepoDb.SqLite](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SqLite) | [![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o6787) | [![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite) | [![SqLiteDL](https://img.shields.io/nuget/dt/repodb.sqlite?style=flat-square)](https://www.nuget.org/packages/RepoDb.SqLite) | [![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests) | [![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests) |
[RepoDb.MySql](https://github.com/mikependon/RepoDb/tree/master/RepoDb.MySql)   | [![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)  | [![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)    | [![MySqlDL](https://img.shields.io/nuget/dt/repodb.mysql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)    | [![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)  | [![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)  |

## Supported Databases

Practically, RepoDb has supported all RDBMS data-providers. Developers has the freedom to write their own SQL statements and execute it against the database through the *Execute()* methods mentioned below.

- [ExecuteQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery)
- [ExecuteNonQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery)
- [ExecuteScalar](https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar)
- [ExecuteReader](https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader)
- [ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)

### Fully supported databases for fluent-methods

<img src="https://github.com/mikependon/RepoDb/blob/master/RepoDb.Raw/Images/SqlServer.png?raw=true" height="64px" title="SqlServer" /> <img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/SqLite.png" height="64px" title="SqLite" /> <img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/Images/MySql.png" height="64px" title="MySql" />

RepoDb has “fluent” methods (see [Operations](https://github.com/mikependon/RepoDb#operations) section) in which the SQL Statements are automatically being constructed as part of the execution context. These methods are the most common operations being used by most developers (see *Operation* section). In this regards, RepoDb only fully supported the *SQL Server*, *SQLite*, *MySQL* and *PostgreSQL (soon)* data providers.

### Extensibility

RepoDb is highly extensible to further support other RDBMS data-providers. The developers only need to implement certain interfaces to make it work. There will be detailed documentation soon. For now, please contact me (as an author) for help.

## Community

RepoDb is rapidly expanding its capability to further support other RDBMS data-providers (in which each implementation differs from each other). Though it is **not** a macro-ORM, but it really requires significant amount of time and effort to maintain.

It is now open for *community contributions* to further enhance the features and as well the *community engagements*. Please help me spread the word about this new library and its capability.

### Engagements

As an author, I would like to build a healthy and active community that would help fellow .NET developers build the knowledge-base when it comes to database accessibility. Please get in touch via:

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

Any help from the community will be highly appreciated as it really helps me eliminate my full-efforts. 

## Contributions

The folder [RepoDb.Core](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Core) is the code-line built via *NetStandard* (supports both *NetFramework* and *NetCore*). **Any pull-request must be done on this code-line.**

The folder [RepoDb](https://github.com/mikependon/RepoDb/tree/master/RepoDb) is the code-line that supports the *NetFramework* solutions. This code-line is already **out-of-support** (since **v1.10.1**).

To contribute, open the [issues](https://github.com/mikependon/RepoDb/issues) tab and filter the list of items with [for-grabs](https://github.com/mikependon/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) label. Otherwise, create a [new issue](https://github.com/mikependon/RepoDb/issues/new) for us to look-at and discuss.

## Benchmark

The benchmark result to be shown on this page will always be referring to the *community-approved* ORM bencher tool (the [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) tool).

Results below is the actual recent official execution [result](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netcore.txt).

<img src="https://raw.githubusercontent.com/mikependon/RepoDb/master/RepoDb.Raw/RDAB/RDAB-Result.PNG" height="460px" />

RepoDb is the *fastest* and the *most-efficient* ORM as per the official [result](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20190520_netcore.txt) of [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) tool.

This section will always be updated with the latest official result.

## Operations

Below are the list of operations available at RepoDb.

Operation                                                                                                 | Normal<TEntity> | Normal<TEntity> (Async) | TableName | TableName (Async) | Packed Execution | Data Providers         |
----------------------------------------------------------------------------------------------------------|-----------------|-------------------------|-----------|-------------------|------------------|------------------------|
[Average](https://repodb.readthedocs.io/en/latest/pages/connection.html#average)                          | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[AverageAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#averageALL)                    | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[BatchQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#batchquery)                    | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[BulkInsert](https://repodb.readthedocs.io/en/latest/pages/connection.html#bulkinsert)                    | YES             | YES                     | YES       | YES               | NO               | SQLSVR, POSTGRESQL     |
[Count](https://repodb.readthedocs.io/en/latest/pages/connection.html#count)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[CountAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#countall)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Delete](https://repodb.readthedocs.io/en/latest/pages/connection.html#delete)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[DeleteAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#deleteall)                      | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[ExecuteNonQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executenonquery)          | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteQuery](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequery)                | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteQueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#executequerymultiple)| YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteReader](https://repodb.readthedocs.io/en/latest/pages/connection.html#executereader)              | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[ExecuteScalar](https://repodb.readthedocs.io/en/latest/pages/connection.html#executescalar)              | YES             | YES                     | NO        | NO                | NO               | ALL                    |
[Exists](https://repodb.readthedocs.io/en/latest/pages/connection.html#exists)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Insert](https://repodb.readthedocs.io/en/latest/pages/connection.html#insert)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[InsertAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#insertall)                      | YES             | YES                     | YES       | YES               | YES         	 | SUPPORTED              |
[Max](https://repodb.readthedocs.io/en/latest/pages/connection.html#max)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MaxAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#maxall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Merge](https://repodb.readthedocs.io/en/latest/pages/connection.html#merge)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MergeAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#mergeall)                        | YES             | YES                     | YES       | YES               | YES              | SUPPORTED              |
[Min](https://repodb.readthedocs.io/en/latest/pages/connection.html#min)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[MinAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#minall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Query](https://repodb.readthedocs.io/en/latest/pages/connection.html#query)                              | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[QueryAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#queryall)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[QueryMultiple](https://repodb.readthedocs.io/en/latest/pages/connection.html#querymultiple)              | YES             | YES                     | NO        | NO                | YES              | SUPPORTED              |
[Sum](https://repodb.readthedocs.io/en/latest/pages/connection.html#sum)                                  | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[SumAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#sumall)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Truncate](https://repodb.readthedocs.io/en/latest/pages/connection.html#truncate)                        | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[Update](https://repodb.readthedocs.io/en/latest/pages/connection.html#update)                            | YES             | YES                     | YES       | YES               | NO               | SUPPORTED              |
[UpdateAll](https://repodb.readthedocs.io/en/latest/pages/connection.html#updateall)                      | YES             | YES                     | YES       | YES               | YES              | SUPPORTED              |

## Credits

- [GitHub](https://github.com/) - for hosting this project.
- [AppVeyor](https://www.appveyor.com/) - for the builds and test-executions.
- [Shields](https://shields.io/) - for the badges.
- [Nuget](https://www.nuget.org/) - for the package delivery.
- [StackEdit](https://stackedit.io) - for being the markdown file editor.
- [Gitter](https://gitter.im/) - for the community engagements.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

-----------------

## Learnings

Below are the links that would help the developers do some *practical* (or *actual*) implementations when using the *RepoDb* library.

- [Getting started](https://github.com/mikependon/RepoDb/wiki/Getting-Started)
- [Implementing a Repository](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Repository)
- [Making the Repositories Dependency-Injectable](https://github.com/mikependon/RepoDb/wiki/Making-the-Repositories-Dependency-Injectable)
- [Bulk-Operations vs Batch-Operations](https://github.com/mikependon/RepoDb/wiki/Batch-Operations-vs-Bulk-Operations)
- [Multiple Resultsets via QueryMultiple and ExecuteQueryMultiple](https://github.com/mikependon/RepoDb/wiki/Multiple-Resultsets-via-QueryMultiple-and-ExecuteQueryMultiple)
- [Working with Transactions](https://github.com/mikependon/RepoDb/wiki/Working-with-Transactions)
- [Expression Trees](https://github.com/mikependon/RepoDb/wiki/Expression-Trees)
- [Advance Field and Type Mapping Implementations](https://github.com/mikependon/RepoDb/wiki/Advance-Field-and-Type-Mapping-Implementation)
- [Customizing a Cache](https://github.com/mikependon/RepoDb/wiki/Customizing-a-Cache)
- [Implementing a Trace](https://github.com/mikependon/RepoDb/wiki/Implementing-a-Trace)
- [The importance of Connection Persistency](https://github.com/mikependon/RepoDb/wiki/The-importance-of-Connection-Persistency)
- [Working with Enumerations](https://github.com/mikependon/RepoDb/wiki/Working-with-Enumerations)
- [Extending the supports for specific DB Provider](https://github.com/mikependon/RepoDb/wiki/Extending-the-supports-for-specific-DB-Provider)

There will also be high-level implementations and sample code-snippets on the following sections to help you start with.

Otherwise, please contact [me](https://repodb.readthedocs.io/en/latest/pages/contact.html) directly or chat us via [Gitter](https://gitter.im/RepoDb/community).

## Installations

RepoDb and its extension is available via Nuget as a NetStandard library. Type the commands below at the *Package Manager Console* window.

```
Install-Package RepoDb
Install-Package RepoDb.SqLite
Install-Package RepoDb.MySql
```

## Snippets and Samples

Let us say you have a customer class named *Customer* that has an equivalent table in the database named `[dbo].[Customer]`.

```csharp
public class Customer
{
	public int Id { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public bool IsActive { get; set; }
	public DateTime LastUpdatedUtc { get; set; }
	public DateTime CreatedDateUtc { get; set; }
}
```

### Querying a Data

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(new QueryField(nameof(Customer.Id), 10045));
}
```

### Querying via TableName

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new { Id = 10045 });
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new QueryField(nameof(Customer.Id), 10045));
}
```
	
Via Object (targetting few fields):

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query("Customer", new QueryField(nameof(Customer.Id), 10045),
		Field.From("Id", "FirstName", "LastName"));
}
```

### Inserting a Data

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer, int>(customer);
}
```

### Inserting via TableName

```csharp
var customer = new
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true,
	LastUpdatedUtc = DateTime.UtcNow,
	CreatedDateUtc = DateTime.UtcNow
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<int>("Customer", customer);
}
```

### Updating a Data

Via DataEntity:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, e => e.Id == 10045);
}
```
	
Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer, new QueryField(nameof(Customer.Id), 10045));
}
```

### Updating via TableName

Via Dynamic Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		Id = 10045,
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, new { Id = 10045 });
}
```
	
Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		FirstName = "John",
		LastUpdatedUtc = DateTime.UtcNow
	};
	var affectedRows = connection.Update("Customer", customer, new QueryField("Id", 10045));
}
```

### Deleting a Data

Via DataEntity:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(new { Id = 10045 });
}
```

Via Expression:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(c => c.Id == 10045);
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete<Customer>(new QueryField(nameof(Customer.Id), 10045));
}
```

### Deleting via TableName

Via PrimaryKey:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", 10045);
}
```

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", { Id = 10045 });
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedCount = connection.Delete("Customer", new QueryField(nameof(Customer.Id), 10045));
}
```

### Merging a Data

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true,
	LastUpdatedUtc = DateTime.Utc,
	CreatedDateUtc = DateTime.Utc
};
using (var connection = new SqlConnection(ConnectionString))
{
	var qualifiers = new []
	{
		new Field(nameof(Customer.FirstName)),
		new Field(nameof(Customer.LastName)),
	};
	var mergeCount = connection.Merge<Customer>(customer, qualifiers);
}
```

### Merging via TableName

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var qualifiers = new []
	{
		new Field(nameof(Customer.FirstName)),
		new Field(nameof(Customer.LastName)),
	};
	var mergeCount = connection.Merge("Customer", customer, qualifiers);
}
```

### Executing a Customized-Query

You can create a class with combined properties of different tables or with stored procedures. It does not need to be 100% identical to the schema, as long the property of the class is part of the result set.

```csharp
public class ComplexClass
{
	public int CustomerId { get; set; }
	public int OrderId { get; set; }
	public int ProductId { get; set; }
	public string CustomerName { get; set; }
	public string ProductName { get; set; }
	public DateTime ProductDescription { get; set; } // This is not in the CommandText, will be ignored
	public DateTime OrderDate { get; set; }
	public int Quantity { get; set; }
	public double Price { get; set; }
}
```

Then you can create this command text.

	var commandText = @"SELECT C.Id AS CustomerId
		, O.Id AS OrderId
		, P.Id AS ProductId
		, CONCAT(C.FirstName, ' ', C.LastName) AS CustomerName
		, P.Name AS ProductName
		, O.OrderDate
		, O.Quantity
		, P.Price
		, (O.Quatity * P.Price) AS Total /* Note: This is not in the class, but still it is valid */
	FROM [dbo].[Customer] C
	INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id
	INNER JOIN [dbo].[OrderItem] OI ON OI.OrderId = O.Id
	INNER JOIN [dbo].[Product] P ON P.Id = OI.ProductId
	WHERE (C.Id = @CustomerId)
		AND (O.OrderDate BETWEEN @OrderDate AND DATEADD(DAY, 1, @OrderDate));";

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<ComplexClass>(commandText, new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date });
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var queryGroup = new QueryGroup(new []
	{
		new QueryField("CustomerId", 10045),
		new QueryField("OrderDate", DateTime.UtcNow.Date)
	});
	var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup);
}
```

The `ExecuteQuery` method is purposely not being supported by `Expression` based query as we are avoiding the user to bind the complex-class to its target query text.

Note: The most optimal when it comes to performance is to used the `Object-Based`.

### Calling a StoredProcedure

Using the complex type above. If you have a stored procedure like below.

	DROP PROCEDURE IF EXISTS [dbo].[sp_get_customer_orders_by_date];
	GO
	CREATE PROCEDURE [dbo].[sp_get_customer_orders_by_date]
	(
		@CustomerId INT
		, @OrderDate DATETIME2(7)
	)
	AS
	BEGIN
		SELECT C.Id AS CustomerId
			, O.Id AS OrderId
			, P.Id AS ProductId
			, CONCAT(C.FirstName, ' ', C.LastName) AS CustomerName
			, P.Name AS ProductName
			, O.OrderDate
			, O.Quantity
			, P.Price
			, (O.Quatity * P.Price) AS Total /* Note: This is not in the class, but still it is valid */
		FROM [dbo].[Customer] C
		INNER JOIN [dbo].[Order] O ON O.CustomerId = C.Id
		INNER JOIN [dbo].[OrderItem] OI ON OI.OrderId = O.Id
		INNER JOIN [dbo].[Product] P ON P.Id = OI.ProductId
		WHERE (C.Id = @CustomerId)
			AND (O.OrderDate BETWEEN @OrderDate AND DATEADD(DAY, 1, @OrderDate));
	END

Then it can be called as below.

Via Dynamic:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<ComplexClass>("[dbo].[sp_get_customer_orders_by_date]",
		param: new { CustomerId = 10045, OrderDate = DateTime.UtcNow.Date },
		commandType: CommandType.StoredProcedure);
}
```

Via Object:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var queryGroup = new QueryGroup(new []
	{
		new QueryField("CustomerId", 10045),
		new QueryField("OrderDate", DateTime.UtcNow.Date)
	});
	var customer = connection.ExecuteQuery<Customer>(commandText, queryGroup,
		commandType: CommandType.StoredProcedure);
}
```

Please visit the [documentation](https://repodb.readthedocs.io/en/latest/) for further details about the library.
