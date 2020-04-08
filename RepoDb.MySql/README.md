[![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![Home](https://img.shields.io/badge/home-github-important)](https://github.com/mikependon/RepoDb)
[![Website](https://img.shields.io/badge/website-information-yellow)](http://repodb.net)
[![GetStarted](https://img.shields.io/badge/tutorial-getstarted-blueviolet)](http://repodb.net/tutorial/get-started-mysql)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

# RepoDb.MySql - a hybrid .NET ORM library for MySQL.

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

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/) - the data provider used for MySql.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of RepoDb.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

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

To learn more, please visit our [reference implementations](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md) page.
