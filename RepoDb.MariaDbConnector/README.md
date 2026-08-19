[![MariaDbConnectorBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mariadbconnector.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mariadbconnector.yml)
[![MariaDbConnectorHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![MariaDbConnectorVersion](https://img.shields.io/nuget/v/RepoDb.MariaDbConnector?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MariaDbConnector)

# [RepoDb.MariaDbConnector](https://repodb.net/tutorial/get-started-mysql) — RepoDB for MariaDB (RepoDb.Connector.MariaDbConnector)

The dedicated MariaDB provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [RepoDb.Connector.MariaDbConnector](https://www.nuget.org/packages/RepoDb.Connector.MariaDbConnector), the dedicated MariaDB ADO.NET provider for RepoDB.

> **Disclaimer:** RepoDb.MariaDbConnector is a direct copy of [RepoDb.MySqlConnector](https://www.nuget.org/packages/RepoDb.MySqlConnector) as MariaDB is largely wire- and SQL-compatible with MySQL. It is published as its own, de-facto dedicated package rather than folded into RepoDb.MySqlConnector so that MariaDB support can be versioned, tuned, and evolved independently.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [RepoDb.Connector.MariaDbConnector](https://www.nuget.org/packages/RepoDb.Connector.MariaDbConnector/) — the dedicated MariaDB ADO.NET provider RepoDb.MariaDbConnector connects through, exposing `MariaDbConnection` and its related objects.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.MariaDbConnector
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
RepoDb.MariaDbBootstrap.Initialize();
```

Then use any RepoDB operation directly on your `MariaDbConnection`, pointed at your MariaDB server:

### Query

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
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
using (var connection = new MariaDbConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM `Customer` WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE `Customer` SET FirstName = @FirstName WHERE (Id = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new MariaDbConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Customer`;");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-mysql) page for the full guide (the same walkthrough applies to MariaDB).
