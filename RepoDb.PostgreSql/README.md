[![PostgreSqlBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-pgsql.yml?logo=github&label=build%20and%20tests&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-pgsql.yml)
[![PostgreSqlHome](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![PostgreSqlVersion](https://img.shields.io/nuget/v/RepoDb.PostgreSql?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.PostgreSql)

# [RepoDb.PostgreSql](https://repodb.net/tutorial/get-started-postgresql) — RepoDB for PostgreSQL

The PostgreSQL provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) — technical questions.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://twitter.com/search?q=%23repodb) — news and updates.

## Dependencies

- [Npgsql](https://www.nuget.org/packages/Npgsql/) — PostgreSQL data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://twitter.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.PostgreSql
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
RepoDb.PostgreSqlBootstrap.Initialize();
```

Then use any RepoDB operation directly on your `NpgsqlConnection`:

### Query

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
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
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-postgresql) page for the full PostgreSQL guide.
