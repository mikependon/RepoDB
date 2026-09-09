# RepoDb.Options

An options-pattern extension library for RepoDB. It adds an alternative overload for the common operations, accepting a single options object instead of many trailing arguments.

## Projects

- `RepoDb.Options` - the library.
- `RepoDb.Options.UnitTests` - unit tests for the options classes.

## Options Classes

Located under `RepoDb.Options/Options`.

### Connection-level

Used with `IDbConnection` extension methods. Mirror the full argument set of the original operations.

- `ConnectionQueryOptions` - Fields, OrderBy, Top, Hints, CacheKey, CacheItemExpiration, CommandTimeout, TraceKey, Transaction, Cache, Trace, StatementBuilder.
- `ConnectionInsertOptions` - Fields, Hints, CommandTimeout, TraceKey, Transaction, Trace, StatementBuilder.
- `ConnectionDeleteOptions` - Hints, CommandTimeout, TraceKey, Transaction, Trace, StatementBuilder.
- `ConnectionUpdateOptions` - Fields, Hints, CommandTimeout, TraceKey, Transaction, Trace, StatementBuilder.
- `ConnectionMergeOptions` - Fields, Hints, CommandTimeout, TraceKey, Transaction, Trace, StatementBuilder.

### Repository-level

Used with `BaseRepository<TEntity, TDbConnection>` and `DbRepository<TDbConnection>` extension methods. Smaller than the connection-level options, since `CommandTimeout`, `Cache`, `Trace`, `StatementBuilder` and `CacheItemExpiration` are fixed on the repository instance instead.

- `RepositoryQueryOptions` - Fields, OrderBy, Top, Hints, CacheKey, TraceKey, Transaction.
- `RepositoryInsertOptions` - Fields, Hints, TraceKey, Transaction.
- `RepositoryDeleteOptions` - Hints, TraceKey, Transaction.
- `RepositoryUpdateOptions` - Fields, Hints, TraceKey, Transaction.
- `RepositoryMergeOptions` - Fields, Hints, TraceKey, Transaction.

## Operations

Located under `RepoDb.Options/Operations`, one folder per target type. Each covers `Query`, `Insert`, `Delete`, `Update` and `Merge`, both sync and async, matching every overload of the operation it wraps.

- `Operations/DbConnection` - extension methods for `IDbConnection`.
- `Operations/BaseRepository` - extension methods for `BaseRepository<TEntity, TDbConnection>`.
- `Operations/DbRepository` - extension methods for `DbRepository<TDbConnection>`.

## Usage

```csharp
var people = connection.Query<Person>(p => p.Id == 1, new ConnectionQueryOptions
{
    Fields = Field.From("Id", "Name"),
    Top = 10
});
```

The `options` argument is required. This keeps it unambiguous against the original overloads, which already default every trailing argument.
