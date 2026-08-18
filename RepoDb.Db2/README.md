[![Db2Build](https://img.shields.io/github/actions/workflow/status/mikependon/RepoDB/build-db2.yml?logo=github&label=build&style=for-the-badge)](https://github.com/mikependon/RepoDB/actions/workflows/build-db2.yml)
[![Db2Home](https://img.shields.io/badge/home-github-important?&logo=github&style=for-the-badge)](https://github.com/mikependon/RepoDb)
[![Db2Version](https://img.shields.io/nuget/v/RepoDb.Db2?&logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/RepoDb.Db2)

# RepoDb.Db2 — RepoDB for Db2 Database

The Db2 provider for RepoDB — a fast, lightweight .NET ORM that lets you use raw SQL and fluent operations side by side on the same connection. Built on top of [RepoDb](https://repodb.net) and the [IBM Data Server .NET Provider (Net.IBM.Data.Db2)](https://www.nuget.org/packages/Net.IBM.Data.Db2).

## Target

Db2 for Linux, UNIX, and Windows (LUW) 10.5 and later. Earlier versions are not supported (the provider relies on `OFFSET ... FETCH NEXT ... ROWS ONLY` paging, which requires 10.5+). Db2 for z/OS and Db2 for i are not currently tested against.

## Important Pages

- [GitHub Home](https://github.com/mikependon/RepoDb) — core library and source code.
- [Website](http://repodb.net) — full documentation, API reference, and blog.

## Community

- [GitHub Issues](https://github.com/mikependon/RepoDb/issues) — bug reports and feature requests.
- [Microsoft Teams](https://teams.live.com/l/community/FEAIJp5q65nfiiWsQ) — live Q&A.
- [X / Twitter](https://x.com/mike_pendon) — news and updates.

## Dependencies

- [Net.IBM.Data.Db2](https://www.nuget.org/packages/Net.IBM.Data.Db2/) — IBM's Data Server .NET provider for Db2. IBM ships this as separate, platform-specific packages instead of one cross-platform package: `Net.IBM.Data.Db2` (Windows x64), `Net.IBM.Data.Db2-lnx` (Linux AMD64), and others (`-osx`, `-zlnx`, `-ppc`) not referenced by this project. The published `RepoDb.Db2` package depends on the Windows package by default. If you consume `RepoDb.Db2` on Linux, add a direct `PackageReference` to `Net.IBM.Data.Db2-lnx` yourself.
- [RepoDb](https://www.nuget.org/packages/RepoDb/) — the RepoDB core library.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)

--------

## Installation

```
Install-Package RepoDb.Db2
```

Unlike the other providers, `RepoDb.Db2` does **not** carry a transitive dependency on the underlying IBM ADO.NET driver. IBM ships a separate driver package per platform, so you must install the matching `Net.IBM.Data.Db2*` package yourself alongside `RepoDb.Db2`.

```csharp
// Windows
> Install-Package RepoDb.Db2
> Install-Package Net.IBM.Data.Db2

// Linux
> Install-Package RepoDb.Db2
> Install-Package Net.IBM.Data.Db2-lnx
```

Pick the IBM driver package that matches your target platform.

| Platform    | IBM package             |
| ----------- | ------------------------|
| Windows x64 | `Net.IBM.Data.Db2`      |
| Linux AMD64 | `Net.IBM.Data.Db2-lnx`  |
| macOS       | `Net.IBM.Data.Db2-osx`  |
| Linux IBM Z | `Net.IBM.Data.Db2-zlnx` |
| Linux Power | `Net.IBM.Data.Db2-ppc`  |

After installation, call the globalized setup method to initialize all dependencies for Db2.

```csharp
GlobalConfiguration
	.Setup()
	.UseDb2();
```

## Get Started

Initialize the bootstrapper once at application startup:

```csharp
GlobalConfiguration
    .Setup()
    .UseDb2();
```

Every statement RepoDb.Db2 generates binds parameters using `":Name"`-style host variables (e.g. `WHERE "Id" = :Id`). IBM's Data Server .NET Provider disables host-variable support by default, so your connection string **must** include `HostVarParameters=True;`, otherwise every parameterized call fails with `DB2Exception` `SQL0313N`:

```
Server=localhost:50000;Database=REPODB;UID=db2inst1;PWD=yourpassword;HostVarParameters=True;
```

Then use any RepoDB operation directly on your `DB2Connection`:

### Query

```csharp
using (var connection = new DB2Connection(ConnectionString))
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
using (var connection = new DB2Connection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```

### ExecuteQuery

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM \"Customer\" WHERE (\"Id\" = :Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### ExecuteNonQuery

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var affectedRows = connection.ExecuteNonQuery("UPDATE \"Customer\" SET \"FirstName\" = :FirstName WHERE (\"Id\" = :Id);", new { FirstName = "John", Id = 10045 });
}
```

### ExecuteScalar

```csharp
using (var connection = new DB2Connection(ConnectionString))
{
	var count = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM \"Customer\";");
}
```

Visit the [get-started](http://repodb.net/tutorial/get-started-db2) page for the full Db2 guide.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) — Copyright © 2026 [Michael Camara Pendon](https://x.com/mike_pendon)
