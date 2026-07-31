# RepoDB Limitations

We want the .NET community to understand the limitations of this library before deciding to use it. RepoDB is a micro-ORM built to handle advanced use cases, but it still has limitations that may not fit every scenario.

**Disclaimer:** This page may not list every limitation, as some use cases are still being discovered. We will keep updating this page as new unsupported scenarios are found.

## Table of Contents

- Core
  - [Composite Keys](#composite-keys)
  - [Auto-Generated Primary Column](#auto-generated-primary-column)
  - [Computed Columns](#computed-columns)
  - [JOIN Query (Support)](#join-query-support)
  - [Cache Invalidation](#cache-invalidation)
  - [Advance Query Tree Expression](#advance-query-tree-expression)
  - [Multiple Identity Columns](#multiple-identity-columns)
- Oracle
  - [QueryMultiple Round Trips](#querymultiple-round-trips)
  - [InsertAll / MergeAll Batching](#insertall--mergeall-batching)
  - [Identity/Primary Key Retrieval](#identityprimary-key-retrieval)
  - [RETURNING on MERGE](#returning-on-merge)
  - [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier)
  - [Bulk Operations and Transactions](#bulk-operations-and-transactions)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table)
  - [Verification Status](#verification-status)

## Core

### Composite Keys

RepoDB does not support Composite Keys as a default qualifier. Push operations ([Insert](https://repodb.net/operation/insert), [Delete](https://repodb.net/operation/delete), [Update](https://repodb.net/operation/update), [Merge](https://repodb.net/operation/merge), etc.) use the primary key as the qualifier, so a table with composite keys behaves unexpectedly unless you explicitly target the composite columns.

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

Here, `10045` points to a single PK column. If your table instead has composite keys on `Name` and `DateOfBirth`, passing a full entity like below will not behave as expected:

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

Earlier versions of RepoDB assumed the identity column was always the primary column, and push operations ([Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge), [Update](https://repodb.net/operation/update), etc.) would fail when the identity and primary columns were separate.

As of issue #1027 (versions > 1.12.10), RepoDB now hydrates only the identity column's value back onto the model and ignores the primary column, along with any other column that has a default value.

This remains a limitation because RepoDB only returns a single value, and it always prioritizes the identity column over other column types.

**Note:** The same applies when the primary column has a default value of its own (e.g., `UUID` in MySQL).

### Computed Columns

Computed columns are supported in all fluent-based GET operations ([Query](https://repodb.net/operation/query), [QueryAll](https://repodb.net/operation/queryall), etc.), but not in fluent-based PUSH operations ([Insert](https://repodb.net/operation/insert), [Merge](https://repodb.net/operation/merge), [Update](https://repodb.net/operation/update), etc.) by default. See Microsoft's [documentation](https://docs.microsoft.com/en-us/sql/relational-databases/tables/specify-computed-columns-in-a-table?view=sql-server-ver15) on computed columns for background.

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

RepoDB does not support JOIN queries. We leave relationship handling (constraints, cascading, delegation, etc.) to the caller, to keep the library predictable and avoid the added complexity ORMs typically take on for this feature.

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

RepoDB does not automatically invalidate cache entries. Adding that layer would mean extra validation and background work, which goes against keeping the library lightweight.

By default, cache items expire after 180 minutes ([Constant.DefaultCacheItemExpirationInMinutes](https://github.com/mikependon/RepoDb/blob/0c3d4b503a0a7da30b344341cbf6860e98955d9e/RepoDb.Core/RepoDb/Constant.cs#L16)).

**Example**

```csharp
var cache = CacheFactory.Create();
using (var connection = new SqlConnection(connectionString).EnsureOpen())
{
	var customers = connection.QueryAll<Customer>(cacheKey: "AllCustomers", cache: cache);
}
```

The result is cached under `"AllCustomers"` for 180 minutes and will not refresh automatically if the underlying data changes.

**Alternative Solution**

Remove the cache entry manually when the data changes.

```csharp
var cache = CacheFactory.Create();
cache.Remove("AllCustomers");
```

Any subsequent fetch using the `"AllCustomers"` key will then read fresh data from the database.

### Advance Query Tree Expression

RepoDB only supports a shallow query tree expression. As noted above, RepoDB also does not support JOINs, so the deeper expression trees common in libraries like Entity Framework are not supported either.

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

RepoDB's core statement builder only supports a single identity column per table, across all supported RDBMS. Any additional identity column beyond the primary one is excluded from parameter passing in push operations, which causes the operation to fail.

There is currently no workaround other than keeping a single identity column per table.

-----

## Oracle

These limitations are specific to the [RepoDb.Oracle](https://www.nuget.org/packages/RepoDb.Oracle) and [RepoDb.Oracle.BulkOperations](https://www.nuget.org/packages/RepoDb.Oracle.BulkOperations) packages, on top of the [Core](#core) limitations above.

### QueryMultiple Round Trips

ODP.NET rejects command text with more than one SQL statement, so [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type instead of a single combined command. The call still works unchanged, but a `QueryMultiple<T1, T2, ...>` that costs one round trip on SQL Server, MySQL, or PostgreSQL costs *N* round trips on Oracle. Keep this in mind for latency-sensitive code paths with many types.

### InsertAll / MergeAll Batching

`InsertAll` and `MergeAll` currently execute one row per round trip, since ODP.NET does not support multi-statement command text. True multi-row batching with a single implicit-result-set return is planned for a later release.

### Identity/Primary Key Retrieval

Identity/primary key retrieval on `Insert`/`Merge` relies on an Oracle 12c+ implicit result set (`DBMS_SQL.RETURN_RESULT`) wrapped in an anonymous PL/SQL block. This works around Oracle's native `RETURNING ... INTO`, which binds to an output parameter that RepoDb's core execution pipeline does not read back.

```sql
DECLARE l_repodb_result "CompleteTable"."Id"%TYPE; l_repodb_cursor SYS_REFCURSOR; BEGIN INSERT INTO "CompleteTable" ( "SessionId", "ColumnVarchar", "ColumnNumber", "ColumnDate", "ColumnTimestamp" ) VALUES ( :SessionId, :ColumnVarchar, :ColumnNumber, :ColumnDate, :ColumnTimestamp ) RETURNING "Id" INTO l_repodb_result; OPEN l_repodb_cursor FOR SELECT l_repodb_result AS "Result" FROM DUAL; DBMS_SQL.RETURN_RESULT(l_repodb_cursor); END;
```

Verify this against your own Oracle instance before relying on it in production.

### RETURNING on MERGE

A `RETURNING` clause on `MERGE` is only supported starting with **Oracle Database 23ai** — it fails with `ORA-00933` on 12c/18c/19c/21c. This provider otherwise targets 12c+, but `Merge` against a table with a primary/identity key needs 23ai+ to get the key value back. `Insert`, `Update`, `Query`, and other operations are unaffected on older versions — only identity-returning `Merge` calls are impacted.

### GUID/UNIQUEIDENTIFIER

Oracle has no native GUID/`UNIQUEIDENTIFIER` type. Unlike `SqlParameter`/`NpgsqlParameter`, ODP.NET does not accept a raw `Guid` value, so binding a `System.Guid` property directly throws `ArgumentException: Value does not fall within the expected range.` from `OracleParameter.Value`.

If a column stores a GUID as `RAW(16)`, either map the property as `byte[]`, or keep it as `Guid` and register `RepoDb.Oracle.PropertyHandlers.GuidToByteArrayPropertyHandler` for that specific property:

```csharp
PropertyHandlerMapper.Add<YourEntity, GuidToByteArrayPropertyHandler>(
    e => e.YourGuidProperty, new GuidToByteArrayPropertyHandler(), true);
```

Register it per-property rather than globally for `typeof(Guid)` if your process also uses another RepoDb provider that handles `Guid` natively — a type-level registration applies process-wide, across all connections.

### Bulk Operations and Transactions

`OracleBulkCopy` — the mechanism behind every bulk load in `RepoDb.Oracle.BulkOperations` except identity-returning `BulkInsert` — is not aware of the caller's transaction. Per Oracle's own ODP.NET documentation, bulk copy operations are agnostic of any local or distributed transaction, and rows it writes commit independently of the caller's transaction.

In practice:

- For a plain `BulkInsert` without `ReturnIdentity`, a rolled-back transaction will **not** remove rows already written by `OracleBulkCopy`.
- For `BulkMerge`/`BulkUpdate`/`BulkDelete`, the final `MERGE`/`UPDATE`/`DELETE` against the real table stays fully transactional, so a rollback behaves correctly for your actual data. Only orphaned rows in the reusable staging table can be left behind, and the next call against that table clears them before loading anything new.

If a plain `BulkInsert` needs all-or-nothing behavior with respect to your transaction, request `identityBehavior: ReturnIdentity` to force the array-bind path (`RETURNING ... INTO`), which does honor your transaction.

### Bulk Operations Staging Table

`BulkMerge`, `BulkUpdate`, and `BulkDelete` stage rows into a per-table pseudo table before running a set-based statement against it. The `pseudoTableType` argument picks the kind of table used:

- **`Memory`** — a Global Temporary Table (GTT), isolated per session. Safe for concurrent connections writing to the same table.
- **`Physical`** — an ordinary heap table, shared by every session. Concurrent connections writing to the same table can corrupt or race each other's staged data. Only use it for sequential, single-threaded workloads.
- **`Auto`** *(default)* — picks `Physical` at 5,000+ rows, otherwise `Memory`.

**`Memory` is currently not usable — every pseudo table resolves to `Physical` regardless of what you pass.** `OracleBulkCopy` always performs a direct-path load, and Oracle's direct-path engine cannot write into a GTT (`ORA-39826`). Until a working strategy exists (e.g., loading a GTT via array-bound `INSERT`s), `Memory` and `Auto`'s row-count threshold both fall back to `Physical`, so the `Physical` concurrency caveat above applies unconditionally.

Because Oracle's `CREATE TABLE`/`CREATE GLOBAL TEMPORARY TABLE` are DDL and cause an implicit commit, the staging table is created once per (table name, pseudo table type) the first time it's needed, not on every call. This means the very first `BulkMerge`/`BulkUpdate`/`BulkDelete` call against a table in a process will implicitly commit any other uncommitted work already pending in that transaction. If this matters for your workload, "warm up" the staging table with a throwaway call at application startup, outside any transaction you care about.

### Verification Status

`RepoDb.Oracle.BulkOperations` has been implemented and reviewed but not yet exercised against a live Oracle instance. In particular, verify the `OracleBulkCopy` load path, the array-bind `RETURNING ... INTO` identity read-back used by `BulkInsert` with `ReturnIdentity`, and the staging table strategy used by `BulkMerge`/`BulkUpdate`/`BulkDelete` end-to-end before relying on this package in production. The same caveat applies to the `DBMS_SQL.RETURN_RESULT` identity trick in the core `RepoDb.Oracle` package (see [Identity/Primary Key Retrieval](#identityprimary-key-retrieval)).
