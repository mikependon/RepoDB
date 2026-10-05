# RepoDb.Turso

RepoDB provider for [Turso's .NET bindings](https://github.com/tursodatabase/turso/tree/main/bindings/dotnet),
copied from `RepoDb.Sqlite.Microsoft` and adapted to the SQLite-compatible
`Turso.Data.Sqlite` facade. Targets .NET 8, 9, and 10; .NET Standard is not
supported by the underlying driver.

## Getting started

Reference `RepoDb.Turso`, then initialize it once before using RepoDB operations:

```csharp
using RepoDb;
using Turso.Data.Sqlite;

GlobalConfiguration.Setup(new()
{
    ConversionType = RepoDb.Enumerations.ConversionType.Automatic
}).UseTurso();

using var connection = new SqliteConnection("Data Source=app.db");
connection.Open();
connection.ExecuteNonQuery(
    "CREATE TABLE IF NOT EXISTS Customer (Id INTEGER PRIMARY KEY, Name TEXT NOT NULL)");

var id = connection.Insert<Customer, long>(new Customer { Name = "Alice" });
var customer = connection.Query<Customer>(id);

public class Customer
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

`Data Source=:memory:` creates a private in-memory database. Keep the connection
open while using it.

## Remote databases and embedded replicas

Use the same `Turso.Data.Sqlite.SqliteConnection` with the driver's remote
connection string:

```csharp
using var connection = new SqliteConnection(
    $"Data Source=libsql://example-org.turso.io;Auth Token={authToken}");
await connection.OpenAsync();
var customers = await connection.QueryAllAsync<Customer>();
```

Obtain `authToken` from secure application configuration; do not hardcode it.
Leave `Read Your Writes=True` (the driver default) so session-dependent operations
such as `last_insert_rowid()` retain their remote session.

For an embedded replica, add `Replica Path=./replica.db;Sync Interval=30` and use
the driver's `SyncAsync()` to pull immediately. Syncing and pushing are driver
responsibilities, not automatic RepoDB operations. See the upstream bindings
documentation for pooling, encryption, replica lifecycle, and conflict policy.

## Compatibility

The provider retains SQLite SQL generation, parameter mapping, schema discovery,
pagination, transactions, batched inserts, and RepoDB's insert/update-based merge
behavior. There is no separate bulk-copy package.

Automatic conversion is recommended when entity types differ from the driver's
SQLite storage types (for example decimal, numeric, boolean, or date columns).
It is an explicit, process-wide RepoDB option; `UseTurso()` does not change it.
`Truncate` deletes rows without running `VACUUM`, which is experimental in the
Turso engine. It does not reclaim file space or reset an `AUTOINCREMENT` sequence.

Only `Turso.Data.Sqlite.SqliteConnection` is registered. The lower-level
`Turso.TursoConnection` is not interchangeable with the facade for this provider.
Provider-specific parameter attributes are in
`RepoDb.Attributes.Parameter.Turso`, avoiding conflicts with
`RepoDb.Sqlite.Microsoft`. Both packages can be referenced together; initialize
each with its own `UseTurso()` / `UseSqlite()` call.

Registration coexistence does not guarantee shared execution-cache compatibility.
Local benchmarking exposed an `InsertAll` parameter-setter cache collision when
the same entity type is used with both providers in one process: a Microsoft
SQLite parameter handler can be reused for a Turso parameter and throw
`InvalidCastException`. That mixed-provider scenario is not currently supported.

This provider includes a corresponding RepoDB core fix to resolve reader column
types after reading the first row. When publishing packages, release the core
from this revision as well; do not pin the provider to an older core package
without that fix.

Turso is not a complete SQLite implementation: raw SQLite handles, built-in FTS
modules, dirty reads, and remote client-side extension/function helpers have
limitations documented upstream. Local async methods may use synchronous native
execution. Remote/replica behavior requires an appropriate Turso endpoint and
credentials; the copied integration suite runs locally without either.
