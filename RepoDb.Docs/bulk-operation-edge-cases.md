# Edge Case on Bulk Operations

In RepoDb, the bulk operations are not just a normal bulk operation. It does address some very important edge-cases that many ORM in .NET space does not provide.

## Parameters

All the bulk operations (i.e: [BulkInsert](https://repodb.net/operation/bulkinsert), [BulkUpdate](https://repodb.net/operation/bulkupdate), [BulkDelete](https://repodb.net/operation/bulkdelete) and [BulkMerge](https://repodb.net/operation/bulkmerge)) are flexible to accept different kind of parameters.
 
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

Both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations are specialized in a way of bulk-processing the data and still be able to retrieve the identities back to the caller.

This is a very common but is an important use-case to everyone. Imagine bulk inserting huge datasets and have all the identities referenced back to your application.

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

In RepoDb, the bulk operations are creating a pseudo-temporary as a stepping stone for the actual operation. The data is first being brought to this pseudo-temporary table with the use of [BulkInsert](https://repodb.net/operation/bulkinsert) operation. Then, this pseudo-temporary is being used to promote the changes towards the target table.

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

**Note:** Use the TEMP pseudo-temporary table if you are working in multi-threading environment. Otherwise, always use the physical pseudo-temporary table to maximize the advantages of having the physical table.