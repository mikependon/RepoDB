# Edge Case on Bulk Operations

In RepoDb, the bulk operations are not just the normal bulk operations. They are addressing some very important edge-cases that many ORMs in .NET space did not solved.

## Parameters

All the bulk operations (i.e: [BulkInsert](https://repodb.net/operation/bulkinsert), [BulkUpdate](https://repodb.net/operation/bulkupdate), [BulkDelete](https://repodb.net/operation/bulkdelete) and [BulkMerge](https://repodb.net/operation/bulkmerge)) are accepting different kind of parameters.
 
#### DataReader

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

#### DataTable

Assuming the `ConvertToDataTable()` method is existing.

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(ConvertToDataTable(people));
}
```

#### IEnumerable&lt;T&gt;

```csharp
var people = new List<Person>();
people.Add(..);

using (var connection = new SqlConnection("Server=.;Database=TestDB;Integrated Security=SSPI;"))
{
	var rowsInserted = connection.BulkInsert(ConvertToDataTable(people));
}
```

## Identity Columns

Both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations are special in a way of bulk-processing the huge datasets but still be able to retrieve the new generated identity columns value back to the data models.

It is a very common and is an important use-case to most. Imagine bulk inserting the huge datasets and have all the identities referenced back to your application for building relationships.

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

In RepoDb, the bulk operations are creating a pseudo-temporary as a stepping stone for the actual operation. The data is first brought into this pseudo-temporary table via [BulkInsert](https://repodb.net/operation/bulkinsert) operation. Then, this pseudo-temporary is being used to promote the actual changes towards the target table.

You can leverage the underlying RDBMS capability by specifying whether to use the TEMP pseudo-temporary table or the physical pseudo-temporary table. To do this, simply set the `usePhysicalPseudoTempTable` argument to `true`.

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

**Note:** Use the TEMP pseudo-temporary table if you are working with multi-threaded environment. Otherwise, always use the physical pseudo-temporary table to maximize the advantages of having a physical table.
