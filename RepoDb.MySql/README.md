[![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![MySqlHome](https://img.shields.io/badge/home-github-important?style=flat-square&logo=github)](https://github.com/mikependon/RepoDb)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?style=flat-square&logo=nuget)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlReleases](https://img.shields.io/badge/releases-core-important?style=flat-square&logo=nuget)](http://repodb.net/release/mysql)
[![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?style=flat-square&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?style=flat-square&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

# RepoDb.MySql - a hybrid .NET ORM library for MySQL.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

It is your best alternative ORM to both Dapper and EntityFramework.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/) - the data provider used for MySql.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of RepoDB.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.MySql
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.MySqlBootstrap.Initialize();
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
