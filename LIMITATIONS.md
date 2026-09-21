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
- ClickHouse
  - [No Real Transactions](#no-real-transactions)
  - [No Identity/Auto-Increment Mechanism](#no-identityauto-increment-mechanism)
  - [Merge Emits a Plain INSERT](#merge-emits-a-plain-insert)
  - [Update Is an Asynchronous Mutation](#update-is-an-asynchronous-mutation)
  - [Delete Uses Lightweight Delete, Inconsistently With Update](#delete-uses-lightweight-delete-inconsistently-with-update)
  - [Composite ORDER BY / PRIMARY KEY](#composite-order-by--primary-key)
  - [QueryMultiple / InsertAll / MergeAll / UpdateAll Batching](#querymultiple--insertall--mergeall--updateall-batching)
  - [Bulk Operations: No ReturnIdentity](#bulk-operations-no-returnidentity)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-4)
  - [Bulk Update/Delete/Merge Row Counts Are Staged Counts, Not Affected Counts](#bulk-updatedeletemerge-row-counts-are-staged-counts-not-affected-counts)
  - [Bulk Operations and Transactions](#bulk-operations-and-transactions-2)
  - [Verification Status](#verification-status-3)
- Firebird
  - [Requires Firebird 3.0+](#requires-firebird-30)
  - [QueryMultiple / InsertAll / MergeAll Batching](#querymultiple--insertall--mergeall-batching)
  - [IN(...) List Limit](#in-list-limit)
  - [Merge Semantics for Identity-as-Qualifier](#merge-semantics-for-identity-as-qualifier)
  - [No Session-Wide Scope Identity](#no-session-wide-scope-identity)
  - [Fixed-Width CHAR/NCHAR Padding](#fixed-width-charnchar-padding)
  - [Fixed TIMESTAMP Precision](#fixed-timestamp-precision)
  - [Enum Mapped via TypeMap to a Text Column](#enum-mapped-via-typemap-to-a-text-column)
  - [RepoDb.Firebird.BulkOperations Does Not Exist Yet](#repodbfirebirdbulkoperations-does-not-exist-yet)
  - [Verification Status](#verification-status-4)
- Vertica
  - [QueryMultiple / MergeAll / UpdateAll Batching](#querymultiple--mergeall--updateall-batching)
  - [No TRUNCATE TABLE Statement](#no-truncate-table-statement)
  - [MERGE Statement Rejected on Tables With an Identity Column](#merge-statement-rejected-on-tables-with-an-identity-column)
  - [Uniform-Width Integer and Floating-Point Types](#uniform-width-integer-and-floating-point-types)
  - [Bulk Insert and Merge Identity Correlation](#bulk-insert-and-merge-identity-correlation)
  - [Bulk Operations: BatchSize Parameter Has No Effect](#bulk-operations-batchsize-parameter-has-no-effect)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-5)
  - [Verification Status](#verification-status-5)
- SAP HANA
  - [QueryMultiple / InsertAll / MergeAll Batching](#querymultiple--insertall--mergeall-batching-1)
  - [Identity/Primary Key Retrieval](#identityprimary-key-retrieval-2)
  - [GUID/UNIQUEIDENTIFIER](#guiduniqueidentifier-3)
  - [Bulk Operations Have No Native Bulk-Copy Path](#bulk-operations-have-no-native-bulk-copy-path)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-6)
  - [Bulk Insert and Merge Identity Correlation](#bulk-insert-and-merge-identity-correlation-1)
  - [Bulk Operations and Transactions](#bulk-operations-and-transactions-3)
  - [Verification Status](#verification-status-6)
- EnterpriseDB
  - [RepoDb.Connector.EnterpriseDb Is Not Published to NuGet](#repodbconnectorenterprisedb-is-not-published-to-nuget)
  - [Bulk Operations Staging Table](#bulk-operations-staging-table-7)
  - [Bulk Insert and Merge Identity Correlation Relies on RETURNING Order](#bulk-insert-and-merge-identity-correlation-relies-on-returning-order)
  - [BulkMerge Without Updateable Columns Undercounts Affected Rows](#bulkmerge-without-updateable-columns-undercounts-affected-rows)
  - [Verification Status](#verification-status-7)

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

-----

## ClickHouse

These limitations are specific to the [RepoDb.ClickHouse](https://www.nuget.org/packages/RepoDb.ClickHouse) and `RepoDb.ClickHouse.BulkOperations` packages, on top of the [Core](#core) limitations above. ClickHouse is a column-oriented analytical database, not a traditional transactional RDBMS — several of the caveats below exist because RepoDB's operation model (Insert/Update/Delete/Merge as immediate, transactional, row-level statements) does not map cleanly onto ClickHouse's actual execution model (append-mostly writes, asynchronous background mutations, no cross-statement transactions). Many of these are called out directly in the provider's own XML doc comments, quoted below.

### No Real Transactions

`ClickHouseConnection.BeginDbTransaction` always returns a `ClickHouseTransaction` whose `Commit()`/`Rollback()` are true no-ops:

```csharp
// ClickHouseTransaction.cs
public override void Commit() { }
public override void Rollback() { }
```

This is deliberate — ClickHouse, especially over the HTTP protocol, offers no cross-statement transactional atomicity the way most RDBMS providers do. Code that wraps RepoDB calls in the familiar `connection.BeginTransaction()` / `transaction.Rollback()` pattern will not get an exception; it will just silently do nothing on rollback. Every statement already issued has already taken effect independently.

**Alternative Solution**

Design ClickHouse write paths to be idempotent and re-runnable (e.g. a `ReplacingMergeTree`/`CollapsingMergeTree` engine with a version or sign column) rather than relying on rollback for correctness. There is no workaround inside the provider itself.

### No Identity/Auto-Increment Mechanism

Unlike SQL Server, MySQL, Oracle (sequence-based), or MariaDB (`AUTO_INCREMENT`), ClickHouse has no identity/auto-increment/sequence concept, and this provider does not attempt to fake one.

The metadata query behind `ClickHouseDbHelper` hardcodes every column's identity flag to `0`, so no column is ever auto-detected as an identity:

```sql
SELECT name AS ColumnName, is_in_primary_key AS IsPrimary, 0 AS IsIdentity, type AS ColumnType ...
```

`GetScopeIdentity<T>`/`GetScopeIdentityAsync<T>` throw unconditionally:

```csharp
public T GetScopeIdentity<T>(IDbConnection connection, IDbTransaction transaction = null) =>
    throw new NotSupportedException("ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism.");
```

And every insert/merge path in `ClickHouseStatementBuilder` guards against a caller manually forcing an `[Identity]` mapping:

```csharp
private static void GuardNoIdentity(DbField identityField)
{
    if (identityField != null)
    {
        throw new NotSupportedException("ClickHouse does not support identity/auto-increment columns.");
    }
}
```

`CreateInsert` also does not chain a trailing `SELECT` after the `INSERT` to confirm the row, since ClickHouse cannot chain a `SELECT` after an `INSERT` in one request — this is consistent with the no-multi-statement limitation below, not a separate bug.

**Alternative Solution**

Assign primary-key values client-side before inserting (e.g. `Guid.NewGuid()`, a snowflake-style generator, or an application-owned sequence table). Never map a ClickHouse entity property with `[Identity]`.

### Merge Emits a Plain INSERT

ClickHouse has no `ON DUPLICATE KEY UPDATE`/`MERGE` statement and no reliable synchronous `UPDATE`. `CreateMerge`/`CreateMergeAll` in `ClickHouseStatementBuilder` therefore emit the exact same plain `INSERT` as `CreateInsert`/`CreateInsertAll` — there is no deduplication or upsert behavior at the statement level:

```csharp
/// ClickHouse has no ON DUPLICATE KEY UPDATE / MERGE statement and no reliable synchronous UPDATE, so
/// this emits the same plain INSERT as CreateInsert. True de-duplication is deferred to the table engine
/// (e.g. ReplacingMergeTree) and its background merges - the idiomatic ClickHouse upsert pattern - rather
/// than hard-failing the Merge/MergeAll operations.
public override string CreateMerge(...) => BuildInsertValues(...);
```

Calling `connection.Merge<T>(entity)` against a plain `MergeTree` table does not update an existing row — it inserts a duplicate. Correctness depends entirely on the target table using a deduplicating engine (`ReplacingMergeTree`, `CollapsingMergeTree`, etc.), and even then, that engine's background merge/deduplication is not immediate — a `Query` issued right after a `Merge` can still see duplicate rows until the engine catches up (or until you query with `FINAL`/`argMax`).

**Alternative Solution**

Use a deduplicating table engine (typically `ReplacingMergeTree`) for any table you call `Merge`/`MergeAll` against, and read it back with `FINAL` or an `argMax`-based query rather than expecting `Merge` itself to have applied an update by the time it returns.

### Update Is an Asynchronous Mutation

ClickHouse has no plain `UPDATE ... SET ... WHERE ...` statement — only `ALTER TABLE table UPDATE col = expr [, ...] WHERE filter`, ClickHouse's "mutation" syntax. `CreateUpdate`/`CreateUpdateAll` build exactly this:

```csharp
/// ClickHouse has no plain UPDATE ... SET ... WHERE ... statement - only
/// ALTER TABLE table UPDATE col = expr [, ...] WHERE filter, an asynchronous mutation applied
/// by background merges rather than immediately.
```

A mutation is *registered* synchronously but *applied* by a background merge afterward — potentially milliseconds later, potentially much longer, depending on table/part size and server load. `connection.Update<T>(entity)` returns as soon as the mutation is queued, not once it has taken effect. A `Query` issued immediately afterward can still return the pre-update value.

**Alternative Solution**

Treat `Update`/`UpdateAll` as "submit an asynchronous mutation," not "the row is now changed." If your workflow needs to know when the mutation has actually applied, poll ClickHouse's own `system.mutations` table (filtering by table, checking `is_done`) — RepoDB does not do this for you.

### Delete Uses Lightweight Delete, Inconsistently With Update

`ClickHouseStatementBuilder` does not override `CreateDelete`/`CreateDeleteAll` at all. Both fall through to the core `BaseStatementBuilder` implementation, which emits a plain `DELETE FROM table WHERE ...` — not the `ALTER TABLE table DELETE WHERE ...` mutation syntax that `CreateUpdate` was explicitly special-cased for in the same file.

This relies on ClickHouse's "lightweight delete" feature (a plain `DELETE FROM ... WHERE ...` that marks rows via an internal row-exists mask rather than an `ALTER TABLE` mutation). Lightweight delete has historically required server-side settings (e.g. `allow_experimental_lightweight_delete`, naming has changed across ClickHouse versions) and only works on `MergeTree`-family engines with those settings enabled — it is not guaranteed to work unconditionally across every target ClickHouse version/engine the way `ALTER TABLE ... DELETE` mutations are.

**Alternative Solution**

Verify lightweight delete is enabled and supported on your target ClickHouse server version before relying on `Delete`/`DeleteAll`. If it isn't available, issue an explicit `ExecuteNonQuery("ALTER TABLE ... DELETE WHERE ...")` instead, keeping in mind that path is also an asynchronous mutation (see [Update Is an Asynchronous Mutation](#update-is-an-asynchronous-mutation)).

### Composite ORDER BY / PRIMARY KEY

A composite sort/primary key (`ORDER BY (col1, col2, ...)`) is the idiomatic default table design for `MergeTree`-family engines, making this a more common trap for ClickHouse than the general [Composite Keys](#composite-keys) limitation is for other providers.

`ClickHouseDbHelper`'s metadata query correctly flags every column that participates in the primary key (`is_in_primary_key`), but RepoDB's core `DbFieldCollection.GetPrimary()` only ever picks the *first* one it finds and silently drops the rest. That single field is what the default qualifier fallback uses — including in `ClickHouseStatementBuilder`'s Merge/UpdateAll guards and in the bulk-operations package's `GetQualifierFields` helper:

```csharp
var primaryOrIdentity = dbFields?.GetPrimary() ?? dbFields?.GetIdentity();
...
return new[] { primaryOrIdentity.AsField() };
```

For a table with `ORDER BY (UserId, EventDate)`, a `Merge`/`Update`/`BulkMerge`/`BulkUpdate`/`BulkDelete` call that omits `qualifiers` matches on `UserId` alone — silently affecting every row sharing that `UserId` regardless of `EventDate`, rather than throwing or requiring the full composite key.

**Alternative Solution**

Always pass `qualifiers` explicitly (e.g. `qualifiers: e => new { e.UserId, e.EventDate }`) for any ClickHouse table with a composite `ORDER BY`/`PRIMARY KEY`. Never rely on the single-column default.

### QueryMultiple / InsertAll / MergeAll / UpdateAll Batching

`IsMultiStatementExecutable` is `false` for this provider, so [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type, same as Oracle and Db2. `CreateUpdateAll`/`CreateMergeAll` call `ValidateMultipleStatementExecution(batchSize)` and throw when `batchSize > 1`, so `UpdateAll`/`MergeAll` execute one row per round trip.

`InsertAll` is the exception: `CreateInsertAll` still builds a single multi-row `INSERT INTO ... VALUES (row0), (row1), ...` statement — the multi-statement restriction is about chaining *separate* statements together in one request, not about a multi-row `VALUES` list, so `InsertAll` does not pay a per-row round-trip cost.

**Alternative Solution**

No workaround needed for `InsertAll`. For `UpdateAll`/`MergeAll` with more than one row, either issue calls one row at a time or use the `RepoDb.ClickHouse.BulkOperations` package instead.

### Bulk Operations: No ReturnIdentity

Consistent with the core provider having no identity mechanism at all (see [No Identity/Auto-Increment Mechanism](#no-identityauto-increment-mechanism)), `RepoDb.ClickHouse.BulkOperations` does not fake one either. `ClickHouseBulkImportIdentityBehavior.KeepIdentity` (the default) is the only supported value; passing `ReturnIdentity` throws immediately:

```csharp
private static void GuardReturnIdentity(ClickHouseBulkImportIdentityBehavior identityBehavior)
{
    if (identityBehavior == ClickHouseBulkImportIdentityBehavior.ReturnIdentity)
    {
        throw new NotSupportedException(
            "ClickHouse has no session-wide scope identity, sequence, or auto-increment mechanism, " +
            "so 'ClickHouseBulkImportIdentityBehavior.ReturnIdentity' is not supported. Use 'KeepIdentity' instead.");
    }
}
```

Unlike MySQL/MariaDB (`AUTO_INCREMENT` pre-assignment) or Oracle (sequence-based generation), there is no fallback mechanism here — this is a hard, immediate `NotSupportedException`, not a silent no-op.

**Alternative Solution**

Generate primary-key values client-side (GUID, snowflake-style ID, or an application-owned counter table) before calling any Bulk* method. Never request `ReturnIdentity`.

### Bulk Operations Staging Table

`BulkUpdate`, `BulkDelete`, `BulkMerge`, and `BulkDeleteByKey` stage rows into a per-call pseudo table before running a mutation/insert against it. `ClickHouseBulkImportPseudoTableType` offers the same three values seen in the other bulk-operations packages:

- **`Physical`** — an ordinary `MergeTree` heap table, globally visible.
- **`Memory`** — intended to be a session-private staging table.
- **`Auto`** *(default)* — intended to pick `Physical` at 5,000+ rows, otherwise `Memory`.

**`Auto` always resolves to `Physical`, regardless of row count** — the same bug pattern seen in `RepoDb.Db2.BulkOperations` and `RepoDb.MariaDb.BulkOperations`:

```csharp
private static ClickHouseBulkImportPseudoTableType ResolvePseudoTableType(ClickHouseBulkImportPseudoTableType pseudoTableType,
    int? rowCount) =>
    pseudoTableType == ClickHouseBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= ClickHouseConstants.RowCountThresholdForPhysicalTable ?
        ClickHouseBulkImportPseudoTableType.Physical :
            ClickHouseBulkImportPseudoTableType.Physical;
```

Both branches of the ternary return `Physical`. Unlike Db2/MariaDb, passing `Memory` *explicitly* does bypass this bug and reach the `Memory`-engine DDL branch — but that branch is itself weaker than its own doc comment claims. `Memory`'s XML doc says rows are *"session-private, making the execution isolated to any concurrent executions from different connections,"* but the actual DDL is a plain `CREATE TABLE ... ENGINE = Memory` — an ordinary, globally-visible, named table that merely stores its data in RAM instead of on disk. It is not ClickHouse's genuinely session-scoped `CREATE TEMPORARY TABLE` construct, which this package does not use. Combined with deterministic, non-unique staging-table names (`{pseudoTableType}{tableName}{Operation}`, e.g. `MemoryPersonUpdate`, with no session ID or GUID suffix), two concurrent callers against the same target table — even both explicitly requesting `Memory` — target the same staging table and can race or corrupt each other's staged rows, just like `Physical`.

Every call also drops and recreates its staging table from scratch (`DROP TABLE IF EXISTS` followed by `CREATE TABLE ... AS SELECT ...`) rather than creating one per (table, pseudo table type) and reusing it — ClickHouse's DDL has no `CREATE TABLE IF NOT EXISTS ... AS SELECT` "replace" form. This means the full DDL cost is paid on every single `BulkUpdate`/`BulkDelete`/`BulkMerge`/`BulkDeleteByKey` call, not just the first.

**Alternative Solution**

Serialize bulk `Update`/`Delete`/`Merge`/`DeleteByKey` operations against the same target table across connections — there is currently no genuinely isolated staging strategy in this package, regardless of which `pseudoTableType` is requested.

### Bulk Update/Delete/Merge Row Counts Are Staged Counts, Not Affected Counts

Because `ALTER TABLE ... UPDATE`/`DELETE` mutations are asynchronous, and ClickHouse.Driver's `ExecuteNonQuery` has no reliable "rows affected" figure for either a mutation or a plain `INSERT`, the internal execution methods that fire these statements are `void`/non-generic `Task` — they issue the statement and return nothing:

```csharp
/// Unlike the MySQL provider's MySqlExecution, none of the merge/update/delete methods here return a
/// meaningful affected-row count: ClickHouse's ALTER TABLE ... UPDATE/DELETE mutations are asynchronous
/// (registered immediately, applied by a background merge afterward), and ClickHouse.Driver's
/// ExecuteNonQuery has no reliable "rows affected" figure for either a mutation or a plain INSERT.
```

Every `BulkUpdate`/`BulkDelete`/`BulkMerge` call site instead returns the row count from the earlier bulk-copy-into-staging-table step — a number that was already known synchronously — rather than any count derived from the mutation itself. In practice this means the `int` returned by these methods is **"how many rows were staged for the mutation,"** not **"how many rows were actually changed."** The method can return successfully with that count before the underlying `ALTER TABLE` mutation has applied against the real table at all.

`BulkUpdate`'s (and matched-rows-of-`BulkMerge`'s) `SET` clause is also built as a correlated scalar subquery rather than a join, since a mutation's `SET` clause has no join-alias for a second table:

```csharp
$"{quotedField} = (SELECT S.{quotedField} FROM {quotedPseudoTableName} S WHERE {correlation} LIMIT 1)"
```

ClickHouse's support for per-row-correlated mutation subqueries has evolved across server versions — verify this pattern against your target ClickHouse version before relying on it in production.

**Alternative Solution**

Treat the returned `int` as "rows submitted for mutation," not "rows changed." If you need to know when a mutation has actually applied, poll `system.mutations` (filtering by table, checking `is_done`) after the call returns — RepoDB does not do this for you.

### Bulk Operations and Transactions

`ClickHouseBulkCopy` (the package's own wrapper) is constructed from a connection only — it has no transaction parameter, and the underlying `ClickHouse.Driver` bulk-copy call is never scoped to a caller-supplied transaction. Given that `ClickHouseTransaction.Commit()`/`Rollback()` are already no-ops at the core provider level (see [No Real Transactions](#no-real-transactions)), this has limited *additional* practical impact — but it does mean this package inherits none of the partial transactional guarantees that Oracle/Db2/MariaDb's bulk-operations packages provide for their surrounding staging-table DDL and final statements. A caller-supplied transaction is still threaded through the plain SQL calls in this package (staging table DDL, the final mutation/insert), but since the underlying `ClickHouseTransaction` does nothing on commit/rollback, none of it is actually undoable.

**Alternative Solution**

Do not rely on transactional rollback for any ClickHouse bulk operation. If partial-failure cleanup matters, design for idempotent re-runs (e.g. `ReplacingMergeTree` plus a version/timestamp column) instead.

### Verification Status

`RepoDb.ClickHouse` has unit test coverage (`DbSettingTest.cs`, `StatementBuilderTest.cs`) exercising the statement-builder guards described above, plus an integration test project and a CI workflow that runs both against a real `clickhouse/clickhouse-server` container. `RepoDb.ClickHouse.BulkOperations` similarly has an integration test project with a `Setup/Database.cs` that provisions real `ReplacingMergeTree` tables against a live server.

One gap worth calling out: the existing bulk-operations integration tests (e.g. for `BulkUpdate`) call the operation and immediately `QueryAll` to assert the result, with no wait/retry/poll for the asynchronous `ALTER TABLE ... UPDATE` mutation described under [Bulk Update/Delete/Merge Row Counts Are Staged Counts, Not Affected Counts](#bulk-updatedeletemerge-row-counts-are-staged-counts-not-affected-counts) to actually apply. These tests only pass reliably because the mutation happens to complete near-instantly against small, freshly-created tables in CI — they do not validate mutation-visibility timing under real-world table sizes or server load. Verify this behavior against your own ClickHouse deployment, and against your target server version specifically for the correlated-subquery `UPDATE` pattern and lightweight-delete support noted above, before relying on either package in production.

-----

## Firebird

These limitations are specific to the [RepoDb.Firebird](https://www.nuget.org/packages/RepoDb.Firebird) package, on top of the [Core](#core) limitations above.

### Requires Firebird 3.0+

Identity-column detection relies on `RDB$RELATION_FIELDS.RDB$IDENTITY_TYPE` and `RDB$GENERATOR_NAME`, which only exist starting with Firebird 3.0. Tables on Firebird 2.5 or earlier that implement auto-increment the pre-3.0 way — a `BEFORE INSERT` trigger plus a bare `RDB$GENERATOR`/`SEQUENCE` — are not detected as identity columns by `FirebirdDbHelper`, and generated-key retrieval on `Insert`/`Merge` will not work for them.

**Alternative Solution**

Upgrade to Firebird 3.0+, or migrate 2.5-era trigger/generator tables to native `GENERATED BY DEFAULT AS IDENTITY` columns.

### QueryMultiple / InsertAll / MergeAll Batching

`FbCommand` cannot execute more than one SQL statement per round trip (`IDbSetting.IsMultiStatementExecutable = false`), the same restriction seen on Oracle and Db2:

- [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type instead of a single combined command.
- `InsertAll`/`MergeAll` execute one statement per row (`batchSize` is forced to 1) rather than a single multi-row command, unlike SQL Server/MySQL/PostgreSQL.

**Alternative Solution**

No workaround inside the provider. For latency-sensitive code paths, be aware that `QueryMultiple<T1, T2, ...>` costs *N* round trips on Firebird, and large `InsertAll`/`MergeAll` batches cost one round trip per row.

### IN(...) List Limit

Firebird's DSQL parser hard-rejects an `IN (...)` list once it exceeds 1500 members, raising `"Implementation limit exceeded... Too many values"`. `IDbSetting.MaxParameterCount` is set to `1500` for this provider so that `DeleteAll(keys)` automatically splits a large key list into batches at that threshold rather than generating an invalid statement.

If you build your own raw `IN (...)` SQL via [ExecuteQuery](http://repodb.net/operation/executequery)/[ExecuteNonQuery](http://repodb.net/operation/executenonquery) instead of using a fluent operation, this automatic batching does not apply — you are responsible for staying under the 1500-item limit yourself.

### Merge Semantics for Identity-as-Qualifier

`Merge`/`MergeAll` are implemented with Firebird's native `UPDATE OR INSERT ... MATCHING (...)` statement rather than an ANSI `MERGE` (which Firebird doesn't have). When the identity column is also a qualifier — the common default case, since qualifiers default to the primary key — a plain `UPDATE OR INSERT ... MATCHING` can't distinguish "match this literal `null`/`0`" from "auto-generate me the way `Insert` would."

To work around this, `FirebirdStatementBuilder` compiles `Merge`/`MergeAll` to an `EXECUTE BLOCK` (an anonymous PL/SQL block) in that specific case. The block branches at runtime: if the identity parameter is `null` or `0`, it runs a plain `INSERT` (letting Firebird auto-generate the identity); otherwise, it runs the ordinary `MATCHING`-based `UPDATE OR INSERT`. This is a genuinely new code path, distinct from every other provider's `Merge` implementation.

**Alternative Solution**

Pass explicit `qualifiers` that exclude the identity column when you want ordinary match-and-update-or-insert semantics without the `EXECUTE BLOCK` branch. Verify `Merge`/`MergeAll` against your own Firebird instance, particularly around the identity-as-qualifier case, before relying on it in production.

### No Session-Wide Scope Identity

Firebird has no equivalent of `SCOPE_IDENTITY()`/`LAST_INSERT_ID()`. `GetScopeIdentity<T>`/`GetScopeIdentityAsync<T>` throw `NotSupportedException` unconditionally:

```csharp
public T GetScopeIdentity<T>(IDbConnection connection, IDbTransaction transaction = null) =>
    throw new NotSupportedException("Firebird has no session-wide scope identity, sequence, or auto-increment mechanism.");
```

This is not a gap — the generated key is already returned directly by `Insert`/`Merge` via Firebird's native `RETURNING` clause, so there is no separate round trip to make.

**Alternative Solution**

Read the generated identity from the return value of `Insert`/`Merge` directly rather than calling `GetScopeIdentity`.

### Fixed-Width CHAR/NCHAR Padding

Like standard SQL (and unlike MySQL, which trims trailing spaces from `CHAR` columns on read), Firebird blank-pads fixed-length `CHAR`/`NCHAR` values out to their declared width on both write and read. A `CHAR(16)` column holding `"C"` reads back as `"C"` followed by 15 trailing spaces, not `"C"` alone.

**Alternative Solution**

Trim the value yourself after reading (`value?.TrimEnd()`) if trailing padding matters to your comparison/display logic, or use `VARCHAR` instead of `CHAR`/`NCHAR` where fixed-width padding isn't actually needed.

### Fixed TIMESTAMP Precision

Firebird's `TIMESTAMP` type has a fixed precision of 4 fractional digits (100-microsecond resolution), regardless of how the value was constructed client-side. A `DateTime` value carrying more than 4 fractional digits (e.g. .NET's default 7-digit tick resolution, or a value built with `"fffff"`/`"ffffff"` formatting) is truncated to 4 digits by the server on write, so a value re-read after insert can compare unequal to the original in-memory value if your comparison doesn't account for this.

**Alternative Solution**

Round/truncate `DateTime` values to 4 fractional digits (100-microsecond precision) before comparing against a value read back from Firebird, or when constructing test/seed data.

### Enum Mapped via TypeMap to a Text Column

Unlike MySQL, Firebird's DSQL layer determines a bind parameter's wire format from the *target column's actual server-side type* (as described by `RDB$RELATION_FIELDS`), not from the client-declared `System.Data.DbType`. Mapping an `enum` property with `[TypeMap(DbType.Int32)]` against a column that is actually `VARCHAR`/`CHAR`/text-typed does not coerce the enum to an integer the way it does on MySQL — it throws `InvalidCastException` instead.

**Alternative Solution**

Store the enum as its natural type on the Firebird side: use an integer-typed column (`SMALLINT`/`INTEGER`) with no `[TypeMap]` override if you want the enum persisted as a number, or a text-typed column with no `[TypeMap]` override if you want it persisted as its name.

### RepoDb.Firebird.BulkOperations Does Not Exist Yet

There is currently no `RepoDb.Firebird.BulkOperations` package. Only a placeholder entry in the root docs and a stub CI pipeline exist so far, pending a follow-up implementation. `BulkInsert`/`BulkMerge`/`BulkUpdate`/`BulkDelete`/`BulkDeleteByKey` are not available for Firebird yet.

**Alternative Solution**

Use `InsertAll`/`MergeAll`/`UpdateAll`/`DeleteAll` from the core `RepoDb.Firebird` package instead, keeping in mind the one-row-per-round-trip cost described under [QueryMultiple / InsertAll / MergeAll Batching](#querymultiple--insertall--mergeall-batching).

### Verification Status

`RepoDb.Firebird` has full unit test coverage (settings, quoting, mapping, statement builder — including a hand-verified SQL text assertion for the `EXECUTE BLOCK` merge path — and attribute/resolver coverage) plus a full integration test suite exercised against a live `jacobalberty/firebird` container via the root `docker-compose.yml` and the `build-firebird` CI job. The `EXECUTE BLOCK`-based merge path described under [Merge Semantics for Identity-as-Qualifier](#merge-semantics-for-identity-as-qualifier) is a genuinely new code path relative to every other provider's `Merge` implementation — it has been exercised against a live instance during development, but deserves extra scrutiny before relying on it in production. `RepoDb.Firebird.BulkOperations` does not exist yet (see above).

## Vertica

These limitations are specific to the [RepoDb.Vertica](https://www.nuget.org/packages/RepoDb.Vertica) and `RepoDb.Vertica.BulkOperations` packages, on top of the [Core](#core) limitations above. Vertica is a distributed, MPP columnar analytical database built on the `Vertica.Data` (`VerticaClient`) ADO.NET provider, and several of the caveats below come directly from behavior that provider exposes (or fails to expose) differently from the single-node RDBMS providers RepoDB otherwise targets.

### QueryMultiple / MergeAll / UpdateAll Batching

`IsMultiStatementExecutable` is `false` for `VerticaDbSetting`, so [QueryMultiple](http://repodb.net/operation/executequerymultiple) falls back to one round trip per requested type, the same as Oracle/Db2/Firebird/ClickHouse. `CreateMergeAll` and `CreateUpdateAll` in `VerticaStatementBuilder` call `ValidateMultipleStatementExecution(batchSize)` internally and throw once `batchSize > 1`, so `MergeAll`/`UpdateAll` execute one row per round trip rather than a single combined statement.

`InsertAll` is the exception — `IsInsertAllBatchable` is `true`, and `CreateInsertAll` still builds a single multi-row `INSERT INTO ... VALUES (row0), (row1), ...` statement (the multi-statement restriction is about chaining *separate* statements together in one request, not about a multi-row `VALUES` list), so `InsertAll` does not pay a per-row round-trip cost.

`IDbSetting.MaxParameterCount` is also set to `1500` for this provider, so `DeleteAll(keys)` automatically splits a large key list into batches at that threshold rather than sending one very large parameter list in a single call.

**Alternative Solution**

No workaround needed for `InsertAll`. For `MergeAll`/`UpdateAll` with more than one row, either issue calls one row at a time or use the `RepoDb.Vertica.BulkOperations` package instead.

### No TRUNCATE TABLE Statement

Vertica (as of 5.0, per `VerticaStatementBuilder`'s own remarks) has no `TRUNCATE TABLE` statement. `CreateTruncate` falls back to a plain `DELETE FROM` with no `WHERE` clause:

```csharp
// Vertica has no TRUNCATE TABLE statement (as of 5.0). DELETE FROM without a WHERE
// clause is the closest equivalent; unlike TRUNCATE elsewhere, it does not reset a
// GENERATED AS IDENTITY column's next value.
builder.Clear()
    .WriteText("DELETE FROM")
    .TableNameFrom(tableName, DbSetting);
```

Unlike a real `TRUNCATE`, this does not reset an `IDENTITY`/`AUTO_INCREMENT` column's next value — a subsequent `Insert` continues the sequence where it left off rather than restarting at its seed value.

**Alternative Solution**

If restarting the identity sequence matters, look up its name via `v_catalog.sequences` and reset it explicitly (`ALTER SEQUENCE ... RESTART WITH ...`) after calling `Truncate`. RepoDB does not do this for you.

### MERGE Statement Rejected on Tables With an Identity Column

Vertica flatly refuses to run a native `MERGE` statement at all against a table that has an `IDENTITY`/`AUTO_INCREMENT` column — *"Sequence or IDENTITY/AUTO_INCREMENT column in merge query is not supported"* — regardless of whether that column even appears in the `SET`/`INSERT` lists. Unlike Firebird (which has `EXECUTE BLOCK`/PSQL as a procedural fallback), Vertica has no equivalent construct to work around this with a single statement.

`VerticaStatementBuilder.CreateMerge`/`CreateMergeAll` instead always compile to an `UPDATE ... WHERE ...` followed by an `INSERT ... WHERE NOT EXISTS (...)`, and — when the identity value needs to be returned — a trailing `SELECT` that reads back `LAST_INSERT_ID()`, all joined by literal `;` characters into a single command text sent as one `VerticaCommand.CommandText`:

```csharp
sb.Append("UPDATE ").Append(quotedTable).Append(" SET ")....Append(" WHERE ").Append(WhereQualifiers()).Append("; ");
sb.Append("INSERT INTO ").Append(quotedTable)...Append(" WHERE NOT EXISTS (SELECT 1 FROM ").Append(quotedTable)...Append(')');
// ...optionally followed by: "; SELECT LAST_INSERT_ID() AS ..."
```

This sits oddly next to `VerticaDbSetting.IsMultiStatementExecutable = false` and the provider's own `TrimTrailingSemicolon` helper, whose remarks state that *"Vertica's DSQL layer treats the semicolon purely as an isql/script statement separator, not as part of the grammar for a single statement submitted through the API"* — the exact reason every other `Create*` method strips its trailing `;`. Whether `Vertica.Data.VerticaClient` actually executes a semicolon-delimited batch of 2–3 statements submitted this way as one `CommandText` (as opposed to erroring, or silently running only the first statement) is not something the unit tests can confirm — `StatementBuilderTest.cs` only asserts the generated SQL *text*, never executes it.

**Alternative Solution**

Verify `Merge`/`MergeAll` against your own Vertica instance — particularly the multi-statement `CommandText` behavior described above — before relying on it in production, especially against a table with an identity column.

### Uniform-Width Integer and Floating-Point Types

Verified directly against `VerticaDataReader.GetSchemaTable()`: Vertica has no distinct storage widths for its integer or floating-point types. `SMALLINT`/`INTEGER`/`BIGINT`/`INT8`/`TINYINT` are all synonyms for one 8-byte integer, and `FLOAT`/`DOUBLE PRECISION`/`REAL` are all synonyms for one 8-byte float. `VerticaDbTypeNameToClientTypeResolver` resolves every one of them to `System.Int64`/`System.Double` — never `Int32`/`Single` — regardless of the column's declared keyword. A property typed `int` or `float` for a Vertica-backed entity is still read back through a `long`/`double` conversion.

**Alternative Solution**

Declare mapped properties as `long`/`double` (or accept the implicit narrowing conversion RepoDB performs) rather than assuming a `SMALLINT` column round-trips as `Int32`.

### Bulk Insert and Merge Identity Correlation

`RepoDb.Vertica.BulkOperations`' `ReturnIdentity` path for `BulkInsert` and `BulkMerge` (when the identity column is itself a qualifier) does not read back each row's generated identity individually. Instead, after the pseudo-table-to-target `INSERT` runs, it calls `GetScopeIdentity` once (`SELECT LAST_INSERT_ID()`) and reconstructs every inserted row's value by subtracting a descending offset from that single last value, relying on the assumption that Vertica assigned the values contiguously in the same order the rows were inserted:

```csharp
// Vertica assigns IDENTITY/AUTO_INCREMENT values contiguously in the order rows are
// inserted (the INSERT is itself ordered by the pseudo table's row-order column), so the
// last value of the underlying sequence minus a descending offset reconstructs every
// inserted row's value.
var lastIdentity = Convert.ToInt64(connection.GetDbHelper().GetScopeIdentity<object>(connection, transaction));
for (var i = 0; i < insertedCount; i++)
{
    identities[i] = lastIdentity - (insertedCount - 1 - i);
}
```

This technique is not verified against a live, concurrently-loaded Vertica instance. Vertica's `IDENTITY`/`AUTO_INCREMENT` columns are backed by a sequence that pre-allocates and caches blocks of values per session/node for performance — a well-documented source of gaps under concurrent load on sequence-based identity mechanisms generally. If another session inserts into the same table between the bulk load's `INSERT ... SELECT` and the `GetScopeIdentity` call that follows it, or if Vertica's own query optimizer executes that `INSERT ... SELECT` across multiple nodes in parallel rather than strictly in the pseudo table's row order, the "last value minus offset" correlation can silently attribute the wrong identity to the wrong entity — with no exception or other signal that it happened.

**Alternative Solution**

Avoid concurrent writers against the same target table while relying on `VerticaBulkImportIdentityBehavior.ReturnIdentity`, and spot-check the returned identities against the actual row data after a bulk load against your own Vertica deployment before depending on this in production. Using `KeepIdentity` with a client-generated key sidesteps the issue entirely.

### Bulk Operations: BatchSize Parameter Has No Effect

Every `Bulk*` method in `RepoDb.Vertica.BulkOperations` accepts a `batchSize` parameter, which is assigned to `VerticaBulkCopy.BatchSize`:

```csharp
if (batchSize.HasValue)
{
    batcher.BatchSize = batchSize.Value;
}
```

`VerticaBulkCopy.BatchSize` is declared as *"the number of rows submitted per round trip"* — but nothing in `VerticaBulkCopy` ever reads that property. Every `WriteToServer`/`WriteToServerAsync` overload buffers the *entire* source (every row of the `DataTable`/`IDataReader`) into one in-memory `MemoryStream`, then hands that whole stream to a single `VerticaCopyStream` `Start`/`AddStream`/`Execute`/`Finish` sequence in one round trip. The `batchSize` argument is silently accepted and has no effect on chunking, round-trip count, or memory usage.

**Alternative Solution**

Do not rely on `batchSize` to bound memory usage or round-trip count for `RepoDb.Vertica.BulkOperations` — the entire row set is always materialized in memory as one COPY payload regardless of the value passed. For very large loads, chunk the source data yourself (e.g. call `BulkInsert` repeatedly against slices of your source) if memory footprint is a concern.

### Bulk Operations Staging Table

`BulkInsert` (with `ReturnIdentity`), `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` all stage rows into a pseudo table via `VerticaCopyStream` before running the real `INSERT`/`UPDATE`/`DELETE` against the target table. `VerticaBulkImportPseudoTableType` offers the same three values seen in the other bulk-operations packages:

- **`Memory`** — a Vertica `GLOBAL TEMPORARY TABLE ... ON COMMIT PRESERVE ROWS`, whose rows are private to the connection that wrote them.
- **`Physical`** — an ordinary heap table, faster to create for very large row counts, at the cost of the rows briefly existing as a real (if uniquely-named) table.
- **`Auto`** *(default)* — resolves to `Physical` at 5,000+ rows, otherwise `Memory`; unlike the `Auto`-always-resolves-to-`Physical` bug documented for `RepoDb.Db2.BulkOperations`/`RepoDb.MariaDb.BulkOperations`/`RepoDb.ClickHouse.BulkOperations`, this provider's `ResolvePseudoTableType` correctly branches both ways.

Every pseudo table is named uniquely per call (`"RDBLK" + operationTag + Guid.NewGuid()`), so — unlike the sibling packages just mentioned, which use a deterministic, non-unique staging-table name and can race when two callers target the same table concurrently — `Physical` and `Memory` are both safe for concurrent callers here; there is no shared staging-table name to collide on.

**Alternative Solution**

No workaround needed for the staging-table naming itself. If you explicitly need a `Physical` staging table to never be visible to other sessions even momentarily, request `Memory` explicitly rather than relying on `Auto`'s row-count threshold.

### Verification Status

`RepoDb.Vertica` has unit test coverage (`DbSettingTest.cs`, `StatementBuilderTest.cs` — including SQL-text assertions for the `Merge`/`MergeAll` statements described above — plus resolver/attribute coverage) and a full integration test suite (`RepoDb.Vertica.IntegrationTests`, including `MergeTest.cs`/`MergeAllTest.cs`) exercised against a live `molo17/vertica-ce:24.1.0-0` container via the root `docker-compose.yml` and the `build-vertica`/`build-pr-vertica` CI jobs. `RepoDb.Vertica.BulkOperations` similarly has its own integration test project and `build-vertica-bulk`/`build-pr-vertica-bulk` CI jobs against the same container.

None of this exercises the two behaviors flagged above as needing the most scrutiny: the integration test suite runs single-connection, so it does not validate the [Merge](#merge-statement-rejected-on-tables-with-an-identity-column) multi-statement `CommandText` under concurrent access, nor the [bulk insert/merge identity correlation](#bulk-insert-and-merge-identity-correlation) technique under a concurrently-loaded table or a multi-node cluster. Verify both against your own Vertica deployment — under realistic concurrency — before relying on them in production.
## SAP HANA

These limitations are specific to the `RepoDb.SapHana` and `RepoDb.SapHana.BulkOperations` packages, on top of the [Core](#core) limitations above. Both are built on the [`Sap.Data.Hana.Net.v6.0`](https://www.nuget.org/packages/Sap.Data.Hana.Net.v6.0) (`Sap.Data.Hana`/`HanaConnection`) ADO.NET driver.

### QueryMultiple / InsertAll / MergeAll Batching

`IsMultiStatementExecutable` is `false` for `SapHanaDbSetting` — HANA's ADO.NET client rejects a command text containing more than one SQL statement. `CreateInsertAll` and `CreateMergeAll` in `SapHanaStatementBuilder` call `ValidateMultipleStatementExecution(batchSize)` and then just reuse the single-row `CreateInsert`/`CreateMerge` statement, so `InsertAll`/`MergeAll` execute one row per round trip rather than a single combined or multi-row statement — unlike Vertica's `InsertAll`, which still builds one multi-row `VALUES (row0), (row1), ...` statement despite the same multi-statement restriction (see [Vertica's QueryMultiple / MergeAll / UpdateAll Batching](#querymultiple--mergeall--updateall-batching)). `QueryMultiple` likewise falls back to one round trip per requested type.

**Alternative Solution**

No workaround needed functionally. If round-trip count matters at scale, see [Bulk Operations Have No Native Bulk-Copy Path](#bulk-operations-have-no-native-bulk-copy-path) below before assuming `RepoDb.SapHana.BulkOperations` avoids the same cost.

### Identity/Primary Key Retrieval

Because `IsMultiStatementExecutable` is `false`, `CreateInsert` can't append a trailing `SELECT ... AS "Result"` the way the SQL-Server-family pattern does to hand back a generated key in the same round trip. Identity retrieval instead falls back to a separate round trip: `SapHanaDbHelper.GetScopeIdentity<T>` runs `SELECT CURRENT_IDENTITY_VALUE() FROM DUMMY`, which SAP documents as returning the last identity value generated by the *current session* — the same one-extra-round-trip, session-scoped pattern as SQL Server's `SCOPE_IDENTITY()`. If anything else executes an identity-generating statement on the same connection between the `INSERT` and this follow-up `SELECT`, the value read back no longer corresponds to the row RepoDb just inserted.

`CreateMerge` has no equivalent fallback at all: RepoDb.Core's merge pipeline never issues a follow-up round trip to read a key back, so a `Merge`/`MergeAll` call that inserts a *new* row never gets its generated identity value, regardless of provider. For SAP HANA specifically, `CreateMerge` compiles to HANA's native `UPSERT <table> (...) VALUES (...) WITH PRIMARY KEY`, which requires `primaryField` to be present (it throws `PrimaryFieldNotFoundException` otherwise) — in practice this isn't a loss, since `UPSERT ... WITH PRIMARY KEY` needs the primary key value up front to match against, so it's already known on the entity being merged. But it does mean `Merge`/`MergeAll` is not a way to obtain a newly generated identity value on SAP HANA; use `Insert`/`InsertAll` for that.

### GUID/UNIQUEIDENTIFIER

HANA has no native GUID/`UNIQUEIDENTIFIER` type. `RepoDb.SapHana` ships `SapHanaGuidToStringPropertyHandler`, which converts a `Guid` entity property to/from a `string` for binding against an `NVARCHAR(36)` column — but unlike the byte-array handlers shipped for Oracle/Db2, it is not registered anywhere in `SapHanaBootstrap.InitializeInternal()`. You must register it yourself, per property:

```csharp
PropertyHandlerMapper.Add<YourEntity, SapHanaGuidToStringPropertyHandler>(
    e => e.YourGuidProperty, new SapHanaGuidToStringPropertyHandler(), true);
```

Without this registration, binding a raw `System.Guid` property against a `HanaParameter` has no compatible native HANA type to fall back on.

**Alternative Solution**

Register the handler per property rather than globally for `typeof(Guid)` if your process also uses another RepoDb provider that handles `Guid` natively — a type-level registration applies process-wide, across all connections.

### Bulk Operations Have No Native Bulk-Copy Path

Unlike `SqlBulkCopy`, `OracleBulkCopy`, or Npgsql's binary `COPY`, `Sap.Data.Hana` has no bulk-copy API. The `RepoDb.SapHana.BulkOperations` README acknowledges this directly ("SAP HANA has no native bulk-copy API equivalent to `SqlBulkCopy`/`MySqlBulkCopy`") and describes the fallback as "chunked, parameterized multi-row `INSERT` statements." The actual implementation in `WriteToServer.cs` is narrower than that description: `BuildRowInsertText` always builds a **single-row** `INSERT INTO tbl (...) VALUES (:p0, :p1, ...)` — never a multi-row `VALUES (row0), (row1), ...` list — and `FlushBatch`/`FlushBatchAsync` prepare that command once per chunk of `batchSize` rows (`500` by default), then call `command.ExecuteNonQuery()` once per row inside the chunk, reusing the same `HanaParameter` objects:

```csharp
var affected = 0;
foreach (var row in buffer)
{
    for (var c = 0; c < destinationColumns.Length; c++)
    {
        parameters[c].Value = NormalizeParameterValue(row[c]);
    }
    affected += command.ExecuteNonQuery();
}
```

So every `Bulk*` call in this package issues one round trip per row — reusing the same prepared statement/parameter objects across a chunk avoids re-preparing and re-allocating them, but it does not reduce the number of round trips. This is functionally closer to a loop of ordinary `Insert` calls than to a bulk-copy load, despite the "high-performant" framing in the package README.

**Alternative Solution**

Do not assume `RepoDb.SapHana.BulkOperations` avoids per-row round trips the way `RepoDb.SqlServer.BulkOperations` does. For very large loads where round-trip count dominates cost, benchmark against your own HANA deployment before assuming a throughput advantage over plain `InsertAll`.

### Bulk Operations Staging Table

`BulkInsert` (with `ReturnIdentity`), `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` all stage rows into a pseudo table — named deterministically as `{pseudoTableType}{tableName}{Operation}` (e.g., `PhysicalPersonMerge`), the same non-unique naming scheme used by `RepoDb.Db2.BulkOperations`/`RepoDb.MariaDb.BulkOperations`/`RepoDb.ClickHouse.BulkOperations` — before running a set-based `INSERT`/`MERGE`/`UPDATE`/`DELETE` against the target table. `SapHanaBulkImportPseudoTableType` documents the same three values as those sibling packages:

- **`Memory`** — a `LOCAL TEMPORARY TABLE`; its own XML doc states rows are session-private.
- **`Physical`** — an ordinary heap table; its own XML doc already states rows are "not session-isolated, so concurrent callers can see and interfere with each other's staged rows."
- **`Auto`** *(default)* — documented to resolve to `Physical` at `SapHanaConstants.RowCountThresholdForPhysicalTable` (5,000) rows or more, otherwise `Memory`.

**`Memory` is not reachable at all — not even by requesting it explicitly.** `ResolvePseudoTableType` in `WriteToServer.cs` is:

```csharp
private static SapHanaBulkImportPseudoTableType ResolvePseudoTableType(SapHanaBulkImportPseudoTableType pseudoTableType,
    int? rowCount) =>
    pseudoTableType == SapHanaBulkImportPseudoTableType.Auto && rowCount.GetValueOrDefault() >= SapHanaConstants.RowCountThresholdForPhysicalTable ?
        SapHanaBulkImportPseudoTableType.Physical :
            SapHanaBulkImportPseudoTableType.Physical;
```

Both branches of the ternary return `Physical`. This is the same "every pseudo table resolves to `Physical` regardless of what you pass" defect already documented for `RepoDb.Db2.BulkOperations`/`RepoDb.MariaDb.BulkOperations`/`RepoDb.ClickHouse.BulkOperations` — except here it isn't even conditional on `Auto`: an explicit `pseudoTableType: SapHanaBulkImportPseudoTableType.Memory` argument is silently overridden too, since the method ignores its input entirely.

Combined with the deterministic, non-unique staging-table name above, and every staging table always being the `Physical`, non-session-isolated kind, two concurrent callers bulk-writing against the same target table target the exact same staging-table name and can truncate, drop, or overwrite each other's staged rows mid-operation.

**Alternative Solution**

Serialize bulk operations against the same table from a single caller at a time until a working `Memory` path exists. Do not rely on requesting `Memory` explicitly — it currently has no effect.

### Bulk Insert and Merge Identity Correlation

`BulkInsert`/`BulkMerge` with `identityBehavior: ReturnIdentity` do not read each row's generated identity back individually after the fact. Instead, before the real `INSERT`/`MERGE` runs, the code precomputes what the new identity values will be and writes them into the pseudo table itself:

1. `GetIdentitySequenceMetadataSql` computes `SELECT CAST(COALESCE(MAX(<identityColumn>), 0) + 1 AS VARCHAR(38)) ...` against the *target* table — a plain, unlocked `MAX(...) + 1` snapshot, not a real sequence `NEXTVAL`.
2. For `BulkInsert`, `GetPreAssignPseudoTableIdentitySql` runs `UPDATE <pseudoTable> SET <identityColumn> = <rowOrderColumn> + (<thatMax+1> - 1)`, assigning every staged row a value sequentially from that snapshot.
3. For `BulkMerge`, unmatched (insert-bound) rows get the same treatment via `GetAssignFreshIdentityToUnmatchedPseudoTableRowsSql`, while matched (update-bound) rows are instead assigned the target row's *existing* identity via `GetAssignMatchedIdentityToPseudoTableRowsSql`.
4. Only then does the real `INSERT ... SELECT` / `MERGE` run, writing the *explicit*, precomputed identity values — the values are decided before the row exists in the real table, not read back afterward.

Because step 1's `MAX(...) + 1` snapshot is not atomic with steps 2–4's actual write, two concurrent `BulkInsert`/`BulkMerge` calls against the same table (or a concurrent plain `Insert`) can compute the same "next" value and then both attempt to write it — colliding against a primary-key/unique constraint on the identity column, or silently duplicating it if none exists.

Separately, `GetIdentitySequenceMetadataSql` also computes an `IsAlwaysGenerated` flag meant to distinguish a `GENERATED ALWAYS AS IDENTITY` column (which rejects explicit values) from `GENERATED BY DEFAULT AS IDENTITY` — but the SQL hardcodes `'NO' AS "GenerationType"` unconditionally, so the flag is always `false`, and `MergeFromPseudoTableForReturnIdentity` discards it entirely (`var (sequenceName, _) = ...`). If the target table's identity column is `GENERATED ALWAYS`, HANA rejects an explicit value for it, so `BulkInsert`/`BulkMerge` with `ReturnIdentity` against such a table fails at the final `INSERT`/`MERGE` step, since the code always attempts to write the identity column explicitly regardless of the column's actual generation mode.

**Alternative Solution**

Avoid concurrent writers against the same target table while relying on `SapHanaBulkImportIdentityBehavior.ReturnIdentity`. Use `GENERATED BY DEFAULT AS IDENTITY` rather than `GENERATED ALWAYS AS IDENTITY` on the target column if you need `ReturnIdentity` from `BulkInsert`/`BulkMerge`, or use `KeepIdentity` with a client-generated key to sidestep both issues.

### Bulk Operations and Transactions

Every `Bulk*` method creates its pseudo table (`DropPseudoTable` then `CREATE [LOCAL TEMPORARY] TABLE`) at the start of the call and drops it again in a `finally` block at the end — on every single call, unlike `RepoDb.Oracle.BulkOperations`, which reuses one staging table per (table, pseudo table type). None of the pseudo-table DDL, the per-row `INSERT`s described in [Bulk Operations Have No Native Bulk-Copy Path](#bulk-operations-have-no-native-bulk-copy-path), or the final drop receives special transactional handling beyond whatever `HanaTransaction transaction` argument the caller already passed through `connection.ExecuteNonQuery(..., transaction: transaction)` — there is no internal `BeginTransaction`/`Commit` wrapping. If the caller doesn't supply a transaction, each step in the sequence (drop old pseudo table, create it, populate it row-by-row, run the real statement, drop it again) commits independently under HANA's default autocommit behavior.

SAP HANA's DDL-and-transactions semantics also differ from the "DDL always force-commits" behavior documented for Oracle/Db2 elsewhere in this document — HANA's `autocommit_ddl` session setting can make `CREATE`/`DROP TABLE` participate in (and be rolled back by) the ambient transaction rather than always committing immediately. Whether `Sap.Data.Hana`'s default connection settings leave `autocommit_ddl` on or off — and therefore whether a caller-supplied transaction that gets rolled back also undoes an interrupted bulk operation's staging-table DDL — isn't asserted anywhere in this codebase.

**Alternative Solution**

If a rollback needs to also undo a bulk operation's target-table changes, pass an explicit `HanaTransaction` and verify against your own SAP HANA instance whether that also protects the staging-table lifecycle, given the `autocommit_ddl`-dependent behavior described above.

### Verification Status

`RepoDb.SapHana` has unit test coverage (`DbSettingTest.cs`, `StatementBuilderTest.cs`, `MappingTest.cs`, `QuotationTest.cs`, plus resolver/attribute tests) and a full integration test suite (`RepoDb.SapHana.IntegrationTests`, including `TransactionTests.cs`). `RepoDb.SapHana.BulkOperations` has an integration test suite (`BulkInsertTest.cs`/`BulkMergeTest.cs`/`BulkUpdateTest.cs`/`BulkDeleteTest.cs`/`BulkDeleteByKeyTest.cs`) but, unlike every sibling `*.BulkOperations` package covered elsewhere in this document, no unit test project of its own — there is no `StatementBuilderTest`-equivalent asserting the generated SQL text for the pseudo-table pipeline in isolation.

Both packages' CI jobs (`build-saphana`/`build-saphana-bulk`, `build-pr-saphana`/`build-pr-saphana-bulk`) start a real `saplabs/hanaexpress` container and run `dotnet test` against it across .NET 8/9/10 — the same pattern used for every DB-backed provider in this repository. This sits oddly next to each package's own README, which states the provider "has not been verified against a live SAP HANA instance." Which claim is current can't be determined from the source alone — the README text may be stale boilerplate carried over from an earlier, genuinely-unverified provider, or the CI job may not yet have been run to completion since this was written. Check the actual CI run history for the `saphana-support` branch before trusting either claim, and in particular verify the two behaviors flagged above as needing the most scrutiny — the [Bulk Insert and Merge Identity Correlation](#bulk-insert-and-merge-identity-correlation-1) race/`GENERATED ALWAYS` gap, and the [Bulk Operations Staging Table](#bulk-operations-staging-table-6) concurrent-caller collision — neither of which a single-connection test run would surface.

-----

## EnterpriseDB

These limitations are specific to the [RepoDb.EnterpriseDb](https://www.nuget.org/packages/RepoDb.EnterpriseDb) and `RepoDb.EnterpriseDb.BulkOperations` packages, on top of the [Core](#core) limitations above. Both target EDB Postgres Advanced Server; `RepoDb.EnterpriseDb.BulkOperations` specifically is built against [`RepoDb.Connector.EnterpriseDb`](https://github.com/mikependon/RepoDB.Connectors) (a Npgsql-based connector) rather than the official [`EnterpriseDB.EDBClient`](https://www.nuget.org/packages/EnterpriseDB.EDBClient) driver that `RepoDb.EnterpriseDb` itself otherwise depends on.

### Bulk Operations Staging Table

`BulkInsert`, `BulkMerge`, `BulkUpdate`, `BulkDelete`, and `BulkDeleteByKey` all stage rows into a pseudo table named deterministically from `{pseudoTableType}{tableName}{Operation}` (e.g. `PhysicalPersonMerge`) — the same non-unique naming scheme documented for `RepoDb.Db2.BulkOperations`/`RepoDb.MariaDb.BulkOperations`/`RepoDb.SapHana.BulkOperations`/`RepoDb.ClickHouse.BulkOperations` elsewhere in this document. `EDBBulkImportPseudoTableType` has the same three values as those sibling packages — `Auto` (default), `Memory`, `Physical` — with `Physical` an ordinary heap table (not session-isolated) and `Memory` a Postgres `TEMP` table (session-private).

Unlike those sibling packages, `Auto`'s row-count threshold actually works here: `ResolvePseudoTableType` in `WriteToServer.cs` correctly resolves to `Physical` at `EDBConstants.RowCountThresholdForPhysicalTable` (5,000) rows or more, and to `Memory` otherwise, and an explicit `Memory` argument is honored rather than silently overridden. So the collision risk below is avoidable by using `Memory` (or staying under the row threshold with `Auto`) — it is not, unlike the sibling packages, a case of `Memory` being unreachable altogether.

The risk remains for `Physical`: two concurrent callers bulk-writing against the same target table with `pseudoTableType: Physical` (or an `Auto` call that crosses the row threshold) target the exact same staging-table name — created, populated, indexed, and dropped within each call — and can race or corrupt each other's staged rows.

**Alternative Solution**

Prefer `Memory` (or stay under the 5,000-row `Auto` threshold) for tables that may see concurrent bulk writers. Serialize `Physical` bulk operations against the same table otherwise.

### Bulk Insert and Merge Identity Correlation Relies on RETURNING Order

`BulkInsert`/`BulkMerge` with `identityBehavior: ReturnIdentity` read generated/existing identity values back via a single `INSERT ... SELECT ... ORDER BY <row-order column> RETURNING <identity> AS "Result"` statement (for `BulkMerge`, `INSERT ... ON CONFLICT (qualifiers) DO UPDATE ...RETURNING`), then assign each returned value positionally — `entities[result]`/`rows[result][identityField.Name]`, incrementing `result` per row read — assuming `RETURNING` emits rows in the same order the source `SELECT` produced them:

```csharp
using var reader = (DbDataReader)connection.ExecuteReader(commandText, transaction: transaction);
var result = 0;
while (reader.Read())
{
    setter(entities[result], Converter.DbNullToNull(reader.GetValue(0)));
    result++;
}
```

This positional-order assumption is not a documented guarantee of the SQL standard or of PostgreSQL/EDB's `RETURNING` clause. It is expected to hold today because PostgreSQL does not parallelize the write (modifying) node of a query plan — `INSERT`/`UPDATE`/`DELETE`/`MERGE` always execute serially even when their underlying `SELECT` could otherwise use a parallel plan — so a single, non-parallel `INSERT ... SELECT ... ORDER BY ... RETURNING` processes and emits its source rows one at a time, in the scan order of that ordered `SELECT`. This is the same class of assumption RepoDB's SQL Server bulk package makes with `MERGE ... OUTPUT` over an `ORDER BY`-carrying subquery (see [SQL Server's Identity Correlation Differs by Input Shape](#identity-correlation-differs-by-input-shape)) — except SQL Server's own documentation explicitly warns against relying on it, where no equivalent warning (or guarantee) exists for PostgreSQL/EDB's `RETURNING`.

**Alternative Solution**

This has not been verified against a live EDB Postgres Advanced Server instance (see [Verification Status](#verification-status-7)). Confirm row-order correlation holds under real load — larger batches in particular — before relying on `ReturnIdentity` in production. If PostgreSQL/EDB ever parallelizes DML execution in a future version, this assumption would need to be revisited.

### BulkMerge Without Updateable Columns Undercounts Affected Rows

`GetMergeFromPseudoTableSql`'s non-return-identity path falls back to `ON CONFLICT (qualifiers) DO NOTHING` when every merged column is either the identity field or a qualifier (i.e., there is nothing left to update once qualifiers are excluded — a merge keyed on effectively every column):

```csharp
var conflictAction = updateableFields.Count > 0
    ? string.Concat("DO UPDATE SET ", ...)
    : "DO NOTHING";
```

`MergeFromPseudoTable` reports its result via `connection.ExecuteNonQuery(commandText, ...)`. PostgreSQL/EDB's command-completion tag for `INSERT ... ON CONFLICT DO NOTHING` counts only rows actually inserted — rows that hit the conflict and were skipped are not included. In this narrow edge case, `BulkMerge`'s returned row count therefore reports only the number of newly inserted rows, silently excluding any rows that matched an existing row and were left untouched by `DO NOTHING`, even though those rows were part of the merge operation.

**Alternative Solution**

This edge case only triggers when a merge's qualifier set covers every non-identity column being merged. Include at least one genuinely updateable (non-qualifier) column in the merge if an accurate affected-row count matters for your use case.

### Verification Status

`RepoDb.EnterpriseDb.BulkOperations` has an integration test suite (`BulkInsertTest.cs`/`BulkMergeTest.cs`/`BulkUpdateTest.cs`/`BulkDeleteTest.cs`/`BulkDeleteByKeyTest.cs`, adapted from `RepoDb.MariaDbConnector.BulkOperations`'s own suite) but, like `RepoDb.SapHana.BulkOperations`, no unit test project of its own — there is no `StatementBuilderTest`-equivalent asserting the `EDBText` SQL-generation logic in isolation. Neither the integration suite nor any manual testing has been run against a live EDB Postgres Advanced Server instance.

The dual-registration wiring added to `RepoDb.EnterpriseDb`'s `EnterpriseDbBootstrap` — registering `DbSettingMapper`/`DbHelperMapper`/`StatementBuilderMapper` for `RepoDb.Connector.EnterpriseDb.EDBConnection` alongside the pre-existing official-driver registration, and the `Activator.CreateInstance`-based connection-retry path in `EnterpriseDbDbHelper` — is exercised by neither `RepoDb.EnterpriseDb.UnitTests` nor `RepoDb.EnterpriseDb.IntegrationTests`, both of which predate this change and only ever construct the official `EnterpriseDB.EDBClient.EDBConnection`. Only `RepoDb.EnterpriseDb.BulkOperations.IntegrationTests` references `RepoDb.Connector.EnterpriseDb.EDBConnection` at all, and that suite itself has not been run.

The `build-enterprisedb`/`build-enterprisedb-bulk` CI workflows are configured to start a real `docker.enterprisedb.com/k8s/edb-postgres-advanced:18` container and run `dotnet test` against it, the same pattern used for every DB-backed provider in this repository. Unlike the other providers' container images, this one requires authentication against EDB's own registry — the workflow's own comment notes the `EDB_DOCKER_USERNAME`/`EDB_DOCKER_PASSWORD` repository secrets must be configured before the job can even pull the image. Whether those secrets are configured, and whether the CI job has actually run to completion, could not be determined from the source alone — check the actual CI run history before trusting a passing badge.
