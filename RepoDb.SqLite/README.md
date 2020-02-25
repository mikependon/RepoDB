[![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787)](https://ci.appveyor.com/project/mikependon/repodb-o6787)
[![Home](https://img.shields.io/badge/home-github-important)](https://github.com/mikependon/RepoDb)
[![Wiki](https://img.shields.io/badge/wiki-information-yellow)](https://github.com/mikependon/RepoDb/wiki)
[![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteDL](https://img.shields.io/nuget/dt/repodb.sqlite)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4?label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests)
[![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p?label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests)

## RepoDb.SqLite - a hybrid .NET ORM library for SqLite.

RepoDb is an open-source .NET ORM that bridge the gaps between micro-ORMs and macro-ORMs. It helps the developer to simplify the switch-over of when to use the “BASIC” and “ADVANCE” operations during the development.

It is the best alternative ORM to both Dapper and EntityFramework.

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Wiki Page](https://github.com/mikependon/RepoDb/wiki) - usabilities, benefits, features, capabilities, learnings, topics and FAQs. 

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/questions/tagged/repodb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of *RepoDb*.
- [System.Data.SQLite](https://www.nuget.org/packages/System.Data.SQLite/) - the data provider used for *SqLite*.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - Michael Camara Pendon

--------

## Installation

At the *Package Manager Console*, write the command below.

```csharp
> Install-Package RepoDb.SqLite
```

## Getting Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.SqLiteBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

### Query

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
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
using (var connection = new SQLiteConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

To learn more, please visit our [reference implementations](https://github.com/mikependon/RepoDb/blob/master/RepoDb.Docs/Reference%20Implementations.md) page.
