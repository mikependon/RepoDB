[![SqLiteSystemBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-sqlite-system.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-sqlite-system.yml)
[![SqLiteSystemHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![SqLiteSystemVersion](https://img.shields.io/nuget/v/RepoDb.SQLite.System?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SQLite.System)

# [RepoDb.SQLite.System](https://repodb.net/tutorial/get-started-sqlite) — RepoDB for SQLite (System.Data.SQLite)

The SQLite provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Backed by `System.Data.SQLite.Core`.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## Dependencies

- [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/) — SQLite data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.SQLite.System
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
RepoDb.SQLiteBootstrap.Initialize();
```

Then use any RepoDB operation directly on your `SQLiteConnection`:

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

Visit the [get-started](http://repodb.net/tutorial/get-started-sqlite) page for the full SQLite guide.
