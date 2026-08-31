[![SapHanaBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-saphana.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-saphana.yml)
[![SapHanaHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![SapHanaVersion](https://img.shields.io/nuget/v/RepoDb.SapHana?&logo=nuget)](https://www.nuget.org/packages/RepoDb.SapHana)

# [RepoDb.SapHana](https://repodb.net/tutorial/get-started-saphana) — RepoDB for SAP HANA

The SAP HANA provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and the [Sap.Data.Hana.Net.v6.0](https://www.nuget.org/packages/Sap.Data.Hana.Net.v6.0) ADO.NET driver.

> This provider has not been verified against a live SAP HANA instance. See the code comments in this project for the specific assumptions made (parameter prefix, driver type names, etc.) and verify them before relying on this in production.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [GitHub Discussions](https://github.com/mikependon/RepoDB/discussions) — ask questions and share ideas.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Sap.Data.Hana.Net.v6.0](https://www.nuget.org/packages/Sap.Data.Hana.Net.v6.0/) — the SAP HANA ADO.NET data provider.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2020 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.SapHana
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseSapHana();
```

Then use any RepoDB operation directly on your `HanaConnection`:

### Query

```csharp
using (var connection = new HanaConnection(ConnectionString))
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
using (var connection = new HanaConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new HanaConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new HanaConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new HanaConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (Id = :Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new HanaConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET FirstName = :FirstName WHERE (Id = :Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new HanaConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Customer\";");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-saphana) page for the full SAP HANA guide.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2019 [Michael Camara Pendon](https://x.com/mike_pendon)
