[![SqlServerBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-paj1k?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-paj1k)
[![SqlServerHome](https://img.shields.io/badge/home-github-important?style=flat-square&logo=github)](https://github.com/mikependon/RepoDb)
[![SqlServerVersion](https://img.shields.io/nuget/v/RepoDb.SqlServer?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RepoDb.SqlServer)
[![SqlServerReleases](https://img.shields.io/badge/releases-core-important?style=flat-square&logo=nuget)](http://repodb.net/release/sqlserver)
[![SqlServerUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-iqu81?style=flat-square&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-iqu81/build/tests)
[![SqlServerIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-qja7a?style=flat-square&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-qja7a/build/tests)

# RepoDb.SqlServer - a hybrid .NET ORM library for SqlServer.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

It is your best alternative ORM to both Dapper and EntityFramework.

## News/Updates

Starting at version 1.12.9, we will issue a deprecation notice to [System.Data.SqlClient](https://www.nuget.org/packages/System.Data.SqlClient/). In the future, RepoDB will only be supporting the [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) package.

### Working with System.Data.SqlClient

If you wish to continue working with this package, you have to bootstrap the `SqlConnection` object with the code below (in the Startup).

```csharp
var dbSetting = new SqlServerDbSetting();

DbSettingMapper
	.Add<System.Data.SqlClient.SqlConnection>(dbSetting, true);
DbHelperMapper
	.Add<System.Data.SqlClient.SqlConnection>(new SqlServerDbHelper(), true);
StatementBuilderMapper
	.Add<System.Data.SqlClient.SqlConnection>(new SqlServerStatementBuilder(dbSetting), true);
```

Or, you can replicate the actual [SqlServerBootstrap](https://github.com/mikependon/RepoDB/blob/master/RepoDb.SqlServer/RepoDb.SqlServer/SqlServerBootstrap.cs) class implementation and attach it to your solution. Then, call the local class `Initialize()` method explicitly.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) - the data provider used for SqlServer.
- [RepoDb](https://www.nuget.org/packages/RepoDb.SqLite/) - the core library of RepoDB.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2020 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

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
