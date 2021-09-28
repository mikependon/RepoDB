[![SqLiteMicrosoftBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-94v1s?&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-94v1s)
[![SqLiteMicrosoftHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![SqLiteMicrosoftVersion](https://img.shields.io/nuget/v/RepoDb.Sqlite.Microsoft?&logo=nuget)](https://www.nuget.org/packages/RepoDb.Sqlite.Microsoft)
[![SqLiteMicrosoftReleases](https://img.shields.io/badge/releases-core-important?&logo=nuget)](http://repodb.net/release/sqlite-microsoft)
[![SqLiteMicrosoftUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-jvodo?&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-jvodo/build/tests)
[![SqLiteMicrosoftIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-9lhxq?&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-9lhxq/build/tests)

# [RepoDb.Sqlite.Microsoft](https://repodb.net/tutorial/get-started-sqlite) - a hybrid .NET ORM library for SQLite (using Microsoft.Data.Sqlite).

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

- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of RepoDB.
- [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/) - the data provider used for SQLite (Microsoft).

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.Sqlite.Microsoft
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.SqliteBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

Or, visit the official [get-started](http://repodb.net/tutorial/get-started-sqlite) page for SQLite.

### Query

```csharp
using (var connection = new SqliteConnection(ConnectionString))
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
using (var connection = new SqliteConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new SqliteConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```
