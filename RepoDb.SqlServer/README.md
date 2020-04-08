[![SqlServerBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-paj1k)](https://ci.appveyor.com/project/mikependon/repodb-paj1k)
[![Home](https://img.shields.io/badge/home-github-important)](https://github.com/mikependon/RepoDb)
[![Website](https://img.shields.io/badge/website-repodb.net-yellow)](http://repodb.net)
[![GetStarted](https://img.shields.io/badge/tutorial-getstarted-blueviolet)](http://repodb.net/tutorial/get-started-sqlserver)
[![SqlServerVersion](https://img.shields.io/nuget/v/RepoDb.SqlServer)](https://www.nuget.org/packages/RepoDb.SqlServer)
[![SqlServerUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-iqu81?label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-iqu81/build/tests)
[![SqlServerIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qja7a?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-qja7a/build/tests)

# RepoDb.SqlServer - a hybrid .NET ORM library for SqlServer.

RepoDb is an open-source .NET ORM that bridge the gaps between micro-ORMs and full-ORMs. It helps the developer to simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

It is the best alternative ORM to both Dapper and EntityFramework.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) - the data provider used for SqlServer.
- [RepoDb](https://www.nuget.org/packages/RepoDb.SqLite/) - the core library of RepoDb.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.SqlServer
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.SqlServerBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

Or, visit the official [get-started](http://repodb.net/tutorial/get-started-sqlserver) page for SQL Server.

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