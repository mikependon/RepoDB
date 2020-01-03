[![MySqlBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-6adn4?style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-6adn4)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlDL](https://img.shields.io/nuget/dt/repodb.mysql?style=flat-square)](https://www.nuget.org/packages/RepoDb.MySql)
[![MySqlUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-t2hy7?label=unit&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-t2hy7/build/tests)
[![MySqlIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-o4t48?label=integration&style=flat-square)](https://ci.appveyor.com/project/mikependon/repodb-o4t48/build/tests)

## RepoDb.MySql - a hybrid .NET ORM library for MySql.

RepoDb provides certain features of both “micro-ORMs” and “macro-ORMs”. It helps the developer to simplify the “switchover” of when to use the “basic” and “advance” operations during the development.

All [operations](https://github.com/mikependon/RepoDb#operations) were implemented as extended methods of the *IDbConnection* object. Once you hold the opened-state of your database connection object, you can then do all the activities you would like to do with your database through those extended methods.

## Links and Resources

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/) - the data provider used for *MySql*.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of *RepoDb*.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

--------

## Installation

At the ***Package Manager Console***, write the command below.

```
Install-Package RepoDb.MySql
```

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

To learn more, please visit our [reference implementations](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md) page.
