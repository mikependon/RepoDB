[![MySqlConnectorBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-7ooj1?&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-7ooj1)
[![MySqlConnectorHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![MySqlConnectorVersion](https://img.shields.io/nuget/v/RepoDb.MySqlConnector?&logo=nuget)](https://www.nuget.org/packages/RepoDb.MySqlConnector)
[![MySqlConnectorReleases](https://img.shields.io/badge/releases-core-important?&logo=nuget)](http://repodb.net/release/mysqlconnector)
[![MySqlConnectorUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-pqvj7?&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-pqvj7/build/tests)
[![MySqlConnectorIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-4iutn?&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-4iutn/build/tests)

# [RepoDb.MySqlConnector](https://repodb.net/tutorial/get-started-mysql) - a hybrid .NET ORM library for MySQL (using MySqlConnector)

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Community Engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/) - the data provider used for MySqlConnector.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of RepoDB.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2020 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.MySqlConnector
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.MySqlConnectorBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

Or, visit the official [get-started](http://repodb.net/tutorial/get-started-mysql) page for MySQL.

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