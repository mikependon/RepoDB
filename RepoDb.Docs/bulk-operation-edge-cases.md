# Edge Case on Bulk Operations

It is quite important for us to relay to you the edge-cases that is being addressed by the bulk operations embedded in this library. We strongly believe that the bulk operations in RepoDB library are addressing the use-cases that is mainly not solved by other ORMs.

## Call Parameters

This is pertained to the simplicity of the invocations when calling the bulk operations. All the bulk operations (i.e: [BulkInsert](https://repodb.net/operation/bulkinsert), [BulkUpdate](https://repodb.net/operation/bulkupdate), [BulkDelete](https://repodb.net/operation/bulkdelete) and [BulkMerge](https://repodb.net/operation/bulkmerge)) are accepting different kind of parameters.
 
#### Via DataReader

```csharp
using (var sourceConnection = new OracleConnection("OracleConnectionString"))
{
	using (var reader = sourceConnection.ExecuteReader("SELECT * FROM Person"))
	{
		using (var destinationConnection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
		{
			var rowsInserted = connection.BulkInsert(people);
		}
	}
}
```

#### Via DataTable

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(ConvertToDataTable(people));
}
```

Assuming the `ConvertToDataTable()` method is existing.

#### Via IEnumerable&lt;T&gt;

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people);
}
```

## Identity Columns

Both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations are special in a way of bulk-processing the huge datasets. By using these operations, you are able to retrieve the newly generated identity column values from your data models, just right after the execution.

We believe that it is a very common and is an important use-case to you (and/or most developers). Imagine bulk inserting the huge datasets and have all the identities referenced back to your application for building relationships.

To enable this, simply set the `isReturnIdentity` argument to `true` when calling the operations.

#### BulkInsert

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people, isReturnIdentity: true);
}
```

#### BulkMerge

```csharp
var people = GetPeople();
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkMerge(people, isReturnIdentity: true);
}
```

## Pseudo-Temporary Table (Physical/Temporary)

In RepoDB, the bulk operations are creating a pseudo-temporary table as a stepping stone for the actual operation. The data is first brought into this pseudo-temporary table via [BulkInsert](https://repodb.net/operation/bulkinsert) operation. After that, the data in the pseudo-temporary table will be promoted towards the actual table. It is happening behind-the-scene when calling the [BulkDelete](https://repodb.net/operation/bulkdelete), [BulkMerge](https://repodb.net/operation/bulkmerge) and [BulkUpdate](https://repodb.net/operation/bulkupdate) bulk operations.

You can leverage the underlying RDBMS capability by specifying whether to use the "temporary" pseudo-temporary table or the "physical" pseudo-temporary table. To do this, simply set the `usePhysicalPseudoTempTable` argument to `true`.

#### BulkInsert

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(people, usePhysicalPseudoTempTable: true);
}
```

#### BulkMerge

```csharp
var people = GetPeople();
using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkMerge(people, usePhysicalPseudoTempTable: true);
}
```

**Note:** Use the "temporary" pseudo-temporary table if you are working with multi-threaded environment. Otherwise, use the "physical" pseudo-temporary table to maximize the advantages of having an actual physical table.
