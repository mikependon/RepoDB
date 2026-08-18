[![MySqlBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-mysql.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-mysql.yml)
[![MySqlHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![MySqlVersion](https://img.shields.io/nuget/v/RepoDb.MySql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.MySql)

# [RepoDb.MySql](https://repodb.net/tutorial/get-started-mysql) — RepoDB for MySQL (MySql.Data)

The MySQL provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [MySql.Data](https://www.nuget.org/packages/MySql.Data).

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [MySql.Data](https://www.nuget.org/packages/MySql.Data/) — MySQL data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.MySql
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseMySql();
```

Then use any RepoDB operation directly on your `MySqlConnection`:

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

### ExecuteQuery

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM `Customer` WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE `Customer` SET FirstName = @FirstName WHERE (Id = @Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new MySqlConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Customer`;");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-mysql) page for the full MySQL guide.

## License

[Apache License 2.0](https://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon) 

This project depends on Oracle's MySQL Connector/NET (`MySql.Data`),
which is separately licensed under GPL-2.0 with the Universal FOSS
Exception, Version 1.0.
