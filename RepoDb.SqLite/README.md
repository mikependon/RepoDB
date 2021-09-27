[![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787?&logo=appveyor)](https://ci.appveyor.com/project/mikependon/repodb-o6787)
[![SqLiteHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite?&logo=nuget)](https://www.nuget.org/packages/RepoDb.SqLite)
[![SqLiteReleases](https://img.shields.io/badge/releases-core-important?&logo=nuget)](http://repodb.net/release/sqlite)
[![SqLiteUnitTests](https://img.shields.io/appveyor/tests/mikependon/repodb-mhpo4?&logo=appveyor&label=unit%20tests)](https://ci.appveyor.com/project/mikependon/repodb-mhpo4/build/tests)
[![SqLiteIntegrationTests](https://img.shields.io/appveyor/tests/mikependon/repodb-eg27p?&logo=appveyor&label=integration%20tests)](https://ci.appveyor.com/project/mikependon/repodb-eg27p/build/tests)

# RepoDb.SqLite - a hybrid .NET ORM library for SQLite.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

## Contribution

```diff
- This code-line will be out-of-support soon. The final version would be v1.1.4.
```

To contribute, please proceed to the new solutions.

- Microsoft.Data.Sqlite - [RepoDb.Sqlite.Microsoft](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Sqlite.Microsoft)
- System.Data.SQLite.Core - [RepoDb.SQLite.System](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SQLite.System)

## Important Pages

- [GitHub Home Page](https://github.com/mikependon/RepoDb) - to learn more about the core library.
- [Website](http://repodb.net) - docs, features, classes, references, releases and blogs.

## Community engagements

- [GitHub](https://github.com/mikependon/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [RepoDb](https://www.nuget.org/packages/RepoDb/) - the core library of RepoDB.
- [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/) - the data provider used for SqLite (Microsoft).
- [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/) - the data provider used for SqLite (SQLite).

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package RepoDb.SqLite
```

Or, visit our [installation](http://repodb.net/tutorial/installation) page for more information.

## Get Started

First, the bootstrapper must be initialized.

```csharp
RepoDb.SqLiteBootstrap.Initialize();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

Or, visit the official [get-started](http://repodb.net/tutorial/get-started-sqlite) page for SQLite.

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
