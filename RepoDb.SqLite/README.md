[![SqLiteBuild](https://img.shields.io/appveyor/ci/mikependon/repodb-o6787?&logo=appveyor&label=build&style=for-the-badge)](https://ci.appveyor.com/project/mikependon/repodb-o6787)
[![SqLiteHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![SqLiteVersion](https://img.shields.io/nuget/v/RepoDb.SqLite?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.SqLite)

# RepoDb.SqLite — RepoDB for SQLite (legacy)

The original SQLite provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection.

```diff
This code-line is out-of-support soon. The final version is v1.1.4.
```

To contribute, use one of the current SQLite providers instead:

- [RepoDb.Sqlite.Microsoft](https://github.com/mikependon/RepoDb/tree/master/RepoDb.Sqlite.Microsoft) — backed by `Microsoft.Data.Sqlite`.
- [RepoDb.SQLite.System](https://github.com/mikependon/RepoDb/tree/master/RepoDb.SQLite.System) — backed by `System.Data.SQLite.Core`.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite.Core/) — one of the SQLite data providers this package can use.
- [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/) — the other supported SQLite data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.SqLite
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
RepoDb.SqLiteBootstrap.Initialize();
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

### ExecuteQuery

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE [Customer] SET FirstName = @FirstName WHERE (Id = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new SQLiteConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM [Customer];");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-sqlite) page for the full SQLite guide.
