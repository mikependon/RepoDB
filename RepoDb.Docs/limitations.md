# RepoDB Limitations

We want the .NET community to understand this library's limitations before using it. RepoDB is a micro-ORM built for advanced use cases. It still has limitations that may not fit every scenario.

**Disclaimer:** This page may not list every limitation. Some use cases are still being discovered. We will update this page as we find new unsupported scenarios.

## Table of Contents

- Core
  - [Composite Keys](#composite-keys)
  - [Auto-Generated Primary Column](#auto-generated-primary-column)
  - [Computed Columns](#computed-columns)
  - [JOIN Query (Support)](#join-query-support)
  - [Cache Invalidation](#cache-invalidation)
  - [Advance Query Tree Expression](#advance-query-tree-expression)
  - [Multiple Identity Columns](#multiple-identity-columns)
- SQL Server
  - [Identity Correlation Differs by Input Shape](#identity-correlation-differs-by-input-shape)
  - [ReturnIdentity Silently Ignored for Anonymous Types](#returnidentity-silently-ignored-for-anonymous-types)
  - [Reflection-Based Access to SqlBulkCopy Internals](#reflection-based-access-to-sqlbulkcopy-internals)
- Oracle
  - [QueryMultiple Round Trips](#querymultiple-round-trips)
  - [InsertAll / MergeAll Batching](#insertall--mergeall-batching)
  - [Identity/Primary Key Retrieval](#identityprimary-key-retrieval)
  - [RETURNING on MERGE](#returning-on-merge)
  - [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier)
  - [Bulk Operations and Transactions](#bulk-operations-and-transactions)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-1)
  - [Verification Status](#verification-status)
- DB2
  - [QueryMultiple Round Trips](#querymultiple-round-trips-1)
  - [InsertAll / MergeAll Batching](#insertall--mergeall-batching-1)
  - [Identity/Primary Key Retrieval](#identityprimary-key-retrieval-1)
  - [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier-1)
  - [Bulk Operations and Transactions](#bulk-operations-and-transactions-1)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-2)
  - [Multi-Step BulkMerge Identity Correlation](#multi-step-bulkmerge-identity-correlation)
  - [Verification Status](#verification-status-1)
- MariaDB
  - [Installing Both MariaDb and MariaDbConnector Together](#installing-both-mariadb-and-mariadbconnector-together)
  - [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier-2)
  - [Bulk Operations and Transactions in RepoDb.MariaDb.BulkOperations](#bulk-operations-and-transactions-in-repodbmariadbbulkoperations)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-3)
  - [BulkMerge ReturnIdentity Correlation](#bulkmerge-returnidentity-correlation)
  - [Verification Status](#verification-status-2)

## Core

### Composite Keys

RepoDB does not support composite keys as a default qualifier. Push operations ([Insert](https://repodb.net/operation/insert), [Delete](https://repodb.net/operation/delete), [Update](https://repodb.net/operation/update), [Merge](https://repodb.net/operation/merge), etc.) use the primary key as the qualifier. A table with composite keys behaves unexpectedly unless you explicitly target the composite columns.

#### Scenario 1 - Insert

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var id = connection.Insert<Person>(new Person { Name = "John Doe" });
}
```

The insert succeeds, but the return value is not the composite key. RepoDB only returns a single scalar value (the identity column, if one exists), never the composite key values. The same is true for [Bulk Operations](https://repodb.net/feature/bulkoperations).

#### Scenario 2 - Update

By default, RepoDB uses the primary key as the qualifier for updates.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Update<Person>(new Person { Name = "John Doe" }, 10045);
}
```

Here, `10045` points to a single PK column. If your table has composite keys on `Name` and `DateOfBirth` instead, passing a full entity like below will not behave as expected:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Update<Person>(new Person { Id = 10045, Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01"), Address = "New York" });
}
```

**Alternative Solution**

Target the composite keys explicitly as qualifiers.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var person = new Person { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01"), Address = "New York" };
    var affectedRows = connection.Update(person, e => e.Name == person.Name && e.DateOfBirth == person.DateOfBirth);
}
```

#### Scenario 3 - Delete

Delete also defaults to the primary key as the qualifier.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Delete<Person>(10045);
}
```

There is no way to map `10045` onto composite keys.

**Alternative Solution**

Use an expression-based or dynamic-based delete that targets the composite keys directly.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Delete<Person>(e => e.Name == "John Doe" && e.DateOfBirth == DateTime.Parse("1970/01/01"));
}
```

Or:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
    var affectedRows = connection.Delete(ClassMappedNameCache.Get<Person>(), new { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01") });
}
```

**Note:** There may be other undiscovered scenarios where RepoDB does not fully support tables with composite keys.

### Auto-Generated Primary Column

Earlier versions of RepoDB assumed the identity column was always the primary column. Push operations ([Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge), [Update](https://repodb.net/operation/update), etc.) would fail when the identity and primary columns were separate.

As of issue #1027 (versions > 1.12.10), RepoDB hydrates only the identity column's value back onto the model. It ignores the primary column, along with any other column that has a default value.

This remains a limitation because RepoDB only returns a single value. It always prioritizes the identity column over other column types.

**Note:** The same applies when the primary column has a default value of its own (e.g., `UUID` in MySQL).

### Computed Columns

Computed columns are supported in all fluent-based GET operations ([Query](https://repodb.net/operation/query), [QueryAll](https://repodb.net/operation/queryall), etc.). They are not supported in fluent-based PUSH operations ([Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge), [Update](https://repodb.net/operation/update), etc.) by default. See Microsoft's [documentation](https://docs.microsoft.com/en-us/sql/relational-databases/tables/specify-computed-columns-in-a-table?view=sql-server-ver15) on computed columns for background.

Non-fluent, table-targeted methods like [Query(TableName)](https://repodb.net/operation/query#targetting-a-table) and [Insert(TableName)](https://repodb.net/operation/insert#targetting-a-table) do support computed columns.

RepoDB's automatic property projection does not exclude computed columns. An earlier `IgnoreAttribute` was removed in favor of auto-projection, so this isn't yet handled automatically.

Given this class and table:

```csharp
public class Person
{
	public long Id { get; set; }
	public string Name { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public int Age { get; set; }
}
```

```csharp
CREATE TABLE [dbo].[Person](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[DateOfBirth] [datetime] NULL,
	[Age] AS (DATEDIFF(YEAR,[DateOfBirth], GETUTCDATE())),
	CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) ON [PRIMARY]
) ON [PRIMARY];
```

`Age` is a computed column. GET operations work fine:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var people = connection.QueryAll<Person>();
}
```

But a push operation fails:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Person, long>(new Person { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01") });
}
```

**Alternative Solution**

Use a table-targeted push instead:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert(ClassMappedNameCache.Get<Person>(), new { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01") });
}
```

Or explicitly restrict the `fields` argument to exclude the computed column:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.Insert<Person>(person, fields: Field.From("Id", "Name", "DateOfBirth"));
}
```

You can get an entity's field list via `FieldCache.Get<T>`.

Or maintain two models — one for GET (with the computed column) and one for PUSH (without it):

```csharp
public class Person
{
	public long Id { get; set; }
	public string Name { get; set; }
	public DateTime? DateOfBirth { get; set; }
}

[Table("[dbo].[Person]")]
public class CompletePerson
{
	public long Id { get; set; }
	public string Name { get; set; }
	public DateTime? DateOfBirth { get; set; }
	public int Age { get; set; }
}
```

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var people = connection.QueryAll<CompletePerson>();
	var id = connection.Insert<Person, long>(new Person { Name = "John Doe", DateOfBirth = DateTime.Parse("1970/01/01") });
}
```

### JOIN Query (Support)

RepoDB does not support JOIN queries. We leave relationship handling (constraints, cascading, delegation, etc.) to the caller. This keeps the library predictable and avoids the added complexity ORMs typically take on for this feature.

**Example**

Given these classes:

```csharp
public class Address
{
    public int Id { get; set; }
    ...
}

public class Supplier
{
    public int Id { get; set; }
    public IEnumerable<Address> Addresses { get; set; }
}
```

There is no built-in equivalent of:

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var supplier = connection
		.Query<Supplier>(e => e.Name == "Amazon")
		.Include<Address>();
}
```

**Alternative Solution**

Use [QueryMultiple](https://repodb.net/operation/querymultiple):

```csharp
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	var result = connection.QueryMultiple<Supplier, Address>(s => s.Id == 10045, a => a.SupplierId == 10045);
	var supplier = result.Item1.FirstOrDefault();
	var addresses = result.Item2.AsList();
}
```

Or [ExecuteQueryMultiple](https://repodb.net/operation/executequerymultiple) with raw SQL:

```csharp
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	using (var result = connection.ExecuteQueryMultiple(@"SELECT * FROM [dbo].[Supplier] WHERE [Id] = @SupplierId;
		SELECT * FROM [dbo].[Address] WHERE SupplierId = @SupplierId;"))
	{
		var supplier = result.Extract<Supplier>().FirstOrDefault();
		var addresses = result.Extract<Address>().AsList();
	}
}
```

[SplitQuery](https://repodb.net/operation/splitquery) can also help, though it still requires you to group the results yourself afterward.

### Cache Invalidation

RepoDB does not automatically invalidate cache entries. Adding that layer would mean extra validation and background work. This goes against keeping the library lightweight.

By default, cache items expire after 180 minutes ([Constant.DefaultCacheItemExpirationInMinutes](https://github.com/mikependon/RepoDb/blob/0c3d4b503a0a7da30b344341cbf6860e98955d9e/RepoDb.Core/RepoDb/Constant.cs#L16)).

**Example**

```csharp
var cache = CacheFactory.Create();
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	var customers = connection.QueryAll<Customer>(cacheKey: "AllCustomers", cache: cache);
}
```

The result is cached under `"AllCustomers"` for 180 minutes. It will not refresh automatically if the underlying data changes.

**Alternative Solution**

Remove the cache entry manually when the data changes.

```csharp
var cache = CacheFactory.Create();
cache.Remove("AllCustomers");
```

Any subsequent fetch using the `"AllCustomers"` key will then read fresh data from the database.

### Advance Query Tree Expression

RepoDB only supports a shallow query tree expression. As noted above, RepoDB also does not support JOINs. The deeper expression trees common in libraries like Entity Framework are not supported either.

#### Scenario 1 - 2nd Level Deep or Deeper

Only the first level of a query tree expression is supported.

```csharp
public class Address
{
	public int Id { get; set; }
	public string Street { get; set; }
}

public class Customer
{
	public int Id { get; set; }
	public string Name { get; set; }
	public Address Address { get; set; }
}
```

```csharp
using (var connection = new SqlConnection(connectionString))
{
	var customers = connection.Query<Customer>(e => e.Address.Country == "New York");
}
```

#### Scenario 2 - Unbound to the Property

Expressions not bound to a property are not supported.

```csharp
using (var connection = new SqlConnection(connectionString))
{
	var customers = connection.Query<Customer>(e => DateTime.UtcNow >= DateTime.UtcNow.Date);
}
```

#### Scenario 3 - Field-to-Field Comparison

Expressions comparing one field to another are not supported.

```csharp
using (var connection = new SqlConnection(connectionString))
{
	var sales = connection.Query<Sale>(e => e.TotalPrice >= (e.Price * e.Quantity));
}
```

#### Other Expressions

Complex first-level expressions may also be unsupported. See our [disclaimer](https://repodb.net/feature/expressiontrees) on expression trees. For complex conditions, use [QueryField](https://repodb.net/class/queryfield) or [QueryGroup](https://repodb.net/class/querygroup) instead.

### Multiple Identity Columns

PostgreSQL allows multiple identity columns in a single table, unlike other RDBMS providers:

```csharp
CREATE TABLE IF NOT EXISTS public."Person"
(
    "Id" bigint NOT NULL DEFAULT nextval('person_id_seq'::regclass),
    "OtherId" bigint NOT NULL DEFAULT nextval('person_otherid_seq'::regclass),
    ...
    CONSTRAINT person_pkey PRIMARY KEY ("Id")
);
```

RepoDB's core statement builder only supports a single identity column per table, across all supported RDBMS. Any additional identity column beyond the primary one is excluded from parameter passing in push operations. This causes the operation to fail.

There is currently no workaround other than keeping a single identity column per table.

-----

## SQL Server

These limitations are specific to the [RepoDb.SqlServer.BulkOperations](https://www.nuget.org/packages/RepoDb.SqlServer.BulkOperations) package, on top of the [Core](#core) limitations above.

### Bulk Operations Staging Table

`SqlServerBulkImportPseudoTableType` has three values: `Auto` (default), `Memory`, and `Physical`. Per its own XML doc comment, `Auto` should resolve to `Physical` once the row/entity count reaches `SqlServerConstants.RowCountThresholdForPhysicalTable` (5,000), and to `Memory` otherwise:

```csharp
/// A value that indicates that the type of the pseudo (staging) table will be automatically determined
/// based on the number of rows/entities being processed. A <see cref="Memory"/> table will be used unless
/// the row/entity count reaches the <see cref="SqlServerConstants.RowCountThresholdForPhysicalTable"/>
/// threshold, in which case a <see cref="Physical"/> table will be used instead. This is the default behavior.
Auto,
```

Every call site across `BulkInsert`, `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` instead does a strict equality check:

```csharp
var tempTableName = CreateBulkMergeTempTableName(tableName, pseudoTableType == SqlServerBulkImportPseudoTableType.Physical, dbSetting);
```

`Auto`'s underlying value is never equal to `Physical`, so `Auto` always behaves exactly like `Memory` — a local `#`-prefixed session-scoped temporary table — no matter how many rows are being bulk-loaded. `RowCountThresholdForPhysicalTable` is never referenced anywhere in the codebase outside its own declaration and doc comment. Unless a caller explicitly passes `SqlServerBulkImportPseudoTableType.Physical`, the documented row-count auto-switch never happens, even for million-row bulk loads.

When a caller does explicitly opt into `Physical`, the resulting table name is deterministic and not scoped to the caller: `_RepoDb_Bulk{Operation}_{TableName}` (e.g. `_RepoDb_BulkMerge_Person`), with no session ID, GUID, or connection-specific suffix. Two concurrent callers bulk-merging/updating/deleting against the same target table from different connections will target the exact same physical staging table — created, populated, indexed, and dropped within each call — and can race or corrupt each other's staged rows.

**Alternative Solution**

Serialize bulk operations against the same table when passing `pseudoTableType: SqlServerBulkImportPseudoTableType.Physical`, or avoid `Physical` altogether and rely on the (always-in-effect) local temp table behavior.

### Identity Correlation Differs by Input Shape

`ReturnIdentity` support for `BulkInsert`/`BulkMerge` runs a `MERGE ... OUTPUT` against the staging table, carrying an explicit `[__RepoDb_OrderColumn]` value through the round trip so a returned identity can be matched back to its originating row:

```sql
MERGE [dbo].[Person] AS T
USING (SELECT TOP 100 PERCENT * FROM [#_RepoDb_BulkInsert_Person] ORDER BY [__RepoDb_OrderColumn] ASC) AS S
ON (1 = 0)
WHEN NOT MATCHED THEN INSERT (...) VALUES (...)
OUTPUT INSERTED.[Id] AS [Result], S.[__RepoDb_OrderColumn] AS [OrderColumn];
```

For the `IEnumerable<TEntity>`/`IDictionary<string,object>` overloads, this is read back correctly — `SetIdentityForEntities` reads both the identity value and the `OrderColumn` from the result set and indexes directly into the source list (`list[index]`), so correctness does not depend on the order rows come back in:

```csharp
var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
var index = reader.GetFieldValue<int>(1);
var entity = list[(index < 0 ? result : index)];
func(entity, value);
```

The `DataTable` overloads of `BulkInsert` and `BulkMerge` do not do this. Their `SetIdentityForEntities(DataTable, DbDataReader, DataColumn)` overload reads only the identity value from column 0 and assigns it positionally, ignoring the `OrderColumn` that the same SQL still selects and outputs:

```csharp
while (reader.Read())
{
    var value = Converter.DbNullToNull(reader.GetFieldValue<object>(0));
    dataTable.Rows[result][identityColumn] = value;
    result++;
}
```

Correctness for the `DataTable` path therefore depends on `MERGE`'s `OUTPUT` clause returning rows in exactly the order they were inserted — something the statement only *attempts* to force via the `SELECT TOP 100 PERCENT ... ORDER BY [__RepoDb_OrderColumn]` subquery shown above. `ORDER BY` inside a derived table/subquery, even under `TOP 100 PERCENT`, is not guaranteed by SQL Server's query optimizer to determine the order of the outer statement's results — Microsoft has warned against relying on this pattern since SQL Server 2005. If the optimizer disregards it (more likely on larger batches or parallel plans), a `DataTable`-based `BulkInsert`/`BulkMerge` with `ReturnIdentity` can silently write identity values back onto the wrong rows.

**Alternative Solution**

Prefer the entity/dictionary-based overloads over the `DataTable` overloads when requesting `ReturnIdentity` — they correlate explicitly by `OrderColumn` and do not depend on implicit statement ordering.

### ReturnIdentity Silently Ignored for Anonymous Types

In `BulkMergeInternalBase<TEntity>`, when `ReturnIdentity` is requested, the choice between reading back identities and just executing the merge is:

```csharp
if (hasOrderingColumn != true || TypeCache.Get(entityType).IsAnonymousType())
{
    result = connection.ExecuteNonQuery(sql, ...);
}
else
{
    using var reader = (DbDataReader)connection.ExecuteReader(sql, ...);
    ...
    result = SetIdentityForEntities<TEntity>(entities, reader, identityField);
}
```

For anonymous types, this silently falls back to `ExecuteNonQuery`. The merge still runs and rows are still inserted/updated, but no identity value is ever read back, and no exception or warning is raised. This is a reasonable consequence of anonymous types being immutable — there is no property setter to write the identity into — but the framework does not surface this to the caller, so a `BulkMerge` call against an anonymous type with `identityBehavior: ReturnIdentity` looks like it should populate identities and silently does not.

`BulkInsert`'s entity-based `SetIdentityForEntities` has the same gap without an explicit anonymous-type check: `Compiler.GetPropertySetterFunc<TEntity>(identityField.Name)` returns `null` for any type with no matching settable property (anonymous types, or an entity that's simply missing that property), and when the setter is `null`, the method returns `0` immediately — again with no exception.

### Reflection-Based Access to SqlBulkCopy Internals

Every `SqlBulkCopy` interaction — including calls to fully public members like `DestinationTableName`, `BatchSize`, `BulkCopyTimeout`, `ColumnMappings`, and `WriteToServer`/`WriteToServerAsync` — goes through `Compiler`, an internal helper that builds and caches compiled `System.Linq.Expressions` trees over reflected `MethodInfo`/`PropertyInfo`, rather than calling these public members directly. The row-count fallback goes further and reaches into a *private* field:

```csharp
var rowsCopiedFieldOrProperty = Compiler.GetFieldGetterFunc<SqlBulkCopy, int>("_rowsCopied") ??
    Compiler.GetPropertyGetterFunc<SqlBulkCopy, int>("RowsCopied");
result = (int)rowsCopiedFieldOrProperty?.Invoke(sqlBulkCopy);
```

`_rowsCopied` is a non-public implementation detail of `Microsoft.Data.SqlClient.SqlBulkCopy`, not part of its public contract. Because the fallback is used whenever `DataEntityDataReader.RecordsAffected`/`reader.RecordsAffected` isn't reliable, a future `Microsoft.Data.SqlClient` release that renames or removes that field would silently degrade this fallback — the getter returns `null`/default instead of throwing — rather than fail loudly at compile time or runtime.

-----

## Oracle

These limitations are specific to the [RepoDb.Oracle](https://www.nuget.org/packages/RepoDb.Oracle) and [RepoDb.Oracle.BulkOperations](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations) packages, on top of the [Core](#core) limitations above.

### QueryMultiple Round Trips

ODP.NET rejects command text with more than one SQL statement. [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type instead of a single combined command. The call still works unchanged, but a `QueryMultiple<T1, T2, ...>` that costs one round trip on SQL Server, MySQL, or PostgreSQL costs *N* round trips on Oracle. Keep this in mind for latency-sensitive code paths with many types.

### InsertAll / MergeAll Batching

`InsertAll` and `MergeAll` currently execute one row per round trip, since ODP.NET does not support multi-statement command text. True multi-row batching with a single implicit-result-set return is planned for a later release.

### Identity/Primary Key Retrieval

Identity/primary key retrieval on `Insert`/`Merge` relies on an Oracle 12c+ implicit result set (`DBMS_SQL.RETURN_RESULT`) wrapped in an anonymous PL/SQL block. This works around Oracle's native `RETURNING ... INTO`. That construct binds to an output parameter that RepoDb's core execution pipeline does not read back.

```sql
DECLARE l_repodb_result "CompleteTable"."Id"%TYPE; l_repodb_cursor SYS_REFCURSOR; BEGIN INSERT INTO "CompleteTable" ( "SessionId", "ColumnVarchar", "ColumnNumber", "ColumnDate", "ColumnTimestamp" ) VALUES ( :SessionId, :ColumnVarchar, :ColumnNumber, :ColumnDate, :ColumnTimestamp ) RETURNING "Id" INTO l_repodb_result; OPEN l_repodb_cursor FOR SELECT l_repodb_result AS "Result" FROM DUAL; DBMS_SQL.RETURN_RESULT(l_repodb_cursor); END;
```

Verify this against your own Oracle instance before relying on it in production.

### RETURNING on MERGE

A `RETURNING` clause on `MERGE` is only supported starting with **Oracle Database 23ai**. It fails with `ORA-00933` on 12c/18c/19c/21c. This provider otherwise targets 12c+, but `Merge` against a table with a primary/identity key needs 23ai+ to get the key value back. `Insert`, `Update`, `Query`, and other operations are unaffected on older versions — only identity-returning `Merge` calls are impacted.

### GUID/UNIQUEIDENTIFIER

Oracle has no native GUID/`UNIQUEIDENTIFIER` type. Unlike `SqlParameter`/`NpgsqlParameter`, ODP.NET does not accept a raw `Guid` value. Binding a `System.Guid` property directly throws `ArgumentException: Value does not fall within the expected range.` from `OracleParameter.Value`.

If a column stores a GUID as `RAW(16)`, either map the property as `byte[]`, or keep it as `Guid` and register `RepoDb.Oracle.PropertyHandlers.GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new GuidToByteArrayPropertyHandler(), true);
```

Register it per-property rather than globally for `typeof(Guid)` if your process also uses another RepoDb provider that handles `Guid` natively. A type-level registration applies process-wide, across all connections.

### Bulk Operations and Transactions

`OracleBulkCopy` — the mechanism behind every bulk load in `RepoDb.Oracle.BulkOperations` except identity-returning `BulkInsert` — is not aware of the caller's transaction. Per Oracle's own ODP.NET documentation, bulk copy operations are agnostic of any local or distributed transaction. Rows it writes commit independently of the caller's transaction.

In practice:

- For a plain `BulkInsert` without `ReturnIdentity`, a rolled-back transaction will **not** remove rows already written by `OracleBulkCopy`.
- For `BulkMerge`/`BulkUpdate`/`BulkDelete`, the final `MERGE`/`UPDATE`/`DELETE` against the real table stays fully transactional, so a rollback behaves correctly for your actual data. Only orphaned rows in the reusable staging table can be left behind. The next call against that table clears them before loading anything new.

If a plain `BulkInsert` needs all-or-nothing behavior with respect to your transaction, request `identityBehavior: ReturnIdentity` to force the array-bind path (`RETURNING ... INTO`). That path does honor your transaction.

### Bulk Operations Staging Table

`BulkMerge`, `BulkUpdate`, and `BulkDelete` stage rows into a per-table pseudo table before running a set-based statement against it. The `pseudoTableType` argument picks the kind of table used:

- **`Memory`** — a Global Temporary Table (GTT), isolated per session. Safe for concurrent connections writing to the same table.
- **`Physical`** — an ordinary heap table, shared by every session. Concurrent connections writing to the same table can corrupt or race each other's staged data. Only use it for sequential, single-threaded workloads.
- **`Auto`** *(default)* — picks `Physical` at 5,000+ rows, otherwise `Memory`.

**`Memory` is currently not usable — every pseudo table resolves to `Physical` regardless of what you pass.** `OracleBulkCopy` always performs a direct-path load. Oracle's direct-path engine cannot write into a GTT (`ORA-39826`). Until a working strategy exists (e.g., loading a GTT via array-bound `INSERT`s), `Memory` and `Auto`'s row-count threshold both fall back to `Physical`. The `Physical` concurrency caveat above applies unconditionally.

Oracle's `CREATE TABLE`/`CREATE GLOBAL TEMPORARY TABLE` are DDL and cause an implicit commit. So the staging table is created once per (table name, pseudo table type) the first time it's needed, not on every call. This means the very first `BulkMerge`/`BulkUpdate`/`BulkDelete` call against a table in a process will implicitly commit any other uncommitted work already pending in that transaction. If this matters for your workload, "warm up" the staging table with a throwaway call at application startup, outside any transaction you care about.

### Verification Status

`RepoDb.Oracle.BulkOperations` has been implemented and reviewed but not yet exercised against a live Oracle instance. Verify these end-to-end before relying on this package in production: the `OracleBulkCopy` load path, the array-bind `RETURNING ... INTO` identity read-back used by `BulkInsert` with `ReturnIdentity`, and the staging table strategy used by `BulkMerge`/`BulkUpdate`/`BulkDelete`. The same caveat applies to the `DBMS_SQL.RETURN_RESULT` identity trick in the core `RepoDb.Oracle` package (see [Identity/Primary Key Retrieval](#identityprimary-key-retrieval)).

-----

## DB2

These limitations are specific to the [RepoDb.Db2](https://www.nuget.org/packages/RepoDb.Db2) and [RepoDb.Db2.BulkOperations](https://www.nuget.org/packages/RepoDb.Db2.BulkOperations) packages, on top of the [Core](#core) limitations above.

### QueryMultiple Round Trips

IBM's Data Server .NET Provider rejects command text containing more than one SQL statement (`IDbSetting.IsMultiStatementExecutable = false` for `RepoDb.Db2`). [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type instead of a single combined command. The call still works unchanged, but a `QueryMultiple<T1, T2, ...>` that costs one round trip on SQL Server, MySQL, or PostgreSQL costs *N* round trips on Db2. Keep this in mind for latency-sensitive code paths with many types.

### InsertAll / MergeAll Batching

`InsertAll` and `MergeAll` currently execute one row per round trip (`IsMultiStatementExecutable = false`). True multi-row batching in a single round trip will follow in a later release.

### Identity/Primary Key Retrieval

Identity/primary key retrieval on `Insert`/`Merge` uses `SELECT ... FROM FINAL TABLE (INSERT INTO ... VALUES (...))`. This ANSI-SQL-adjacent construct returns the post-insert row (including any identity-generated column) as an ordinary result set, with no PL/SQL block, output parameter, or cursor plumbing required. This same mechanism works uniformly for both `Insert` and `Merge`, on any Db2 version 9.7+ (well within this provider's 10.5+ target). There is no version gate to worry about.

An earlier revision of this provider wrapped the key column in an Oracle-style `DECLARE ... DBMS_SQL.RETURN_RESULT(...)` PL/SQL block, which doesn't exist in Db2. That has been replaced with the `FINAL TABLE` form described above. Verify `Insert`/`Merge` calls that request the generated key against your own Db2 instance before relying on this in production.

### GUID/UNIQUEIDENTIFIER

Db2 has no native GUID/`UNIQUEIDENTIFIER` type. A `System.Guid` data entity property cannot be bound directly to a `DB2Parameter` the way it can with `SqlParameter`/`NpgsqlParameter`. The idiomatic Db2 storage for a GUID is a fixed-length 16-byte `CHAR(16) FOR BIT DATA` column. Map it as `byte[]` on the entity, or keep it as `Guid` and register `RepoDb.Db2.PropertyHandlers.Db2GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, Db2GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new Db2GuidToByteArrayPropertyHandler(), true);
```

Register it per-property (not globally for `typeof(Guid)`) if your process also uses another RepoDb provider that handles `Guid` natively. A type-level `PropertyHandlerMapper` registration applies process-wide across all connections.

### Bulk Operations and Transactions

`DB2BulkCopy` — the mechanism behind every bulk load in `RepoDb.Db2.BulkOperations` — is constructed without the ambient `transaction` argument that the operation itself accepts (`new DB2BulkCopy(connection, bulkCopyOptions)`, with no `DB2Transaction` passed in). Only the surrounding staging-table DDL (create/drop) and the final `INSERT`/`MERGE`/`UPDATE`/`DELETE` against the real table are issued through `connection.ExecuteNonQuery(commandText, transaction: transaction)` and therefore honor a caller-supplied transaction. The bulk-copy load into the staging table does not.

In practice:

- The final statement against the real table (`INSERT ... FROM FINAL TABLE`, `MERGE`, `UPDATE`, or `DELETE`) is fully transactional, so a rollback still behaves correctly for your actual data.
- The staging table itself is dropped in a `finally` block at the end of every call regardless of outcome, so a non-transactional load into it does not normally leak visible rows.
- The bigger consequence is the DDL below: since a fresh staging table is created and dropped on *every* call rather than reused, and `CREATE TABLE`/`DROP TABLE` commonly force a commit boundary in Db2, any bulk operation issued inside an existing transaction can implicitly commit other work already pending in that same transaction as a side effect — every call, not just the first.

### Bulk Operations Staging Table

`BulkInsert` (when `ReturnIdentity` is requested), `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` all stage rows into a per-call pseudo table — named deterministically from `{pseudoTableType}{tableName}{operation}` (e.g., `PhysicalPersonMerge`) — before running a set-based statement against it. The `pseudoTableType` argument is meant to pick the kind of table used:

- **`Physical`** — an ordinary heap table, not session-isolated.
- **`Memory`** — intended to be a session-private staging table.
- **`Auto`** *(default)* — intended to pick `Physical` at higher row counts, otherwise `Memory`.

**`Memory` is currently not usable — every pseudo table resolves to `Physical` regardless of what you pass, and regardless of row count for `Auto`.** The internal resolution logic returns `Physical` on every branch. There is no session-private staging path implemented yet, despite `Memory` being a documented enum value.

Unlike `RepoDb.Oracle.BulkOperations`, which creates a staging table once per (table, pseudo table type) and reuses it across calls, `RepoDb.Db2.BulkOperations` creates its staging table with `CREATE TABLE ... AS (...) DEFINITION ONLY` and drops it again on *every single call*. Combined with `Physical` staging always being in effect, and staging tables not being session-isolated, two concurrent callers bulk-writing against the same target table (and therefore the same deterministic staging-table name) can contend for, truncate, or drop the same physical table out from under each other. Serialize bulk operations against the same table from a single caller at a time until a proper `Memory`/session-isolated path is implemented.

### Multi-Step BulkMerge Identity Correlation

Db2 LUW's `MERGE` statement has no `FINAL TABLE` support (the same restriction noted for the core provider's `Merge`/`MergeAll` under [Identity/Primary Key Retrieval](#identityprimary-key-retrieval-1)). `BulkMerge` with `ReturnIdentity` works around this without a single atomic statement:

1. A `LEFT JOIN` snapshot query classifies every staged row as matched or unmatched against the target table.
2. A separate `MERGE ... WHEN MATCHED THEN UPDATE` applies the updates for matched rows.
3. A separate `MERGE ... WHEN NOT MATCHED THEN INSERT`, wrapped in `SELECT ... FROM FINAL TABLE (...)`, inserts the unmatched rows and reads back their newly generated identities.

This is three round trips instead of one, and the classification from step 1 can go stale if another connection concurrently inserts, updates, or deletes matching rows in the target table before steps 2 and 3 run — there is no snapshot isolation guarantee across the three statements beyond whatever isolation level the ambient transaction already provides.

Separately, both this insert-only step and a plain `BulkInsert` with `ReturnIdentity` correlate the generated identities back to source rows by sorting the `FINAL TABLE` result by the new identity value ascending and assuming that ascending order matches the source row order (established via a row-order column on the staging table). This is the same unverified ordering assumption already called out for `InsertAll` under [Identity/Primary Key Retrieval](#identityprimary-key-retrieval-1) — verify it against your own Db2 instance before relying on it in production.

### Verification Status

`RepoDb.Db2.BulkOperations` has been implemented and reviewed. The entity-to-`DataTable` property-handler path has been spot-checked against a live Db2 LUW instance — routing entities through an in-memory `DataTable` rather than streaming them via a data reader turned out to be required for a `Guid`-backed `CHAR(n) FOR BIT DATA` column (see [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier-1)) to bulk-load correctly. The package has not otherwise been fully exercised end-to-end. Verify the staging-table lifecycle described above, the `FINAL TABLE` identity read-back ordering, and the multi-step `BulkMerge` correlation before relying on this package in production.

-----

## MariaDB

These limitations are specific to the [RepoDb.MariaDb](https://www.nuget.org/packages/RepoDb.MariaDb), [RepoDb.MariaDbConnector](https://www.nuget.org/packages/RepoDb.MariaDbConnector), [RepoDb.MariaDb.BulkOperations](https://www.nuget.org/packages/RepoDb.MariaDb.BulkOperations), and [RepoDb.MariaDbConnector.BulkOperations](https://www.nuget.org/packages/RepoDb.MariaDbConnector.BulkOperations) packages, on top of the [Core](#core) limitations above.

### Installing Both MariaDb and MariaDbConnector Together

MariaDB support ships as two separate packages that deliberately expose the identical `MariaDb`-prefixed API surface: `RepoDb.MariaDb` (built on `RepoDb.Connector.MariaDb`, a wrapper over `MySql.Data`) and `RepoDb.MariaDbConnector` (built on `RepoDb.Connector.MariaDbConnector`, a wrapper over `MySqlConnector`). Both packages declare their bootstrapping and infrastructure types under the exact same namespace and class name — the two source files differ only in which underlying connector namespace they import, not in their own declared namespace or type name:

```csharp
// RepoDb.MariaDb/MariaDbBootstrap.cs
using RepoDb.Connector.MariaDb;
namespace RepoDb
{
    public static class MariaDbBootstrap { ... }
}

// RepoDb.MariaDbConnector/MariaDbBootstrap.cs
using RepoDb.Connector.MariaDbConnector;
namespace RepoDb
{
    public static class MariaDbBootstrap { ... }
}
```

The same is true of `RepoDb.MariaDbGlobalConfiguration` (and its `UseMariaDb()` extension method), `RepoDb.DbHelpers.MariaDbDbHelper`, `RepoDb.DbSettings.MariaDbDbSetting`, `RepoDb.StatementBuilders.MariaDbStatementBuilder`, and `RepoDb.Attributes.Parameter.MariaDb.MariaDbTypeAttribute`.

If a project references both `RepoDb.MariaDb` and `RepoDb.MariaDbConnector` — directly, or transitively through a package that depends on one of them — any code that touches one of these shared type names, including the call every consumer needs to make (`GlobalConfiguration.Setup().UseMariaDb()`), fails to compile with `CS0433` ("The type '...' exists in both '...'"). The C# compiler cannot resolve an unqualified reference to a type that two different referenced assemblies both define under the identical full name. This is a hard, immediate compile-time failure, not a silent runtime mapping overwrite.

**Alternative Solution**

Reference only one of `RepoDb.MariaDb` or `RepoDb.MariaDbConnector` per project — whichever underlying driver (`MySql.Data` or `MySqlConnector`) best fits. If a single application genuinely needs both drivers, isolate them into separate projects/assemblies rather than referencing both packages from the same compilation unit.

### GUID/UNIQUEIDENTIFIER

Like MySQL, MariaDB has no native GUID/`UNIQUEIDENTIFIER` type, and neither `RepoDb.MariaDb` nor `RepoDb.MariaDbConnector` ships a built-in property handler for one — unlike `RepoDb.Oracle`'s `GuidToByteArrayPropertyHandler` or `RepoDb.Db2`'s `Db2GuidToByteArrayPropertyHandler`, neither MariaDb project has a `PropertyHandlers` folder at all. Map a `Guid` property as `string` or `byte[]`, or write and register your own `IPropertyHandler` for the specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, YourGuidToStringPropertyHandler>(
    e => e.YourGuidProperty, new YourGuidToStringPropertyHandler(), true);
```

### Bulk Operations and Transactions in RepoDb.MariaDb.BulkOperations

`MariaDbBulkCopy` in `RepoDb.MariaDb.BulkOperations` (the `MySql.Data`-based package) is a hand-rolled class built on `RepoDb.Connector.MariaDb`'s `MariaDbBulkLoader`, since the underlying `MySql.Data` driver has no reader-streaming bulk-copy API of its own. It serializes rows to a temporary tab-delimited file and loads them via `LOAD DATA LOCAL INFILE`, issued directly against a bare `MariaDbConnection` that never receives the caller's `MariaDbTransaction`:

```csharp
private static (MariaDbBulkCopy BulkCopy, IDataReader Reader) CreateBulkCopyForDataReader(MariaDbConnection connection,
    string tableName, IDataReader reader, IEnumerable<MariaDbBulkInsertMapItem> mappings,
    int? bulkCopyTimeout, MariaDbTransaction transaction, Field excludeField = null)
{
    var bulkCopy = new MariaDbBulkCopy(connection) { DestinationTableName = ... }; // transaction unused
    ...
}
```

The surrounding staging-table DDL and the final cascading `INSERT`/`UPDATE`/`DELETE` against the real table do participate in the caller-supplied transaction (they run through `connection.ExecuteNonQuery(commandText, transaction: transaction)`), but whether a rolled-back transaction leaves already-`LOAD DATA`-loaded rows behind has not been verified against a live server. `RepoDb.MariaDbConnector.BulkOperations` (the `MySqlConnector`-based package) instead uses `RepoDb.Connector.MariaDbConnector`'s own `MariaDbBulkCopy` type directly, the same way `RepoDb.MySqlConnector.BulkOperations` uses `MySqlConnector`'s own `MySqlBulkCopy` — this specific caveat is not expected to apply there, but that has likewise not been independently verified.

**Alternative Solution**

If all-or-nothing transactional behavior matters for a plain `BulkInsert` against `RepoDb.MariaDb.BulkOperations`, request `identityBehavior: ReturnIdentity` — that path routes through a staging table, and its cascading `INSERT` does honor the caller's transaction.

### Bulk Operations Staging Table

`RepoDb.MariaDb.BulkOperations` and `RepoDb.MariaDbConnector.BulkOperations` share the same `MariaDbBulkImportPseudoTableType` enum and staging-table SQL generator (`MariaDbText`) — byte-for-byte identical between the two packages apart from their `using` imports. It offers three values:

- **`Physical`** — an ordinary heap table, not session-isolated.
- **`Memory`** — intended to be a `TEMPORARY` table, private to each session.
- **`Auto`** *(default)* — intended to pick `Physical` at 5,000+ rows (`MariaDbConstants.RowCountThresholdForPhysicalTable`), otherwise `Memory`.

**`Memory` is currently not usable in either package — every pseudo table resolves to `Physical` regardless of what you pass, and regardless of row count for `Auto`:**

```csharp
private static MariaDbBulkImportPseudoTableType ResolvePseudoTableType(MariaDbBulkImportPseudoTableType pseudoTableType, int? rowCount) =>
    pseudoTableType == MariaDbBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= MariaDbConstants.RowCountThresholdForPhysicalTable ?
        MariaDbBulkImportPseudoTableType.Physical :
            MariaDbBulkImportPseudoTableType.Physical;
```

Both branches of the conditional return `Physical`. The `TEMPORARY TABLE` DDL branch exists in `MariaDbText.GetCreatePseudoTableSql` and is written correctly, but the resolution step never reaches it. Because a physical pseudo-table has no per-session isolation, and every `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey` call targets a deterministic name derived only from the real table name, the operation, and the pseudo table type (e.g. `PhysicalPersonMerge`), two concurrent callers bulk-writing against the same target table from different connections can race or corrupt each other's staged rows.

Every call also (re)creates its own staging table from scratch — an unconditional `DROP TABLE IF EXISTS` followed by `CREATE TABLE ... AS SELECT ... WHERE (1 = 0)` — and drops it again once the call completes, rather than creating one per (table, pseudo table type) and reusing it the way `RepoDb.Oracle.BulkOperations` does. Since `CREATE TABLE`/`DROP TABLE` are DDL, and DDL causes an implicit commit in MariaDB, this happens on *every* `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey`/`BulkInsert`-with-`ReturnIdentity` call, not just the first — each one implicitly commits any other uncommitted work already pending on that connection.

**Alternative Solution**

Serialize bulk operations against the same table until session-isolated `Memory` staging is wired up. If you're bulk-writing inside a larger transaction alongside other statements, keep in mind that the pseudo-table DDL will implicitly commit that work.

### BulkMerge ReturnIdentity Correlation

MariaDB's `AUTO_INCREMENT` has no equivalent to Oracle's per-row `SEQUENCE.NEXTVAL`, and relying on `LAST_INSERT_ID()` plus positional arithmetic after a multi-row `INSERT`/`MERGE` is not safe under MariaDB's default interleaved `innodb_autoinc_lock_mode`, which does not guarantee gap-free identity allocation for that statement shape under concurrent writers. So when `identityBehavior: ReturnIdentity` is requested, both bulk packages instead pre-assign identity values into the pseudo table using a session user variable, seeded from a value read live as `MAX(identityColumn) + 1` directly off the target table's own row data — deliberately not from `information_schema.TABLES.AUTO_INCREMENT`, which MariaDB can cache for up to `information_schema_stats_expiry` seconds (24 hours by default, refreshed only via `ANALYZE TABLE` or expiry). An earlier revision of this seed query read that cached `information_schema` value instead, and a return-identity bulk insert issued immediately after other rows had already been inserted into the same table collided on a duplicate primary key as a result.

For `BulkInsert`, this is a single pre-assignment step. For `BulkMerge` with `ReturnIdentity`, it takes five separate statements against the database, not one round trip:

1. Matched rows keep their existing identity value — copied from the real table onto the staged row via `UPDATE ... INNER JOIN`.
2. Unmatched rows get a freshly pre-assigned value via the session-variable technique above.
3. Matched rows in the real table are updated (`UPDATE ... INNER JOIN`).
4. Unmatched rows are inserted into the real table (`INSERT ... SELECT`, anti-joined against the real table).
5. A final `SELECT ... ORDER BY __RepoDbBulkRowOrder__` reads every row's resulting identity value back, in original bulk-load order.

The seed lookup and the pre-assignment statement are also two separate round trips, leaving a small race window against a concurrent writer to the same table — no table-level locking is used to close it, since `LOCK TABLES` would silently commit any transaction already open on the connection. Requesting `ReturnIdentity` — and, separately, the pseudo table's identity-column nullability toggle, which is rebuilt dynamically via `PREPARE`/`EXECUTE`/`DEALLOCATE PREPARE` since MariaDB's `MODIFY COLUMN` has no "nullability only" form — both require `AllowUserVariables=True` on the connection string.

### Verification Status

Neither `RepoDb.MariaDb.BulkOperations` nor `RepoDb.MariaDbConnector.BulkOperations` has been exercised against a live MariaDB instance yet. Verify the following end-to-end before relying on either package in production: the bulk-load path (`LOAD DATA LOCAL INFILE` for `RepoDb.MariaDb.BulkOperations`, the connector's own `MariaDbBulkCopy` for `RepoDb.MariaDbConnector.BulkOperations`), the identity pre-assignment/read-back described above, and the staging-table strategy used by `BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey`.
