[![ClickHouseBuild](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-clickhouse.yml?logo=github&label=build)](https://github.com/mikependon/RepoDB/actions/workflows/build-clickhouse.yml)
[![ClickHouseHome](https://img.shields.io/badge/home-github-important?&logo=github)](https://github.com/mikependon/RepoDb)
[![ClickHouseVersion](https://img.shields.io/nuget/v/RepoDb.ClickHouse?&logo=nuget)](https://www.nuget.org/packages/RepoDb.ClickHouse)

# [RepoDb.ClickHouse](https://repodb.net/tutorial/get-started-clickhouse) — RepoDB for ClickHouse

The dedicated ClickHouse provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and [ClickHouse.Driver](https://www.nuget.org/packages/ClickHouse.Driver), the official .NET ADO.NET client for ClickHouse.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [ClickHouse.Driver](https://www.nuget.org/packages/ClickHouse.Driver/) — the official ADO.NET client for ClickHouse that RepoDb.ClickHouse connects through, exposing `ClickHouseConnection`, which RepoDb.ClickHouse wraps as `RepoDbClickHouseConnection` (see the note above) - use that type, not the raw driver one. Include `UseCustomDecimals=false` in your connection string: without it, `Decimal` columns come back as the driver's own `ClickHouseDecimal` numeric type rather than a plain `decimal`, which RepoDb's compiled reader cannot cast to a `decimal`/`decimal?` entity property.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## Disclaimer

> **DateTime64 columns:** ClickHouse.Driver infers a plain .NET `DateTime` parameter's type as `DateTime` (whole-second precision) - it has no visibility into the actual target column's type. Writing a `DateTime` value that carries fractional seconds into a `DateTime64(N)` column silently truncates them to whole seconds unless the parameter's type is stated explicitly. Decorate the property with `[RepoDb.Attributes.Parameter.ClickHouse.ClickHouseType("Nullable(DateTime64(3))")]` (matching the column's actual scale) to preserve sub-second precision on Insert/Update.

```csharp
public class Customer
{
	public long Id { get; set; }

	[ClickHouseType("Nullable(DateTime64(3))")]
	public DateTime? LastUpdatedUtc { get; set; }
}
```

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.ClickHouse
```

Or visit the [installation](http://repodb.net/tutorial/installation) page for more options.

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseClickHouse();
```

> **Waiting for mutations:** ClickHouse's `ALTER TABLE ... UPDATE`/`DELETE` run as asynchronous background
> mutations rather than synchronous statements, so a row change is not guaranteed to be visible the instant
> the call returns. Pass `isWaitForMutationsEnabled: true` to `UseClickHouse()` to have this waited on -
> honored by `RepoDb.ClickHouse.BulkOperations`'s `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey`,
> which block until each mutation actually completes (up to a 5-second timeout) before returning, at the
> cost of extra latency per call. It defaults to `false` here (fire-and-forget); the underlying
> `ClickHouseDbSetting.IsWaitForMutationsEnabled` property itself defaults to `true` when the setting is
> constructed directly rather than through `UseClickHouse()`.

Then use any RepoDB operation directly on your `RepoDbClickHouseConnection`, pointed at your ClickHouse server:

### Query

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

### Insert

```csharp
var customer = new Customer
{
	Id = 10046,
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	// ClickHouse has no server-generated identity - the primary key is supplied by the caller
	connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045).First();
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;

	// Translated to an asynchronous 'ALTER TABLE ... UPDATE ... WHERE' mutation
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045).First();
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM `Customer` WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("ALTER TABLE `Customer` UPDATE FirstName = @FirstName WHERE Id = @Id;", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new RepoDbClickHouseConnection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM `Customer`;");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-clickhouse) page for the full guide.
